using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class PlayGame : NetworkBehaviour
{
    private GameObject PL_uparupa;
    private GameObject PL_mendako;
    private GameObject manageGrid;
    private GameObject player;
    private PlayerState playerState;
    private NetworkRunner runner;
    void Awake()
    {
        Debug.Log("Awake SC_Game");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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

        PL_uparupa.GetComponent<PlayerState>().toStartMyTurn();
        PL_mendako.GetComponent<PlayerState>().toNoMyTurn();


        // 自分のプレイヤーオブジェクトを取得
        GameObject[] runners = GameObject.FindGameObjectsWithTag("Runner");
        runner = runners[0].GetComponent<NetworkRunner>();
        foreach (GameObject g in runners)
        {
            if (g.GetComponent<NetworkRunner>().IsRunning)
            { //アクティブのものを検出
                runner = g.GetComponent<NetworkRunner>();
                break;
            }
        }
        if (runner.TryGetPlayerObject(runner.LocalPlayer, out var plObject))
        {
            player = plObject.gameObject;
        }
        //コンポネント取得
        playerState = player.GetComponent<PlayerState>();
        //カメラ設定
        CameraSetting.SetCamera(playerState.getsetTeam == PlayerState.Team.uparupa);

    }
}
