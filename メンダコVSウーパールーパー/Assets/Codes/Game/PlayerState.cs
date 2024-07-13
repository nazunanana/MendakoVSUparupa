using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using Fusion;

/// <summary>
/// プレイヤーの状態
/// </summary>
public class PlayerState : MonoBehaviour
{
    // ウパルパ陣営かメンダコ陣営か
    public enum Team
    {
        uparupa,
        mendako,
    }
    public Team team;
    /// <summary> 状態遷移 </summary>
    public enum SelectMode
    {
        SetPiece, // 駒選択中
        SetPosition, //駒配置マス選択中
        SetAllPieces, //駒配置完了
        MovePiece, // ゲーム中 動かす駒選択中
        MovePosition, // ゲーム中 動かすマス選択中
        NoMyTurn // 相手ターン中
    }
    public SelectMode selectMode;

    /// <summary> 得点 </summary>
    // 本物得点 (相手の本物を取ると+1)
    private int realPoint = 0;
    // 偽物得点 (相手の偽物を取ると+1)
    private int fakePoint = 0;

    /// <summary> 駒 </summary>
    public GameObject realUparupaPrehab; // インスペクターで指定
    public GameObject fakeUparupaPrehab; // インスペクターで指定
    public GameObject realMendakoPrehab; // インスペクターで指定
    public GameObject fakeMendakoPrehab; // インスペクターで指定
    private GameObject manager; //SC_SetPiecesのSetPieceManager
    // 自陣営の駒生成
    private GameObject piece;
    // グリッドマネージャー
    private GameObject manageGrid;
    private List<GameObject> myPieces;
    // 場に出ている自陣営の駒 <位置のID, 駒オブジェクト>
    private Dictionary<Vector2Int, GameObject> activePieces;
    // 獲得した相手陣営の駒
    private List<GameObject> getPieces;

    // 選択した駒位置
    private Vector2Int piecePos;
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
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void StartGameSetting(Team team)
    {
        myPieces = new List<GameObject>();
        activePieces = new Dictionary<Vector2Int, GameObject>();
        if (team == Team.uparupa)
        {
            for (int i = 0; i < 4; i++)
            {
                piece = Instantiate(realUparupaPrehab, realUparupaPrehab.transform.position, Quaternion.identity);
                myPieces.Add(piece);
                activePieces.Add(new Vector2Int(2 * i, 1), piece);
                piece = Instantiate(fakeUparupaPrehab, fakeUparupaPrehab.transform.position, Quaternion.identity);
                myPieces.Add(piece);
                activePieces.Add(new Vector2Int(2 * i + 1, 1), piece);
            }
            selectMode = SelectMode.MovePiece;
            Debug.Log("相手ターン開始");
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                piece = Instantiate(realMendakoPrehab, realMendakoPrehab.transform.position, Quaternion.identity);
                myPieces.Add(piece);
                activePieces.Add(new Vector2Int(2 * i, 1), piece);
                piece = Instantiate(fakeMendakoPrehab, fakeMendakoPrehab.transform.position, Quaternion.identity);
                myPieces.Add(piece);
                activePieces.Add(new Vector2Int(2 * i + 1, 1), piece);
            }
            selectMode = SelectMode.NoMyTurn;
            Debug.Log("自分のターン開始");
        }
    }
    public GameObject setManageGrid
    {
        set { manageGrid = value; }
    }
    public GameObject setManager
    {
        set { manager = value; }
    }
    // 陣営をSC_Readyでスポーン時に決定
    public Team getsetTeam
    {
        get { return team; }
        set { team = value; }
    }
    public void InitArray()
    {
        myPieces = new List<GameObject>();
        activePieces = new Dictionary<Vector2Int, GameObject>();
    }

    public int getsetRealPoint
    {
        get { return realPoint; }
        set { realPoint = value; }
    }

    public List<GameObject> getsetMyPieces
    {
        get { return myPieces; }
        set { myPieces = value; }
    }

    public int getsetFakePoint
    {
        get { return fakePoint; }
        set { fakePoint = value; }
    }

    public Vector2Int getPiecePosition
    {
        get { return piecePos; }
    }

    public Vector2Int getMoveToPos
    {
        get { return moveToPos; }
    }

    public SelectMode getsetSelectMode
    {
        get { return selectMode; }
        set { selectMode = value; }
    }

    public NetworkObject getsetObject
    {
        get { return playerObj; }
        set { playerObj = value; }
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
    { //TODO:どれがどの状況か分かってない！！！
        piecePos = posID;
        piece = activePieces[piecePos]; // 選択中の駒
        selectMode = SelectMode.MovePosition;
        // Debug.Log("select piece on "+piecePos[0]+","+piecePos[1]);
    }

    /// <summary>
    /// 移動マス選択してターンエンドする時の処理
    /// </summary>
    public void toMovePiece(Vector2Int posID)
    {
        moveToPos = posID;
        activePieces.Remove(piecePos); //現在位置の登録解除
        activePieces.Add(moveToPos, piece); //移動先で登録
        Debug.Log("move to " + moveToPos[0] + "," + moveToPos[1]);
        selectMode = SelectMode.NoMyTurn;
    }
    /// <summary>
    /// あらゆるハイライトをクリア
    /// </summary>
    public void ClearAllHighLight()
    {
        foreach (GameObject obj in myPieces)
        {
            if (obj != null) obj.GetComponent<PieceState>().HighLightPiece(false);
        }
        manageGrid.GetComponent<ManageGrid>().ClearHighLight();
    }
}