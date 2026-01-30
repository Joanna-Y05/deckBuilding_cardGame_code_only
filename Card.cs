using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;
using Unity.Mathematics;
using UnityEngine.Rendering;
using System.ComponentModel;
using UnityEditor;

public class Card : MonoBehaviour
{
    public CardData cardData;
    public bool isPlayer;
    public int powerLevel;
    public string cardName;
    public string cardPack;
    public string element;

    public Animator anim;

    public GameObject[] sigilLocations;

    public GameObject[] sigilPrefabs;

    public TMP_Text powerText, nameText;

[Header("moving settings")]
    private Vector3 targetPoint;
    private Quaternion targetRotation;
    public float moveSpeed = 5f, rotateSpeed = 540f;
    public Vector3 cardHover;
    public Vector3 cardSelect;

[Header("hand controller settings")]
    public bool inHand;
    public int handPosition;
    private HandController theHC;
    private bool isSelected;
    private Collider theCol;

    public LayerMask whatIsDesktop, whatIsPlacement, whatIsDesk;
    
    private bool justPressed;
    public CardPlacePoint assignedPlace;
    public int manaCost;

    void Start()
    {
        SetupCard();
        theHC = FindObjectOfType<HandController>();
        theCol = GetComponent<Collider>();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        if (isSelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, whatIsDesk))
            {
                //this vector used to be 0,0,-2
                MoveToPoint(hit.point + cardSelect, transform.rotation);
            }
            if (Input.GetMouseButtonDown(1) && BattleController.instance.battleEnded == false)
            {
                ReturnToHand();
            }
            if (Input.GetMouseButtonDown(0) && justPressed == false && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive)
            {
                if (Physics.Raycast(ray, out hit, 1000f, whatIsPlacement))
                {
                    CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();

                    if (selectedPoint.activeCard == null && selectedPoint.spaceState == SpaceState.free)
                    {
                        if (BattleController.instance.playerMana >= manaCost)
                        {
                            AudioManager.Instance.PlaySFX(4);
                            selectedPoint.activeCard = this;
                            assignedPlace = selectedPoint;

                            //this euler used to be 0,270,90
                            MoveToPoint(selectedPoint.transform.position, Quaternion.Euler(0f,90f,0f));
                            inHand = false;
                            isSelected = false;
                            theHC.RemoveCardFromHand(this);
                            selectedPoint.spaceState = SpaceState.player;

                            CardPointController.instance.currentPos = assignedPlace;
                            BattleController.instance.SpendPlayerMana(manaCost);
                        }
                        else
                        {
                            ReturnToHand(); 
                        }
                    }
                    else
                    {
                        ReturnToHand();
                    }
                }
                else
                {
                    ReturnToHand();
                }
            }
        }
        justPressed = false;
    }

    public void SetupCard()
    {
        powerLevel = cardData.power;
        cardName = cardData.cardName;
        cardPack = cardData.packName;
        element = cardData.affinity.ToString();
        manaCost = cardData.manaCost;

        //handles spawning in the sigil prefabs to their correct locations
        for (int i = 0; i < cardData.sigils.Length; i++)
        {
            Transform parentTransform = sigilLocations[i].transform;

            if (cardData.sigils[i] == SigilType.sun)
            {
                GameObject newObject = Instantiate(sigilPrefabs[0], sigilLocations[i].transform.position, sigilLocations[i].transform.rotation);
                newObject.transform.SetParent(parentTransform);
                newObject.transform.localPosition = Vector3.zero;
                newObject.transform.localRotation = Quaternion.identity;
            }
            if (cardData.sigils[i] == SigilType.moon)
            {
                GameObject newObject = Instantiate(sigilPrefabs[1], sigilLocations[i].transform.position, sigilLocations[i].transform.rotation);
                newObject.transform.SetParent(parentTransform);
                newObject.transform.localPosition = Vector3.zero;
                newObject.transform.localRotation = Quaternion.identity;
            }
            if (cardData.sigils[i] == SigilType.flame)
            {
                GameObject newObject = Instantiate(sigilPrefabs[2], sigilLocations[i].transform.position, sigilLocations[i].transform.rotation);
                newObject.transform.SetParent(parentTransform);
                newObject.transform.localPosition = Vector3.zero;
                newObject.transform.localRotation = Quaternion.identity;
            }
            if (cardData.sigils[i] == SigilType.star)
            {
                GameObject newObject = Instantiate(sigilPrefabs[3], sigilLocations[i].transform.position, sigilLocations[i].transform.rotation);
                newObject.transform.SetParent(parentTransform);
                newObject.transform.localPosition = Vector3.zero;
                newObject.transform.localRotation = Quaternion.identity;
            }
        }
        if (powerLevel == 10)
        {
            powerText.text = "A";
        }
        else if (powerLevel == 11)
        {
            powerText.text = "S";
        }
        else
        {
            powerText.text = powerLevel.ToString();
        }

        nameText.text = cardName.ToString();
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRotation = rotToMatch;
    }
    private void OnMouseOver()
    {
        if (inHand && isPlayer && !BattleController.instance.battleEnded == false)
        {
            //this vector used to be 0,0.5,-1
            MoveToPoint(theHC.cardPositions[handPosition] + cardHover, Quaternion.identity);
            Debug.Log("hovering over " + this.cardData.cardName);
        }
    }
    private void OnMouseExit()
    {
        if (inHand && isPlayer)
        {
            MoveToPoint(theHC.cardPositions[handPosition], theHC.minPos.rotation);
            Debug.Log("stopped hovering over " + this.cardData.cardName);
        }
    }
    private void OnMouseDown()
    {
        if (inHand && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && isPlayer)
        {
            isSelected = true;
            theCol.enabled = false;

            justPressed = true;
            Debug.Log("picking up " + this.cardData.cardName);
        }
    }
    public void ReturnToHand()
    {
        isSelected = false;
        theCol.enabled = true;
        MoveToPoint(theHC.cardPositions[handPosition], theHC.minPos.rotation);
        Debug.Log("returning " + this.cardData.cardName);
    }

    public void TriggerFlipAnimation()
    {
        anim.SetTrigger("flip_card");
        Debug.Log("animation triggered");
    }
}