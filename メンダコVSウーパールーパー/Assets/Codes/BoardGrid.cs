using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGrid : MonoBehaviour {
    private Vector2 posID;
    private int type; // マスの種類
    public enum types {
        mendako, // メンダコ陣営
        uparupa, // ウパルパ陣営
        corner_mendako, // メンダコ脱出マス
        corner_uparupa // ウパルパ脱出マス
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void FixedUpdate() {

    }

    public void SetType(int type){
        this.type = type;
    }

    public void SetPosID(Vector2 posID) {
        this.posID = posID;
    }

    void OnMouseOver() {
        Debug.Log(posID[0]+","+posID[1]+"hovering");
    }
}
