using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReadEnergyFromPlayer : MonoBehaviour
{
    public CardPlayerController playerController;
    TextMeshProUGUI tmp;
    bool animating;
    // Start is called before the first frame update
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController!=null)
            tmp.text = playerController.currentEnergy + "/" + playerController.maxEnergy;
    }
    public void OOM()
    {
            if (!animating)
                StartCoroutine(FlashRed());
    }
    IEnumerator FlashRed()
    {
        animating = true;
        float gb = 0;
        do
        {
            gb += Time.deltaTime;
            tmp.color = new Color(1, gb, gb);
            yield return new WaitForSeconds(0);
        }
        while (gb<=1);
        animating = false;
        yield return null;
    }
}
