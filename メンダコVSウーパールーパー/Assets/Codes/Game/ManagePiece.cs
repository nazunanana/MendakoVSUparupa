using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// attach to Playerオブジェクト
/// </summary>
public class ManagePiece : NetworkBehaviour
{
    private GameObject player; // 自分のプレイヤーオブジェクト
    [Networked, OnChangedRender(nameof(AnimReal))]
    public int getRealPieceNum { get; set; } // 獲得した本物駒数
    [Networked, OnChangedRender(nameof(AnimFake))]
    public int getFakePieceNum { get; set; } // 獲得した偽物駒数
    private const int GRID_NUM = 6;
    public Dictionary<Vector2Int, PieceState> pieceDic { get; set; } // 位置ID, 自陣営の駒comp

    [Networked, Capacity(8)]
    public NetworkDictionary<Vector2Int, NetworkBool> syncDic => default; // posID, real?
    // [Networked, Capacity(8)]
    // public NetworkArray<NetworkId> IDlist => default;

    void Awake()
    {
        pieceDic = new Dictionary<Vector2Int, PieceState>();
        player = this.gameObject;
    }
    public bool EndGameCounter(bool real)
    {
        return (real ? getRealPieceNum : getFakePieceNum) >= 4;
    }
    public void AnimReal()
    {
        GameObject manager = GameObject.FindGameObjectWithTag("GameManager");
        SetAnimation anim = manager.GetComponent<SetAnimation>();
        PlayerState.Team myteam = player.GetComponent<PlayerState>().team;
        anim.StartPlay(true, myteam == PlayerState.Team.mendako);
    }
    public void AnimFake()
    {
        GameObject manager = GameObject.FindGameObjectWithTag("GameManager");
        SetAnimation anim = manager.GetComponent<SetAnimation>();
        PlayerState.Team myteam = player.GetComponent<PlayerState>().team;
        anim.StartPlay(false, myteam == PlayerState.Team.mendako);
    }
}
