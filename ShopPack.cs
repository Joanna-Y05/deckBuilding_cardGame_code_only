using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPack : MonoBehaviour
{

    public List<CardData> cardsInPack;
    public string packName;
    public bool random = false;
    public int numOfCards;
    public int cost = 4; 
    public bool bought = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetFromPack (ShopPack source)
    {
        random = source.random;
        numOfCards = source.numOfCards;

        cardsInPack.Clear();
        foreach(CardData card in source.cardsInPack)
        {
            cardsInPack.Add(card);
        }

        numOfCards = cardsInPack.Count;

        if(random == true)
        {
            packName = "random pack of cards";
        }
        /*else
        {
            packName = cardsInPack[0].packName;
        }*/
    }

}
