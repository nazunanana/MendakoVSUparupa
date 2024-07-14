using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    /// <summary> ウパルパ陣営のカメラ位置、向き（定数） </summary>
    private static readonly int UPA_CAMERA_POSITION_Z = 13;
    private static readonly int UPA_CAMERA_ROTATION_Y = 180;
    /// <summary> メンダコ陣営のカメラ位置、向き（定数） </summary>
    private static readonly int MEN_CAMERA_POSITION_Z = -13;
    private static readonly int MEN_CAMERA_ROTATION_Y = 0;
    // カメラオブジェクト
    private static GameObject cam;

    public static void SetCamera(bool isUparupa){
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        if(isUparupa){
            cam.transform.position = new Vector3(0, 10, UPA_CAMERA_POSITION_Z);
            cam.transform.rotation = Quaternion.Euler(48, UPA_CAMERA_ROTATION_Y, 0);
        }else{
            cam.transform.position = new Vector3(0, 10, MEN_CAMERA_POSITION_Z);
            cam.transform.rotation = Quaternion.Euler(48, MEN_CAMERA_ROTATION_Y, 0);
        }
    }
}
