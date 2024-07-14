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
    private GameObject nowPlayer;
    private PlayerState playerState;
    private NetworkRunner runner;
    void Awake()
    {
        Debug.Log("Awake SC_Game");
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayerState.OnChangeMode += ChangeToMyTurn;
    }
    void OnDestroy()
    {
        PlayerState.OnChangeMode -= ChangeToMyTurn; // イベントから登録解除
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
        Destroy(myplayer.GetComponent<SettingUI>());
        Destroy(partnerplayer.GetComponent<SettingUI>());
        // 管理オブジェクト検索
        manageGrid = GameObject.FindGameObjectWithTag("GridSystem");

        if (playerState.team == PlayerState.Team.uparupa)
        {
            //カメラ設定
            CameraSetting.SetCamera(true);
            this.gameObject.GetComponent<GameUI>().SetUIPosition(true);
            myplayer.GetComponent<PlayerState>().toStartMyTurn();
            partnerplayer.GetComponent<PlayerState>().toNoMyTurn();
            nowPlayer = myplayer;
        }
        else
        {
            //カメラ設定
            CameraSetting.SetCamera(false);
            this.gameObject.GetComponent<GameUI>().SetUIPosition(false);
            myplayer.GetComponent<PlayerState>().toNoMyTurn();
            partnerplayer.GetComponent<PlayerState>().toStartMyTurn();
            nowPlayer = partnerplayer;
        }
        // ウパターン
        this.gameObject.GetComponent<GameUI>().ChangeTurn(true);
    }

    void ChangeToMyTurn()
    {
        //Debug.Log("ターン遷移！");
        PlayerState.SelectMode mymode = myplayer.GetComponent<PlayerState>().selectMode;
        PlayerState.SelectMode partnermode = partnerplayer.GetComponent<PlayerState>().selectMode;
        WaitLoading(0.5f);
        //Debug.Log("nowname:相手name" + (nowPlayer.name) +":"+ partnerplayer.name);
        //Debug.Log("自分がnoturn" + (mymode == PlayerState.SelectMode.NoMyTurn));
        //Debug.Log("相手がnoturn" + (partnermode == PlayerState.SelectMode.NoMyTurn));

        // ターン遷移 相手ターンかつ両者がターン終了状態なら自分のターン開始
        if (nowPlayer == partnerplayer && mymode == PlayerState.SelectMode.NoMyTurn && partnermode == PlayerState.SelectMode.NoMyTurn)
        {
            myplayer.GetComponent<PlayerState>().toStartMyTurn();
            nowPlayer = myplayer;
        }else if(mymode == PlayerState.SelectMode.NoMyTurn && partnermode == PlayerState.SelectMode.NoMyTurn){
            nowPlayer = partnerplayer;
        }
    }
    IEnumerator WaitLoading(float time)
    {
        // 待つ
        yield return new WaitForSeconds(time);
    }
}

