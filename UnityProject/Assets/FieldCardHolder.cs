using CardActions;
using Characters;
using DeckData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FieldCardHolder : MonoBehaviour
{
    public static FieldCardHolder instance;
    public List<FieldCardData> currentFieldCards;
    public List<bool> isFieldCardOwnedByPlayers;
    [SerializeField] int fieldCardLimit = 1;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    //pass a proc type into here
    public void Proc(FieldCardData.ProcType procType, CardData usedCard, GenericBody target, GenericBody user, List<EnemyGenericBody> enemies, List<GenericBody> allies, GenericBody healthPool)
    {
        Debug.Log("processing proc event " + procType);

        for (int i = 0; i < currentFieldCards.Count; i++)
        {
            for(int x = 0; x < currentFieldCards[i].procTypeSets.Count; x++)
            {
                if(currentFieldCards[i].procTypeSets[x] == procType)
                {
                    Debug.Log("field card " + currentFieldCards[i].name + " proced");
                    //make a fake card with all of the proc effects and Use it?
                    var procCard = gameObject.AddComponent<CardBase>();
                    var procEffect = Instantiate(ScriptableObject.CreateInstance<CardData>());
                    procEffect.energyCost = 0;
                    procEffect.cardActionDataList = currentFieldCards[i].procActionSets[x].procActions;


                    procCard.CardData = procEffect;
                    procCard.Use(user, target, enemies, allies, healthPool);

                    Destroy(procCard);
                }
            }
/*            if (currentFieldCards[i].procTypeSets.Contains(procType))
            {
                
                

            }*/
        }

    }

    public void AddFieldCard(FieldCardData cardData, bool isPlayerOwned)
    {
        Debug.Log("added field card " + cardData);
        if (!currentFieldCards.Contains(cardData))
        {
            if (currentFieldCards.Count >= fieldCardLimit)
            {
                currentFieldCards.RemoveAt(0);
                isFieldCardOwnedByPlayers.RemoveAt(0);
            }
            currentFieldCards.Add(cardData);
            isFieldCardOwnedByPlayers.Add(isPlayerOwned);
        }
        else
        {
            //overwrite existing card 
            for(int i = 0; i < currentFieldCards.Count; i++)
            {

            }
        }
        GetComponent<TextMeshPro>().text = cardData.name;
    }
}
