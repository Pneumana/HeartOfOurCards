using CardActions;
using DeckData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckViewer : MonoBehaviour
{
    List<CardData> cards = new List<CardData>();
    public List<GameObject> renderedHand = new List<GameObject>();
    public Dictionary<GameObject, int> hand = new Dictionary<GameObject, int>();

    public GameObject cardPrefab;

    bool Displayed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //onClick thing to raycast

        //see if hit deck object,

        //turn on if clicked

        //clicking again closes the deck viewer.
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray;
            RaycastHit hit;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log("deck viewer clicked");
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    Debug.Log(hit.collider.gameObject.name);
                    if (hit.collider.gameObject.name == "Player1Deck")
                    {
                        Debug.Log("displaying player 1 deck");
                        //set cards to player1 deck
                        cards.Clear();
                        cards = AmbidexterousManager.Instance.PlayerList[0].combatScene.transform.Find("Player1").GetComponent<CardDeck>().deck;

                        RefreshHand();
                    }
                    else if (hit.collider.gameObject.name == "Player1Discard")
                    {
                        Debug.Log("displaying player 1 discard");
                        cards.Clear();
                        cards = AmbidexterousManager.Instance.PlayerList[0].combatScene.transform.Find("Player1").GetComponent<CardDeck>().discardPile;

                        RefreshHand();
                        //set cards to player1 discard
                    }
                }
                else
                {
                    Debug.Log("deck viewer raycast hit nothing");
                }
            }
        }        
    }

    void UpdateCards()
    {
        if (Displayed)
        {

        }
    }

    public void RefreshHand()
    {
        //hand = cards;
        foreach (GameObject go in renderedHand)
        {
            Destroy(go);
        }
        renderedHand.Clear();
        hand.Clear();
        //var baseAngle = -angleMax / 2;
        //var spacing = angleMax / (renderedHand.Count);
        for (int i = 0; i < cards.Count; i++)
        {
            var currentSpace = 0.1f * (i + 1);
            //Debug.Log("rendering new card");
            var n = Instantiate(cardPrefab);

            //n.GetComponent<ConnorCardController>().card = renderedHand[i];
            n.GetComponent<CardBase>().CardData = cards[i];

            n.transform.forward = -Camera.main.transform.forward;
            //n.transform.rotation = Quaternion.Euler(n.transform.rotation.eulerAngles.x, 180, 45 + currentSpace);
            n.transform.position = new Vector3(i - (Mathf.FloorToInt(i / 3)*3), Mathf.FloorToInt(i/3));
            n.transform.SetParent(transform);
            renderedHand.Add(n);
            hand.Add(n, i);
        }

    }
}
