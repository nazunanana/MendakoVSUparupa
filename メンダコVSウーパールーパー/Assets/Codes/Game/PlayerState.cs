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
    // 本物得点 (相手の本物を取ると+1)
    private int realPoint = 0;
    // 偽物得点 (相手の偽物を取ると+1)
    private int fakePoint = 0;
    // ウパルパ陣営のカメラ位置、向き（定数）
    private const int UPA_CAMERA_POSITION_Z = -13;
    private const int UPA_CAMERA_ROTATION_Y = 0;
    // メンダコ陣営のカメラ位置、向き（定数）
    private const int MEN_CAMERA_POSITION_Z = 13;
    private const int MEN_CAMERA_ROTATION_Y = 180;
    private bool isMyTurn;
    // 駒を選択中
    private bool isSelectingPiece;
    // 移動マスを選択中
    private bool isSelectingMove;
    // 選択した駒位置
    private Vector2Int piecePos;
    // 選択した移動先
    private Vector2Int moveToPos;
    
    public void Init(Team team){
        if(team==Team.uparupa){
            this.isMyTurn = true;
            this.isSelectingPiece = true;
        }else{
            this.isMyTurn = false;
            this.isSelectingPiece = false;
        }
        this.isSelectingMove = false;
    }
    public Team setgetTeam{
        get{ return team; }
        set{
            team = value;
            Init(team);
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

    public bool getisSelectingPiece{
        get{ return isSelectingPiece; }
    }
    public bool getisSelectingMove{
        get{ return isSelectingMove; }
    }


    // ターン開始
    public void StartMyTurn(){
        isSelectingPiece = true;
        isSelectingMove = false;
    }
    // 駒選択した
    public void SelectPiece(Vector2Int posID){
        piecePos = posID;
        isSelectingPiece = false;
        isSelectingMove = true;
        Debug.Log("select piece on "+piecePos[0]+","+piecePos[1]);
    }
    // 移動マス選択したターンエンド
    public void MovePiece(Vector2Int posID){
        moveToPos = posID;
        Debug.Log("move to "+moveToPos[0]+","+moveToPos[1]);
        isSelectingPiece = false;
        isSelectingMove = false;
    }
}