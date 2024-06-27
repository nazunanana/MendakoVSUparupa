using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public Team setgetTeam{
        get{ return team; }
        set{ team = value; }
    }

    public int getsetRealPoint{
        get{ return realPoint; }
        set{ realPoint = value; }
    }

    public int getsetFakePoint{
        get{ return fakePoint; }
        set{ fakePoint = value; }
    }
}