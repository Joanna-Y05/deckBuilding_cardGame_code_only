using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine.UIElements;
using UnityEditor;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Core Game UI")]
    public TMP_Text playerManaText, enemyManaText;
    public GameObject drawCardButton, endTurnButton,movesLeftText;

    [Header("Game Over screen UI")]
    public GameObject gameOverScreen, mainUI, deck;
    public TMP_Text playerCardsText, enemyCardsText, playerScoreText, enemyScoreText, resultText;

    [Header("Shop UI")]
    public TMP_Text textField;

    [Header("other UI")]
    public GameObject mainMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(BattleController.instance.battleEnded == true)
        {
            GameOver();
        }
        else
        {
            gameOverScreen.SetActive(false);
        }
    }

    public void SetPlayerManaText(int manaAmount)
    {
        playerManaText.text = "moves left: " + manaAmount;
    }
    public void SetEnemyManaText(int manaAmount)
    {
        enemyManaText.text = "moves left: " + manaAmount;
    }
    public void DrawCard()
    {
        AudioManager.Instance.PlaySFX(3);
        DeckController.instance.DrawCardForMana();
    }
    public void EndPlayerTurn()
    {
        AudioManager.Instance.PlaySFX(3);
        BattleController.instance.EndPlayerTurn();
    }

    public void GameOver()
    {
        StartCoroutine(GameOverCo());
    }
    IEnumerator GameOverCo()
    {
        yield return new WaitForSeconds(0f);

        gameOverScreen.SetActive(true);
        
        playerScoreText.text = "Player Score: " + ScoreCalculator.Instance.playerScore.ToString();
        enemyScoreText.text = "Enemy Score: " + ScoreCalculator.Instance.enemyScore.ToString();

        playerCardsText.text = "Cards owned by Player: " + ScoreCalculator.Instance.playerCards.ToString();
        enemyCardsText.text = "Cards owned by Enemy: " + ScoreCalculator.Instance.enemyCards.ToString();

        if(ScoreCalculator.Instance.winner == true)
        {
            resultText.text = "You Won!";
        }
        else
        {
            resultText.text = "You Lost!";
        }
    }

    public void MainMenu()
    {
        AudioManager.Instance.PlaySFX(3);
        mainMenuUI.SetActive(true);
        CameraMovementSystem.instance.MenuCamFocus();

    }

    public void Continue()
    {
        AudioManager.Instance.PlaySFX(3);
        //this will be the reset function
        CameraMovementSystem.instance.MainCamFocus();
        CardPointController.instance.EmptySpaces();
        RaycastController.instance.gameStarted = false;
        BattleController.instance.battleEnded = false;
        CardCollectionManager.instance.ResetPower();
        gameOverScreen.SetActive(false);
        CameraMovementSystem.instance.card_gameUI.SetActive(false);
        Debug.Log("player chose to continue");
    }

    public void OpenDeck()
    {
        deck.SetActive(true);
        SetCardPositionsInDeck();
    }
    
    public void CloseDeck()
    {
        DeckSlot[] exsistingSlots = FindObjectsOfType<DeckSlot>();
        foreach(DeckSlot deck in exsistingSlots)
        {
            Destroy(deck.gameObject);
        }
        cards.Clear();
        deck.SetActive(false);
    }


    [Header("deck slot settings")]
    private List<Vector3> cardPositions = new List<Vector3>();
    public Transform minPos, maxPos;
    private List<CardData> heldCards;
    public List<DeckSlot> cards;
    public DeckSlot deckSlot;
    
    


    public void SetCardPositionsInDeck()
    {
        heldCards = CardCollectionManager.instance.playerDeck;

        cardPositions.Clear();
        cards.Clear();

        foreach (CardData data in heldCards)
        {
            DeckSlot newSlot = Instantiate(deckSlot, transform);
            newSlot.data = data;
            newSlot.power.text = data.currentPower.ToString();
            newSlot.cardName.text = data.cardName;

            cards.Add(newSlot);

        }

        Vector3 distanceBetweenPoints = Vector3.zero;
        if (cards.Count > 1)
        {
            distanceBetweenPoints = (maxPos.localPosition - minPos.localPosition) / (heldCards.Count - 1);
        }
        for (int i = 0; i < cards.Count; i++)
        {
            cardPositions.Add(minPos.localPosition + (distanceBetweenPoints * i));

            //heldCards[i].transform.position = cardPositions[i];
            //heldCards[i].transform.rotation = minPos.rotation;

            //this will set where the card should be and its rotation

            cards[i].transform.localPosition = cardPositions[i];

            //heldCards[i].MoveToPoint(cardPositions[i], minPos.rotation);
            //heldCards[i].inHand = true;
            //heldCards[i].handPosition = i;
        }
    }

}
