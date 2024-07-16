using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーの状態
/// </summary>
public class PlayerState : NetworkBehaviour
{
    // ウパルパ陣営かメンダコ陣営か
    public enum Team
    {
        uparupa,
        mendako,
    }

    [Networked]
    public Team team { get; set; }

    /// <summary> 状態遷移 </summary>
    public enum SelectMode
    {
        SetPiece, // 駒選択中
        SetPosition, //駒を置くマスを選択中
        SetAllPieces, //駒配置完了
        MovePiece, // ゲーム中 動かす駒選択中
        MovePosition, // ゲーム中 移動先のマスを選択中
        NoMyTurn // 相手ターン中
    }

    [Networked, OnChangedRender(nameof(ModeEvent))]
    public SelectMode selectMode { get; set; }

    // 駒がゲットされたときの通知
    [Networked, OnChangedRender(nameof(DespawnPiece))]
    public Vector2Int desPosID { get; set; }

    // モード遷移イベント
    public static event Action OnChangeMode;

    [Networked, OnChangedRender(nameof(OnCanDestroyChanged))]
    public NetworkBool canDestroy { get; set; }

    /// <summary> 駒 </summary>
    public GameObject realUparupaPrehab; // インスペクターで指定
    public GameObject fakeUparupaPrehab; // インスペクターで指定
    public GameObject realMendakoPrehab; // インスペクターで指定
    public GameObject fakeMendakoPrehab; // インスペクターで指定
    private GameObject manager; //SC_SetPiecesのSetPieceManager

    // グリッドマネージャー
    public GameObject manageGrid;

    // private List<GameObject> myPieces;
    // // 場に出ている自陣営の駒 <位置のID, 駒オブジェクト>
    // private Dictionary<Vector2Int, GameObject> activePieces;
    // // 獲得した相手陣営の駒
    // private List<GameObject> getPieces;
    Dictionary<Vector2Int, PieceState> pieceDic;

    // 選択した駒位置
    private Vector2Int piecePos;

    // 選択中の駒
    private PieceState piece;

    // 選択した移動先
    private Vector2Int moveToPos;

    // ネットワークオブジェクト(スポーンしたプレイヤーオブジェクト)
    private NetworkObject playerObj;

    /// <summary>
    /// SC_SetPieces開始時
    /// </summary>
    public void Start()
    {
        // マルチピアモードのとき親がある
        // Debug.Log("このシーンは"+SceneManager.GetActiveScene().name);
        // Debug.Log("親オブジェクトは"+this.gameObject.transform.parent.gameObject.name);
        canDestroy = false;
        DontDestroyOnLoad(this.gameObject);
    }

    public GameObject setManageGrid
    {
        set { manageGrid = value; }
    }
    public GameObject setManager
    {
        set { manager = value; }
    }

    // public void InitArray()
    // {
    //     myPieces = new List<GameObject>();
    //     activePieces = new Dictionary<Vector2Int, GameObject>();
    // }

    // public List<GameObject> getsetMyPieces
    // {
    //     get { return myPieces; }
    //     set { myPieces = value; }
    // }

    public Vector2Int getPiecePosition
    {
        get { return piecePos; }
    }

    public Vector2Int getMoveToPos
    {
        get { return moveToPos; }
    }

    // public NetworkObject getsetObject
    // {
    //     get { return playerObj; }
    //     set { playerObj = value; }
    // }

    void OnCanDestroyChanged()
    {
        GameObject
            .FindGameObjectWithTag("GameManager")
            .GetComponent<PlayGame>()
            .OnCanDestroyChanged();
    }

    /// <summary>
    /// 状態遷移メソッド
    /// </summary>

    /// <summary>
    /// 駒選択開始に
    /// </summary>
    public void toStartSetPieces()
    {
        selectMode = SelectMode.SetPiece;
    }

    /// <summary>
    /// 配置マス選択に
    /// </summary>
    public void toSelectSetPosition(Vector2Int posID)
    {
        piecePos = posID;
        selectMode = SelectMode.SetPosition;
    }

    /// <summary>
    /// 配置駒の移動をして駒選択状態に
    /// </summary>
    public void toMoveSetPosition(Vector2Int posID)
    {
        moveToPos = posID;
        // 配列の登録変更
        //activePieces.Add(moveToPos, piece); //選択位置で登録 //pieceを設定してないからおかしいはず
        ClearAllHighLight(); //ハイライト解除
        toStartSetPieces(); //状態遷移
    }

