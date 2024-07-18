using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardState : MonoBehaviour
{
    // カード番号
    [SerializeField] private int cardNum;
    private GameObject player;
    private PlayerState playerComp;
    private ManageCard manageComp;
    //  このターンにカードを使ったかどうか
    public bool canUse { get; set; }

    public GameObject SetPlayer
    {
        set
        {
            player = value;
            playerComp = player.GetComponent<PlayerState>();
        }
    }
    void Start()
    {
        manageComp = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ManageCard>();
        canUse = true;
    }


    public void OnImageClick() // カードがクリックされたとき
    {
        // 自分の行動中でないときはクリックできない
        if ((playerComp.selectMode != PlayerState.SelectMode.MovePiece)
            || canUse == false)
        {
            return;
        }
        canUse = false;
        Debug.Log("カードクリック");
        manageComp.ActiveCard(cardNum);

    }
}
