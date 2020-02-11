using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        
        gameManager = GameManager.instance;
        
    }
    public IEnumerator EnemyTurn()
    {
        Debug.Log("Enemyのターン");
        // フィールドのカードを取得する
        // フィールドのカードを攻撃可能にする。
        CardController[] enemyFieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        gameManager.SettingCanAttackView(enemyFieldCardList, true);
        /*->関数化
        foreach (CardController Card in enemyFieldCardList)
                {

                    Card.SetCanAttack(true); // cardを攻撃可能にする

                }
        */
        yield return new WaitForSeconds(1);
        /* 場にカードを出す */
        // 手札のカードリストを取得
        CardController[] handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();
        // コスト以下のカードがあれば、カードをフィールドに出し続ける
        // 条件：モンスターカード
        // 条件：スペルならコストと、使用可能かどうか(CanUseSpell)
        while (Array.Exists(handCardList, card => (card.model.cost <= gameManager.enemy.manaCost)
        &&(!card.IsSpell || (card.IsSpell && card.CanUseSpell()))))
        {
            // コスト以下のカードリストを取得
            CardController[] selectablehandCardList = Array.FindAll(handCardList, card => (card.model.cost <= gameManager.enemy.manaCost) && (!card.IsSpell || (card.IsSpell && card.CanUseSpell())));
            // 場に出すカードを選択
            CardController selectCard = selectablehandCardList[0];
            // カードを表にする
            selectCard.Show();
            // スペルカードなら使用する
            if (selectCard.IsSpell)
            {
                StartCoroutine(CastSpellOf(selectCard));
            }
            else
            {
                // カードを移動(Enemy)Field
                StartCoroutine(selectCard.movement.MoveToField(gameManager.enemyFieldTransform));
                selectCard.OnField();
            }
            // 手札のカードリストを更新
            yield return new WaitForSeconds(1);
            handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();
        }

        yield return new WaitForSeconds(1);


        /* 攻撃 */
        // フィールドのカードリストを取得
        CardController[] enemyfieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        // 攻撃可能カードがあれば攻撃を繰り返す
        while (Array.Exists(enemyfieldCardList, card => card.model.canAttack)) // 検索：：Array.FindAll
        {
            // 攻撃可能カードを取得
            CardController[] enemyCanAttackCardList = Array.FindAll(enemyfieldCardList, card => card.model.canAttack); // 検索：：Array.FindAll
            CardController[] playerfieldCardList = gameManager.playerFieldTransform.GetComponentsInChildren<CardController>();
            // attackerカードを選択
            CardController attacker = enemyCanAttackCardList[0];
            /*playercardが存在すればdefenderを指定する*/
            if (playerfieldCardList.Length > 0)
            {
                // defenderカードを選択(Playerフィールドより)
                // シールドカードのみ攻撃対象にする
                if (Array.Exists(playerfieldCardList, card => card.model.ability == ABILITY.SHIELD))
                {
                    playerfieldCardList = Array.FindAll(playerfieldCardList, card => card.model.ability == ABILITY.SHIELD);
                }
                CardController defender = playerfieldCardList[0];
                // attackerとdegfenderを戦わせる
                StartCoroutine(attacker.movement.MoveToTarget(defender.transform));
                yield return new WaitForSeconds(0.51f);
                gameManager.CardsBattle(attacker, defender);

            }
            else
            {
                StartCoroutine(attacker.movement.MoveToTarget(gameManager.playerHero));
                yield return new WaitForSeconds(0.25f);
                gameManager.AttackToHero(attacker);
                yield return new WaitForSeconds(0.25f);
                gameManager.CheckHeroHP();
            }
            enemyfieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
            yield return new WaitForSeconds(1);

        }

        yield return new WaitForSeconds(1);

        gameManager.ChangeTurn();   //Enemyターン終了時は自動でターンを明け渡す
    }

    IEnumerator CastSpellOf(CardController card)
    {
        CardController target = null;
        Transform movePosition = null;
        switch (card.model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
                target = gameManager.GetFriendFieldCards(card.model.isPlayerCard)[0];
                movePosition = target.transform;
                break;
            case SPELL.HEAL_FRIEND_CARD:
                target = gameManager.GetFriendFieldCards(card.model.isPlayerCard)[0];
                movePosition = target.transform;
                break;
            case SPELL.DAMAGE_ENEMY_CARDS:
                movePosition = gameManager.playerFieldTransform;
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                movePosition = gameManager.enemyFieldTransform;
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                movePosition = gameManager.playerHero;
                break;
            case SPELL.HEAL_FRIEND_HERO:
                movePosition = gameManager.enemyHero;
                break;
        }
        /*      
         *      if (card.model.spell == SPELL.HEAL_FRIEND_CARD)
                {
                    target = gameManager.GetFriendFieldCards(card.model.isPlayerCard)[0];
                }
                else if        (card.model.spell == SPELL.DAMAGE_ENEMY_CARD)
                {
                    target = gameManager.GetEnemyFieldCards(card.model.isPlayerCard)[0];

                }
        */
        // 移動先としてターゲット/それぞれのフィールド/それぞれのHeroのTransformが必要
        StartCoroutine(card.movement.MoveToField(movePosition));

        yield return new WaitForSeconds(0.25f);
        card.UseSpellTo(target); // カードを使用したら破壊する
    }
}
