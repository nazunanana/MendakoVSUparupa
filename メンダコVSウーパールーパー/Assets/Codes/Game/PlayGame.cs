using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGame : NetworkBehaviour
{
    // private GameObject PL_uparupa;
    // private GameObject PL_mendako;
    public static event Action OnCreateDicComplete;
    private ManageGrid manageGrid;
    public GameObject myplayer { get; set; }
    public GameObject partnerplayer { get; set; }
    public GameObject nowPlayer { get; set; }
    private PlayerState playerState;
    public NetworkRunner runner { get; set; }
    private const int GRID_NUM = 6;
    // リザルト遷移許可
    public static bool destroyProcess { get; set; }
    private static bool isAnimationComplete = false;

    void Awake()
    {
        Debug.Log("Awake SC_Game");
        destroyProcess = false;
        isAnimationComplete = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayerState.OnChangeMode += ChangeToMyTurn;
        PlayerState.OnChangeMode += EndGameChecker;
        //AnimationEnd.OnAnimationComplete += OnAnimationComplete;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerState.OnChangeMode -= ChangeToMyTurn; // イベントから登録解除
        PlayerState.OnChangeMode -= EndGameChecker;
        //AnimationEnd.OnAnimationComplete -= DestroyAll;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 自分のプレイヤーオブジェクトを取得
        GameObject[] runners = GameObject.FindGameObjectsWithTag("Runner");
        runner = runners[0].GetComponent<NetworkRunner>();
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

        if (runner == null)
        {
            Debug.LogError("NetworkRunnerが初期化されていません");
            return;
        }
        // コンポネント取得
        playerState = myplayer.GetComponent<PlayerState>();

        // 生成系コンポネントを破棄
        Destroy(myplayer.GetComponent<SettingUI>());
        Destroy(partnerplayer.GetComponent<SettingUI>());
        // 管理オブジェクト検索
        manageGrid = GameObject.FindGameObjectWithTag("GridSystem").GetComponent<ManageGrid>();
        manageGrid.EnableGridColliders(true);
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
        OnCreateDicComplete?.Invoke();
        // ウパターン
        this.gameObject.GetComponent<GameUI>().ChangeTurn(true, myplayer == nowPlayer);
    }

    // selectMode変更で同期実行されるイベントで発火。
    void ChangeToMyTurn()
    {
        PlayerState.SelectMode mymode = myplayer.GetComponent<PlayerState>().selectMode;
        PlayerState.SelectMode partnermode = partnerplayer.GetComponent<PlayerState>().selectMode;
        //StartCoroutine(WaitLoading(0.5f));

        Debug.Log("mymode " + mymode + " : " + "partnermode " + partnermode);
        Debug.Log(myplayer.GetComponent<PlayerState>().canChangeTurn + "ならチェンジターン可能");
        Debug.Log(playerState.team + "のisDespawnは " + playerState.isDespawn);

        // ターン遷移 相手ターンかつ両者がターン終了状態なら自分のターン開始
        if (
            nowPlayer == partnerplayer
            && mymode == PlayerState.SelectMode.NoMyTurn
            && partnermode == PlayerState.SelectMode.NoMyTurn
        )
        { //次は自分ターン
          //アニメーション終了後なら
            Debug.Log("→自分のターン");
            myplayer.GetComponent<PlayerState>().toStartMyTurn();
            // ターン遷移UI
            this.gameObject.GetComponent<GameUI>()
                .ChangeTurn(myplayer.GetComponent<PlayerState>().team == PlayerState.Team.uparupa, true); //自分を大きく
            nowPlayer = myplayer;
            Debug.Log("nowPlayer:team."+nowPlayer.GetComponent<PlayerState>().team);
        }
        else if (
            nowPlayer == myplayer
            && mymode == PlayerState.SelectMode.NoMyTurn
            && partnermode == PlayerState.SelectMode.NoMyTurn
        )
        { //次は相手ターン
            // if (myplayer.GetComponent<PlayerState>().canChangeTurn)
            // {
            Debug.Log("→相手ターン");
            nowPlayer = partnerplayer;
            if (playerState.isLateAnim)
            {
                this.gameObject.GetComponent<GameUI>()
                    .ChangeTurn(
                        partnerplayer.GetComponent<PlayerState>().team == PlayerState.Team.uparupa, false); //相手を大きく
            }
        }
        else { Debug.Log("ターン遷移しない"); }
    }

    public void ChangeTurnUI()
    {
        this.gameObject.GetComponent<GameUI>()
                    .ChangeTurn(
                        partnerplayer.GetComponent<PlayerState>().team == PlayerState.Team.uparupa, false); //相手を大きく
        playerState.isLateAnim = false;
    }

    /// <summary>
    /// 前後左右の駒を検索
    /// </summary>
    public int[] SearchWASD(Vector2Int posID)
    {
        int id_x = posID.x;
        int id_z = posID.y;
        int w = -1,
            a = -1,
            s = -1,
            d = -1;

        if (0 <= id_x - 1)
            w = SearchPieceByPos(new Vector2Int(id_x - 1, id_z)); // 上のマス
        if (0 <= id_z - 1)
            a = SearchPieceByPos(new Vector2Int(id_x, id_z - 1)); // 左のマス
        if (id_x + 1 < GRID_NUM)
            s = SearchPieceByPos(new Vector2Int(id_x + 1, id_z)); // 下のマス
        if (id_z + 1 < GRID_NUM)
            d = SearchPieceByPos(new Vector2Int(id_x, id_z + 1)); // 右のマス
        return new int[] { w, a, s, d }; //上左下右 -1:範囲外 0:null 1:自陣の駒 2:相手の駒
    }

    /// <summary>
    /// 指定位置の駒を検索
    /// </summary>
    public int SearchPieceByPos(Vector2Int posID)
    {
        if (myplayer.GetComponent<ManagePiece>().pieceDic.ContainsKey(posID))
            return 1;
        else if (partnerplayer.GetComponent<ManagePiece>().syncDic.ContainsKey(posID))
            return 2;
        else
            return 0;
    }

    public void SearchRealFromPartner()
    {
        foreach (var dic in partnerplayer.GetComponent<ManagePiece>().syncDic)
        {
            if (dic.Value == true)
            {
                playerState.realPosID = dic.Key;
                break;
            }
        }
    }

    /// <summary>
    /// 指定位置にある駒のDictionaryを削除(取られた側のみ)
    /// </summary>
    public void RemovePieceOfDictionary(Vector2Int posID)
    {
        myplayer.GetComponent<ManagePiece>().pieceDic.Remove(posID);
        myplayer.GetComponent<ManagePiece>().syncDic.Remove(posID);
        Debug.Log(playerState.team + "の駒は残り:" + myplayer.GetComponent<ManagePiece>().syncDic.Count);
    }

    public bool IsRealPiece(Vector2Int posID)
    {
        foreach (var dic in myplayer.GetComponent<ManagePiece>().pieceDic)
        {
            if (dic.Key == posID)
            {
                return dic.Value.isReal;
            }
        }
        return false;
    }

    public void EndGameChecker()
    {
        //TODO: 脱出駒に入ったらの条件がない
        if (
            myplayer.GetComponent<ManagePiece>().EndGameCounter(true)
            || partnerplayer.GetComponent<ManagePiece>().EndGameCounter(false)
        )
        {
            GameUI.endGame = true;
            // 自分が勝利
            ResultUI.win = true;
            destroyProcess = true;
        }
        else if (
            partnerplayer.GetComponent<ManagePiece>().EndGameCounter(true)
            || myplayer.GetComponent<ManagePiece>().EndGameCounter(false)
        )
        {
            GameUI.endGame = true;
            // 相手が勝利
            ResultUI.win = false;
            destroyProcess = true;
        }
    }

    // private void OnAnimationComplete() //Anim終了時毎回
    // {
    //     isAnimationComplete = true;
    //     CheckAndDestroy();
    // }

    // canDestroyが変更されたらプレイヤーから同期実行()
    public void OnCanDestroyChanged()
    {
        isAnimationComplete = true;
        CheckAndDestroy();
    }

    /// <summary>
    /// シーン遷移時にデストロイ
    /// </summary>
    private void CheckAndDestroy()
    {
        if (destroyProcess && isAnimationComplete)
        {
            Debug.Log("全部削除");
            runner.Shutdown();
            foreach (PieceState p in myplayer.GetComponent<ManagePiece>().pieceDic.Values)
            {
                Destroy(p.gameObject);
            }
            myplayer
                .GetComponent<PlayerState>()
                .manageGrid.GetComponent<ManageGrid>()
                .DestroyGrids();
            Destroy(myplayer.GetComponent<PlayerState>().manageGrid);
            Destroy(myplayer);

            // WaitLoading(1.0f);
            // runner.Shutdown();
            Debug.Log("シーン遷移");
            isAnimationComplete = false;
            SceneManager.LoadScene("SC_Result");
        }
        else
        {
            // まだ終了条件満たしてない
            isAnimationComplete = false;
        }
    }

    /// <summary>
    /// シーン遷移時にデストロイ
    /// </summary>
    // private void OnAnimationComplete()
    // {
    //     Debug.Log("全部削除");
    //     foreach (PieceState p in myplayer.GetComponent<ManagePiece>().pieceDic.Values)
    //     {
    //         Destroy(p.gameObject);
    //     }
    //     Destroy(myplayer);
    //     runner.Shutdown();
    //     // シーン遷移
    //     Debug.Log("シーン遷移");
    //     SceneManager.LoadScene("SC_Result");
    // }
    
    /// <summary>
    /// 駒獲得時、相手のそのマスの駒チームで分岐
    /// 獲得数増やす→Animation
    /// </summary>
    public void GetPieceAction(Vector2Int posID)
    {
        bool real = partnerplayer.GetComponent<ManagePiece>().syncDic.Get(posID);
        if (real)
        {
            Debug.Log("本物駒を獲得");
            myplayer.GetComponent<ManagePiece>().getRealPieceNum++;
            // 相手側で発火するのでそこでアニメーションしてもらう
            // UI変化
            this.gameObject.GetComponent<GameUI>().ChangeGetPieceNum(real, true);
        }
        else
        {
            Debug.Log("偽物駒を獲得");
            myplayer.GetComponent<ManagePiece>().getFakePieceNum++;
            this.gameObject.GetComponent<GameUI>().ChangeGetPieceNum(real, true);
        }
    }

    IEnumerator WaitLoading(float time)
    {
        // 待つ
        yield return new WaitForSeconds(time);
    }

}
