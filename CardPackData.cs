using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewCardPackData", menuName = "CardPackData")]
public class CardPackData : ScriptableObject
{
    [Header("Identity")]
    public string packName;
    public int packID;

    [Header("Core Status")]
    public bool completed;
    public bool discovered;
    public int ownedCards;

    [Header("Visuals")]
    //public shader artwork; when i make shader graphs for card packs add this
    [TextArea] public string description;

    [Header("Cards in pack")]
    public List<CardData> cardsInPack;

    [Header("Shop Settings")]
    public bool available;
    public int packCost;

}
