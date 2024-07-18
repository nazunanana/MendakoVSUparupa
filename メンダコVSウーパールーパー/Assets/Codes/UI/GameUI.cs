using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// attach to GameManager
/// </summary>
public class GameUI : MonoBehaviour
{
    public GameObject uparupaIcon;
    public GameObject mendakoIcon;
    public GameObject realPieceNum;
    public GameObject fakePieceNum;
    public GameObject myturn;
    public GameObject noturn;
    public GameObject getCard;

    private bool imUparupa;
    public static bool endGame;
    private Animator animator_myturn;
    private Animator animator_noturn;
    private Animator animator_getCard;

    void Awake()
    {
        animator_myturn = myturn.GetComponent<Animator>();
        animator_noturn = noturn.GetComponent<Animator>();
        animator_getCard = getCard.GetComponent<Animator>();
    }

    public void SetUIPosition(bool imUparupa)
    {
        this.imUparupa = imUparupa;
        (imUparupa ? uparupaIcon : mendakoIcon).GetComponent<RectTransform>().anchoredPosition = new Vector3(-830, -410, 0);
        (imUparupa ? mendakoIcon : uparupaIcon).GetComponent<RectTransform>().anchoredPosition = new Vector3(830, 410, 0);
    }
    public void ChangeTurn(bool upaTurn, bool myturn)
    {
        if (!endGame)
        {
            uparupaIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, upaTurn ? 200 : 120);
            uparupaIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, upaTurn ? 200 : 120);
            mendakoIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, upaTurn ? 120 : 200);
            mendakoIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, upaTurn ? 120 : 200);
            //ターンアニメーション
            if (myturn) animator_myturn.SetTrigger("Anim");
            else animator_noturn.SetTrigger("Anim");
        }
    }
    public void ChangeGetPieceNum(bool real, bool plus)
    {
        int now = 0;
        try
        {
            now = int.Parse((real ? realPieceNum : fakePieceNum).GetComponent<TextMeshProUGUI>().text.Substring(3));
        }
        catch (FormatException)
        {
            Debug.Log("Invalid String");
        }
        if (now < 4)
        {
            now += plus ? 1 : -1;
            (real ? realPieceNum : fakePieceNum).GetComponent<TextMeshProUGUI>().text = (real ? "本物×" : "偽物×") + now.ToString();
        }
        else
        {
            Debug.Log("4駒獲得済みです");
        }

    }
    public void GetPieceUI(){
        animator_getCard.SetTrigger("Anim");
    }
}
