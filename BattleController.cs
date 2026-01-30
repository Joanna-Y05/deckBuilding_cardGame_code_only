using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;

    private void Awake()
    {
        instance = this;
    }

    public int startingMana = 1, maxMana = 3;
    public int playerMana, enemyMana;
    public int currentPlayerMaxMana, currentEnemyMaxMana;

    public int startingCardsAmount = 5;
    public int cardsToDrawPerTurn = 1;

    public enum TurnOrder
    {
        playerActive,
        playerCardAttacks,
        enemyActive,
        enemyCardAttacks,
        finished
    }
    public TurnOrder currentPhase;

    public bool battleEnded = false;

    public Transform discardPoint;

    public void StartGame()
    {
        currentPlayerMaxMana = startingMana;
        currentEnemyMaxMana = startingMana;
        FillPlayerMana();
        FillEnemyMana();
        DeckController.instance.DrawMultipleCards(startingCardsAmount);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AdvanceTurn();
        }
        int filledSpaces = 0;
        for (int i = 0; i < CardPointController.instance.cardPoints.Length; i++)
        {
            if (CardPointController.instance.cardPoints[i].activeCard != null)
            {
                filledSpaces++;
            }
        }
        if(filledSpaces == CardPointController.instance.cardPoints.Length)
        {
            currentPhase = TurnOrder.finished;
            EndBattle();
        }
    }
    public void SpendPlayerMana(int amountToSpend)
    {
        playerMana -= amountToSpend;

        if (playerMana < 0)
        {
            playerMana = 0;
        }
        UIController.instance.SetPlayerManaText(playerMana);
    }
    public void FillPlayerMana()
    {
        playerMana = currentPlayerMaxMana;
        UIController.instance.SetPlayerManaText(playerMana);
    }

    public void SpendEnemyMana(int amountToSpend)
    {
        enemyMana -= amountToSpend;

        if (enemyMana < 0)
        {
            enemyMana = 0;
        }
        UIController.instance.SetEnemyManaText(enemyMana);
    }
    public void FillEnemyMana()
    {
        enemyMana = currentEnemyMaxMana;
        UIController.instance.SetEnemyManaText(enemyMana);
    }
    public void AdvanceTurn()
    {
        if (!battleEnded){
            currentPhase++;

            if ((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length-1)
            {
                currentPhase = 0;
            }

            switch (currentPhase)
            {
                case TurnOrder.playerActive:
                    UIController.instance.endTurnButton.SetActive(true);
                    UIController.instance.drawCardButton.SetActive(true);
                    UIController.instance.movesLeftText.SetActive(true);

                    if (currentPlayerMaxMana < maxMana)
                    {
                        currentPlayerMaxMana++;
                    }

                    FillPlayerMana();

                    DeckController.instance.DrawMultipleCards(cardsToDrawPerTurn);
                    
                    break;

                case TurnOrder.playerCardAttacks:

                    CardPointController.instance.PlayerAttack();
                    AdvanceTurn();

                    break;

                case TurnOrder.enemyActive:

                    if (currentEnemyMaxMana < maxMana)
                        {
                            currentEnemyMaxMana++;
                        }

                    FillEnemyMana();

                    EnemyController.instance.StartAction();

                    break;

                case TurnOrder.enemyCardAttacks:

                    CardPointController.instance.EnemyAttack();
                    AdvanceTurn();

                    break;

                case TurnOrder.finished:
                    //to end the game
                    EndBattle();
                    break;
            }
        }
    }
    public void EndPlayerTurn()
    {
        UIController.instance.endTurnButton.SetActive(false);
        UIController.instance.drawCardButton.SetActive(false);
        UIController.instance.movesLeftText.SetActive(false);
        AdvanceTurn();
    }

    void EndBattle()
    {
        Debug.Log("no more spaces, game over");
        battleEnded = true;
        HandController.instance.EmptyHand();
    }
}
