using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ManageGrid : MonoBehaviour
{
    public GameObject oneGridPrehab = null;
    private GameObject[,] gridArray;
    private GameObject[] players;
    public GameObject player;
    public bool ImUparupa;
    
    private const float FIRST_X = -6.48f; //ウパルパ陣営側
    private const float FIRST_Z = -6.48f;
    private const float Y_POS = 0.04f;
    private const int GRID_NUM = 6;
    private float gridSize = Mathf.Abs(FIRST_X*2/5);
    private Vector2Int piecePosID;
    private Vector2Int movePosID;
    public Vector2Int pieceID {get; set;}
    public void Init(){
        gridArray = new GameObject[GRID_NUM, GRID_NUM];
        pieceID = new Vector2Int(0, 0);
        CreateGrids();
    }
    public GameObject[] SetPlayers{
        set{
            players = value;
            Init();
        }
    }
    public GameObject SetPlayer{
        set{
            player = value;
            Init();
        }
    }
    public bool setImUparupa{
        set{
            ImUparupa = value;
            //Debug.Log("Upa?"+ImUparupa);
        }
    }

    /// <summary>
    /// マスのコライダーオブジェクトを生成
    /// </summary>
    void CreateGrids()
    {
        for(int i=0; i<GRID_NUM; ++i){
            for(int j=0; j<GRID_NUM; ++j){
                Vector3 position = new Vector3(FIRST_X+j*gridSize, Y_POS, FIRST_Z+i*gridSize);
                GameObject newGridObj = Instantiate(oneGridPrehab, position, Quaternion.identity);
                DontDestroyOnLoad(newGridObj);
                BoardGrid gridComponent = newGridObj.GetComponent<BoardGrid>();
                // それぞれのIDと位置を教える
                gridComponent.SetPosition(new Vector2Int(j,i), position);
                gridComponent.SetGridSystemObj = this.gameObject;
                // 配置中なら自分  最初のプレイヤーはウパルパ
                if(SceneManager.GetActiveScene().name == "SC_SetPieces"){
                    //gridComponent.SetNowPlayer = ImUparupa ? this.players[0] : this.players[1];
                    gridComponent.SetNowPlayer = player;
                } else {
                    //gridComponent.SetNowPlayer = this.players[0];
                    gridComponent.SetNowPlayer = player;
                }
                // 生成したマスを配列に登録
                gridArray[j,i] = newGridObj;
                if(i==0 && (j==0 || j==5)) {
                    gridComponent.SetType = (int)BoardGrid.types.corner_mendako;
                }else if(i==5 && (j==0 || j==5)) {
                    gridComponent.SetType = (int)BoardGrid.types.corner_uparupa;
                }else if(i<3) {
                    gridComponent.SetType = (int)BoardGrid.types.mendako;
                }else{
                    gridComponent.SetType = (int)BoardGrid.types.uparupa;
                }
            }
        }
    }

    /// <summary>
    /// カーソル位置ハイライト
    /// </summary>
    public void HighLightGrid(Vector2Int posID, bool highlight){
        //Debug.Log("HighLight: " + posID.x + ", " + posID.y + " : " + highlight);
        EnableUnEnableGrid(new Vector2Int(posID.x, posID.y), highlight);
    }
    /// <summary>
    /// 前後左右のハイライト
    /// </summary>
    public void HighLightWASDGrid(Vector2Int posID, bool highlight){
        //Debug.Log("HighLightWASD: " + posID.x + ", " + posID.y + " : " + highlight);
        int id_x = posID.x;
        int id_z = posID.y;

        EnableUnEnableGrid(new Vector2Int(id_x, id_z), highlight); // 中心のマス
        if(0 <= id_x-1) EnableUnEnableGrid(new Vector2Int(id_x - 1, id_z), highlight); // 上のマス
        if(id_x+1 < GRID_NUM) EnableUnEnableGrid(new Vector2Int(id_x + 1, id_z), highlight); // 下のマス
        if(0 <= id_z-1) EnableUnEnableGrid(new Vector2Int(id_x, id_z - 1), highlight); // 左のマス
        if(id_z+1 < GRID_NUM) EnableUnEnableGrid(new Vector2Int(id_x, id_z + 1), highlight); // 右のマス
    }
    /// <summary>
    /// 指定位置のマスを 強調する・強調を解除
    /// </summary>
    private void EnableUnEnableGrid(Vector2Int posID, bool enable){
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
    public void ClearHighLight(){
        foreach(GameObject gridObj in gridArray){
            gridObj.GetComponent<BoardGrid>().EnableUnEnebleMyGrid(false);
        }
    }

}
