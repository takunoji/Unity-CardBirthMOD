using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CardEntiy",menuName ="Create CardEntity")]
// カードデータそのものとその処理
public class CardEntity : ScriptableObject
{
    public new string name;
    public int hp;
    public int at;
    public int cost;
    public Sprite icon;
    public ABILITY ability;
    public SPELL spell;
}

public enum ABILITY
{
    NONE,           //なし
    INIT_ATTACKABLE,    //速攻
    SHIELD,             //挑発
}
public enum SPELL
{
    NONE,
    DAMAGE_ENEMY_CARD,
    DAMAGE_ENEMY_CARDS,
    DAMAGE_ENEMY_HERO,
    HEAL_FRIEND_CARD,
    HEAL_FRIEND_CARDS,
    HEAL_FRIEND_HERO,

}
