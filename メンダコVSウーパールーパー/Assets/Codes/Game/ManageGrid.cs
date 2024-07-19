using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ManageGrid : MonoBehaviour
{
    public GameObject oneGridPrehab = null;
    public GameObject[,] gridArray;
    private GameObject[] players;
    public GameObject player;
    public bool ImUparupa;

    private const float FIRST_X = -6.48f; //ウパルパ陣営側
    private const float FIRST_Z = -6.48f;
    private const float Y_POS = 0.04f;
    private const int GRID_NUM = 6;
    private float gridSize = Mathf.Abs(FIRST_X * 2 / 5);
    private Vector2Int piecePosID;
    private Vector2Int movePosID;
    public Vector2Int pieceID { get; set; }
    public void Init()
    {
        gridArray = new GameObject[GRID_NUM, GRID_NUM];
        pieceID = new Vector2Int(0, 0);
        CreateGrids();
    }
    public GameObject[] SetPlayers
    {
        set
        {
            players = value;
            Init();
        }
    }
    public GameObject SetPlayer
    {
        set
        {
            player = value;
            Init();
        }
    }
    public bool setImUparupa
    {
        set
        {
            ImUparupa = value;
            //Debug.Log("Upa?"+ImUparupa);
        }
    }

    /// <summary>
    /// マスのコライダーオブジェクトを生成
    /// </summary>
    void CreateGrids()
    {
        for (int i = 0; i < GRID_NUM; ++i)
        {
            for (int j = 0; j < GRID_NUM; ++j)
            {
                Vector3 position = new Vector3(FIRST_X + j * gridSize, Y_POS, FIRST_Z + i * gridSize);
                GameObject newGridObj = Instantiate(oneGridPrehab, position, Quaternion.identity);
                DontDestroyOnLoad(newGridObj);
                BoardGrid gridComponent = newGridObj.GetComponent<BoardGrid>();
                // それぞれのIDと位置を教える
                gridComponent.SetPosition(new Vector2Int(j, i), position);
                gridComponent.SetGridSystemObj = this.gameObject;
                // 配置中なら自分  最初のプレイヤーはウパルパ
                if (SceneManager.GetActiveScene().name == "SC_SetPieces")
                {
                    //gridComponent.SetNowPlayer = ImUparupa ? this.players[0] : this.players[1];
                    gridComponent.SetPlayer = player;
                }
                else
                {
                    //gridComponent.SetNowPlayer = this.players[0];
                    gridComponent.SetPlayer = player;
                }
                // 生成したマスを配列に登録
                gridArray[j, i] = newGridObj;
                if (i == 0 && (j == 0 || j == 5))
                {
                    gridComponent.SetType = (int)BoardGrid.types.corner_mendako;
                }
                else if (i == 5 && (j == 0 || j == 5))
                {
                    gridComponent.SetType = (int)BoardGrid.types.corner_uparupa;
                }
                else if (i < 3)
                {
                    gridComponent.SetType = (int)BoardGrid.types.mendako;
                }
                else
                {
                    gridComponent.SetType = (int)BoardGrid.types.uparupa;
                }
            }
        }
    }
    /// <summary>
    /// マスコライダーオブジェクトを全て削除
    /// </summary>
    public void DestroyGrids()
    {
        for (int i = 0; i < GRID_NUM; ++i)
        {
            for (int j = 0; j < GRID_NUM; ++j)
            {
                Destroy(gridArray[j, i]);
            }
        }
    }

    /// <summary>
    /// カーソル位置ハイライト
    /// </summary>
    public void HighLightGrid(Vector2Int posID, bool highlight)
    {
        //Debug.Log("HighLight: " + posID.x + ", " + posID.y + " : " + highlight);
        EnableUnEnableGrid(new Vector2Int(posID.x, posID.y), highlight);
    }
    /// <summary>
    /// 前後左右のハイライト
    /// </summary>
    public void HighLightWASDGrid(Vector2Int posID, bool highlight)
    {
        //Debug.Log("HighLightWASD: " + posID.x + ", " + posID.y + " : " + highlight);
        int id_x = posID.x;
        int id_z = posID.y;

        EnableUnEnableGrid(new Vector2Int(id_x, id_z), highlight); // 中心のマス
        if (0 <= id_x - 1 && !SearchPieceHere(new Vector2Int(id_x - 1, id_z)))
        {
            EnableUnEnableGrid(new Vector2Int(id_x - 1, id_z), highlight); // 上のマス
        }
        if (id_x + 1 < GRID_NUM && !SearchPieceHere(new Vector2Int(id_x + 1, id_z)))
        {
            EnableUnEnableGrid(new Vector2Int(id_x + 1, id_z), highlight); // 下のマス
        }
        if (0 <= id_z - 1 && !SearchPieceHere(new Vector2Int(id_x, id_z - 1)))
        {
            EnableUnEnableGrid(new Vector2Int(id_x, id_z - 1), highlight); // 左のマス
        }
        if (id_z + 1 < GRID_NUM && !SearchPieceHere(new Vector2Int(id_x, id_z + 1)))
        {
            EnableUnEnableGrid(new Vector2Int(id_x, id_z + 1), highlight); // 右のマス
        }

        // 斜め移動可能のとき
        if (GameObject
            .FindGameObjectWithTag("GameManager")
            .GetComponent<ManageCard>()
            .card == ManageCard.Card.Naname)
        {
            if (0 <= id_x - 1 && 0 <= id_z - 1 && !SearchPieceHere(new Vector2Int(id_x - 1, id_z - 1)))
            {
                EnableUnEnableGrid(new Vector2Int(id_x - 1, id_z - 1), highlight); // 左上マス
            }
            if (id_x + 1 < GRID_NUM && 0 <= id_z - 1 && !SearchPieceHere(new Vector2Int(id_x + 1, id_z - 1)))
            {
                EnableUnEnableGrid(new Vector2Int(id_x + 1, id_z - 1), highlight); // 左下マス
            }
            if (0 <= id_z - 1 && id_z + 1 < GRID_NUM && !SearchPieceHere(new Vector2Int(id_x - 1, id_z + 1)))
            {
                EnableUnEnableGrid(new Vector2Int(id_x - 1, id_z + 1), highlight); // 右上マス
            }
            if (id_z + 1 < GRID_NUM && id_z + 1 < GRID_NUM && !SearchPieceHere(new Vector2Int(id_x + 1, id_z + 1)))
            {
                EnableUnEnableGrid(new Vector2Int(id_x + 1, id_z + 1), highlight); // 右下マス
            }
        }

    }
    /// <summary>
    /// 指定位置のマスを 強調する・強調を解除
    /// </summary>
    private void EnableUnEnableGrid(Vector2Int posID, bool enable)
    {
        //Debug.Log(gridArray[posID.x, posID.y]);
        BoardGrid grid = gridArray[posID.x, posID.y].GetComponent<BoardGrid>();
        if (grid != null)
        {
            grid.EnableUnEnebleMyGrid(enable);
        }
    }
    /// <summary>
    /// 全てのマスのハイライトを解除
    /// </summary>
    public void ClearHighLight()
    {
        foreach (GameObject gridObj in gridArray)
        {
            gridObj.GetComponent<BoardGrid>().EnableUnEnebleMyGrid(false);
        }
    }
    /// <summary>
    /// マスのコライダーを一括で有効/無効にする  有効にするならtrue
    /// </summary>
    public void EnableGridColliders(bool tf)
    {
        foreach (var grid in FindObjectsOfType<BoardGrid>())
        {
            grid.EnableGridCollider(tf);
        }
    }
    /// <summary>
    /// 前後左右のマスに自分の駒が無いところだけコライダーを有効に
    /// </summary>
    public void EnableWASDColliders(Vector2Int posID)
    {
        foreach (var grid in FindObjectsOfType<BoardGrid>())
        {
            if (grid.posID[0] == posID[0] - 1 && grid.posID[1] == posID[1] &&
                !SearchPieceHere(new Vector2Int(posID[0] - 1, posID[1])))
            { grid.EnableGridCollider(true); }
            else if (grid.posID[0] == posID[0] + 1 && grid.posID[1] == posID[1] &&
                !SearchPieceHere(new Vector2Int(posID[0] + 1, posID[1])))
            { grid.EnableGridCollider(true); }
            else if (grid.posID[0] == posID[0] && grid.posID[1] == posID[1] - 1 &&
                !SearchPieceHere(new Vector2Int(posID[0], posID[1] - 1)))
            { grid.EnableGridCollider(true); }
            else if (grid.posID[0] == posID[0] && grid.posID[1] == posID[1] + 1 &&
                !SearchPieceHere(new Vector2Int(posID[0] - 1, posID[1] + 1)))
            { grid.EnableGridCollider(true); }
            else { grid.EnableGridCollider(false); }

            // 斜め移動可能のとき
            if (GameObject
                .FindGameObjectWithTag("GameManager")
                .GetComponent<ManageCard>()
                .card == ManageCard.Card.Naname)
            {
                if (grid.posID[0] == posID[0] - 1 && grid.posID[1] == posID[1] - 1 &&
                    !SearchPieceHere(new Vector2Int(posID[0] - 1, posID[1] - 1)))
                { grid.EnableGridCollider(true); }
                else if (grid.posID[0] == posID[0] + 1 && grid.posID[1] == posID[1] - 1 &&
                    !SearchPieceHere(new Vector2Int(posID[0] + 1, posID[1] - 1)))
                { grid.EnableGridCollider(true); }
                else if (grid.posID[0] == posID[0] - 1 && grid.posID[1] == posID[1] + 1 &&
                    !SearchPieceHere(new Vector2Int(posID[0] - 1, posID[1] + 1)))
                { grid.EnableGridCollider(true); }
                else if (grid.posID[0] == posID[0] + 1 && grid.posID[1] == posID[1] + 1 &&
                    !SearchPieceHere(new Vector2Int(posID[0] + 1, posID[1] + 1)))
                { grid.EnableGridCollider(true); }
            }
        }
    }
    /// <summary>
    /// そのマスに自分の駒があるか
    /// </summary>
    private bool SearchPieceHere(Vector2Int posID)
    {
        Debug.Log("ここに自分の駒がある" + player.GetComponent<ManagePiece>().syncDic.ContainsKey(posID));
        return player.GetComponent<ManagePiece>().syncDic.ContainsKey(posID);
    }
}
