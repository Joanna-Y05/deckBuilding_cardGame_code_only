using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPack : MonoBehaviour
{
    public CardPackData packData;
    public int cardsBought;
    public Animator tearAnim;
    public Animator cardAnim;
    public string packName;
    public TMP_Text nameText;

    [Header("moving settings")]
    private Vector3 targetPoint;
    private Quaternion targetRotation;
    public float moveSpeed = 5f, rotateSpeed = 540f;
    public Vector3 cardHover;
    public Vector3 cardSelect;


    [Header("shop controller settings")]
    public bool inShop;
    public int shopPosition;
    private ShopManager theSM;
    private bool isSelected;
    private Collider theCol;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupPack()
    {
        packName = packData.packName;
        nameText.text = packName;
    }
    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRotation = rotToMatch;
    }

    public void OnTriggerOpen()
    {
      tearAnim.SetTrigger("openPack");
      cardAnim.SetTrigger("pack_open");
    }
}
