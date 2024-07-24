using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGame : NetworkBehaviour
{
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
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerState.OnChangeMode -= ChangeToMyTurn; // イベントから登録解除
        PlayerState.OnChangeMode -= EndGameChecker;
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
        // ウパルパターン
        this.gameObject.GetComponent<GameUI>().ChangeTurn(true, myplayer == nowPlayer);
    }

    // selectMode変更で同期実行されるイベントで発火。
    void ChangeToMyTurn()
    {
        PlayerState.SelectMode mymode = myplayer.GetComponent<PlayerState>().selectMode;
        PlayerState.SelectMode partnermode = partnerplayer.GetComponent<PlayerState>().selectMode;

        Debug.Log("mymode " + mymode + " : " + "partnermode " + partnermode);
        Debug.Log(myplayer.GetComponent<PlayerState>().canChangeTurn + "ならチェンジターン可能");

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
        }
        else if (
            nowPlayer == myplayer
            && mymode == PlayerState.SelectMode.NoMyTurn
            && partnermode == PlayerState.SelectMode.NoMyTurn
        )
        { //次は相手ターン
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
        if (this.gameObject != null && partnerplayer != null)
        {
            this.gameObject.GetComponent<GameUI>()
                        .ChangeTurn(
                            partnerplayer.GetComponent<PlayerState>().team == PlayerState.Team.uparupa, false); //相手を大きく
            playerState.isLateAnim = false;
        }
        else
        {
            GameUI.endGame = true;
            // 相手が勝利
            ResultUI.win = false;
            destroyProcess = true;
            Debug.Log("存在しないので終了処理へ");
        }

    }

    /// <summary>
    /// 指定位置の駒を検索 自分の駒:1 相手の駒:2 なし:0
    /// </summary>
    public int SearchPieceByPos(Vector2Int posID)
    {
        Debug.Log(myplayer.GetComponent<ManagePiece>().pieceDic.Count+" 自分駒数");
        Debug.Log(partnerplayer.GetComponent<ManagePiece>().pieceDic.Count+" 相手駒数");

        if (myplayer.GetComponent<ManagePiece>().pieceDic.ContainsKey(posID))
            return 1;
        else if (partnerplayer.GetComponent<ManagePiece>().syncDic.ContainsKey(posID)) //TODO:0になる
            return 2;
        else
            return 0;
    }

    /// <summary>
    /// 相手の駒の偽物を探す
    /// </summary>
    public void SearchRealFromPartner()
    {
        foreach (var dic in partnerplayer.GetComponent<ManagePiece>().syncDic)
        {
            if (dic.Value == false)
            {
                playerState.fakePosID = dic.Key;
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

            Debug.Log("リザルトシーンへ");
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
}
