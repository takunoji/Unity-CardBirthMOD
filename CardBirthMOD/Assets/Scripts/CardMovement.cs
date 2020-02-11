using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardMovement : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    // 親を作る
    public Transform defaultParent;

    public bool isDraggable;    // ドラッグ可能判定

    public void OnBeginDrag(PointerEventData eventData)
    {
        // カードのコストとPlayerのManaコストを比較
        CardController card = GetComponent<CardController>();
        if (card.model.isPlayerCard&&
            GameManager.instance.isPlayerTurn&&
            !card.model.isFieldCard && 
            card.model.cost <= GameManager.instance.player.manaCost)
        {
            isDraggable = true;
        }
        else if (card.model.isPlayerCard&&
            GameManager.instance.isPlayerTurn && 
            card.model.isFieldCard && 
            card.model.canAttack)
        {
            isDraggable = true;
        }
        else
        {
            isDraggable = false;
        }
        if (!isDraggable)
        {
            return;
        }
        //　自分自身の親を取得する
        defaultParent = transform.parent;
        // 親の親を自分の親に設定する
        transform.SetParent(defaultParent.parent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
//        throw new System.NotImplementedException();
    }

    // ドラッグ移動させる処理
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable)
        {
            return;
        }
        //        throw new System.NotImplementedException();
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable)
        {
            return;
        }
        //        throw new System.NotImplementedException();
        //離した時に、自分の親のところに戻る
        transform.SetParent(defaultParent, false);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public IEnumerator MoveToField(Transform field)
    {
        // 一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);
        // DOTweenでカードをフィールドに移動
        transform.DOMove(field.position, 0.25f);    

        yield return new WaitForSeconds(0.25f);

        defaultParent = field;
        transform.SetParent(defaultParent);
    }
    public IEnumerator MoveToTarget(Transform target)
    {
        // 自分の場所を覚えておく
        Vector3 currentPotision = transform.position;
        int siblingIndex = transform.GetSiblingIndex(); //子要素の並びが取得可能

        // 一度親をCanvasに変更する
        transform.SetParent(defaultParent.parent);
        // DOTweenでカードをTargetに移動
        transform.DOMove(target.position, 0.25f);
        yield return new WaitForSeconds(0.25f);
        // DOTweenでカードを元の位置に戻す
        transform.DOMove(currentPotision, 0.25f);
        yield return new WaitForSeconds(0.25f);
        if (this != null)
        {
            // 親を戻す
            transform.SetParent(defaultParent);
            transform.SetSiblingIndex(siblingIndex); //子要素の並びが取得可能

        }
    }
    /*  -> MoveToFieldに移動
        public void SetCardTransform(Transform parentTransform)
        {
            defaultParent = parentTransform;
            transform.SetParent(defaultParent);
        }
    */
    void Start()
    {

        defaultParent = transform.parent;
    }
}
