using CardActions;
using DeckData;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandManager : NetworkBehaviour
{
    public GameObject cardPrefab;
    public CardPlayerController player;

    public GameObject selectedCard;

    public Vector3 HandPosition;
    public float cardSpread = 3f;
    static float angleMax = 90;
    //define bounds here and divide between hand size

    List<CardData> renderedHand = new List<CardData>();
    public List<GameObject> cards = new List<GameObject> ();
    public Dictionary<GameObject, int> hand = new Dictionary<GameObject, int>();
    //compare the local player to this number. if they dont match,
    //any attempted plays will result in "suggest playing card targeting X" with a visual ping
    public int OwnedBy;

    GameObject playedCard;
    public string clickedCardName;
    GameObject previewLine;

    bool refeshStart;

    //this should only do stuff for local player, probably

    // Start is called before the first frame update
    void Start()
    {
        previewLine = GameObject.Find("LocalPlayerLine");
        //RefreshHand();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "ConnorTest")
        {
            refeshStart = true;
            return;
        }

        //add a condition to make this only update when a card is drawn
        if (refeshStart)
        {
            //HandPosition = transform.position + (transform.forward * 2 + transform.up * 2);
            RefreshHand();
            refeshStart = false;
            previewLine = GameObject.Find("LocalPlayerLine");
        }
        //raycast to world from mouse position.
        //if result is one of the renderedHand cards, and the player clicks, play that card
        CheckMouseOverCard();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (selectedCard != null)
            {
                //discover the hand id
                playedCard = selectedCard;
                playedCard.transform.position += playedCard.transform.forward * (player.deck.hand.Count * 0.1f);
                selectedCard.transform.position -= selectedCard.transform.forward * (player.deck.hand.Count * 0.1f);
                selectedCard = null;
            }
        }
        if (playedCard != null && !player.TurnEnded)
        {
            if (previewLine != null)
                previewLine.GetComponent<PointToTarget>().Display();
        }
        //check for click on enemy
        CheckMouseOnEnemy();
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {

            if (playedCard != null)
            {
                playedCard.transform.position -= playedCard.transform.forward * (player.deck.hand.Count * 0.1f);
                playedCard = null;
            }
        }
    }
    void CheckMouseOnEnemy()
    {
        if(playedCard==null)
            return;

        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<CardEnemyController>() != null)
                {
                    if (previewLine != null)
                        previewLine.GetComponent<PointToTarget>().Display();
                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        int playedCardIndex = 0;
                        /*for (int i = 0; i > cards.Count; i++)
                        {
                            if (cards[i] == playedCard)
                            {
                                Debug.Log(renderedHand[i].cardName + " with index " + i + " is the played card");
                                playedCardIndex = i;
                                break;
                            }
                        }*/
                        foreach(GameObject card in hand.Keys)
                        {
                            if(card==playedCard)
                            {
                                playedCardIndex = hand[card];
                                Debug.Log(renderedHand[hand[card]].CardName + " with index " + hand[card] + " is the played card");
                                break;
                            }
                        }
                        if (previewLine != null)
                            previewLine.GetComponent<PointToTarget>().Hide();
                        if (renderedHand[playedCardIndex].EnergyCost <= player.currentEnergy)
                        {
                            //local player check  here
                            if (isLocalPlayer)
                            {
                                //
                                CMDChangeStam(playedCardIndex);
                                Debug.Log("played card with ID " + playedCardIndex);
                                player.deck.ServerPlayCard(netId, hit.collider.gameObject.transform.position, playedCardIndex);
                                RefreshHand();
                                playedCard = null;
                            }
                            else
                            {
                                player.deck.ServerSuggestCard(netId, hit.collider.gameObject.transform.position, playedCardIndex);
                                playedCard = null;
                            }
                        }
                        else
                        {
                            player.energydisplay.OOM();
                            Debug.Log("not enough energy");
                        }
                    }
                }
                else
                {
                    if (previewLine != null)
                        previewLine.GetComponent<PointToTarget>().Hide();
                }
            }
        }
        
    }

    [Command(requiresAuthority = false)]
    void CMDChangeStam(int i)
    {
        ChangeStam(i);
    }
    [ClientRpc]
    void ChangeStam(int i)
    {
        player.currentEnergy -= renderedHand[i].EnergyCost;
    }

    void CheckMouseOverCard()
    {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                GameObject card = null;
                try { card = hit.collider.transform.parent.gameObject; } catch { }
                if (cards.Contains(card))
                {
                    if (card != selectedCard)
                    {
                        if (selectedCard != null)
                            selectedCard.transform.position -= selectedCard.transform.forward * (player.deck.hand.Count * 0.1f);
                        selectedCard = card;
                        card.transform.position += card.transform.forward * (player.deck.hand.Count * 0.1f);
                        if(previewLine!=null)
                            previewLine.transform.position = transform.position + transform.forward;
                    }

                }
                else
                {
                    if (selectedCard != null)
                    {
                        selectedCard.transform.position -= selectedCard.transform.forward * (player.deck.hand.Count * 0.1f);
                        selectedCard = null;

                    }
                }
            }
            else
            {
                if (selectedCard != null)
                {
                    selectedCard.transform.position -= selectedCard.transform.forward * (player.deck.hand.Count * 0.1f);
                    selectedCard = null;
                }

            }
        }
    }
    [ContextMenu("Refresh Hand")]
    public void RefreshHand()
    {
        renderedHand = player.deck.hand;
        foreach (GameObject go in cards)
        {
            Destroy(go);
        }
        cards.Clear();
        hand.Clear();
        var baseAngle = -angleMax / 2;
        var spacing = angleMax / (renderedHand.Count);
        var posSpacing = cardSpread / renderedHand.Count;
        for (int i = 0; i < renderedHand.Count; i++)
        {
            var currentSpace = spacing * (i + 1);
            //Debug.Log("rendering new card");
            var n = Instantiate(cardPrefab);

            //n.GetComponent<ConnorCardController>().card = renderedHand[i];
            n.GetComponent<CardBase>().CardData = renderedHand[i];

            
            //n.transform.rotation = Quaternion.Euler(n.transform.rotation.eulerAngles.x, 180, baseAngle + currentSpace);
            
            n.transform.position = HandPosition + ((Vector3.right) * posSpacing * i) + (n.transform.forward * i * 0.1f);
            var tempPos = n.transform.position; /*<--- var to try to store position*/
            //var tempPos = HandPosition + ((Vector3.right) * posSpacing * i) + (n.transform.forward * i * 0.1f);

            n.transform.SetParent(transform, false);
            n.transform.position = tempPos; /*<---- this was to set it back to old position, works for the most part but leaves cards blurry and overlapping */
            n.transform.LookAt(Camera.main.transform);
            cards.Add(n);
            hand.Add(n, i);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(HandPosition, 0.1f);
    }
}
