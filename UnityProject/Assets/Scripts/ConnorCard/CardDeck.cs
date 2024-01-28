using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    public List<ConnorCard> hand = new List<ConnorCard>();
    public List<ConnorCard> deck = new List<ConnorCard> ();
    public List<ConnorCard> discardPile = new List<ConnorCard>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DrawCard();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayCard(transform.forward);
        }
    }
    public void DrawCard(int drawcount = 1)
    {
        while(drawcount > 0)
        {
            if (deck.Count == 0)
            {
                if (discardPile.Count > 0)
                {
                    var hold = discardPile.Count;
                    //deck = hold;
                    for (int i = 0; i < hold; i++)
                    {
                        deck.Add(discardPile[i]);
                    }
                    //discardPile.Clear();
                    Debug.Log("shuffling deck");
                    discardPile.Clear();
                }
                else
                {
                    Debug.Log("deck is empty");
                    return;
                }
            }
            else
            {
                var drawnCard = Random.Range(0, deck.Count - 1);
                hand.Add(deck[drawnCard]);
                deck.Remove(deck[drawnCard]);
                Debug.Log("Drew card " + hand[hand.Count - 1].cardName);
            }
            drawcount--;
        }
        

    }
    public void ServerPlayCard(int netID, Vector3 target, int playedCard = -1)
    {
        //run play card on all clients, checking for netID match so the same player uses them
    }
    public void PlayCard(Vector3 target, int playedCard = -1)
    {
        //if it doesn't meet the energy requirment, uhh yeah
        if (hand.Count == 0)
        {
            Debug.Log("hand is empty");
            return;
        }
        if(playedCard==-1)
            playedCard = Random.Range(0, hand.Count - 1);
        foreach(GameObject proj in hand[playedCard].attackPrefab)
        {
            var attack = Instantiate(proj);
            attack.transform.position = transform.position + (transform.forward * 2);
            attack.transform.forward = new Vector3(target.x, attack.transform.position.y, target.z) - attack.transform.position;
            attack.GetComponent<Rigidbody>().AddForce(attack.transform.forward * 10,ForceMode.Impulse);
        }
        discardPile.Add(hand[playedCard]);
        Debug.Log("Played card " + hand[playedCard].cardName);
        hand.Remove(hand[playedCard]);

    }
}
