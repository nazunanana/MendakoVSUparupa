using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 駒の状態
/// </summary>
public class PieceState : MonoBehaviour
{
    private GameObject player;
    // 駒が本物である
    private bool isReal;
    // 駒の種類
    private PlayerState.Team team;
    // 駒ID
    private int pieceID;
    // どのマスにいるか
    private Vector2Int posID;
    // グローバル位置換算
    private const float FIRST_X = -6.48f; //ウパルパ陣営側
    private const float FIRST_Z = -6.48f;
    private const float Y_POS = 0.04f;
    private const int GRID_NUM = 6;
    private float gridSize = Mathf.Abs(FIRST_X*2/5);

    public bool getsetIsReal
    {
        get { return isReal; }
        set { isReal = value; }
    }

    public PlayerState.Team getsetTeam
    {
        get { return team; }
        set { team = value; }
    }

    public Vector2Int getsetPosID
    {
        get { return posID; }
        set { posID = value; }
    }
    public int getsetPieceID
    {
        get { return pieceID; }
        set { pieceID = value; }
    }
    public GameObject setPlayer
    {
        set { player = value; }
    }

    void OnMouseOver()
    {
        // Debug.Log("piece over");
        // マテリアルをハイライト
        if (team == player.GetComponent<PlayerState>().getsetTeam) //自陣の駒なら
        {
            switch (player.GetComponent<PlayerState>().getsetSelectMode)
            {
                case PlayerState.SelectMode.SetPiece: //設置駒選択なら
                    HighLightPiece(true);
                    break;
                case PlayerState.SelectMode.MovePiece: //ゲーム中 動かす駒選択中なら
                    HighLightPiece(true);
                    break;
                default:
                    break;
            }
        }else{ Debug.Log("not my team's piece"); }
    }
    void OnMouseExit()
    {
        // ハイライトを解除
        if (team == player.GetComponent<PlayerState>().getsetTeam) //自陣の駒なら
        {
            switch (player.GetComponent<PlayerState>().getsetSelectMode)
            {
                case PlayerState.SelectMode.SetPiece: //設置駒選択なら
                    HighLightPiece(false);
                    break;
                case PlayerState.SelectMode.MovePiece: //ゲーム中 動かす駒選択中なら
                    HighLightPiece(false);
                    break;
                default:
                    break;
            }
        }
    }
    void OnMouseDown()
    {
        if (team == player.GetComponent<PlayerState>().getsetTeam) //自陣の駒なら
        {
            switch (player.GetComponent<PlayerState>().getsetSelectMode)
            {
                case PlayerState.SelectMode.SetPiece: //設置駒選択なら
                    player.GetComponent<CreatePiece>().SelectPiece(posID, getsetIsReal); // 状態遷移
                    HighLightPiece(true);
                    Debug.Log("Select Piece");
                    break;
                case PlayerState.SelectMode.MovePiece: //ゲーム中 動かす駒選択中なら
                    player.GetComponent<PlayerState>().toMoveSetPosition(posID); // 状態遷移
                    HighLightPiece(false);
                    Debug.Log("Select Piece");
                    break;
                default:
                    break;
            }
        }else{ Debug.Log("not my team's piece"); }
    }

    // マテリアルを強調
    public void HighLightPiece(bool tf)
    {
        if (isReal)
        {
            Material mat = this.gameObject.GetComponent<MeshRenderer>().material;
            mat.color = tf ? Color.gray : Color.white;
        }
        else
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in meshRenderers)
            {
                Material mat = mr.material;
                mat.color = tf ? Color.gray : Color.white;
            }
        }
    }
    // 位置を変更
    public void MovePiecePos(Vector2Int posID)
    {
        getsetPosID = posID;
        // 移動させる
        this.gameObject.transform.position = Id2Pos(posID);
    }
    /// <summary>
    /// 位置換算
    /// </summary>
    public Vector3 Id2Pos(Vector2Int posID){
        return new Vector3(FIRST_X+posID[0]*gridSize, Y_POS, FIRST_Z+posID[1]*gridSize);
    }
}
