using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardCollectionManager : MonoBehaviour
{

    public static CardCollectionManager instance;

    void Awake()
    {
        instance = this;
        //generate deck
        GenerateFullCollection();
        GenerateIDs();
    }

    public List<CardData> fullCardCollection;

    public List<CardData> playerCollection;
    public List<CardData> shopCollection;
    public List<CardData> enemyCollection;

    

    // Start is called before the first frame update
    void Start()
    {
        playerCollection.Clear();
        enemyCollection.Clear();
        shopCollection.Clear();

        foreach(CardData card in fullCardCollection)
        {
            //forgets any changes and sets back to default
            card.cardState = CardState.None;
            card.cardState = card.defaultState;

            //add to playercollection
            if(card.cardState == CardState.owned)
            {
                playerCollection.Add(card);
            }

            //add to shop collection
            else if(card.cardState == CardState.InShop)
            {
                shopCollection.Add(card);
            }

            //add to enemy deck
            else if(card.cardState == CardState.EnemyOwned)
            {
                enemyCollection.Add(card);
            }
        }

        Debug.Log($"added {playerCollection.Count} cards to player collection");
        Debug.Log($"added {shopCollection.Count} cards to shop collection");
        Debug.Log($"added {enemyCollection.Count} cards to enemy collection");

        //then update any special enemy decks this will require more logic for 
        //instance adding a sorted bool where it will first check if the card is sorted before adding it to the enemy deck
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveFromPlayerCollection(int cardID)
    {
        for(int i = 0; i < playerCollection.Count; i++)
        {
            if(playerCollection[i].cardID == cardID)
            {
                Debug.Log(playerCollection[i].cardName + " removed from player collection");
                playerCollection.RemoveAt(i);
                break;
            }
        }
    }
    public void RemoveFromEnemyCollection(int cardID)
    {
        for(int i = 0; i < enemyCollection.Count; i++)
        {
            if(enemyCollection[i].cardID == cardID)
            {
                Debug.Log(enemyCollection[i].cardName + " removed from enemy collection");
                enemyCollection.RemoveAt(i);
                break;
            }
        }
    }

    //when a card is bought remove from shop
    public void RemoveFromShopCollection(CardData cardToRemove)
    {
        for(int i = 0; i < shopCollection.Count; i++)
        {
            if(shopCollection[i] == cardToRemove)
            {
                Debug.Log(shopCollection[i].cardName + " removed from shop collection");
                shopCollection.RemoveAt(i);
                break;
            }
        }
    }

    public void AddToPlayerCollection (CardData cardToAdd)
    {
        playerCollection.Add(cardToAdd);
        Debug.Log(cardToAdd.cardName + " has been added to the player's collection");
    }

    private void GenerateFullCollection()
    {
        fullCardCollection.Clear();
        CardData[] cards = Resources.LoadAll<CardData>("Scriptables/Cards");
        fullCardCollection.AddRange(cards);

        Debug.Log($"loaded {fullCardCollection.Count} cards");
    }

    private void GenerateIDs()
    {
        for(int i = 0; i < fullCardCollection.Count; i++)
        {
            fullCardCollection[i].cardID = i;
        }
        Debug.Log($"{fullCardCollection.Count} IDs generated for deck");
    }
}
