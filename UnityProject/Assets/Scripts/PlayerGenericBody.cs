using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters 
{
    public class PlayerGenericBody : GenericBody
    {
        [Header("Health")]
/*        public int health;
        public int maxHealth;*/
        [Space(10)]
        [Header("Buffs/Debuffs")]
/*        public int shield;
        public int burn;
        public int freeze;
        public int vulnerable;
        public int stun;
        public int regen;
        public int bleed;*/


        public RunManager RM;

        private void Start()
        {
            RM = GetComponent<RunManager>();
            maxHealth = ((RM.player1Stats.CON * 2) + (RM.player2Stats.CON * 2));
        }

        public void PlayerTakeDamage(int damageRecieved)
        {
            var damageToTake = damageRecieved;
            var shieldThisTurn = shield;
            //shield damage block
            if(damageRecieved > 0)
            {
                while (shieldThisTurn > 0 && damageToTake > 0)
                {
                    shieldThisTurn--;
                    damageToTake--;
                }
            }

            health -= damageToTake;
            health = Mathf.Clamp(health, 0, maxHealth);
            //sync damage here
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
    }
}
