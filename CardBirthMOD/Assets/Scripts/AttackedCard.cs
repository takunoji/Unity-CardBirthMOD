using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 攻撃される側
public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {

        /* 攻撃 */
        // attackerカードを選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        // defenderカードを選択(Playerフィールドより)
        CardController defender = GetComponent<CardController>();

        if (null == attacker || null == defender)
        {
            return;
        }

        // 敵フィールドにシールドカードがある場合はシールドカード以外は攻撃できない
        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCards(attacker.model.isPlayerCard);
        if (Array.Exists(enemyFieldCards, card => card.model.ability == ABILITY.SHIELD) &&
            defender.model.ability != ABILITY.SHIELD)
        {
            return;
        }

        if (attacker.model.canAttack)
        {
            // attackerとdegfenderを戦わせる
            GameManager.instance.CardsBattle(attacker, defender);
        }
    }
}
