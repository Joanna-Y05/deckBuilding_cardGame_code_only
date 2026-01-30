using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPlacePoint : MonoBehaviour
{
    public Card activeCard;
    public SpaceState spaceState = SpaceState.free;
    public Color playerColor = Color.green;
    public Color enemyColor = Color.red;
    public Color freeColor = Color.white;
    public Color blockedColor = Color.grey;
    private SpriteRenderer spriteRenderer;
    public float changeSpeed = 2f;

    void Start()
    {
      spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("No SpriteRenderer found on " + gameObject.name);
        }  
    }
    void Update()
    {
        switch (spaceState)
        {
            case SpaceState.free:
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, freeColor, changeSpeed * Time.deltaTime);
                break;

            case SpaceState.blocked:
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, blockedColor, changeSpeed * Time.deltaTime);
                break;
            
            case SpaceState.player:
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, playerColor, changeSpeed * Time.deltaTime);
                break;

            case SpaceState.enemy:
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, enemyColor, changeSpeed * Time.deltaTime);
                break;
            
        }
        
    }


}

public enum SpaceState
{
    free,
    blocked,
    player,
    enemy
}
