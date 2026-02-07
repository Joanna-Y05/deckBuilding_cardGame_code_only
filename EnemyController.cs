using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    public void Awake()
    {
        instance = this;
    }

    public List<CardData> deckToUse;
    public List<CardData> activeCards = new List<CardData>();

    public Card cardToSpawn;
    public Transform cardSpawnPoint;
    public int drawCardCost = 1;

    public enum AIType { placeFromDeck, handRandomPlace, handDefensive, handAttacking }
    public AIType enemyAIType;

    private List<CardData> cardsInHand = new List<CardData>();
    public int startHandSize;

    public void StartGame()
    {

        SetupDeck();
        if (enemyAIType != AIType.placeFromDeck)
        {
            SetupHand();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetupDeck()
    {
        deckToUse = CardCollectionManager.instance.enemyCollection;
        activeCards.Clear();

        List<CardData> tempDeck = new List<CardData>();
        tempDeck.AddRange(deckToUse);

        int iterations = 0;
        while (tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);

            iterations++;
        }
    }

    public void StartAction()
    {
        enemyAIType = AIType.handAttacking;
        StartCoroutine(EnemyActionCo());
    }

    IEnumerator EnemyActionCo()
    {
        yield return new WaitForSeconds(CardPointController.instance.timeBetweenTurns);
        if (activeCards.Count == 0)
        {
            SetupDeck();
        }

        yield return new WaitForSeconds(0.5f);

        if(enemyAIType != AIType.placeFromDeck)
        {
            for(int i = 0; i < BattleController.instance.cardsToDrawPerTurn; i++)
            {
                cardsInHand.Add(activeCards[0]);
                activeCards.RemoveAt(0);

                if(activeCards.Count == 0)
                {
                    SetupDeck();
                }
            }
        }

        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();
        cardPoints.AddRange(CardPointController.instance.cardPoints);

        int randomPoint = Random.Range(0, cardPoints.Count);
        CardPlacePoint selectedPoint = cardPoints[randomPoint];

        if (enemyAIType == AIType.placeFromDeck || enemyAIType == AIType.handRandomPlace)
        {
            cardPoints.Remove(selectedPoint);
            while (selectedPoint.activeCard != null && cardPoints.Count > 0)
            {
                randomPoint = Random.Range(0, cardPoints.Count);
                selectedPoint = cardPoints[randomPoint];
                cardPoints.RemoveAt(randomPoint);
            }
        }

        CardData selectedCard = null;
        int iterations = 0;
        List<CardPlacePoint> preferredPoints = new List<CardPlacePoint>();
        List<CardPlacePoint> SecondaryPoints = new List<CardPlacePoint>();

        switch (enemyAIType)
        {
            case AIType.placeFromDeck:

                if (selectedPoint.activeCard == null)
                {
                    Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
                    newCard.cardData = activeCards[0];
                    activeCards.RemoveAt(0);
                    newCard.SetupCard();
                    newCard.MoveToPoint(selectedPoint.transform.position, Quaternion.Euler(0f, 270f, 90f));

                    selectedPoint.activeCard = newCard;
                    newCard.assignedPlace = selectedPoint;
                    CardPointController.instance.currentPos = selectedPoint;
                    newCard.assignedPlace.spaceState = SpaceState.enemy;
                }
                break;

            case AIType.handRandomPlace:
                selectedCard = selectedCardToPlay();
                iterations = 50;
                
                ExecuteRandomPlace();

                //this sections works it plays 1 card at a turn
                /*
                if(selectedCard != null)
                {
                    PlayCard(selectedCard, selectedPoint);
                }
                */
                break;

            case AIType.handDefensive:
                //draws a card instead or places action card
                
                break;

            case AIType.handAttacking:
                // (1) first checks spaces that are owned by the player

                List<int> playerOwnedIndices = new List<int>();
                for (int i = 0; i < CardPointController.instance.cardPoints.Length; i++)
                {
                    if(CardPointController.instance.cardPoints[i].spaceState == SpaceState.player && CardPointController.instance.cardPoints[i].activeCard != null)
                    {
                        playerOwnedIndices.Add(i);
                    }
                }
                // (2) checks if spaces that can flip it are free (may need a map of spaces and spaces connected)
                
                HashSet<int> candidatePlacementIndices = new HashSet<int>();

                foreach (int playerIndex in playerOwnedIndices)
                {
                    List<int> adj = CardPointController.instance.GetAdjacentIndices(playerIndex);
                    foreach (int adjIndex in adj)
                    {
                        if(CardPointController.instance.cardPoints[adjIndex].activeCard == null && CardPointController.instance.cardPoints[adjIndex].spaceState == SpaceState.free)
                        {
                            candidatePlacementIndices.Add(adjIndex);
                        }
                    }
                }

                //if not candidate placements exists, place randomly
                if(candidatePlacementIndices.Count == 0)
                {
                    ExecuteRandomPlace();
                }

                // (3) checks if there is a card currently in hand, if so flip place it in empty space to flip the card
                CardData bestCard = null;
                CardPlacePoint bestPoint = null;
                int bestScore = 0;

                foreach(int placeIndex in candidatePlacementIndices)
                {
                    List<int> attackedIndices = CardPointController.instance.GetAdjacentIndices(placeIndex);

                    foreach(CardData handCard in cardsInHand)
                    {
                        if(handCard.manaCost > BattleController.instance.enemyMana)
                        {
                            continue;
                        }
                        int score = 0;

                        //count how many adjacent player cards this hand card can flip
                        foreach(int targetIndex in attackedIndices)
                        {
                            var targetPoint = CardPointController.instance.cardPoints[targetIndex];

                            if(targetPoint.spaceState != SpaceState.player || targetPoint.activeCard == null)
                            {
                                continue;
                            }

                            //compare hand card power to the target card power (same rule as your flipCard)
                            if(handCard.power > targetPoint.activeCard.powerLevel)
                            {
                                score++;
                            }
                        }
                        // (extra) if more than one card can be flipped do like an algorithms which calculates optimal space to place it in
                        if(score > bestScore)
                        {
                            bestScore = score;
                            bestCard = handCard;
                            bestPoint = CardPointController.instance.cardPoints[placeIndex];
                        }

                    }

                    //if we found a move that flips at least one card, play it and trigger the attack
                    if(bestCard != null && bestPoint != null && bestScore > 0)
                    {
                        PlayCard(bestCard,bestPoint);
                        CardPointController.instance.EnemyAttack();
                        Debug.Log("placing " + bestCard.cardName + " strategically at " + bestPoint.gameObject.name);
                        break;
                    }
                    else
                    {
                        ExecuteRandomPlace();
                        break;
                    }
                }
                
                break;
        }

        yield return new WaitForSeconds(0.5f);

        BattleController.instance.AdvanceTurn();
    }

    void ExecuteRandomPlace()
    {
         CardData selectedCard = selectedCardToPlay();
         if(selectedCard == null)
        {
            return;
        }
        List<CardPlacePoint> places = new List<CardPlacePoint>();
        foreach(CardPlacePoint point in CardPointController.instance.cardPoints)
        {
            if(point.activeCard == null && point.spaceState == SpaceState.free)
            {
                places.Add(point);
            }
        }

        if(places.Count == 0)
        {
            return;
        }

        CardPlacePoint randomPoint = places[Random.Range(0,places.Count)];

        PlayCard(selectedCard,randomPoint);
        Debug.Log("placing " +selectedCard.cardName + " randomly at " + randomPoint.gameObject.name);
    }

    void SetupHand()
    {
        for(int i = 0; i < startHandSize; i++)
        {
            if (activeCards.Count == 0)
            {
                SetupDeck();
            }

            cardsInHand.Add(activeCards[0]);
            activeCards.RemoveAt(0);
        }
    }

    public void PlayCard(CardData cardSO, CardPlacePoint placePoint)
    {
        Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);
        newCard.cardData = cardSO;
        //activeCards.RemoveAt(0);
        newCard.SetupCard();
        //this euler used to be 0,270,90
        newCard.MoveToPoint(placePoint.transform.position, Quaternion.Euler(0f, 90f, 0f));

        placePoint.activeCard = newCard;
        newCard.assignedPlace = placePoint;
        CardPointController.instance.currentPos = placePoint;
        newCard.assignedPlace.spaceState = SpaceState.enemy;

        cardsInHand.Remove(cardSO);
        BattleController.instance.SpendEnemyMana(cardSO.manaCost);
    }

    CardData selectedCardToPlay()
    {
        CardData cardToPlay = null;

        List<CardData> cardsToPlay = new List<CardData>();
        foreach(CardData card in cardsInHand)
        {
            if(card.manaCost <= BattleController.instance.enemyMana)
            {
                cardsToPlay.Add(card);
            }
        }
        if(cardsToPlay.Count > 0)
        {
            int selected = Random.Range(0,cardsToPlay.Count);
            cardToPlay = cardsToPlay[selected];
        }

        return cardToPlay;
    }

    public void DrawCardForMana()
    {
        if (BattleController.instance.playerMana >= drawCardCost)
        {
            DrawCardToHand();
            BattleController.instance.SpendPlayerMana(drawCardCost);

        }
        else
        {
            UIController.instance.drawCardButton.SetActive(false);
        }
    }
    public void DrawCardToHand()
    {
        if (activeCards.Count == 0)
        {
            SetupDeck();
        }

        Card newCard = cardToSpawn;
        newCard.cardData = activeCards[0];
        newCard.SetupCard();

        activeCards.RemoveAt(0);

        HandController.instance.AddCardToHand(newCard);
    }
}
