using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class PlayGame : NetworkBehaviour
{
    // private GameObject PL_uparupa;
    // private GameObject PL_mendako;
    public static event Action OnCreateDicComplete;
    private GameObject manageGrid;
    private GameObject myplayer;
    private GameObject partnerplayer;
    private GameObject nowPlayer;
    private PlayerState playerState;
    private NetworkRunner runner;
    private const int GRID_NUM = 6;

    void Awake()
    {
        Debug.Log("Awake SC_Game");
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayerState.OnChangeMode += ChangeToMyTurn;
        PlayerState.OnChangeMode += EndGameChecker;
    }
    void OnDestroy()
    {
        PlayerState.OnChangeMode -= ChangeToMyTurn; // イベントから登録解除
        PlayerState.OnChangeMode -= EndGameChecker;

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
        // // 相手のIDで辞書作成
        // Debug.Log("相手辞書作成");
        // myplayer.GetComponent<ManagePiece>().CreateDic(partnerplayer.GetComponent<ManagePiece>().IDlist);
        OnCreateDicComplete?.Invoke();
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
            this.gameObject.GetComponent<GameUI>().ChangeTurn(myplayer.GetComponent<PlayerState>().team == PlayerState.Team.uparupa); //自分を大きく
            nowPlayer = myplayer;
        }
        else if (mymode == PlayerState.SelectMode.NoMyTurn && partnermode == PlayerState.SelectMode.NoMyTurn)
        {
            nowPlayer = partnerplayer;
            this.gameObject.GetComponent<GameUI>().ChangeTurn(partnerplayer.GetComponent<PlayerState>().team == PlayerState.Team.uparupa); //相手を大きく
        }
    }

    /// <summary>
    /// 前後左右の駒を検索
    /// </summary>
    public int[] SearchWASD(Vector2Int posID)
    {
        int id_x = posID.x;
        int id_z = posID.y;
        int w = -1, a = -1, s = -1, d = -1;

        if (0 <= id_x - 1) w = SearchPieceByPos(new Vector2Int(id_x - 1, id_z)); // 上のマス
        if (0 <= id_z - 1) a = SearchPieceByPos(new Vector2Int(id_x, id_z - 1)); // 左のマス
        if (id_x + 1 < GRID_NUM) s = SearchPieceByPos(new Vector2Int(id_x + 1, id_z)); // 下のマス
        if (id_z + 1 < GRID_NUM) d = SearchPieceByPos(new Vector2Int(id_x, id_z + 1)); // 右のマス
        return new int[] { w, a, s, d }; //上左下右 -1:範囲外 0:null 1:自陣の駒 2:相手の駒
    }
    /// <summary>
    /// 指定位置の駒を検索
    /// </summary>
    public int SearchPieceByPos(Vector2Int posID)
    {
        if (myplayer.GetComponent<ManagePiece>().pieceDic.ContainsKey(posID)) return 1;
        else if (partnerplayer.GetComponent<ManagePiece>().syncDic.ContainsKey(posID)) return 2;
        else return 0;
    }

    public void EndGameChecker()
    {
        //TODO: 脱出駒に入ったらの条件がない
        if (myplayer.GetComponent<ManagePiece>().EndGameCounter(true) || partnerplayer.GetComponent<ManagePiece>().EndGameCounter(false))
        {
            // 自分が勝利
            ResultUI.win = true;
            SceneManager.LoadScene("SC_Result");
        }
        else if (partnerplayer.GetComponent<ManagePiece>().EndGameCounter(true) || myplayer.GetComponent<ManagePiece>().EndGameCounter(false))
        {
            // 相手が勝利
            ResultUI.win = false;
            SceneManager.LoadScene("SC_Result");
        }
    }

    IEnumerator WaitLoading(float time)
    {
        // 待つ
        yield return new WaitForSeconds(time);
    }
}

