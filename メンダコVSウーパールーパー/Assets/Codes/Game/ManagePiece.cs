using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagePiece : MonoBehaviour
{
    private GameObject player; // 自分のプレイヤーオブジェクト
    private int getRealPieceNum = 0; // 獲得した本物駒数
    private int getFakePieceNum = 0; // 獲得した偽物駒数
    private const int GRID_NUM = 6;
    public Dictionary<Vector2Int, PieceState> pieceDic { get; set; } // 位置ID, 自陣営の駒comp

    void Awake()
    {
        pieceDic = new Dictionary<Vector2Int, PieceState>();
    }
    public bool EndGameCounter(bool real)
    {
        return (real ? getRealPieceNum : getFakePieceNum) >= 4;
    }


}
