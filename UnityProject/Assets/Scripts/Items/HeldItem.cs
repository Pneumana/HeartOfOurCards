using CardActions;
using Characters;
using DeckData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    int procAmount;
    public ItemBase itemData;

    private void Start()
    {
        
    }


    public void Proc(FieldCardData.ProcType procType, CardData usedCard, GenericBody target, GenericBody user, List<EnemyGenericBody> enemies, List<GenericBody> allies, GenericBody healthPool)
    {
        Debug.Log("processing item proc event " + procType);


            for (int x = 0; x < itemData.procTypeSets.Count; x++)
            {
                if (itemData.procTypeSets[x] == procType)
                {
                    Debug.Log("item " + itemData.name + " proced");
                    //make a fake card with all of the proc effects and Use it?
                    var procCard = gameObject.AddComponent<CardBase>();
                    var procEffect = Instantiate(ScriptableObject.CreateInstance<CardData>());
                    procEffect.energyCost = 0;
                    procEffect.cardActionDataList = itemData.procActionSets[x].procActions;


                    procCard.CardData = procEffect;
                    procCard.Use(user, target, enemies, allies, healthPool);

                    Destroy(procCard);
                }
            }

    }


}
