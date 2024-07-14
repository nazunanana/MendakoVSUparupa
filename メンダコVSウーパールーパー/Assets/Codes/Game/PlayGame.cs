using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class PlayGame : NetworkBehaviour
{
    // private GameObject PL_uparupa;
    // private GameObject PL_mendako;
    private GameObject manageGrid;
    private GameObject myplayer;
    private GameObject partnerplayer;
    private PlayerState playerState;
    private NetworkRunner runner;
    void Awake()
    {
        Debug.Log("Awake SC_Game");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 自分のプレイヤーオブジェクトを取得
        GameObject[] runners = GameObject.FindGameObjectsWithTag("Runner");
        NetworkRunner runner = runners[0].GetComponent<NetworkRunner>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
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
            foreach (GameObject pl in players)
            {
                if (pl.name == plObject.gameObject.name)
                {
                    myplayer = plObject.gameObject;
                }
                else
                {
                    partnerplayer = pl;
                }
            }
        }
        // コンポネント取得
        PlayerState playerState = myplayer.GetComponent<PlayerState>();

        // 生成系コンポネントを破棄
        Destroy(myplayer.GetComponent<CreatePiece>());
        Destroy(partnerplayer.GetComponent<CreatePiece>());
        Destroy(myplayer.GetComponent<SettingUI>());
        Destroy(partnerplayer.GetComponent<SettingUI>());
        // 管理オブジェクト検索
        manageGrid = GameObject.FindGameObjectWithTag("GridSystem");

        if (playerState.getsetTeam == PlayerState.Team.uparupa)
        {
            //カメラ設定
            CameraSetting.SetCamera(true);
            this.gameObject.GetComponent<GameUI>().SetUIPosition(true);
            myplayer.GetComponent<PlayerState>().toStartMyTurn();
            partnerplayer.GetComponent<PlayerState>().toNoMyTurn();
        }
        else
        {
            //カメラ設定
            CameraSetting.SetCamera(false);
            this.gameObject.GetComponent<GameUI>().SetUIPosition(false);
            myplayer.GetComponent<PlayerState>().toNoMyTurn();
            partnerplayer.GetComponent<PlayerState>().toStartMyTurn();
        }
        // ウパターン
        this.gameObject.GetComponent<GameUI>().ChangeTurn(true);
    }
}
