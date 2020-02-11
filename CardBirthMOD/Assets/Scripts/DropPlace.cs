using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropPlace : MonoBehaviour,IDropHandler
{
    public enum TYPE
    {
        HAND,
        FIELD,

    }
    public TYPE type;
    public void OnDrop(PointerEventData eventData)
    {
        if (type == TYPE.HAND)
        {
            return;
        }
        /* ManaCost実装のため修正-> */
        //        CardMovement card = eventData.pointerDrag.GetComponent<CardMovement>();
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        if (card != null)
        {
            // isDraggableがfalseの時（＝ドラッグ不可、ManaCost足りない）、
            if (!card.movement.isDraggable)
            {
                return;
            }
//            if (card != null)
//            {
            Debug.Log("SPELL?  " + card.IsSpell);
            /* スペルカードはフィールドに置けない */
            if (card.IsSpell)
            {
                return;
            }

                // カードを取得
                card.movement.defaultParent = this.transform;

            if (card.model.isFieldCard)
            {
                return;
            }
                card.OnField();
//            }
        }
    }
}
