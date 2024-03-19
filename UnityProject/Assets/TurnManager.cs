using Characters;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class TurnManager : NetworkBehaviour
    {
        public List<CardPlayerController> playerTeam = new List<CardPlayerController>();
        public List<CardEnemyController> enemyTeam = new List<CardEnemyController>();

        public List<CardPlayerController> turnEnded = new List<CardPlayerController>();
        public List<CardEnemyController> enemyTurnEnded = new List<CardEnemyController>();


        public bool isPlayerTurn;

        public int endedTurns;

        public static TurnManager instance;

        /*[Header("References")]
        [SerializeField] private List<Transform> enemyPosList;
        [SerializeField] private List<Transform> allyPosList;*/

        public List<GenericBody> CurrentAlliesList  = new List<GenericBody>();
        public List<EnemyGenericBody> CurrentEnemiesList = new List<EnemyGenericBody>();

        public PlayerGenericBody CurrentMainAlly;

        [SerializeField] GameObject mapScroll;
        Vector3 mapStartPos;

        GameObject victoryUI;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            //SceneManager.sceneLoaded += this.OnLoadCallback;
            Invoke("OnLoadCallback", 0.5f);
            mapStartPos = mapScroll.transform.position;

            victoryUI = GameObject.Find("VictoryUI");
            victoryUI.SetActive(false);
        }
        private void OnDisable()
        {
            //SceneManager.sceneLoaded -= this.OnLoadCallback;
        }
        void OnLoadCallback()
        {

            CurrentMainAlly = GameObject.Find("Health Pool").GetComponent<PlayerGenericBody>();
/*            RunManager.instance.Health = (20 + (RunManager.instance.playerStatList[0].CON * 2) + (RunManager.instance.playerStatList[1].CON * 2));
            CurrentMainAlly.health = RunManager.instance.Health;
            CurrentMainAlly.OnHealthChanged?.Invoke(CurrentMainAlly.health, CurrentMainAlly.maxHealth);*/
            Debug.Log("scene load callback");
            foreach (CardPlayerController player in FindObjectsByType<CardPlayerController>(FindObjectsSortMode.None))
            {
                Debug.Log(player.gameObject);
                player.CMDStartEncounter();
                if (player != null)
                {
                    playerTeam.Add(player);
                    CurrentAlliesList.Add(player.GetComponent<GenericBody>());
                    try
                    {
                        //if(!isServerOnly)
                        
                        player.gameObject.GetComponent<HandManager>().RefreshHand();
                    }
                    catch { }
                }
            }
            foreach (PlayerGenericBody player in FindObjectsByType<PlayerGenericBody>(FindObjectsSortMode.None))
            {
                Debug.Log(player.gameObject);
                if (player != null)
                {
                    
                }

            }
            foreach (CardEnemyController enemy in FindObjectsByType<CardEnemyController>(FindObjectsSortMode.None))
            {
                Debug.Log("added new enemy: " + enemy.gameObject);
                if (!enemyTeam.Contains(enemy))
                {
                    var enemyGB = enemy.GetComponent<EnemyGenericBody>();
                    if(!CurrentEnemiesList.Contains(enemyGB))
                        CurrentEnemiesList.Add(enemyGB);
                    Debug.Log("adding " + enemy.gameObject.name + " to enemy team. It's genericbody has " + enemyGB.health + " health");
                    enemyTeam.Add(enemy);
                    enemy.deck = enemy.GetComponent<CardDeck>();
                    //enemy.deck.ServerDrawCard(1);
                    enemy.FirstCardDraw(0);
                    //enemy.ServerDisplayEnemyCard();
                }
                else
                {
                    Debug.Log("enemy is already in the enemyTeam list");
                    var enemyGB = enemy.GetComponent<EnemyGenericBody>();
                    if (!CurrentEnemiesList.Contains(enemyGB))
                        CurrentEnemiesList.Add(enemyGB);
                }
            }
        }
        //enemy plays card(s), waits about 0.5s then the next enemy takes it's turn
        private void Update()
        {

            /*if(FindObjectsByType<CardPlayerController>(FindObjectsSortMode.None).Length != playerTeam.Count)
            {
                foreach (CardPlayerController player in FindObjectsByType<CardPlayerController>(FindObjectsSortMode.None))
                {
                    playerTeam.Add(player);
                }
            }*/
        }
        [Command(requiresAuthority = false)]
        public void ServerPlayerEndTurn(uint netID)
        {
            Debug.Log("ended turn on server");
            PlayerEndTurn(netID);
        }
        [ClientRpc]
        public void PlayerEndTurn(uint netID)
        {
            Debug.Log("ended turn on client rpc");
            //CardPlayerController turnEnder = null;
            //find object with netID
            foreach (NetworkIdentity netid in FindObjectsByType<NetworkIdentity>(FindObjectsSortMode.None))
            {
                if (netid.netId == netID)
                {

                    Debug.Log("found netID " + netid.name);
                    //turnEnder = netid.gameObject.GetComponent<CardPlayerController>();
                    var localPlayerControllers = netid.gameObject.GetComponentsInChildren<CardPlayerController>();
                    Debug.Log("matched player has " + localPlayerControllers.Length + " player controllers");
                    foreach (CardPlayerController turnEnder in localPlayerControllers)
                    {
                        Debug.Log("looping though controllers");
                        if (isPlayerTurn)
                        {
                            if (playerTeam.Contains(turnEnder) && !turnEnded.Contains(turnEnder))
                            {
                                //Debug.Log(turnEnder.gameObject.name + " ended turn");
                                turnEnder.deck.ServerDiscard(turnEnder.deck.hand.Count, new int[0]);
                                turnEnded.Add(turnEnder);
                            }
                        }
                    }
                    //Debug.Log(netid.gameObject.name + " is DONE");
                }
            }
            if (turnEnded.Count == playerTeam.Count)
            {
                isPlayerTurn = false;
                turnEnded.Clear();
                foreach (IndividualPlayerGenericBody player in CurrentAlliesList)
                {
                    player.OnPlayerTurnEnd();
                }
                CurrentMainAlly.OnPlayerTurnEnd();
                Debug.Log("player turn ended");
                if(isServer)
                    ServerStartEnemyTurns();
            }
            /*if (isPlayerTurn)
            {
                if (playerTeam.Contains(turnEnder) && !turnEnded.Contains(turnEnder))
                {
                    //Debug.Log(turnEnder.gameObject.name + " ended turn");
                    turnEnded.Add(turnEnder);
                }
                if(turnEnded.Count == playerTeam.Count)
                {
                    isPlayerTurn = false;
                    turnEnded.Clear();
                    Debug.Log("player turn ended");
                    ServerStartEnemyTurns();
                }
            }*/

        }
        public void EnemyEndTurn(CardEnemyController turnEnder)
        {
            if (!isPlayerTurn)
            {
                if (enemyTeam.Contains(turnEnder) && !enemyTurnEnded.Contains(turnEnder))
                {
                    Debug.Log(turnEnder.gameObject.name + " ended turn");
                    enemyTurnEnded.Add(turnEnder);
                }
                if (enemyTurnEnded.Count == enemyTeam.Count)
                {
                    Invoke("KickstartPlayerTurns", 1f);
                }
            }
        }

        void KickstartPlayerTurns()
        {
            isPlayerTurn = true;
            enemyTurnEnded.Clear();
            Debug.Log("enemy turn ended");
            CurrentMainAlly.OnPlayerTurnStart();
            foreach (CardPlayerController plr in playerTeam)
            {
                //if(isClient)
                    plr.CMDStartTurn();
            }
        }

        [Command(requiresAuthority = false)]
        void ServerStartEnemyTurns()
        {
            //StartCoroutine(EnemyTurnLoop());

            StartCoroutine(EnemyTurnLoop());

            //ClientStartEnemyTurns();
        }