    /// <summary>
    /// 4駒配置完了状態に
    /// </summary>
    public void toFinishSet()
    {
        Debug.Log(team + "が8駒配置完了");
        // UI差し替え
        this.gameObject.GetComponent<SettingUI>().FinishSetting();
        selectMode = SelectMode.SetAllPieces;
        ClearAllHighLight();
    }

    /// <summary>
    /// ターンではない
    /// </summary>
    public void toNoMyTurn()
    {
        selectMode = SelectMode.NoMyTurn;
    }

    /// <summary>
    /// ターン開始時に
    /// </summary>
    public void toStartMyTurn()
    {
        selectMode = SelectMode.MovePiece;
    }

    /// <summary>
    /// 駒選択した時の処理
    /// </summary>
    public void toSelectPiece(Vector2Int posID)
    {
        piecePos = posID;
        piece = this.gameObject.GetComponent<ManagePiece>().pieceDic[piecePos];
        manageGrid.GetComponent<ManageGrid>().HighLightWASDGrid(piecePos, true);
        Debug.Log("playerState:駒選択中");
        selectMode = SelectMode.MovePosition;
        // Debug.Log("select piece on "+piecePos[0]+","+piecePos[1]);
    }

    /// <summary>
    /// 移動マス選択してターンエンドする時の処理
    /// </summary>
    public void toMovePiece(Vector2Int posID)
    {
        moveToPos = posID;
        // 入れる配列取得
        pieceDic = this.gameObject.GetComponent<ManagePiece>().pieceDic;
        pieceDic.Remove(piecePos); //現在位置の登録解除
        pieceDic.Add(moveToPos, piece); //移動先で登録
        // ID伝えて移動させる
        piece.GetComponent<PieceState>().MovePiecePos(posID);

        Debug.Log("move to " + moveToPos[0] + "," + moveToPos[1]);
        selectMode = SelectMode.NoMyTurn;
    }

    /// <summary>
    /// あらゆるハイライトをクリア
    /// </summary>
    public void ClearAllHighLight()
    {
        pieceDic = this.gameObject.GetComponent<ManagePiece>().pieceDic;
        foreach (PieceState piece in pieceDic.Values)
        {
            if (piece != null)
                piece.HighLightPiece(false);
        }
        manageGrid.GetComponent<ManageGrid>().ClearHighLight();
    }

    private void ModeEvent()
    {
        // イベント通知
        OnChangeMode?.Invoke();
        Debug.Log("現在" + team + "は" + selectMode + "です。");
        // 位置同期
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
        foreach (GameObject p in pieces)
        {
            p.GetComponent<PieceState>().SyncPos();
        }
    }

    // 駒がゲットされたときのデスポーン処理
    public void DespawnPiece()
    {
        Debug.Log("メソッドが呼び出されました");
        //if(selectMode != SelectMode.NoMyTurn){
        if (this.gameObject == GameObject.FindWithTag("GameManager").GetComponent<PlayGame>().nowPlayer)
        {
            Debug.Log("取った側");
            //GameObject.FindWithTag("GameManager").GetComponent<PlayGame>().RemovePieceOfDictionary(desPosID);
            return;
        }
        //Debug.Log("メソッドが呼び出されました");
        // runner検出
        GameObject[] runners = GameObject.FindGameObjectsWithTag("Runner");
        NetworkRunner runner = runners[0].GetComponent<NetworkRunner>();
        foreach (GameObject g in runners)
        {
            if (g.GetComponent<NetworkRunner>().IsRunning)
            { //アクティブのものを検出
                runner = g.GetComponent<NetworkRunner>();
                break;
            }
        }
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
        foreach (GameObject desPiece in pieces)
        {
            if (desPiece.GetComponent<PieceState>().posID == desPosID)
            {
                Debug.Log("Piece取得できました");
                NetworkObject pieceNet = desPiece.GetComponent<NetworkObject>();
                if (pieceNet != null)
                {
                    runner.Despawn(pieceNet);
                    // 配列からも削除
                    GameObject.FindWithTag("GameManager").GetComponent<PlayGame>().RemovePieceOfDictionary(desPosID);
                    return;
                }
            }
        }
    }
}
