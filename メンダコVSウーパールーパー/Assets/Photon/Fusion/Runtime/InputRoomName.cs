using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputRoomName : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputText;

    [SerializeField]
    private Button button;

    void Start()
    {
        button.interactable = false;
    }

    public void InputPassword()
    {
        StaticData.roomName = inputText.text;
        // 入力欄が空欄の時
        if (string.IsNullOrEmpty(StaticData.roomName))
        {
            button.interactable = false;
            return;
        }else{
            button.interactable = true;
        }
    }
}