/*        [ClientRpc]
        void ClientStartEnemyTurns()
        {
            if(isServer)
                StartCoroutine(EnemyTurnLoop());
        }*/
        IEnumerator EnemyTurnLoop()
        {
            int enemyLoopIndex = 0;
            int defeated = 0;
            do
            {
                if (CurrentEnemiesList[enemyLoopIndex].health <= 0)
                {
                    defeated++;
                    CurrentEnemiesList[enemyLoopIndex].OnEnemyTurnStart();
                    enemyTeam[enemyLoopIndex].TakeTurn();
                    enemyLoopIndex++;
                    continue;
                }
                    CurrentEnemiesList[enemyLoopIndex].OnEnemyTurnStart();
                    yield return new WaitForSeconds(0.5f);
                    enemyTeam[enemyLoopIndex].TakeTurn();
                    //CurrentEnemiesList[enemyLoopIndex].OnEnemyTurnStart();
                enemyLoopIndex++;
            }
            while (enemyLoopIndex < enemyTeam.Count);

            if (defeated == enemyTeam.Count)
            {
                Debug.Log("won!");
                StartCoroutine(BringUpScroll());
                WinEncounter();
            }

            yield return null;
        }
        [Command(requiresAuthority =false)]
        public void CMDGetEnemyList()
        {
            //Invoke("DisplayEnemyCards", 0.1f);
            //GetEnemyList();
            foreach (CardEnemyController enemy in FindObjectsByType<CardEnemyController>(FindObjectsSortMode.None))
            {
                Debug.Log("added new enemy: " + enemy.gameObject);
                if (enemy != null)
                {
                    //enemyTeam.Add(enemy);
                    //enemy.deck = enemy.GetComponent<CardDeck>();
                    //enemy.deck.ServerDrawCard(1);
                    //enemy.FirstCardDraw(0);
                    //enemy.ServerDisplayEnemyCard();
                }

            }
            DisplayEnemyCards();
            //Command();
        }
        public void CheckWinCondition()
        {
            var defeated = 0;
            for(int i = 0; i < enemyTeam.Count; i++)
            {
                if(CurrentEnemiesList[i].health <= 0)
                {
                    defeated++;
                }
            }
            if(defeated == enemyTeam.Count)
            {
                StartCoroutine(BringUpScroll());
                WinEncounter();
            }
        }
        IEnumerator BringUpScroll()
        {
            float anim = 0;
            do
            {
                anim += Time.deltaTime;
                mapScroll.transform.position = Vector3.Lerp(mapStartPos, mapStartPos + mapScroll.transform.up * 6.5f, anim);
                yield return new WaitForSeconds(0);
            }
            while (anim < 1);
            yield return null;
        }
        void WinEncounter()
        {
            foreach (CardPlayerController plr in playerTeam)
            {
                ServerPlayerEndTurn(plr.netId);
            }

            GameObject.FindFirstObjectByType<MapCamera>().CenterCamera();
            Random.InitState(RunManager.instance.seed + RunManager.instance.completedRooms.Count);
            var gold = Random.Range(25, 50);
            victoryUI.SetActive(true);
            victoryUI.transform.Find("GoldEarned").gameObject.GetComponent<TextMeshProUGUI>().text = "+" + gold + " gold to both players!";
            //show a gold counter on each side
            //var plr1 = RunManager.instance.playerStatList[0];
            if (!isServer)
                return;

            //plr1.Gold += gold;


            //plr2.Gold += gold;


            var plr1 = RunManager.instance.playerStatList[0];
            var plr2 = RunManager.instance.playerStatList[1];
            Debug.Log("player1 has " + RunManager.instance.playerStatList[0].Gold + " player2 has gold " + RunManager.instance.playerStatList[1].Gold);
            RunManager.instance.playerStatList[0] = new RunManager.PlayerStats(plr1.DMG, plr1.INT, plr1.NRG, plr1.CON,
                plr1.Kitsune, plr1.Lich, plr1.Naga, plr1.Mermaid, plr1.Dragon, plr1.Vampire, plr1.Producer,
                plr1.Gold + gold, plr1.RepDMG, plr1.RepINT, plr1.RepNRG, plr1.RepCON, plr1.usedBreak);
            RunManager.instance.playerStatList[1] = new RunManager.PlayerStats(plr2.DMG, plr2.INT, plr2.NRG, plr2.CON,
                plr2.Kitsune, plr2.Lich, plr2.Naga, plr2.Mermaid, plr2.Dragon, plr2.Vampire, plr2.Producer,
                plr2.Gold + gold, plr2.RepDMG, plr2.RepINT, plr2.RepNRG, plr2.RepCON, plr2.usedBreak);
            Debug.Log("player1 has " + RunManager.instance.playerStatList[0].Gold + " player2 has gold " + RunManager.instance.playerStatList[1].Gold);
            RunManager.instance.Ratings += 10;

            if (RunManager.instance.fightingBoss)
            {
                RunManager.instance.fightingBoss = false;
                AmbidexterousManager.Instance.ChangeScene("TownScene");
            }

            //RunManager.instance.playerStatList[1].Gold = gold;
        }

        [ClientRpc]
        void DisplayEnemyCards()
        {
            Debug.Log("command GetEnemyList");
            //GetEnemyList();
            Debug.Log("ClientRPC GetEnemyList");
            foreach (CardEnemyController enemy in FindObjectsByType<CardEnemyController>(FindObjectsSortMode.None))
            {
                Debug.Log("added new enemy: " + enemy.gameObject);
                if (enemy != null)
                {
                    enemyTeam.Add(enemy);
                    enemy.deck = enemy.GetComponent<CardDeck>();
                    //enemy.deck.ServerDrawCard(1);
                    enemy.FirstCardDraw(0);
                    //enemy.ServerDisplayEnemyCard();
                }

            }
        }
       
        /*[ClientRpc]
        public void GetEnemyList()
        {
            Debug.Log("ClientRPC GetEnemyList");
            foreach (CardEnemyController enemy in FindObjectsByType<CardEnemyController>(FindObjectsSortMode.None))
            {
                Debug.Log("added new enemy: " + enemy.gameObject);
                if (enemy != null)
                {
                    enemyTeam.Add(enemy);
                    enemy.deck = enemy.GetComponent<CardDeck>();
                    //enemy.deck.ServerDrawCard(1);
                    enemy.FirstCardDraw(0);
                    enemy.ServerDisplayEnemyCard();
                }

            }
        }*/
    }
}
