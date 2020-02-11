using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    CardView view;  // 見かけに関することを操作（view）
    public CardModel model; // データに関することを操作（model）
    public CardMovement movement;  // 移動(movement)に関することを操作

    GameManager gameManager;

    public bool IsSpell
    {
        get { return model.spell != SPELL.NONE; }
    }

    private void Awake()
    {
        view = GetComponent<CardView>();
        movement = GetComponent<CardMovement>();
        gameManager = GameManager.instance;
    }
    public void Init(int cardID,bool isPlayer)
    {
        model = new CardModel(cardID, isPlayer);
        view.SetCard(model);
    }

    public void Attack(CardController enemyCard)
    {
        model.Attack(enemyCard);
        SetCanAttack(false);
//        model.canAttack = false;
//        view.SetActiveSelectablePanel(false);
    }

    public void Heal(CardController friendCard)
    {
        model.Heal(friendCard);
        friendCard.RefreshView();
    }

    public void Show()
    {
        view.Show();
    }

    public void SetCanAttack(bool canAttack)
    {
        model.canAttack = canAttack;
        view.SetActiveSelectablePanel(canAttack);
    }

    public void RefreshView()
    {
        view.Refresh(model);
    }

    public void OnField()
    {
        //            Debug.Log(card.model.cost);
        //ManaCostをcardのcost分減らす
        gameManager.ReduceManaCost(model.cost, model.isPlayerCard);
        model.isFieldCard = true;
        /* INIT_ATTACKABLEを持っている場合は攻撃可能とする */
        if (model.ability == ABILITY.INIT_ATTACKABLE)
        {
            SetCanAttack(true);
        }

    }

    public void Refresh()
    {
        view.Refresh(model);
    }

    public void CheckAlive()
    {
        if (model.isAlive)
        {
            RefreshView();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /*
     *  敵がいないのに攻撃しようとしている => 敵AIのチェックと同様にすればよい
     *
     * 
     */

    public void UseSpellTo(CardController target)
    {
        switch (model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
                if (target == null)
                {
                    return;
                }
                if (target.model.isPlayerCard == model.isPlayerCard)
                {
                    return;
                }
                // 特定の敵を攻撃する
                Attack(target);
                target.CheckAlive();
                break;
            case SPELL.DAMAGE_ENEMY_CARDS:
                // 相手フィールドのすべてのカードを攻撃する
                CardController[] enemyCards = gameManager.GetEnemyFieldCards(this.model.isPlayerCard);
                foreach (CardController enemyCard in enemyCards)
                {
                    Attack(enemyCard);

                }
                foreach (CardController enemyCard in enemyCards)
                {
                    enemyCard.CheckAlive();
                }
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                gameManager.AttackToHero(this);
                break;
            case SPELL.HEAL_FRIEND_CARD:
                if (target == null)
                {
                    return;
                }
                if (target.model.isPlayerCard != model.isPlayerCard)
                {
                    return;
                }
                Heal(target);
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                // 相手フィールドのすべてのカードを攻撃する
                CardController[] friendCards = gameManager.GetFriendFieldCards(this.model.isPlayerCard);
                foreach (CardController friendCard in friendCards)
                {
                    Heal(friendCard);

                }
                break;
            case SPELL.HEAL_FRIEND_HERO:
                gameManager.HealToHero(this);
                break;
            case SPELL.NONE:
                return;
        }
        //ManaCostをcardのcost分減らす
        gameManager.ReduceManaCost(model.cost, model.isPlayerCard);

        Destroy(this.gameObject);
    }

    public bool CanUseSpell()
    {
        switch (model.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
            case SPELL.DAMAGE_ENEMY_CARDS:
                // 相手フィールドのすべてのカードを攻撃する
                CardController[] enemyCards = gameManager.GetEnemyFieldCards(this.model.isPlayerCard);
                if (enemyCards.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case SPELL.DAMAGE_ENEMY_HERO:
            case SPELL.HEAL_FRIEND_HERO:
                return true;
            case SPELL.HEAL_FRIEND_CARD:
            case SPELL.HEAL_FRIEND_CARDS:
                // 相手フィールドのすべてのカードを攻撃する
                CardController[] friendCards = gameManager.GetFriendFieldCards(this.model.isPlayerCard);
                if (friendCards.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case SPELL.NONE:
                return false;
        }
        return false;
    }
}
