using Enums;
using Managers;
using Interfaces;
using UnityEngine;

namespace Characters
{
    public abstract class CharacterBase : MonoBehaviour, ICharacter
    {
        [Header("Base settings")]
        [SerializeField] private CharacterType characterType;
        [SerializeField] private Transform textSpawnRoot;


        public CharacterType CharacterType => characterType;
        public Transform TextSpawnRoot => textSpawnRoot;

        public CharacterBase GetCharacterBase()
        {
            return this;
        }

        public CharacterType GetCharacterType()
        {
            return CharacterType;
        }
    }
}