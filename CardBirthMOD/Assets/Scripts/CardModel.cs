using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//　カードデータとその処理
public class CardModel
{
    public string name;
    public int hp;
    public int at;
    public int cost;
    public Sprite icon;
    public ABILITY ability;
    public SPELL spell;

    public bool isAlive;
    public bool canAttack;      // Arrack可能かどうか判定
    public bool isFieldCard;    // Fieldかどうか判定
    public bool isPlayerCard;

    public CardModel(int cardID,bool isPlayer)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardEntityList/Card"+cardID);
        name = cardEntity.name;
        hp = cardEntity.hp;
        at = cardEntity.at;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
        ability = cardEntity.ability;
        spell = cardEntity.spell;

        isAlive = true;
        isPlayerCard = isPlayer;
    }

    void Damage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            hp = 0;
            isAlive = false;
        }
    }

    //自分を回復する
    void RecoberyHP(int point)
    {
        hp += point;
    }

    public void Attack(CardController card)
    {
        card.model.Damage(at);
    }

    //Cardを回復させる。
    public void Heal(CardController card)
    {
        card.model.RecoberyHP(at);
    }
}
