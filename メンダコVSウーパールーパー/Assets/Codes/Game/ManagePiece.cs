using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

// playerプレハブにアタッチされている
public class ManagePiece : NetworkBehaviour
{
    private GameObject player; // 自分のプレイヤーオブジェクト
    private int getRealPieceNum = 0; // 獲得した本物駒数
    private int getFakePieceNum = 0; // 獲得した偽物駒数
    private const int GRID_NUM = 6;
    public Dictionary<Vector2Int, PieceState> pieceDic { get; set; } // 位置ID, 自陣営の駒comp
    [Networked, Capacity(8)]
    public NetworkDictionary<Vector2Int, NetworkBool> syncDic => default; // posID, real?
    // [Networked, Capacity(8)]
    // public NetworkArray<NetworkId> IDlist => default;

    void Awake()
    {
        pieceDic = new Dictionary<Vector2Int, PieceState>();
    }
    public bool EndGameCounter(bool real)
    {
        return (real ? getRealPieceNum : getFakePieceNum) >= 4;
    }

    // // CreatePieceで呼び出し
    // public void GetNetworkId()
    // {
    //     //IDlist = MakeInitializer(new NetworkString<NetworkId>[]());
    //     int i = 0;
    //     foreach (PieceState p in pieceDic.Values)
    //     {
    //         IDlist.Set(i, p.gameObject.GetComponent<NetworkObject>().Id);
    //         i++;
    //     }
    // }


    // PlayGameで呼び出し
    // public void CreatePosVector(NetworkArray<NetworkId> partnerIDlist)
    // {
    //     //runner検索
    //     GameObject[] runners = GameObject.FindGameObjectsWithTag("Runner");
    //     NetworkRunner runner = runners[0].GetComponent<NetworkRunner>();
    //     foreach (GameObject g in runners)
    //     {
    //         if (g.GetComponent<NetworkRunner>().IsRunning)
    //         { //アクティブのものを検出
    //             runner = g.GetComponent<NetworkRunner>();
    //             break;
    //         }
    //     }
    //     foreach (NetworkId nid in partnerIDlist)
    //     {
    //         Debug.Log("ID"+nid);
    //         if (Runner.TryFindObject(nid, out var obj))
    //         {
    //             Vector2Int posID = obj.gameObject.GetComponent<PieceState>().posID;



    //             PieceState pieceState = obj.gameObject.GetComponent<PieceState>();
    //             partnerPieceDic.Add(posID, pieceState);
    //         }
    //         else { Debug.Log("TryFindObjがfalse!!"); }
    //     }
    // }
}
