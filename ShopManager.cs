using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{

    public static ShopManager instance;

    void Awake()
    {
      instance = this;  
    }

    public List<CardData> shopCollection;
    public Animator anim;
    public bool shopOpen;
    public CardPack selectedPack;
    public List<CardPack> packsInShop;

    // Start is called before the first frame update
    void Start()
    {
        shopOpen = false;
        shopCollection = CardCollectionManager.instance.shopCollection;
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void SetUpPack()
    {
        
    }

    public void TriggerShopAnim()
    {
        CameraMovementSystem.instance.ShopCamFocus();
        CameraMovementSystem.instance.shop_UI.SetActive(true);
        shopOpen = true;
        anim.SetBool("shop_open", true);
    }
    public void TriggerShopCloseAnim()
    {
        Debug.Log("shop opened");
        CameraMovementSystem.instance.shop_UI.SetActive(false);
        CameraMovementSystem.instance.MainCamFocus();
        anim.SetBool("shop_open", false);
    }

    public void GenerateShopPacks()
    {
        //generates packs in shop
    }

    public void buy()
    {
        //buy cards
        CardPack newPack = Instantiate(selectedPack, transform.position, Quaternion.Euler(0f, 270f, 270f));
    }

    public void closeShop()
    {
        EmptyShopSelection();
        Debug.Log("shop closed");
        TriggerShopCloseAnim();
        shopOpen = false;
    }

    public void SelectPack()
    {
        //when a pack is selected
    }

    public void changeText()
    {
        
    }
    public void EmptyShopSelection()
    {
       foreach(CardPack heldPack in packsInShop)
        {
            heldPack.inShop = false;
            heldPack.MoveToPoint(BattleController.instance.discardPoint.position, heldPack.transform.rotation);
        } 
        packsInShop.Clear();
    }
}
