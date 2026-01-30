using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementSystem : MonoBehaviour
{
    public static CameraMovementSystem instance;

    public void Awake()
    {
        instance = this;
    }

    public GameObject cardCam;
    public GameObject boardCam;
    public GameObject deckCam;
    public GameObject shopCam;

    public GameObject fullviewCam;
    public GameObject menuCam;
    public GameObject card_gameUI;
    public GameObject shop_UI;
    
    // Start is called before the first frame update
    void Start()
    {
       MenuCamFocus();

    }

    // Update is called once per frame
    void Update()
    {
        //change camera to card camera
        if (Input.GetKeyDown(KeyCode.S))
        {
            CardCamFocus();
        }

        //change camera to board camera
        if (Input.GetKeyDown(KeyCode.W))
        {
            GridCamFocus();
        }

        //change camera to card deck camera
        if (Input.GetKeyDown(KeyCode.D))
        {
            DeckCamFocus();
        }
    }

    public void CardCamFocus()
    {
        cardCam.SetActive(true);
            boardCam.SetActive(false);
            deckCam.SetActive(false);
            fullviewCam.SetActive(false);
            menuCam.SetActive(false);
            shopCam.SetActive(false);
    }
    public void DeckCamFocus()
    {
        cardCam.SetActive(false);
            boardCam.SetActive(false);
            deckCam.SetActive(true);
            fullviewCam.SetActive(false);
            menuCam.SetActive(false);
            shopCam.SetActive(false);
    }
    public void GridCamFocus()
    {
        cardCam.SetActive(false);
            boardCam.SetActive(true);
            deckCam.SetActive(false);
            fullviewCam.SetActive(false);
            menuCam.SetActive(false);
            shopCam.SetActive(false);
    }
    public void MainCamFocus()
    {
        boardCam.SetActive(false);
        deckCam.SetActive(false);
        cardCam.SetActive(false);
        fullviewCam.SetActive(true);
        menuCam.SetActive(false);
        shopCam.SetActive(false);
    }
    public void MenuCamFocus()
    {
        //starting view is the card deck
        boardCam.SetActive(false);
        deckCam.SetActive(false);
        cardCam.SetActive(false);
        fullviewCam.SetActive(false);
        menuCam.SetActive(true);
        shopCam.SetActive(false);
    }
    public void ShopCamFocus()
    {
        boardCam.SetActive(false);
        deckCam.SetActive(false);
        cardCam.SetActive(false);
        fullviewCam.SetActive(false);
        menuCam.SetActive(false);
        shopCam.SetActive(true);
    }
}
