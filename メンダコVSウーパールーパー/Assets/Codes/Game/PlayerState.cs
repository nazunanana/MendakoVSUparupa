using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// プレイヤーの状態
/// </summary>
public class PlayerState : MonoBehaviour
{
    // ウパルパ陣営かメンダコ陣営か
    public enum Team{
        uparupa,
        mendako,
    }
    private Team team;
    /// <summary> 状態遷移 </summary>
    public enum SelectMode{
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
    // 自陣営の駒生成
    private GameObject piece;
    private List<GameObject> myPieces;
    // 場に出ている自陣営の駒 <位置のID, 駒オブジェクト>
    private Dictionary<Vector2Int, GameObject> activePieces;
    // 獲得した相手陣営の駒
    private List<GameObject> getPieces;
    
    /// <summary> ウパルパ陣営のカメラ位置、向き（定数） </summary>
    private const int UPA_CAMERA_POSITION_Z = -13;
    private const int UPA_CAMERA_ROTATION_Y = 0;
    /// <summary> メンダコ陣営のカメラ位置、向き（定数） </summary>
    private const int MEN_CAMERA_POSITION_Z = 13;
    private const int MEN_CAMERA_ROTATION_Y = 180;

    // 選択した駒位置
    private Vector2Int piecePos;
    // 選択した移動先
    private Vector2Int moveToPos;
    

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void Setting(Team team){
        myPieces = new List<GameObject>();
        activePieces = new Dictionary<Vector2Int, GameObject>();
        if(team==Team.uparupa){
            for(int i=0; i<4; i++){
                piece = Instantiate(realUparupaPrehab, realUparupaPrehab.transform.position, Quaternion.identity);
                myPieces.Add(piece);
                activePieces.Add(new Vector2Int(2*i,1), piece);
                piece = Instantiate(fakeUparupaPrehab, fakeUparupaPrehab.transform.position, Quaternion.identity);
                myPieces.Add(piece);
                activePieces.Add(new Vector2Int(2*i+1,1), piece);
            }
            selectMode = SelectMode.MovePiece;
        }else{
            for(int i=0; i<4; i++){
                piece = Instantiate(realMendakoPrehab, realMendakoPrehab.transform.position, Quaternion.identity);
                myPieces.Add(piece);
                activePieces.Add(new Vector2Int(2*i,1), piece);
                piece = Instantiate(fakeMendakoPrehab, fakeMendakoPrehab.transform.position, Quaternion.identity);
                myPieces.Add(piece);
                activePieces.Add(new Vector2Int(2*i+1,1), piece);
            }
            selectMode = SelectMode.NoMyTurn;
        }
    }
    public Team getsetTeam{
        get{ return team; }
        set{
            team = value;
        }
    }

    public int getsetRealPoint{
        get{ return realPoint; }
        set{ realPoint = value; }
    }

    public int getsetFakePoint{
        get{ return fakePoint; }
        set{ fakePoint = value; }
    }

    public Vector2Int getPiecePosition{
        get{ return piecePos; }
    }

    public Vector2Int getMoveToPos{
        get{ return moveToPos; }
    }

    public SelectMode getsetSelectMode{
        get{ return selectMode; }
        set{ selectMode = value; }
    }



    /// <summary>
    /// 状態遷移メソッド
    /// </summary>

    /// <summary>
    /// 駒選択開始に
    /// </summary>
    public void toStartSetPieces(Vector2Int posID){
        selectMode = SelectMode.SetPiece;
    }
    /// <summary>
    /// 配置駒選択に
    /// </summary>
    public void toSelectSetPosition(Vector2Int posID){
        piecePos = posID;
        selectMode = SelectMode.SetPosition;
    }
    /// <summary>
    /// 配置駒の移動をして駒選択状態に
    /// </summary>
    public void toMoveSetPosition(Vector2Int posID){
        // 配列の位置を変える
        activePieces.remove(new Vector2Int(2*i,1), piece);
        MovePiece(posID); //移動
        StartSetPieces();
    }

    /// <summary>
    /// ターン開始時に
    /// </summary>
    public void toStartMyTurn(){
        selectMode = SelectMode.MovePiece;
    }
    /// <summary>
    /// 4駒配置完了状態に
    /// </summary>
    public void toFinishSet(){
        selectMode = SelectMode.SetAllPieces;
    }

    /// <summary>
    /// 駒選択した時の処理
    /// </summary>
    public void toSelectPiece(Vector2Int posID){
        piecePos = posID;
        selectMode = SelectMode.MovePosition;
        Debug.Log("select piece on "+piecePos[0]+","+piecePos[1]);
    }
    
    /// <summary>
    /// 移動マス選択してターンエンドする時の処理
    /// </summary>
    public void toMovePiece(Vector2Int posID){
        moveToPos = posID;
        Debug.Log("move to "+moveToPos[0]+","+moveToPos[1]);
        selectMode = SelectMode.NoMyTurn;
    }
}