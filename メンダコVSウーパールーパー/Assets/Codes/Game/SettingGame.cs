using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class SettingGame : NetworkBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject playerPrehab;
    private GameObject manageGrid;
    [SerializeField] private GameObject gridSystemPrehab;
    //private GameObject cam;
    public bool debug;
    public bool debug_upa;
    // [SerializeField] private bool isUparupaTeam = true;
    // UI
    [SerializeField] private GameObject UI_message;
    [SerializeField] private GameObject UI_finishMessage;
    [SerializeField] private GameObject UI_real;
    [SerializeField] private GameObject UI_fake;
    // /// <summary> ウパルパ陣営のカメラ位置、向き（定数） </summary>
    // private const int UPA_CAMERA_POSITION_Z = 13;
    // private const int UPA_CAMERA_ROTATION_Y = 180;
    // /// <summary> メンダコ陣営のカメラ位置、向き（定数） </summary>
    // private const int MEN_CAMERA_POSITION_Z = -13;
    // private const int MEN_CAMERA_ROTATION_Y = 0;
    private bool isInitialized = false; // 初期化完了フラグ
    void Awake()
    {
        Debug.Log("Awake");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("シーンロード完了。初期化処理を行います。");
        //cam = GameObject.FindGameObjectWithTag("MainCamera");
        WaitLoading(2);
        if (debug) DebugInit();
        else Initialize();

    }
    IEnumerator WaitLoading(float time)
    {
        // 待つ
        yield return new WaitForSeconds(time);
    }
    void Start()
    {
        if (isInitialized) PlayerState.OnChangeMode += LoadGameScene;
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (isInitialized) PlayerState.OnChangeMode -= LoadGameScene;

    }
    private void Initialize()
    {
        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // 自分のプレイヤーオブジェクトを取得
        GameObject[] runners = GameObject.FindGameObjectsWithTag("Runner");
        NetworkRunner runner = runners[0].GetComponent<NetworkRunner>();
        foreach (GameObject g in runners)
        {
            if (g.GetComponent<NetworkRunner>().IsRunning)
            { //アクティブのものを検出
                runner = g.GetComponent<NetworkRunner>();
                break;
            }
        }
        if (runner.TryGetPlayerObject(runner.LocalPlayer, out var plObject))
        {
            player = plObject.gameObject; //自分プレイヤー
        }
        // コンポネント取得
        PlayerState playerState = player.GetComponent<PlayerState>();
        // DEBUG:
        Debug.Log("チームは" + playerState.team);
        // マネージャーを通知
        playerState.setManager = this.gameObject;
        // UIを渡す
        GameObject[] UIarray = new GameObject[] { UI_message, UI_finishMessage, UI_real, UI_fake };
        player.GetComponent<SettingUI>().setUIObject(UIarray);
        // カメラ位置設定
        //SetCameraPos(playerState.getsetTeam == PlayerState.Team.uparupa);
        CameraSetting.SetCamera(playerState.team == PlayerState.Team.uparupa);
        // ピース配置開始
        player.GetComponent<CreatePiece>().CreateInitPieces();
        // グリッド管理オブジェクト生成、プレイヤーオブジェクトを伝達
        manageGrid = Instantiate(gridSystemPrehab, gridSystemPrehab.transform.position, Quaternion.identity);
        DontDestroyOnLoad(manageGrid);
        manageGrid.GetComponent<ManageGrid>().setImUparupa = (playerState.team == PlayerState.Team.uparupa);
        manageGrid.GetComponent<ManageGrid>().SetPlayer = player.gameObject;
        //manageGrid.GetComponent<ManageGrid>().SetPlayers = GameObject.FindGameObjectsWithTag("Player");
        player.GetComponent<PlayerState>().setManageGrid = manageGrid;

        isInitialized = true; // 初期化完了
    }

    //デバッグ用
    private void DebugInit()
    {
        // プレイヤーオブジェクトを作成
        if (debug_upa)
        {
            player = Instantiate(playerPrehab, playerPrehab.transform.position, Quaternion.identity);
            DontDestroyOnLoad(player);
            player.name = "PL_uparupa";
            player.GetComponent<PlayerState>().team = PlayerState.Team.uparupa;
        }
        else
        {
            player = Instantiate(playerPrehab, playerPrehab.transform.position, Quaternion.identity);
            DontDestroyOnLoad(player);
            player.name = "PL_mendako";
            player.GetComponent<PlayerState>().team = PlayerState.Team.mendako;
        }
        // マネージャーを通知
        player.GetComponent<PlayerState>().setManager = this.gameObject;
        // UIを渡す
        GameObject[] UIarray = new GameObject[] { UI_message, UI_finishMessage, UI_real, UI_fake };
        player.GetComponent<SettingUI>().setUIObject(UIarray);
        // カメラ位置設定
        //SetCameraPos(debug_upa);
        CameraSetting.SetCamera(debug_upa);
        // ピース配置開始
        player.GetComponent<CreatePiece>().CreateInitPieces();
        // グリッド管理オブジェクト生成、プレイヤーオブジェクトを伝達
        manageGrid = Instantiate(gridSystemPrehab, gridSystemPrehab.transform.position, Quaternion.identity);
        DontDestroyOnLoad(manageGrid);
        manageGrid.GetComponent<ManageGrid>().setImUparupa = debug_upa ? true : false;
        manageGrid.GetComponent<ManageGrid>().SetPlayer = player.gameObject;
        player.GetComponent<PlayerState>().setManageGrid = manageGrid;

        isInitialized = true; // 初期化完了
    }

    // Update is called once per frame

    /// <summary>
    /// 両者の駒配置完了を検出
    /// </summary>
    void LoadGameScene()
    {
        if (!isInitialized) return; // 初期化が完了していない場合は何もしない
        bool finishSet = false;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            // Debug.Log("モード:"+obj.GetComponent<PlayerState>().selectMode);
            PlayerState.SelectMode mode = obj.GetComponent<PlayerState>().selectMode;
            if (mode == PlayerState.SelectMode.SetAllPieces || mode == PlayerState.SelectMode.MovePiece || mode == PlayerState.SelectMode.NoMyTurn)
            {
                finishSet = true;
            }
            else
            {
                return;
            }
        }

        if (finishSet)
        {
            SceneManager.LoadScene("SC_Game");
        }
    }

    // private void SetCameraPos(bool isUparupaTeam)
    // {
    //     if (isUparupaTeam)
    //     {
    //         cam.transform.position = new Vector3(0, 10, UPA_CAMERA_POSITION_Z);
    //         cam.transform.rotation = Quaternion.Euler(48, UPA_CAMERA_ROTATION_Y, 0);
    //     }
    //     else
    //     {
    //         cam.transform.position = new Vector3(0, 10, MEN_CAMERA_POSITION_Z);
    //         cam.transform.rotation = Quaternion.Euler(48, MEN_CAMERA_ROTATION_Y, 0);
    //     }
    // }

    // public void setTeam(int num)
    // { // 陣営決め
    //     if (num == 1)
    //     {
    //         isUparupaTeam = true;
    //     }
    //     else if (num == 2)
    //     {
    //         isUparupaTeam = false;
    //     }
    // }
    // public bool getTeam()
    // {
    //     return isUparupaTeam;
    // }
}
