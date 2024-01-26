using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public CardPlayerController player;

    public GameObject selectedCard;

    public Vector3 HandPosition;
    static float cardSpread = 0.1f;
    static float angleMax = 90;
    //define bounds here and divide between hand size

    List<ConnorCard> renderedHand = new List<ConnorCard>();
    public List<GameObject> cards = new List<GameObject> ();

    //this should only do stuff for local player, probably

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //add a condition to make this only update when a card is drawn

        //raycast to world from mouse position.
        //if result is one of the renderedHand cards, and the player clicks, play that card
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
                        Debug.Log("cursor is over a card");
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
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (selectedCard != null)
            {
                //discover the hand id
                int playedCardIndex = 0;
                for(int i = 0; i > cards.Count; i++)
                {
                    if (cards[i] == selectedCard)
                    {
                        playedCardIndex = i;
                        break;
                    }
                }
                player.deck.PlayCard(playedCardIndex);
                RefreshHand();
            }
        }
    }
    public void RefreshHand()
    {
        renderedHand = player.deck.hand;
        foreach (GameObject go in cards)
        {
            Destroy(go);
        }
        cards.Clear();
        for (int i = 0; i < renderedHand.Count; i++)
        {
            //Debug.Log("rendering new card");
            var n = Instantiate(cardPrefab);
            n.transform.position = HandPosition + (new Vector3(0.1f, 0.1f, 0) * i);
            n.GetComponent<ConnorCardController>().card = renderedHand[i];
            n.transform.LookAt(Camera.main.transform.position);
            cards.Add(n);
        }
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(HandPosition, 0.1f);
    }
}
