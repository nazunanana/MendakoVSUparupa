using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagePiece : MonoBehaviour
{
    private GameObject player; // 自分のプレイヤーオブジェクト
    private int getRealPieceNum; // 獲得した本物駒数
    private int getFakePieceNum; // 獲得した偽物駒数
    private const int GRID_NUM = 6;

    public Dictionary<Vector2Int, PieceState> pieceDic { get; set; } // 位置ID, 自陣営の駒comp

    void Awake()
    {
        pieceDic = new Dictionary<Vector2Int, PieceState>();
    }
    // /// <summary>
    // /// 前後左右の駒を検索
    // /// </summary>
    // public int[] SearchWASD(Vector2Int posID)
    // {
    //     int id_x = posID.x;
    //     int id_z = posID.y;
    //     int w = -1, a = -1, s = -1, d = -1;

    //     if (0 <= id_x - 1) w = SearchPieceByPos(new Vector2Int(id_x - 1, id_z)); // 上のマス
    //     if (0 <= id_z - 1) a = SearchPieceByPos(new Vector2Int(id_x, id_z - 1)); // 左のマス
    //     if (id_x + 1 < GRID_NUM) s = SearchPieceByPos(new Vector2Int(id_x + 1, id_z)); // 下のマス
    //     if (id_z + 1 < GRID_NUM) d = SearchPieceByPos(new Vector2Int(id_x, id_z + 1)); // 右のマス
    //     return new int[] { w, a, s, d }; //上左下右 -1:範囲外 0:null 1:自陣の駒 2:相手の駒
    // }


}
