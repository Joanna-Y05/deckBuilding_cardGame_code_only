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

    public List<CardPackData> fullPackCollection;
    public List<CardData> fullCardCollection;
    
    // Start is called before the first frame update
    void Start()
    {
        fullCardCollection = CardCollectionManager.instance.fullCardCollection;

      //generate card packs
        GeneratePackCollection();
        foreach(CardPackData pack in fullPackCollection)
        {
            SetUpPack(pack);
        }
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
    private void GeneratePackCollection()
    {
        CardPackData[] packs = Resources.LoadAll<CardPackData>("Scriptables/Card Packs");
        fullPackCollection.AddRange(packs);

        Debug.Log($"loaded {fullPackCollection.Count} card packs");
    }

    private void SetUpPack(CardPackData pack)
    {
        foreach(CardData card in fullCardCollection)
        {
            if(card.packName == pack.packName)
            {
                pack.cardsInPack.Add(card);
            }
        }
    }
}
