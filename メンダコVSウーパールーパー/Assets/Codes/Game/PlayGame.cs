using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGame : MonoBehaviour
{
    private GameObject PL_uparupa;
    private GameObject PL_mendako;
    private GameObject manageGrid;
    void Start()
    {
        // プレイヤーオブジェクトを作成
        PL_uparupa = GameObject.FindGameObjectsWithTag("Player")[0];
        PL_mendako = GameObject.FindGameObjectsWithTag("Player")[1];
        Destroy(PL_uparupa.GetComponent<CreatePiece>());
        Destroy(PL_mendako.GetComponent<CreatePiece>());
        Destroy(PL_uparupa.GetComponent<SettingUI>());
        Destroy(PL_mendako.GetComponent<SettingUI>());
        // 管理オブジェクト生成
        manageGrid = GameObject.FindGameObjectWithTag("GridSystem");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
