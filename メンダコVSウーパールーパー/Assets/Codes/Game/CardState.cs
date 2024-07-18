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

    // public GameObject SetPlayer
    // {
    //     set
    //     {
    //         player = value;
    //         playerComp = player.GetComponent<PlayerState>();
    //     }
    // }
    void Start()
    {
        manageComp = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ManageCard>();
        canUse = true;
        playerComp = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayGame>().myplayer.GetComponent<PlayerState>();
    }


    public void OnImageClick() // カードがクリックされたとき
    {
        // 自分の行動中でないときはクリックできない
        Debug.Log("CardState playerComp "+playerComp.selectMode);
        if ((playerComp.selectMode != PlayerState.SelectMode.MovePiece)
            || canUse == false)
        {
            Debug.Log("現在はカードを使用できません");
            return;
        }
        canUse = false;
        Debug.Log("カードをクリックしました");

        manageComp.ActiveCard(cardNum);

    }
}
