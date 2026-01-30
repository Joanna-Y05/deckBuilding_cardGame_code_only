using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PackCollectionManager : MonoBehaviour
{
    public static PackCollectionManager instance;

    void Awake()
    {
      instance = this;  
    }

    public List<CardPackData> cardPacks;
    // Start is called before the first frame update
    void Start()
    {
        cardPacks = CardCollectionManager.instance.fullPackCollection;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

/*
    public bool CheckIfAvailable(CardPackData pack)
    {
        bool available = false;
        int cardIndex = 0;

        while (!available && cardIndex != 0)
        {
            cardIndex = Random.Range(0, pack.cardsInPack.Count);

            if(pack.cardsInPack[cardIndex].cardState == CardState.InShop)
            {
                return true;
            }
        }
        return false;
        
    }
    */
    public void CheckIfDiscovered()
    {
        
    }
}
