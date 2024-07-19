using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 駒生成 > PLオブジェクト
/// </summary>
public class CreatePiece : NetworkBehaviour
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
    public void CreateInitPieces()
    {
        player = this.gameObject;
        myPieces = new List<GameObject>();
        piecePosID = new Vector2Int[8];
        // ウパルパ
        if (player.GetComponent<PlayerState>().team == PlayerState.Team.uparupa)
        {
            myRealPrehab = realUparupaPrehab;
            myFakePrehab = fakeUparupaPrehab;
            for (int i = 0; i < PIECE_NUM; i++)
            {
                var realPieceNet = Runner.Spawn(myRealPrehab, new Vector3(4f, 0.15f, 9.3f), myRealPrehab.transform.rotation);
                Debug.Log("Spawn");
                DontDestroyOnLoad(realPieceNet.gameObject);
                realPieceNet.gameObject.GetComponent<PieceState>().SetAbsPos(new Vector3(4f, 0.15f, 9.3f));
                realPieceNet.gameObject.GetComponent<PieceState>().setPlayer = player;
                realPieceNet.gameObject.GetComponent<PieceState>().team = PlayerState.Team.uparupa;
                realPieceNet.gameObject.GetComponent<PieceState>().isReal = true;
                myPieces.Add(realPieceNet.gameObject);
                var fakePieceNet = Runner.Spawn(myFakePrehab, new Vector3(-4f, 0.15f, 9.3f), myFakePrehab.transform.rotation);
                DontDestroyOnLoad(fakePieceNet.gameObject);
                fakePieceNet.gameObject.GetComponent<PieceState>().SetAbsPos(new Vector3(-4f, 0.15f, 9.3f));
                fakePieceNet.gameObject.GetComponent<PieceState>().setPlayer = player;
                fakePieceNet.gameObject.GetComponent<PieceState>().team = PlayerState.Team.uparupa;
                fakePieceNet.gameObject.GetComponent<PieceState>().isReal = false;
                myPieces.Add(fakePieceNet.gameObject);
            }
            // メンダコ
        }
        else
        {
            myRealPrehab = realMendakoPrehab;
            myFakePrehab = fakeMendakoPrehab;
            for (int i = 0; i < PIECE_NUM; i++)
            {
                var realPieceNet = Runner.Spawn(myRealPrehab, new Vector3(-4f, 0.15f, -9.3f), myRealPrehab.transform.rotation);
                DontDestroyOnLoad(realPieceNet.gameObject);
                realPieceNet.gameObject.GetComponent<PieceState>().SetAbsPos(new Vector3(-4f, 0.15f, -9.3f));
                realPieceNet.gameObject.GetComponent<PieceState>().setPlayer = player;
                realPieceNet.gameObject.GetComponent<PieceState>().team = PlayerState.Team.mendako;
                realPieceNet.gameObject.GetComponent<PieceState>().isReal = true;
                myPieces.Add(realPieceNet.gameObject);
                var fakePieceNet = Runner.Spawn(myFakePrehab, new Vector3(4f, 0.15f, -9.3f), myFakePrehab.transform.rotation);
                DontDestroyOnLoad(fakePieceNet.gameObject);
                fakePieceNet.gameObject.GetComponent<PieceState>().SetAbsPos(new Vector3(4f, 0.15f, -9.3f));
                fakePieceNet.gameObject.GetComponent<PieceState>().setPlayer = player;
                fakePieceNet.gameObject.GetComponent<PieceState>().team = PlayerState.Team.mendako;
                fakePieceNet.gameObject.GetComponent<PieceState>().isReal = false;
                myPieces.Add(fakePieceNet.gameObject);
            }
        }
        //player.GetComponent<PlayerState>().getsetMyPieces = myPieces;
        SetRealPiece = 0;
        SetFakePiece = 0;
        player.GetComponent<PlayerState>().toStartSetPieces();
    }
    public GameObject getPlayer
    {
        get { return player; }
    }

    public void setPiecePosID(Vector2Int posID, bool real)
    {
        if (real)
        {
            piecePosID[2 * SetRealPiece] = posID;
            SetRealPiece++;
        }
        else
        {
            piecePosID[2 * SetFakePiece + 1] = posID;
            SetFakePiece++;
        }
        //Debug.Log(SetRealPiece+" : "+SetFakePiece);
    }
    // 駒を選択
    public void StartSelect()
    {
        player.GetComponent<PlayerState>().toStartSetPieces(); //状態遷移、動かす駒設定
    }
    public void SelectPiece(Vector2Int posID, bool isReal)
    {
        player.GetComponent<PlayerState>().toSelectSetPosition(posID); //状態遷移、動かす駒設定
        pieceType = isReal;
    }
    /// <summary>
    /// 配置位置を決定
    /// </summary>
    public void SelectPosition(Vector2Int posID)
    {
        // 入れる配列取得
        Dictionary<Vector2Int, PieceState> pieceDic = player.GetComponent<ManagePiece>().pieceDic;
        var syncDic = player.GetComponent<ManagePiece>().syncDic;
        // 対象id
        int id = (pieceType) ? 2 * SetRealPiece : 2 * SetFakePiece + 1;
        if (!syncDic.ContainsKey(piecePosID[id]))
        {
            // 駒の位置IDを変更して移動
            myPieces[id].GetComponent<PieceState>().MovePiecePos(posID); //移動
                                                                         // 配置済み数を増加
            setPiecePosID(posID, pieceType);
            // 登録
            player.GetComponent<ManagePiece>().pieceDic.Add(piecePosID[id], myPieces[id].GetComponent<PieceState>());
            player.GetComponent<ManagePiece>().syncDic.Add(piecePosID[id], pieceType);
            // 残り駒数のUI変化
            player.GetComponent<SettingUI>().DecreasePieceNum(pieceType);
            // 状態遷移
            player.GetComponent<PlayerState>().toMoveSetPosition(posID);
        }

        // ぜんぶ配置完了したら
        if (SetRealPiece == PIECE_NUM && SetFakePiece == PIECE_NUM && pieceDic.Count == 8)
        {
            // // IDリストを作成しておく
            //player.GetComponent<ManagePiece>().GetNetworkId();
            player.GetComponent<PlayerState>().toFinishSet();
        }
    }
}