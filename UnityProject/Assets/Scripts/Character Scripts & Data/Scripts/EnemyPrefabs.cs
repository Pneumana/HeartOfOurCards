using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    [CreateAssetMenu(fileName = "Enemy Encounters", menuName = "Card Stuff/Enemy Encounters", order = 0)]
    public class EnemyPrefabs : ScriptableObject
    {
        [SerializeField] private List<GameObject> enemyEncounters;
    }
}
