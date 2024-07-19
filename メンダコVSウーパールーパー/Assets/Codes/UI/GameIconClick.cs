using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// イラストアイコンのクリックをsprite名で判別して画面遷移 > アイコンボタンオブジェクト
/// </summary>
public class GameIconClick : MonoBehaviour
{
    private Button buttonComponent;
    private Image imageComponent;
    private string imageName;
    // Start is called before the first frame update
    void Start()
    {
        imageComponent = GetComponent<Image>();
        buttonComponent = GetComponent<Button>();

        if (imageComponent == null || buttonComponent == null)
        {
            Debug.LogError("conmponents not found on this GameObject.");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnImageClick()
    {
        string spriteName = imageComponent.sprite.name;
        Debug.Log("Image clicked: " + spriteName);

        switch (spriteName)
        {
            // case "設定アイコン":
            //     SceneManager.LoadScene("SC_Start");
            //     break;
            case "カードアイコン":
                GameObject
                .FindGameObjectWithTag("GameManager")
                .GetComponent<ManageCard>()
                .Click();
                break;
            default:
                Debug.LogWarning("Unknown icon: " + spriteName);
                break;
        }
    }
}
