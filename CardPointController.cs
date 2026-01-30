using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CardPointController : MonoBehaviour
{
    public static CardPointController instance;
    public void Awake()
    {
        instance = this;
    }

    [Header("Grid")]
    public CardPlacePoint[] cardPoints;
    public int gridSize = 3; //3x3, 4x4 etc

    [Header("Combat")]
    public CardPlacePoint currentPos = null;
    public float timeBetweenAttacks = 0.25f;
    public float timeBetweenTurns = 1.2f;

    void Start()
    {
        
    }
    void Update()
    {

    }
    public void PlayerAttack()
    {
        StartCoroutine(PlayerAttackCo());
    }
    //this will do the flip
    IEnumerator PlayerAttackCo()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        ResolveAttack(true);

    }


    public void EnemyAttack()
    {
        StartCoroutine(EnemyAttackCo());
    }
    
    IEnumerator EnemyAttackCo()
    {   
        //yield return new WaitForSeconds(timeBetweenTurns);
        yield return new WaitForSeconds(timeBetweenAttacks);
        ResolveAttack(false);
       
    }

    //attack logic

    void ResolveAttack(bool player)
    {
        if(currentPos == null || currentPos.activeCard == null)
        {
            return;
        }

        int originIndex = GetIndexOfCurrent();
        Vector2Int origin = IndexToCoord(originIndex);

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int target = origin + dir;

            if(InBounds(target.x, target.y))
            {
                int targetIndex = CoordToIndex(target.x, target.y);
                FlipCard(targetIndex, player);
            }
        }
    }

    //use in enemy ai class

    public List<int> GetAdjacentIndices(int originIndex)
    {
        List<int> results = new List<int>();
        Vector2Int origin = IndexToCoord(originIndex);

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int target = origin + dir;

            if(InBounds(target.x, target.y))
            {
                results.Add(CoordToIndex(target.x,target.y));
            }
        }
        return results;
    }

    public bool CanFlip(int attackerIndex, int targetIndex)
    {
        Card attacker = cardPoints[attackerIndex].activeCard;
        Card defender = cardPoints[targetIndex].activeCard;

        if (attacker == null || defender == null)
        {
            return false;
        }
        return attacker.powerLevel > defender.powerLevel;
    }

    public int EvaluateMove(int attackerIndex)
    {
        int flips = 0;

        foreach (int targetIndex in GetAdjacentIndices(attackerIndex))
        {
            if (CanFlip(attackerIndex, targetIndex))
            {
                flips++;
            }
        }
        return flips;
    }

    // grid math helpers

    int GetIndexOfCurrent()
    {
        return System.Array.IndexOf(cardPoints, currentPos);
    }

    Vector2Int IndexToCoord(int index)
    {
        int x = index % gridSize;
        int y = index / gridSize;

        return new Vector2Int(x,y);
    }

    int CoordToIndex(int x, int y)
    {
        return y * gridSize + x;
    }

    bool InBounds(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y <gridSize;
    } 

    //flip logic
    private void FlipCard(int card, bool player)
    {

        Card placedCard = currentPos.activeCard;
        Card targetCard = cardPoints[card].activeCard;

        if(targetCard == null)
        {
            return;
        }
        
        if(placedCard.powerLevel > targetCard.powerLevel)
        {
                if(player == true && cardPoints[card].spaceState != SpaceState.player)
                {
                    //flip to player side
                    targetCard.isPlayer = true;
                    cardPoints[card].spaceState = SpaceState.player;
                    AudioManager.Instance.PlaySFX(1);
                    targetCard.TriggerFlipAnimation();
                    Debug.Log("flipping enemy card: " + targetCard.cardName + ", after being attacked by " + placedCard.cardName);
                }
                if(player == false && cardPoints[card].spaceState == SpaceState.player)
                {
                    //flip to enemy side
                    targetCard.isPlayer = false;
                    cardPoints[card].spaceState = SpaceState.enemy;
                    AudioManager.Instance.PlaySFX(2);
                    targetCard.TriggerFlipAnimation();
                    Debug.Log("flipping player card: " + targetCard.cardName + ", after being attacked by " + placedCard.cardName);
                }

        }
    }
}
