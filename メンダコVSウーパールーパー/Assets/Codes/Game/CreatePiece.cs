using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 駒生成 > PLオブジェクト
/// </summary>
public class CreatePiece : MonoBehaviour
{
    /// <summary> 駒 </summary>
    public GameObject realUparupaPrehab; // インスペクターで指定
    public GameObject fakeUparupaPrehab; // インスペクターで指定
    public GameObject realMendakoPrehab; // インスペクターで指定
    public GameObject fakeMendakoPrehab; // インスペクターで指定
    // プレイヤー
    private GameObject player;
    private GameObject myRealPrehab;
    private GameObject myFakePrehab;
    // 駒の位置
    private Vector2Int[] piecePosID;
    // 駒一覧
    List<GameObject> myPieces;
    // 選択中の駒のタイプ
    private bool pieceType;
    // 配置した駒の数
    private int SetRealPiece;
    private int SetFakePiece;
    private const int PIECE_NUM = 4;
    
    // 駒配置のための初期化
    public void CreateInitPieces(){
        player = this.gameObject;
        myPieces= new List<GameObject>();
        piecePosID = new Vector2Int[8];
        // ウパルパ
        if(player.GetComponent<PlayerState>().getsetTeam == PlayerState.Team.uparupa){
            myRealPrehab = realUparupaPrehab;
            myFakePrehab = fakeUparupaPrehab;
            for(int i=0; i<PIECE_NUM; i++){
                GameObject realPiece = Instantiate(myRealPrehab, new Vector3(4f,0.15f,9.3f), myRealPrehab.transform.rotation);
                realPiece.GetComponent<PieceState>().setPlayer = player;
                realPiece.GetComponent<PieceState>().getsetTeam = PlayerState.Team.uparupa;
                realPiece.GetComponent<PieceState>().getsetIsReal = true;
                myPieces.Add(realPiece);
                GameObject fakePiece = Instantiate(myFakePrehab, new Vector3(-4f,0.15f,9.3f), myFakePrehab.transform.rotation);
                fakePiece.GetComponent<PieceState>().setPlayer = player;
                fakePiece.GetComponent<PieceState>().getsetTeam = PlayerState.Team.uparupa;
                fakePiece.GetComponent<PieceState>().getsetIsReal = false;
                myPieces.Add(fakePiece);
            }
        // メンダコ
        }else{
            myRealPrehab = realMendakoPrehab;
            myFakePrehab = fakeMendakoPrehab;
            for(int i=0; i<PIECE_NUM; i++){
                GameObject realPiece = Instantiate(myRealPrehab, new Vector3(-4f,0.15f,-9.3f), myRealPrehab.transform.rotation);
                realPiece.GetComponent<PieceState>().setPlayer = player;
                realPiece.GetComponent<PieceState>().getsetTeam = PlayerState.Team.mendako;
                realPiece.GetComponent<PieceState>().getsetIsReal = true;
                myPieces.Add(realPiece);
                GameObject fakePiece = Instantiate(myFakePrehab, new Vector3(4f,0.15f,-9.3f), myFakePrehab.transform.rotation);
                fakePiece.GetComponent<PieceState>().setPlayer = player;
                fakePiece.GetComponent<PieceState>().getsetTeam = PlayerState.Team.mendako;
                fakePiece.GetComponent<PieceState>().getsetIsReal = false;
                myPieces.Add(fakePiece);
            }
        }
        player.GetComponent<PlayerState>().getsetMyPieces = myPieces;
        SetRealPiece = 0;
        SetFakePiece = 0;
        player.GetComponent<PlayerState>().toStartSetPieces();
    }
    public GameObject getPlayer{
        get{ return player; }
    }

    public void setPiecePosID(Vector2Int posID, bool real){
        if(real){
            piecePosID[2*SetRealPiece] = posID;
            SetRealPiece++;
        }else{
            piecePosID[2*SetFakePiece+1] = posID;
            SetFakePiece++;
        }
        Debug.Log(SetRealPiece+" : "+SetFakePiece);
    }
    // 駒を選択
    public void StartSelect(){
        player.GetComponent<PlayerState>().toStartSetPieces(); //状態遷移、動かす駒設定
    }
    public void SelectPiece(Vector2Int posID, bool isReal){
        player.GetComponent<PlayerState>().toSelectSetPosition(posID); //状態遷移、動かす駒設定
        pieceType = isReal;
    }
    public void SelectPosition(Vector2Int posID){
        player.GetComponent<PlayerState>().toMoveSetPosition(posID); //状態遷移、登録
        int id = (pieceType) ? 2*SetRealPiece : 2*SetFakePiece+1;
        myPieces[id].GetComponent<PieceState>().MovePiecePos(posID); //移動
        setPiecePosID(posID, pieceType); // 配置数登録
        player.GetComponent<SettingUI>().DecreasePieceNum(pieceType); //UI

        // 配置完了
        if(SetRealPiece==PIECE_NUM && SetFakePiece==PIECE_NUM){
            player.GetComponent<PlayerState>().toFinishSet();
        }
    }
}