using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    public GameObject uparupaIcon;
    public GameObject mendakoIcon;
    public GameObject realPieceNum;
    public GameObject fakePieceNum;

    private bool imUparupa;

    public void SetUIPosition(bool imUparupa)
    {
        this.imUparupa = imUparupa;
        (imUparupa ? uparupaIcon : mendakoIcon).GetComponent<RectTransform>().anchoredPosition = new Vector3(-830, -410, 0);
        (imUparupa ? mendakoIcon : uparupaIcon).GetComponent<RectTransform>().anchoredPosition = new Vector3(830, 410, 0);
    }
    public void ChangeTurn(bool upaTurn)
    {
        uparupaIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (upaTurn ? 200 : 120));
        uparupaIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (upaTurn ? 200 : 120));
        mendakoIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (upaTurn ? 120 : 200));
        mendakoIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (upaTurn ? 120 : 200));
    }
    public void ChangeGetPieceNum(bool real, bool plus)
    {

    }
}
