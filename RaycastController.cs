using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController : MonoBehaviour
{

    public static RaycastController instance;

    public void Awake()
    {
      instance = this;  
    }

    public LayerMask whatIsDesk, whatIsShop, whatIsCollection;
    private bool gameStarted = false;
    public GameObject startText, shopText, collectionText;


    // Start is called before the first frame update
    void Start()
    {
        gameStarted = false;
        startText.SetActive(false);
        shopText.SetActive(false);
        collectionText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && !gameStarted && !ShopManager.instance.shopOpen)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, whatIsDesk))
            {
                Debug.Log("game started");

                //for now this will start the whole game
                BattleController.instance.StartGame();
                DeckController.instance.StartGame();
                EnemyController.instance.StartGame();
                HandController.instance.StartGame();
                ScoreCalculator.Instance.StartGame();

                CameraMovementSystem.instance.card_gameUI.SetActive(true);

                CameraMovementSystem.instance.CardCamFocus();
                startText.SetActive(false);
                shopText.SetActive(false);
                collectionText.SetActive(false);
                gameStarted = true;
            }

            else if (Physics.Raycast(ray, out hit, 1000f, whatIsShop))
            {
                startText.SetActive(false);
                shopText.SetActive(false);
                collectionText.SetActive(false);

                ShopManager.instance.TriggerShopAnim();
                Debug.Log("shop pressed");
            }

            else if(Physics.Raycast(ray, out hit, 1000f, whatIsCollection))
            {
                Debug.Log("collection pressed");
            }
        } 

        if (!gameStarted || !ShopManager.instance.shopOpen)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, whatIsDesk))
            {
                startText.SetActive(true);
                //Debug.Log("desk hovered");
            }

            else if (Physics.Raycast(ray, out hit, 1000f, whatIsShop))
            {
                shopText.SetActive(true);
                //Debug.Log("shop hovered");
            }

            else if(Physics.Raycast(ray, out hit, 1000f, whatIsCollection))
            {
                collectionText.SetActive(true);
                //Debug.Log("collection hovered");
            }
            else
            {
                startText.SetActive(false);
                shopText.SetActive(false);
                collectionText.SetActive(false);
            }
        }
        else
        {
            startText.SetActive(false);
            shopText.SetActive(false);
            collectionText.SetActive(false);
        }
    }
}
