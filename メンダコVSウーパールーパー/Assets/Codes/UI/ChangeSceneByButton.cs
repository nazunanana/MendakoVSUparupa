using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// ボタンのテキストで画面遷移 > 文字ボタンオブジェクト
/// </summary>
public class ChangeSceneByButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI buttonText;
    private string textValue;
    // Start is called before the first frame update
    void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            // テキストの文字列を取得
            textValue = buttonText.text;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in child objects.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPushedButton()
    {
        Debug.Log("シーン遷移");
        if (textValue == "遊び方")
        {
            SceneManager.LoadScene("SC_Tutorial");
        }
        else if (textValue == "スタート")
        {
            SceneManager.LoadScene("SC_EnterRoom");
        }
        else if (textValue == "設定")
        {
            SceneManager.LoadScene("SC_Setting");
        }
        else if (textValue == "戻る")
        {
            SceneManager.LoadScene("SC_Start");
        }
        else if (textValue == "入室")
        {
            SceneManager.LoadScene("SC_Ready");
        }
        else if (textValue == "Ready")
        {
            SceneManager.LoadScene("SC_SetPieces");
        }
        else if (textValue == "タイトルへ")
        {
            SceneManager.LoadScene("SC_Start");
        }
        else if (textValue == "もう一度")
        {
            SceneManager.LoadScene("SC_Ready");
        }
        // [debug]
        else if (textValue == "ゲームシーンへ")
        {
            SceneManager.LoadScene("SC_Game");
        }
        else if (textValue == "リザルトへ")
        {
            SceneManager.LoadScene("SC_Result");
        }

        //Debug.Log("Button text: " + textValue);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("反転");
        buttonText.color = Color.black;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("反転");
        buttonText.color = Color.white;
    }

}
