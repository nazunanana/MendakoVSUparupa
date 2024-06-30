using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// gridオブジェクトクラス > OneGridプレハブ
/// </summary>
public class BoardGrid : MonoBehaviour {
    // ゲームオブジェクトをインスペクターで指定
    public GameObject gridSystemObj;
    private GameObject nowPlayer;
    // コンポネント
    private ManageGrid gridSystemComp;
    private PlayerState nowPlayerComp;
    // 位置
    public Vector2Int posID;
    private Vector3 myPosition;
    private int type; // マスの種類
    public enum types {
        mendako, // メンダコ陣営
        uparupa, // ウパルパ陣営
        corner_mendako, // メンダコ脱出マス
        corner_uparupa // ウパルパ脱出マス
    }

    public int SetType{
        set{ type = value; }
    }

    public GameObject SetGridSystemObj{
        set{
            gridSystemObj = value;
            gridSystemComp = gridSystemObj.GetComponent<ManageGrid>();
        }
    }

    public GameObject SetNowPlayer{
        set{
            nowPlayer = value;
            nowPlayerComp = nowPlayer.GetComponent<PlayerState>();
        }
    }

    public void SetPosition(Vector2Int posID, Vector3 myPosition)
    {
        this.posID = posID;
        this.myPosition = myPosition;
    }

    // マスを強調
    void OnMouseOver() {
        Debug.Log(posID.x+","+posID.y+"hovering");
        ChangeHighLight(true);
    }
    void OnMouseExit() {
        Debug.Log(posID.x+","+posID.y+"exit");
        ChangeHighLight(false);
    }
    // クリック時
    void OnMouseDown(){
        Debug.Log(posID.x+","+posID.y+"click");
        // 駒選択中→駒を選択
        if(nowPlayerComp.getisSelectingPiece){
            nowPlayerComp.SelectPiece(posID);
            Debug.Log("駒選択"+nowPlayerComp.getisSelectingMove);
        // 駒選択済みなら移動先を選択
        }else if(!nowPlayerComp.getisSelectingPiece && nowPlayerComp.getisSelectingMove){
            // 状態変化、移動先を設定
            nowPlayerComp.MovePiece(posID);
            Debug.Log("移動マス選択"+nowPlayerComp.getisSelectingMove);
        }
        // 現在のハイライトをクリア
        gridSystemComp.ClearHighLight();
    }

    // 状態によって強調させる・解除する
    // 駒選択前なら前後左右 / 駒選択済みならこのマスだけ
    private void ChangeHighLight(bool tf){
        if(nowPlayerComp.getisSelectingPiece){
            Debug.Log("piece selecting!");
            gridSystemComp.HighLightWASDGrid(posID,tf);
        }else if(!nowPlayerComp.getisSelectingPiece && nowPlayerComp.getisSelectingMove){
            Debug.Log("move selecting!");
            gridSystemComp.HighLightGrid(posID,tf);
        }else{
            Debug.Log("Not your turn now!");
        }
    }

    // 自身を強調・強調を解除
    public void EnableUnEnebleMyGrid(bool tf){
        Debug.Log("highlight my grid");
        MeshRenderer meshrender = this.gameObject.GetComponent<MeshRenderer>();
        if (meshrender != null){
            meshrender.enabled = tf;
        }
    }
}
