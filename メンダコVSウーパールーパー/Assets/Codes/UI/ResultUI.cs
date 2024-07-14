using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    public GameObject UI_result;
    public static bool win;

    void Start(){
        UI_result.GetComponent<TextMeshProUGUI>().text = win?"You Win!!!":"You Lose...";
    }
}
