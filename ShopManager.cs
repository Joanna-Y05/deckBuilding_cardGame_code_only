using System.Buffers;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    public static ShopManager instance;

    void Awake()
    {
      instance = this;  
    }

    public CardCollectionManager ccM;
    public List<CardData> shopCollection;
    public List<CardPackData> packCollection;
    public Animator anim;
    public bool shopOpen;
    //public CardPack selectedPack;
    public List<ShopPack> packsInShop;

    public bool sucessfullyGenerated = false;

    public ShopPack randomPack1;
    public ShopPack randomPack2;
    public ShopPack specificPack1;
    public ShopPack specificPack2;
    public GameObject button1, button2, button3, button4;
    public List<GameObject> buttons;
    public TMP_Text shopText;

    [Header("buying cards data")]
    public ShopPack selectedPack = null;
    public GameObject BoughtCardPanel;
    public TMP_Text cardBoughtText;
    

    // Start is called before the first frame update
    void Start()
    {
        shopOpen = false;
        shopCollection = ccM.shopCollection;
        packCollection = PackCollectionManager.instance.fullPackCollection;

        textOptions = new List<string>();

        buttons.Add(button1);
        buttons.Add(button2);
        buttons.Add(button3);
        buttons.Add(button4);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject b in buttons)
        {
            if(b.GetComponent<ShopPack>().cardsInPack.Count == 0)
            {
                b.SetActive(false);
            }
            else
            {
                b.SetActive(true);
            }
        }

    }

    public ShopPack makeRandomPack(ShopPack packToFill, int numOfCards)
    {
        packToFill.random = true;
        packToFill.cardsInPack.Clear();

        while(packToFill.cardsInPack.Count < numOfCards-1)
        {
            int selectedCard = Random.Range(0, shopCollection.Count);
            CardData card = shopCollection[selectedCard];

            packToFill.cardsInPack.Add(card);
        }
        Debug.Log("made random pack comprised of: ");
        foreach (CardData card in packToFill.cardsInPack)
        {
            
           Debug.Log(card.name);
        }
        
        packToFill.packName = "Random Pack";

        return packToFill;
    }
    public ShopPack makeSpecificPack(ShopPack packToFill, int numOfCards)
    {
        packToFill.random = false;
        packToFill.cardsInPack.Clear();
            
            //list of packs to attempt
            List<CardPackData> packsToTry = new List<CardPackData>(packCollection);


            while(packToFill.cardsInPack.Count < numOfCards && packsToTry.Count > 0)
            {

                int selectedPack = Random.Range(0,packCollection.Count);
                CardPackData pack = packsToTry[selectedPack];

                List<CardData> cardsToTry = new List<CardData>(pack.cardsInPack);

                while(cardsToTry.Count > 0 && packToFill.cardsInPack.Count < numOfCards)
                {
                    int selectCard = Random.Range(0,cardsToTry.Count);

                    if(cardsToTry[selectCard].cardState == CardState.InShop)
                    {
                        packToFill.cardsInPack.Add(cardsToTry[selectCard]);
                    }
                    cardsToTry.RemoveAt(selectCard);
                }

                packToFill.packName = pack.packName;
                packsToTry.RemoveAt(selectedPack);

            }

        if(packToFill.cardsInPack.Count < numOfCards)
        {
            Debug.LogWarning("shop could not fill specific pack");
        }

        Debug.Log("made specific pack comprised of: ");
        foreach (CardData card in packToFill.cardsInPack)
        {
            
           Debug.Log(card.name);
        }

        return packToFill;
    }

    public void TriggerShopAnim()
    {
        shopOpen = true;
        
        CameraMovementSystem.instance.ShopCamFocus();
        CameraMovementSystem.instance.shop_UI.SetActive(true);
        OpenShop();
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
        packsInShop.Clear();

        //generates packs in shop

        //two random packs
        makeRandomPack(randomPack1,3);
        makeRandomPack(randomPack2,3);

        //two specific packs
        makeSpecificPack(specificPack1,2);
        makeSpecificPack(specificPack2,2);


        //adds these packs to packsInShop list
        packsInShop.Add(randomPack1);
        packsInShop.Add(randomPack2);
        packsInShop.Add(specificPack1);
        packsInShop.Add(specificPack2);




    }
    public void OpenShop()
    {
        GeneralText(0);
        GenerateShopPacks();
        mapToButtons();
    }
    public void mapToButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject buttonObj = buttons[i];

            ShopPack buttonPack = buttonObj.GetComponent<ShopPack>();


            //buttonPack.SetFromPack(packsInShop[i]);
            buttonPack = packsInShop[i];
            buttonPack.numOfCards = buttonPack.cardsInPack.Count;

            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = buttonPack.packName;
        }
    }

    public void buy()
    {
        //buy cards
        //CardPack newPack = Instantiate(selectedPack, transform.position, Quaternion.Euler(0f, 270f, 270f));
        GeneralText(1);
        StartCoroutine(TriggerBuyPanel());

        foreach(CardData card in selectedPack.cardsInPack)
        {
            //remove from shop collection

            ccM.RemoveFromShopCollection(card);

            //change state from inshop to player owned

            card.cardState = CardState.None; //removes all states
            card.cardState = CardState.owned;

            //add to player collection
            ccM.AddToPlayerCollection(card);

            //set button to inactive by setting shop pack to bought 
            selectedPack.bought = true;

        }
        selectedPack.cardsInPack.Clear();

       // BoughtCardPanel.SetActive(false);

        selectedPack = null;

        
    }

    public void closeShop()
    {
       // EmptyShopSelection();
       GeneralText(2);
        Debug.Log("shop closed");
        TriggerShopCloseAnim();
        shopOpen = false;
    }

    public void OnPackButtonClicked(GameObject buttonObj)
    {

        ShopPack selected = buttonObj.GetComponent<ShopPack>();

        if (selected == null) return;

        Debug.Log("selected pack: " + selected.packName + " | cards: " + selected.cardsInPack.Count);
        changeText(selected);

        selectedPack = selected;
    }

    private List<string> textOptions;
    public void changeText(ShopPack selected)
    {
    textOptions.Clear();

    string text2a = $"so you want the {selected.packName}, i guess i can give u a discount, that will be {selected.cost}";
    string text2b = $"ah the {selected.packName}, it has been getting more popular recently, its usually {selected.cost + 5} but for you i will do it for {selected.cost}";
    string text2c = $"are you sure you want the {selected.packName}, its not very popular may be hard to make connections with... fine i will sell it for {selected.cost}";
    string text2d = $"the {selected.packName}? hmm i will sell it to you for {selected.cost}";
    string text2e = $"ah the {selected.packName}, i've been wanting to get rid of that one for a while, i will sell it to you for {selected.cost}";

    textOptions.Add(text2a);
    textOptions.Add(text2b);
    textOptions.Add(text2c);
    textOptions.Add(text2d);
    textOptions.Add(text2e);

    int option = Random.Range(0,textOptions.Count);
    shopText.text = textOptions[option];

    Debug.Log("cards in this pack: ");
    foreach(CardData card in selected.cardsInPack)
        {
            Debug.Log(card.cardName);
        }

    }
    public void GeneralText(int text)
    {
        if (text == 0)
        {
            shopText.text = "welcome to the shop what can i get for you today";
        }
        if(text == 1)
        {
            shopText.text = "thank you for buying a pack hope it helps you win";
        }
        if(text == 2)
        {
            shopText.text = "well then, till next time happy playing";
        }
    }
    public void EmptyShopSelection()
    {
        packsInShop.Clear();
    }

    IEnumerator TriggerBuyPanel()
    {
        BoughtCardPanel.SetActive(true);

        List<string> s = new List<string>();

        foreach (CardData card in selectedPack.cardsInPack)
        {
            s.Add(card.cardName.ToString() + ",");
        }

        // need to add text implementation so it changes the text on the buy panel

        cardBoughtText.text = string.Join("\n",s);

        yield return new WaitForSeconds(3f);

        BoughtCardPanel.SetActive(false);

    }
}
