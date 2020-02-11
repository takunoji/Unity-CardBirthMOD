using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public  GamePlayerManager player;
    public  GamePlayerManager enemy;
    [SerializeField] AI enemyAI;
    [SerializeField] UIManager uiManager;

    // 手札にカードを生成

    public Transform playerHandTransform,
                                playerFieldTransform,
                                enemyHandTransform,
                                enemyFieldTransform;
    [SerializeField] CardController cardPrefab;
    public bool isPlayerTurn;

    public Transform playerHero;
    public Transform enemyHero;


    // 時間管理
    int timeCount;

    // シングルトン化（どこからでもアクセスできるようにする）
    public static GameManager instance;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        uiManager.HideResultPanel();
        player.Init(new List<int>() { 9, 6,4 , 1, 2, 3,1,2,3 });
        enemy.Init(new List<int>() { 1,9, 8, 7, 6,            5, 4,3 });
        uiManager.ShowHeroHP(player.heroHp, enemy.heroHp);
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        SettingInitHand();  // 手札の初回ドロー
        isPlayerTurn = true;
        TurnCalc();
    }

    public void ReduceManaCost(int cost,bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            player.manaCost -= cost;
        }
        else
        {
            enemy.manaCost -= cost;
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
    }

    public void Restart()
    {
        // hand と　Fieldのカードを削除
        foreach (Transform card in playerHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in playerFieldTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyFieldTransform)
        {
            Destroy(card.gameObject);
        }

        // デッキを再生成
        player.deck = new List<int>() { 3, 4, 5, 1, 2, 3 };
        enemy.deck = new List<int>() { 2, 1, 3, 1, 3, 2 };

        StartGame();
    }

    void SettingInitHand()
    {
        // カードをそれぞれに3まい配る
        for (int i = 0; i < 3; i++)
        {
            //            CreateCard(playerHandTransform);
            //            CreateCard(enemyHandTransform);
            GiveCardToHand(player.deck, playerHandTransform);
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }

    }

    // Deckからhandにカードを取得する
    void GiveCardToHand(List<int> deck,Transform hand)
    {
        if (0 == deck.Count)
        {
            return;
        }
        int cardID = deck[0];   // deckの先頭を取得
        deck.RemoveAt(0);       // deckの先頭を削除
        CreateCard(cardID, hand);
    }

    void CreateCard(int cardID,Transform hand)
    {
        // カードの生成とデータの受け渡し
        CardController card = Instantiate(cardPrefab, hand, false);    //Prefab,カード位置（親要素）、
        if (hand.name == "PlayerHand")
        {
            card.Init(cardID,true);   // カードの種類を決める
        }
        else
        {
            card.Init(cardID, false);   // カードの種類を決める

        }
    }

    void TurnCalc()
    {
        StopAllCoroutines();
        StartCoroutine(CountDown());
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            StartCoroutine(enemyAI.EnemyTurn());
        }
    }

    IEnumerator CountDown()
    {
        timeCount = 20;
        uiManager.UpdateTime(timeCount);

        while (timeCount>0)
        {
            yield return new WaitForSeconds(1); // 1s待機
            timeCount--;
            uiManager.UpdateTime(timeCount);
        }
        ChangeTurn();
    }


    public void TurnEnd()
    {

    }

    public CardController[] GetEnemyFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();

        }
        else
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();

        }
    }


    public CardController[] GetFriendFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
    }

    /* -> 相手TrunにTrunEndButtonが押せる問題の対策 */
    public void OnClickTurnEndButton()
    {
        if (isPlayerTurn)
        {
            ChangeTurn();
        }
    }

    public void ChangeTurn()
    {
        // フィールドのカードを攻撃不可にする。
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, false);
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList, false);

        isPlayerTurn = !isPlayerTurn;
        // 初めてのターンではドローしない
        if (isPlayerTurn)
        {
            player.IncreaseManaCost();
            GiveCardToHand(player.deck, playerHandTransform);
        }
        else
        {
            enemy.IncreaseManaCost();
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
        TurnCalc();
    }
    
    public void SettingCanAttackView(CardController[] fieldCardlist, bool canAttack)
    {
        // フィールドのカードを攻撃可能にする。
        foreach (CardController card in fieldCardlist)
        {
            card.SetCanAttack(canAttack);
        }
    }

    void PlayerTurn()
    {
//        CreateCard(playerHandTransform);
        Debug.Log("Playerのターン");
        // フィールドのカードを攻撃可能にする。
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, true);
        /*-> 関数化
        foreach(CardController Card in playerFieldCardList)
        {
            
            Card.SetCanAttack(true); // cardを攻撃可能にする

        }
        */
    }

    public void CardsBattle(CardController attacker,CardController defender)
    {
        Debug.Log("CardsBattle");
        Debug.Log("Attacker HP:" + attacker.model.hp);
        Debug.Log("Defender HP:" + defender.model.hp);
        attacker.Attack(defender);
        defender.Attack(attacker);
        Debug.Log("Attacker HP:" + attacker.model.hp);
        Debug.Log("Defender HP:" + defender.model.hp);
        attacker.CheckAlive();
        defender.CheckAlive();
    }

    public void HealToHero(CardController healer)
    {
        Debug.Log("Heroに回復");
        if (healer.model.isPlayerCard)
        {
            player.heroHp += healer.model.at;
        }
        else
        {
            enemy.heroHp += healer.model.at;
        }
        healer.SetCanAttack(false);
        uiManager.ShowHeroHP(player.heroHp, enemy.heroHp);
        //        CheckHeroHP();
    }

    public void AttackToHero(CardController attacker)
    {
        Debug.Log("Heroに攻撃");
        if (attacker.model.isPlayerCard)
        {
            enemy.heroHp -= attacker.model.at;
        }
        else
        {
            player.heroHp -= attacker.model.at;
        }
        attacker.SetCanAttack(false);
        uiManager.ShowHeroHP(player.heroHp, enemy.heroHp);
//        CheckHeroHP();
    }


    public void CheckHeroHP()
    {
        if (player.heroHp <= 0 || enemy.heroHp <= 0)
        {
            ShowResultPanel(player.heroHp);
        }
    }
    void ShowResultPanel(int herohp)
    {
        StopAllCoroutines();
        uiManager.ShowResultPanel(herohp);

    }
}
