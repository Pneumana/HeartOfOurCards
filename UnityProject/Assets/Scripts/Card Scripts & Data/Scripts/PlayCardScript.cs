//using CardActions;
//using Enums;
//using Managers;
//using UnityEngine;
//using Characters;
//using Interfaces;
//using Managers;

//public class PlayCardScript : MonoBehaviour
//{
//    private CardBase _heldCard;
//    int currentEnergy;
//    protected TurnManager turnManager => TurnManager.instance;

//    private void PlayCard(Vector2 mousePos)
//    {
//        // Use Card
//        var mouseButtonUp = Input.GetMouseButtonUp(0);
//        if (!mouseButtonUp) return;

//        bool backToHand = true;

//        if (currentEnergy >= _heldCard.CardData.EnergyCost)
//        {
//            RaycastHit hit;
//            var mainRay = Camera.main.ScreenPointToRay(mousePos);
//            var _canUse = false;
//            GenericBody selfCharacter = turnManager.CurrentMainAlly;
//            GenericBody targetCharacter = null;

//            _canUse = _heldCard.CardData.UsableWithoutTarget || CheckPlayOnCharacter(mainRay, _canUse, ref selfCharacter, ref targetCharacter);

//            if (_canUse)
//            {
//                backToHand = false;
//                _heldCard.Use(selfCharacter, targetCharacter, turnManager.CurrentEnemiesList, turnManager.CurrentAlliesList);
//            }
//        }

//        //if (backToHand)
//            /*Not sure if necessary but something that would return it to hand here*/

//        _heldCard = null;
//    }

//    private bool CheckPlayOnCharacter(Ray mainRay, bool _canUse, ref GenericBody selfCharacter, ref GenericBody targetCharacter)
//    {
//        RaycastHit hit;
//        if (Physics.Raycast(mainRay, out hit, 1000/*, targetLayer*/))
//        {
//            var character = hit.collider.gameObject.GetComponent<GenericBody>();

//            if (character != null)
//            {
//                var checkEnemy = (_heldCard.CardData.CardActionDataList[0].ActionTargetType == ActionTargetType.Enemy && character.GetCharacterType() == CharacterType.Enemy);
//                var checkAlly = (_heldCard.CardData.CardActionDataList[0].ActionTargetType == ActionTargetType.Ally && character.GetCharacterType() == CharacterType.Ally);

//                if (checkEnemy || checkAlly)
//                {
//                    _canUse = true;
//                    selfCharacter = turnManager.CurrentMainAlly;
//                    targetCharacter = character.GetCharacterBase();
//                }
//            }
//        }

//        return _canUse;
//    }
//}
