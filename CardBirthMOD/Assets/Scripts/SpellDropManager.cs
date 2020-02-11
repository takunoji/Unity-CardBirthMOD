using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 攻撃される側
public class SpellDropManager : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {

        /* 攻撃 */
        // attackerカードを選択
        CardController spellCard = eventData.pointerDrag.GetComponent<CardController>();
        // defenderカードを選択(Playerフィールドより)
        CardController target = GetComponent<CardController>();// nullの可能性あり

        if (null == spellCard)
        {
            return;
        }

        if (spellCard.CanUseSpell())
        {
            spellCard.UseSpellTo(target);

        }
    }
}
