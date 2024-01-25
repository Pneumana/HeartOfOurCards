using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public CardPlayerController player;

    public Vector3 HandPosition;

    List<ConnorCard> renderedHand = new List<ConnorCard>();
    List<GameObject> cards = new List<GameObject> ();

    //this should only do stuff for local player, probably

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            //add a condition to make this only update when a card is drawn
            renderedHand = player.deck.hand;
            foreach(GameObject go in cards)
            {
                Destroy(go);
            }
            cards.Clear();
            for(int i = 0; i<renderedHand.Count; i++)
            {
                Debug.Log("rendering new card");
                var n = Instantiate(cardPrefab);
                n.transform.position = HandPosition + (new Vector3(0.1f, 0.1f, 0) * i);
                n.GetComponent<ConnorCardController>().card = renderedHand[i];
                cards.Add(n);
            }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(HandPosition, 0.1f);
    }
}
