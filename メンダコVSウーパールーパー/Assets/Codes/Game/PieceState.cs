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

    public bool getIsReal()
    {
        return isReal;
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
        }
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
                    player.GetComponent<CreatePiece>().SelectPiece(posID); // 状態遷移
                    HighLightPiece(true);
                    Debug.Log("置く駒決めた");
                    break;
                case PlayerState.SelectMode.MovePiece: //ゲーム中 動かす駒選択中なら
                    player.GetComponent<PlayerState>().toMoveSetPosition(posID); // 状態遷移
                    HighLightPiece(true);
                    Debug.Log("移動させる駒選択した");
                    break;
                default:
                    break;
            }
        }
    }

    // マテリアルを強調
    private void HighLightPiece(bool tf)
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
    private void MovePiecePos(Vector2Int posID)
    {
        getsetPosID = posID;
        // 移動させる
        this.gameObject.transform.position(ManageGrid.Id2Pos(posID));
    }
}
