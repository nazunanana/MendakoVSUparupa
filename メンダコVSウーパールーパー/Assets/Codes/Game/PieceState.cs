using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 駒の状態 attach to 駒オブジェクト
/// </summary>
public class PieceState : NetworkBehaviour
{
    private GameObject player;
    private GameObject gridmanager;

    // 駒が本物である
    public bool isReal { get; set; }

    // 駒の種類
    public PlayerState.Team team { get; set; }

    // ロード待機
    private bool dicflag;

    // 駒ID
    private int pieceID; // 使われていない

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
    public int getsetPieceID
    {
        get { return pieceID; }
        set { pieceID = value; }
    }
    public GameObject setPlayer
    {
        set { player = value; }
    }

    void Awake()
    {
        PlayGame.OnCreateDicComplete += CheckFlag;
    }

    void OnDestroy()
    {
        PlayGame.OnCreateDicComplete -= CheckFlag;
        Debug.Log($"{gameObject.name}がデスポーンされました");
    }

    void Start()
    {
        gridmanager = GameObject.FindGameObjectWithTag("GridSystem");
    }

    IEnumerator WaitLoading(float time)
    {
        // 待つ
        yield return new WaitForSeconds(time);
    }

    void CheckFlag()
    {
        dicflag = true;
    }

    // public void Shining()
    // {
    //     Debug.Log("Shining実行");
    //     // 光らせたい
    //     HighLightPiece(true);
    //     StartCoroutine(WaitUntilHL(3.0f));
    // }

    // IEnumerator WaitUntilHL(float time)
    // {
    //     // 待つ
    //     yield return new WaitForSeconds(time);
    //     // 光を消す
    //     HighLightPiece(false);
    //     Debug.Log("Shining切れた");
    // }

    void OnMouseOver()
    {
        if (player != null && player.GetComponent<PlayerState>() != null)
        {
            // マテリアルをハイライト
            if (team == player.GetComponent<PlayerState>().team) //自陣の駒なら
            {
                switch (player.GetComponent<PlayerState>().selectMode)
                {
                    case PlayerState.SelectMode.SetPiece: //設置駒選択なら
                        HighLightPiece(true);
                        break;
                    case PlayerState.SelectMode.MovePiece: //ゲーム中 動かす駒選択中なら
                        // カード使用状態なら斜め可能
                        bool naname = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ManageCard>().card == ManageCard.Card.Naname;
                        // 置けるなら
                        if (canSelectPiece(posID, naname))
                        {
                            Debug.Log("置ける");
                            HighLightPiece(true);
                            gridmanager.GetComponent<ManageGrid>().HighLightWASDGrid(posID, true);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Debug.Log("not my team's piece");
            }
        }
        else
            return;
    }

    void OnMouseExit()
    {
        if (player != null && player.GetComponent<PlayerState>() != null)
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
                        // カード使用状態なら斜め可能
                        bool naname = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ManageCard>().card == ManageCard.Card.Naname;
                        // 置けるなら
                        if (canSelectPiece(posID, naname))
                        {
                            HighLightPiece(false);
                            gridmanager.GetComponent<ManageGrid>().HighLightWASDGrid(posID, false);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        else
            return;
    }

    void OnMouseDown()
    {
        if (player.GetComponent<PlayerState>() != null)
        {
            if (team == player.GetComponent<PlayerState>().team) //自陣の駒なら
            {
                switch (player.GetComponent<PlayerState>().selectMode)
                {
                    case PlayerState.SelectMode.SetPiece: //設置駒選択なら
                        player.GetComponent<CreatePiece>().SelectPiece(posID, isReal); // 状態遷移
                        HighLightPiece(true);
                        player.GetComponent<ManagePiece>().EnableOpponentColliders(false);
                        //Debug.Log("設置する駒を選択");
                        break;
                    case PlayerState.SelectMode.MovePiece: //ゲーム中 動かす駒選択中なら
                        // カード使用状態なら斜め可能
                        bool naname = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ManageCard>().card == ManageCard.Card.Naname;
                        // 置けるなら
                        if (canSelectPiece(posID, naname))
                        {
                            Debug.Log("canSelectだから選択したよ");
                            gridmanager.GetComponent<ManageGrid>().pieceID = posID;
                            player.GetComponent<PlayerState>().toSelectPiece(posID); // 状態遷移
                            HighLightPiece(false);
                            gridmanager.GetComponent<ManageGrid>().HighLightWASDGrid(posID, false);
                            player.GetComponent<ManagePiece>().EnableOpponentColliders(false);
                            // マスのコライダー前後左右
                            gridmanager.GetComponent<ManageGrid>().EnableWASDColliders(posID);
                            //Debug.Log("PieceState : " + posID[0] + ", " + posID[1] + "の駒を選択しています");
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Debug.Log("not my team's piece");
            }
        }
        else
            return;
    }

    private bool canSelectPiece(Vector2Int posID, bool useCard)
    {
        // 前後左右どこかに動けるなら
        PlayGame pg = GameObject.FindWithTag("GameManager").GetComponent<PlayGame>();
        bool w = pg.SearchPieceByPos(new Vector2Int(posID[0] - 1, posID[1])) != 1;
        bool a = pg.SearchPieceByPos(new Vector2Int(posID[0], posID[1] - 1)) != 1;
        bool s = pg.SearchPieceByPos(new Vector2Int(posID[0] + 1, posID[1])) != 1;
        bool d = pg.SearchPieceByPos(new Vector2Int(posID[0], posID[1] + 1)) != 1;
        bool q = pg.SearchPieceByPos(new Vector2Int(posID[0] - 1, posID[1] - 1)) != 1;
        bool z = pg.SearchPieceByPos(new Vector2Int(posID[0] + 1, posID[1] - 1)) != 1;
        bool c = pg.SearchPieceByPos(new Vector2Int(posID[0] + 1, posID[1] + 1)) != 1;
        bool e = pg.SearchPieceByPos(new Vector2Int(posID[0] - 1, posID[1] + 1)) != 1;
        Debug.Log("wasd"+w+","+a+","+s+","+d);
        // 縦横
        bool canSelect = useCard ? (q || a || s || d || q || z || c || e) : (w || a || s || d);
        return canSelect;
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
        //Debug.Log("player");
        this.gameObject.transform.position = absPos;

        if (dicflag)
        {
            // 共有dicの登録を変える
            // リセット
            player.GetComponent<ManagePiece>().syncDic.Clear();
            // 登録しなおし
            foreach (var dic in player.GetComponent<ManagePiece>().pieceDic)
            {
                //更新
                player.GetComponent<ManagePiece>().syncDic.Set(dic.Key, dic.Value.isReal);
            }
        }
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
}
