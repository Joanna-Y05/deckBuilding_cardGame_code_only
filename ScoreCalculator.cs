using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{   
    public static ScoreCalculator Instance;
    public int playerScore = 0;
    public int enemyScore = 0;

    public int enemyCards = 0;
    public int playerCards = 0;

    private int totalPlayerAttack = 0;
    private int totalEnemyAttack = 0;

    public bool calculated;
    public bool winner;

    public CardPlacePoint[] points;
    
    private void Awake()
    {
        Instance = this;
    }
    public void StartGame()
    {
        calculated = false;
        points= CardPointController.instance.cardPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if(BattleController.instance.battleEnded == true && calculated == false)
        {
            CalculateScore();
        }
    }

    public void CalculateScore()
    {
        //to get what points belong to who
        for (int i = 0; i < points.Length; i++)
        {
            if(points[i].spaceState == SpaceState.player)
            {
                playerCards++;
                totalPlayerAttack += points[i].activeCard.powerLevel;
            }
            if(points[i].spaceState == SpaceState.enemy)
            {
                enemyCards++;
                totalEnemyAttack += points[i].activeCard.powerLevel;
            }
        }

        playerScore = (playerCards * 10) + (totalPlayerAttack*4);
        enemyScore = (enemyCards * 10) + (totalEnemyAttack*4);

        if(playerScore > enemyScore)
        {
            winner = true;
        }
        else
        {
            winner = false;
        }

        calculated = true;
    }
    
}
