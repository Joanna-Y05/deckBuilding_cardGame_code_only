using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "NewCardData", menuName = "CardData")]
public class CardData : ScriptableObject
{
    [Header("Identity")]
    public string cardName;
    public string packName;
    public int cardID;

    [Header("Core Status")]

    [Range(0, 11)] public int power; // 0-9, A=10, S=11
    public int manaCost;
    public ElementAffinity affinity;

    [Header("Sigils (top, right, bottom, left)")]
    public SigilType[] sigils = new SigilType[4];

    [Header("Card Effects")]
    public EffectType effectType = EffectType.None;
    public List<CardEffect> additionalEffects;

    [Header("State")]
    public CardState cardState = CardState.Found;

    [Header("Visuals")]
    public Sprite artwork;
    [TextArea] public string description;
}
public enum ElementAffinity { None, Fire, Water, Wind, Earth, Light, Shadow }
public enum SigilType { sun, moon, star, flame }
public enum EffectType {None, Passive, Triggered, AttackCommand }

[System.Flags]
public enum CardState
{
    None = 0,
    Found = 1 << 0,
    Lost = 1 << 1,
    Injured = 1 << 2,
    Destroyed = 1 << 3,
    EnemyOwned = 1 << 4,
    InShop = 1 << 5,
    owned = 1 << 6
}


[System.Serializable]
public class CardEffect
{
    public string effectName;
    public string description;
    public int potency;
    //public Sprite icon;
}
