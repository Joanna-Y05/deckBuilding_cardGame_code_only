using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;

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
    public GameObject gameOverScreen;
    public TMP_Text playerCardsText, enemyCardsText, playerScoreText, enemyScoreText, resultText;

    [Header("Shop UI")]
    public TMP_Text textField;

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
        yield return new WaitForSeconds(3f);

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
    }

    public void RestartLevel()
    {
        AudioManager.Instance.PlaySFX(3);
    }

    public void ChooseNewBattle()
    {
        AudioManager.Instance.PlaySFX(3);
    }

    //shop UI stuff

    /*
    text 1 = welcome to the shop what can i get for you today

    text 2 = when an object is selected the shop owner should say so you want so and so pack that will be 
    but this text will alternate based on an array of appriorite responses
    (1) so you want the [card pack name], i guess i can give u a discount, that will be [pack cost]
    (2) ah the [card pack name], it has been getting more popular recently, its usually [pack cost + 5] but for you i will do it for [pack cost]
    (3) are you sure you want the [card pack name], its not very popular may be hard to make connections with... fine i will sell it for [pack cost]
    (4) the [card pack name]? hmm i will sell it to you for [pack cost]
    (5) ah the [card pack name], i've been wanting to get rid of that one for a while, i sell it to you for [pack cost]

    text 3 = when a pack is bought 
    thank you for buying a pack hope it helps you win 

    text 4 = closing the shop
    well then, till next time happy playing
    */

}
