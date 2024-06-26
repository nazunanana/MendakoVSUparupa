using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

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
        else if (textValue == "部屋を立てる")
        {
            SceneManager.LoadScene("SC_CreateRoom");
        }
        else if (textValue == "部屋に入る")
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
        else if (textValue == "作成")
        {
            SceneManager.LoadScene("SC_Game");
        }
        else if (textValue == "入室")
        {
            SceneManager.LoadScene("SC_Game");
        }
        Debug.Log("Button text: " + textValue);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("反転");
        buttonText.color = Color.black;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("反転");
        buttonText.color = Color.white;
    }

}
