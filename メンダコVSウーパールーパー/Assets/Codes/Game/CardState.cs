using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardState : MonoBehaviour
{
    // カード番号
    [SerializeField] private int cardNum;
    private PlayerState playerComp;
    private ManageCard manageComp;
    //  このターンにカードを使ったかどうか
    public bool canUse { get; set; }


    void Start()
    {
        manageComp = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ManageCard>();
        canUse = true;
        playerComp = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayGame>().myplayer.GetComponent<PlayerState>();
    }


    public void OnImageClick() // カードがクリックされたとき
    {
        // 自分の行動中でないときはクリックできない
        Debug.Log("CardState "+manageComp.card);
        if ((playerComp.selectMode != PlayerState.SelectMode.MovePiece)
            || canUse == false)
        {
            Debug.Log("現在はカードを使用できません");
            return;
        }
        canUse = false;
        Debug.Log("カードクリック");
        manageComp.ActiveCard(cardNum);

    }
}
