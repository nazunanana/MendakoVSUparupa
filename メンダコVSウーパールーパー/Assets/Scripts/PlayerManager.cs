using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // ウパルパならtrue メンダコならfalse
    private bool isUparupa = true;
    // 本物得点 (相手の本物を取ると+1)
    private int realPoint = 0;
    // 偽物得点 (相手の偽物を取ると+1)
    private int fakePoint = 0;
    // ウパルパ陣営のカメラ座標
    private int cameraPositionZ = -13;
    private int cameraRotationY = 0;

    public bool getsetIsUparupa{
        get{ return isUparupa; }
        set{ isUparupa = value; }
    }

    public int getsetRealPoint{
        get{ return realPoint; }
        set{ realPoint = value; }
    }

    public int getsetFakePoint{
        get{ return fakePoint; }
        set{ fakePoint = value; }
    }

    public int getCameraPositionZ{
        get{ return cameraPositionZ; }
    }

    public int getCameraRatationY{
        get{ return cameraRotationY; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // メンダコ陣営の時は、カメラ逆向きになるようにする
        if (!isUparupa)
        {
            cameraPositionZ = cameraPositionZ * -1;
            cameraRotationY = cameraRotationY + 180;
        }
    }
}
