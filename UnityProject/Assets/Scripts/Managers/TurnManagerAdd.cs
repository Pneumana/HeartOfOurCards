using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Managers
{
    public class TurnManagerAdd : MonoBehaviour
    {
        private TurnManagerAdd() { }
        public static TurnManagerAdd Instance { get; private set; }

        [Header("References")]
        [SerializeField] private List<Transform> enemyPosList;
        [SerializeField] private List<Transform> allyPosList;

        public List<PlayerGenericBody> CurrentAlliesList { get; private set; } = new List<PlayerGenericBody>();
        public List<GenericBody> CurrentEnemiesList { get; private set; } = new List<GenericBody>();

        public PlayerGenericBody CurrentMainAlly => CurrentAlliesList.Count > 0 ? CurrentAlliesList[0] : null;

    }
}