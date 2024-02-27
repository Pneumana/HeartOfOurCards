using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;
    public Image damageRecieved;
    int healthBeforeDamage;
    public float endInstances;
    public GameObject currentDamageRecievedObj;

    float damageShake;

    // Start is called before the first frame update
    public void OnEnable()
    {
        var RM = RunManager.instance;
        maxHealth = ((RM.playerStatList[0].CON * 2) + (RM.playerStatList[1].CON * 2));
        health = RM.Health;
        healthBeforeDamage = health;
        ChangeHealth(0);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeHealth(-5);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            ChangeHealth(1);
        }*/
        if(endInstances > 0)
        {
            endInstances -= Time.unscaledDeltaTime;
        }
        else
        {
            healthBeforeDamage = health;
            if (currentDamageRecievedObj != null)
            {
                StartCoroutine(AnimateDamageChunk(currentDamageRecievedObj));
                currentDamageRecievedObj = null;
            }
        }
        damageShake = Mathf.MoveTowards(damageShake, 0, Time.deltaTime * 50);
        transform.rotation = Quaternion.Euler(0, 0, damageShake);
    }
    public void GetHealthChange(int currentHealth, int maxHealth)
    {
        var diff = currentHealth - health;
        ChangeHealth(diff);
    }
    void ChangeHealth(int change)
    {
        if (change < 0)
        {
            damageShake = /*returns -1 or 1*/((Random.Range(0, 2) * -2) + 1) * 5;
        }
        health += change;
        health = Mathf.Clamp(health, 0, maxHealth);
        var percent = (float)health / maxHealth;
        var percentLost = (float)healthBeforeDamage / maxHealth;
        if (currentDamageRecievedObj == null && percentLost > percent)
        {
            
            currentDamageRecievedObj = Instantiate(damageRecieved.gameObject);
            currentDamageRecievedObj.transform.SetParent(transform, false);
            currentDamageRecievedObj.transform.position = Vector3.zero;
            currentDamageRecievedObj.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);
            currentDamageRecievedObj.SetActive(true);
        }
        GetComponent<Image>().material.SetFloat("_fillAmount", percent);
        if (currentDamageRecievedObj != null)
        {
            currentDamageRecievedObj.GetComponent<RectTransform>().anchoredPosition = new Vector2((percentLost - 1) * GetComponent<RectTransform>().sizeDelta.x, 0);
            currentDamageRecievedObj.GetComponent<RectTransform>().sizeDelta = new Vector2(((percentLost) - percent) * GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);
            endInstances = 0.25f;
        }

        //currentDamageRecievedObj.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
    IEnumerator AnimateDamageChunk(GameObject dr)
    {
        float animStep = 0;
        var rect = dr.GetComponent<RectTransform>();
        var startSize = rect.sizeDelta.y;
        do
        {
            animStep += Time.deltaTime * 4;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, startSize + animStep * 16);
            var oneMinus = 1 - animStep;
            dr.GetComponent<Image>().color = new Color(1, 1, 1, oneMinus);
            yield return new WaitForSeconds(0);
        } while (animStep < 1);
        Destroy(dr);
        yield return null;
    }
}
