using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// gridオブジェクトクラス > OneGridプレハブ
/// </summary>
public class BoardGrid : MonoBehaviour
{
    // ゲームオブジェクトをインスペクターで指定
    public GameObject gridSystemObj;
    private GameObject player; // 自分のプレイヤーオブジェクト
    // コンポネント
    private ManageGrid gridSystemComp;
    private PlayerState playerComp;
    // 位置
    public Vector2Int posID;
    private Vector3 myPosition;
    private int type; // マスの種類
    public enum types
    {
        mendako, // メンダコ陣営
        uparupa, // ウパルパ陣営
        corner_mendako, // メンダコ脱出マス
        corner_uparupa // ウパルパ脱出マス
    }

    public int SetType
    {
        set { type = value; }
    }

    public GameObject SetGridSystemObj
    {
        set
        {
            gridSystemObj = value;
            gridSystemComp = gridSystemObj.GetComponent<ManageGrid>();
        }
    }

    public GameObject SetPlayer
    {
        set
        {
            player = value;
            playerComp = player.GetComponent<PlayerState>();
        }
    }

    public void SetPosition(Vector2Int posID, Vector3 myPosition)
    {
        this.posID = posID;
        this.myPosition = myPosition;
    }

    // マスを強調
    void OnMouseOver()
    {
        //Debug.Log(posID.x+","+posID.y+"hovering");
        ChangeHighLight(true);
    }
    void OnMouseExit()
    {
        //Debug.Log(posID.x+","+posID.y+"exit");
        ChangeHighLight(false);
    }
    // クリック時
    void OnMouseDown()
    {
        //Debug.Log(posID.x+","+posID.y+"click");
        switch (playerComp.selectMode)
        {
            case PlayerState.SelectMode.SetPosition: //配置シーン中
                // 配置可能領域なら
                if (0 < posID[0] && posID[0] < 5 && (4 <= posID[1] || posID[1] <= 1))
                {
                    player.GetComponent<CreatePiece>().SelectPosition(posID);
                    ChangeHighLight(false);
                }
                break;
            case PlayerState.SelectMode.MovePosition: //ゲーム中
                PlayGame state = GameObject.FindWithTag("GameManager").GetComponent<PlayGame>();
                //TODO: 移動先が自分の駒の時は移動できない
                if (state.SearchPieceByPos(posID) == 1)
                {
                    Debug.Log("移動できないよ！");
                    break;
                }
                //TODO: 移動先が相手の駒だったら倒す
                else if (state.SearchPieceByPos(posID) == 2)
                {

                    // true/falseによって点数変化→(更新を検知してアニメーション)
                    GameObject.FindWithTag("GameManager").GetComponent<PlayGame>().GetPieceAction(posID);

                    // デスポーン
                    GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
                    foreach(GameObject piece in pieces){
                        if(piece != null){
                        }
                        if(piece.GetComponent<PieceState>().posID == posID){//PosIDがPieceのpieceIDと一致したらデスポーン
                            NetworkObject pieceNet = piece.GetComponent<NetworkObject>();
                            if(pieceNet!=null){
                                Debug.Log("pieceNetは存在してます");//ちゃんと出る
                            }
                            state.DespawnPiece(pieceNet);
                        }
                    }
                    // 自駒移動
                    ChangeHighLight(false);
                    nowPlayerComp.toMovePiece(posID);
                    Debug.Log("ですとろい！");


                    // 削除
                    
                    // 状態遷移
                    ChangeHighLight(false);
                    playerComp.toMovePiece(posID);

                }
                // TODO: 移動先が脱出マスの時
                // else if ()
                // {

                // }
                else
                { // 移動先に何もないとき
                    Debug.Log("OnMouseDown in BoardGrid");
                    ChangeHighLight(false);
                    playerComp.toMovePiece(posID);
                }
                break;

            // case PlayerState.SelectMode.MovePosition:
            //     nowPlayerComp.toMovePiece(posID);
            //     Debug.Log("移動した");
            //     ChangeHighLight(false);
            //     break;
            case PlayerState.SelectMode.NoMyTurn:
                break;
            default:
                break;
        }
        // 現在のハイライトをクリア
        //gridSystemComp.ClearHighLight();
    }

    // 状態によって強調させる・解除する
    // 駒選択前なら前後左右 / 駒選択済みならこのマスだけ
    private void ChangeHighLight(bool tf)
    {
        switch (playerComp.selectMode)
        {
            case PlayerState.SelectMode.SetPosition: //位置決め
                //配置可能領域なら(左右ID1~4,前後ID01 or 45)。相手領域は壁があるから選択されない
                if (0 < posID[0] && posID[0] < 5 && (4 <= posID[1] || posID[1] <= 1))
                {
                    gridSystemComp.HighLightGrid(posID, tf);
                }
                break;
            case PlayerState.SelectMode.MovePosition: //位置決め
                Vector2Int pieceID = gridSystemComp.pieceID; // 選択している駒の座標
                //Debug.Log("PieceID : " + pieceID);
                int pieceX = pieceID[0];
                int pieceY = pieceID[1];
                bool w = (posID[0] == pieceX - 1) && (posID[1] == pieceY);
                bool a = (posID[0] == pieceX) && (posID[1] == pieceY - 1);
                bool s = (posID[0] == pieceX + 1) && (posID[1] == pieceY);
                bool d = (posID[0] == pieceX) && (posID[1] == pieceY + 1);
                if (w || a || s || d)
                {
                    gridSystemComp.HighLightGrid(posID, tf);
                }
                break;
            case PlayerState.SelectMode.NoMyTurn:
                break;
            default:
                break;
        }
    }

    // 自身を強調・強調を解除
    public void EnableUnEnebleMyGrid(bool tf)
    {
        // Debug.Log("highlight my grid");
        MeshRenderer meshrender = this.gameObject.GetComponent<MeshRenderer>();
        if (meshrender != null)
        {
            meshrender.enabled = tf;
        }
    }
}
