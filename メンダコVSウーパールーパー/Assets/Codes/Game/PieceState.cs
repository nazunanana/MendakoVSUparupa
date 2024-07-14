using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// 駒の状態
/// </summary>
public class PieceState : NetworkBehaviour
{
    private GameObject player;
    private GameObject gridmanager;
    // 駒が本物である
    private bool isReal;
    // 駒の種類
    public PlayerState.Team team { get; set; }
    // ロード待機
    private bool wait;
    // 駒ID
    private int pieceID;
    // どのマスにいるか
    [Networked, OnChangedRender(nameof(SyncPos))]
    public Vector2Int posID { get; set; }
    [Networked, OnChangedRender(nameof(SyncPos))]
    public Vector3 absPos { get; set; }
    // グローバル位置換算
    private const float FIRST_X = -6.48f; //ウパルパ陣営側
    private const float FIRST_Z = -6.48f;
    private const float Y_POS = 0.04f;
    private const int GRID_NUM = 6;
    private float gridSize = Mathf.Abs(FIRST_X * 2 / 5);

    public bool getsetIsReal
    {
        get { return isReal; }
        set { isReal = value; }
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
    void Start()
    {
        gridmanager = GameObject.FindGameObjectWithTag("GridSystem");
        // WaitLoading(1.0f);
        // wait = true;
    }
    IEnumerator WaitLoading(float time)
    {
        // 待つ
        yield return new WaitForSeconds(time);
    }
    void OnMouseOver()
    {
        if (player.GetComponent<PlayerState>()!=null)
        {
            // Debug.Log("piece over");
            Debug.Log("team" + team);
            Debug.Log("player" + player);
            Debug.Log("comp" + player.GetComponent<PlayerState>());
            // マテリアルをハイライト
            if (team == player.GetComponent<PlayerState>().team) //自陣の駒なら
            {
                switch (player.GetComponent<PlayerState>().selectMode)
                {
                    case PlayerState.SelectMode.SetPiece: //設置駒選択なら
                        HighLightPiece(true);
                        break;
                    case PlayerState.SelectMode.MovePiece: //ゲーム中 動かす駒選択中なら
                        HighLightPiece(true);
                        gridmanager.GetComponent<ManageGrid>().HighLightWASDGrid(posID, true);
                        break;
                    default:
                        break;
                }
            }
            else { Debug.Log("not my team's piece"); }
        }else return;
    }
    void OnMouseExit()
    {
        if (player.GetComponent<PlayerState>()!=null)
        {
            // ハイライトを解除
            if (team == player.GetComponent<PlayerState>().team) //自陣の駒なら
            {
                switch (player.GetComponent<PlayerState>().selectMode)
                {
                    case PlayerState.SelectMode.SetPiece: //設置駒選択なら
                        HighLightPiece(false);
                        break;
                    case PlayerState.SelectMode.MovePiece: //ゲーム中 動かす駒選択中なら
                        HighLightPiece(false);
                        gridmanager.GetComponent<ManageGrid>().HighLightWASDGrid(posID, false);
                        break;
                    default:
                        break;
                }
            }
        }else return;
    }
    void OnMouseDown()
    {
        if (player.GetComponent<PlayerState>()!=null)
        {
            if (team == player.GetComponent<PlayerState>().team) //自陣の駒なら
            {
                switch (player.GetComponent<PlayerState>().selectMode)
                {
                    case PlayerState.SelectMode.SetPiece: //設置駒選択なら
                        player.GetComponent<CreatePiece>().SelectPiece(posID, getsetIsReal); // 状態遷移
                        HighLightPiece(true);
                        Debug.Log("設置する駒を選択");
                        break;
                    case PlayerState.SelectMode.MovePiece: //ゲーム中 動かす駒選択中なら
                        gridmanager.GetComponent<ManageGrid>().pieceID = posID;
                        player.GetComponent<PlayerState>().toSelectPiece(posID); // 状態遷移
                        HighLightPiece(false);
                        gridmanager.GetComponent<ManageGrid>().HighLightWASDGrid(posID, false);
                        Debug.Log("PieceState : " + posID[0] + ", " + posID[1] + "の駒を選択しています");

                        break;
                    default:
                        break;
                }
            }
            else { Debug.Log("not my team's piece"); }
        }else return;
    }

    // マテリアルを強調
    public void HighLightPiece(bool tf)
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
    public void MovePiecePos(Vector2Int posID)
    {
        this.posID = posID;
        //Debug.Log("移動"+posID[0]+":"+posID[1]);
        // 移動させる
        this.gameObject.transform.position = Id2Pos(posID);
    }
    public void SyncPos()
    {
        //Debug.Log("移動"+posID[0]+":"+posID[1]);
        // 移動させる
        Debug.Log("player");
        this.gameObject.transform.position = absPos;
    }
    public void SetAbsPos(Vector3 pos)
    {
        absPos = pos;
    }
    /// <summary>
    /// ID:位置の換算
    /// </summary>
    public Vector3 Id2Pos(Vector2Int posID)
    {
        absPos = new Vector3(FIRST_X + posID[0] * gridSize, Y_POS, FIRST_Z + posID[1] * gridSize);
        return absPos;
    }

    // public void PieceDespawn(){
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
    //     runner.Despawn()
    // }
}
