using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// 駒配置シーンでの個人UI遷移 > Playerオブジェクト
/// </summary>
public class SettingUI : MonoBehaviour
{
    private GameObject UI_message;
    private GameObject UI_finishMessage;
    private GameObject UI_real;
    private GameObject UI_fake;
    public void setUIObject(GameObject[] array){
        this.UI_message = array[0];
        this.UI_finishMessage = array[1];
        this.UI_real = array[2];
        this.UI_fake = array[3];
    }

    public void DecreasePieceNum(bool real)
    {
        int now = 0;
        try
        {
            now = int.Parse((real ? UI_real : UI_fake).GetComponent<TextMeshProUGUI>().text.Substring(3));
        }
        catch (FormatException)
        {
            Debug.Log("Invalid String");
        }
        if (now > 0)
        {
            (real ? UI_real : UI_fake).GetComponent<TextMeshProUGUI>().text = (real ? "本物×" : "偽物×") + (now - 1).ToString();
        }
        else
        {
            Debug.Log("駒がもうありません！");
        }
    }
    public void FinishSetting()
    {
        UI_message.SetActive(false);
        UI_finishMessage.SetActive(true);
    }
}
