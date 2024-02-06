using Characters;
using Enums;

namespace Interfaces
{
    public interface ICharacter
    {
        public CharacterBase GetCharacterBase();
        public CharacterType GetCharacterType();
    }
}