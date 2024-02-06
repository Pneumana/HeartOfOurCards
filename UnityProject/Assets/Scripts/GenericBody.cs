using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Characters
{
    public class GenericBody : MonoBehaviour
    {
        [Header("Info")]
        [SerializeField] private CharacterType characterType;
        [Header("Health")]
        public int health;
        public int maxHealth;

        public RunManager.PlayerStats stats;

        [Space(10)]
        [Header("Buffs/Debuffs")]
        public int shield;
        public int burn;
        public int freeze;
        public int vulnerable;
        public int stun;
        public int stength;
        public int dexterity;
        public int regen;
        public int bleed;

        public CharacterType CharacterType => characterType;
        //any % increases should use CeilToInt() so it all stays as ints

        //replace gameobject with netID
        public void TakeDamage(int damageRecieved)
        {
            Debug.Log("body taking " + damageRecieved + " damage");
            var damageToTake = damageRecieved;
            var shieldThisTurn = shield;
            //shield damage block
            if (vulnerable > 0)
            {
                damageToTake = Mathf.CeilToInt(damageToTake * 1.5f);
            }
            while (shieldThisTurn > 0 && damageToTake > 0)
            {
                shieldThisTurn--;
                damageToTake--;
            }
            health -= damageToTake;

        }

        public void HealDamage(int healRecieved)
        {
            health += healRecieved;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }

        public void GainShield(int shieldRecieved)
        {
            shield += shieldRecieved;
        }

        public void OnTurnStart()
        {
            shield = 0;

            if (regen > 0)
            {
                HealDamage(regen);
                regen--;
            }

            if (burn > 0)
            {
                TakeDamage(burn);
                burn--;
            }

            if (bleed > 0)
            {
                TakeDamage(bleed);
                bleed--;
            }

            if (freeze > 0)
            {
                freeze--;
            }

            if (vulnerable > 0)
            {
                vulnerable--;
            }
        }

        public CharacterType GetCharacterType()
        {
            return CharacterType;
        }

        public GenericBody GetCharacterBase()
        {
            return this;
        }
    }
}
