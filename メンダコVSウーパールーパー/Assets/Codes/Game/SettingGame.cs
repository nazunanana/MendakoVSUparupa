using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class SettingGame : NetworkBehaviour
{
    // private GameObject PL_uparupa;
    // private GameObject PL_mendako;
    private GameObject player;
    [SerializeField] private GameObject playerPrehab;
    private GameObject manageGrid;
    [SerializeField] private GameObject gridSystemPrehab;
    private GameObject cam;
    [SerializeField] private bool isUparupaTeam = true;
    // UI
    [SerializeField] private GameObject UI_message;
    [SerializeField] private GameObject UI_finishMessage;
    [SerializeField] private GameObject UI_real;
    [SerializeField] private GameObject UI_fake;
    /// <summary> ウパルパ陣営のカメラ位置、向き（定数） </summary>
    private const int UPA_CAMERA_POSITION_Z = 13;
    private const int UPA_CAMERA_ROTATION_Y = 180;
    /// <summary> メンダコ陣営のカメラ位置、向き（定数） </summary>
    private const int MEN_CAMERA_POSITION_Z = -13;
    private const int MEN_CAMERA_ROTATION_Y = 0;
    private bool isInitialized = false; // 初期化完了フラグ
    void Awake()
    {
        Debug.Log("Awake");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("シーンロード完了。初期化処理を行います。");
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        Debug.Log("team : " + isUparupaTeam);
        WaitLoading(2);
        NetworkRunner runner = GameObject.FindGameObjectWithTag("Runner").GetComponent<NetworkRunner>();
        Debug.Log("runnning:"+runner.IsRunning);
        Initialize();

    }
    IEnumerator WaitLoading(float time)
    {
        // 待つ
        yield return new WaitForSeconds(time);
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Initialize()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] runners = GameObject.FindGameObjectsWithTag("Runner");
        NetworkRunner runner = runners[0].GetComponent<NetworkRunner>();
        foreach(GameObject g in runners){
            if(g.GetComponent<NetworkRunner>().IsRunning){
                runner = g.GetComponent<NetworkRunner>();
                break;
            }
        }
        //Debug.Log("runnning:"+runner.IsRunning);
        
        //NetworkRunner plyerred = GameObject.FindGameObjectsWithTag("Runner").GetComponent<PlayerSpawner>().getRunner;
        if (runner.TryGetPlayerObject(runner.LocalPlayer, out var plObject))
        {
            player = plObject.gameObject;
        }

        player.GetComponent<PlayerState>().InitArray();
        // マネージャーを通知
        player.GetComponent<PlayerState>().setManager = this.gameObject;
        // UIを渡す
        GameObject[] UIarray = new GameObject[] { UI_message, UI_finishMessage, UI_real, UI_fake };
        player.GetComponent<SettingUI>().setUIObject(UIarray);
        // カメラ位置設定
        SetCameraPos(isUparupaTeam);
        // ピース配置開始
        player.GetComponent<CreatePiece>().CreateInitPieces();
        // グリッド管理オブジェクト生成、プレイヤーオブジェクトを伝達
        manageGrid = Instantiate(gridSystemPrehab, gridSystemPrehab.transform.position, Quaternion.identity);
        DontDestroyOnLoad(manageGrid);
        manageGrid.GetComponent<ManageGrid>().setImUparupa = getTeam();
        manageGrid.GetComponent<ManageGrid>().SetPlayers = GameObject.FindGameObjectsWithTag("Player"); //グリッド生成
        player.GetComponent<PlayerState>().setManageGrid = manageGrid;

        isInitialized = true; // 初期化完了
    }

    // バックアップ！！！！！！！！！
    // private void Initialize_backup()
    // {
    //     // プレイヤーオブジェクトを作成
    //     PL_uparupa = Instantiate(playerPrehab, playerPrehab.transform.position, Quaternion.identity);
    //     DontDestroyOnLoad(PL_uparupa);
    //     //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    //     //PL_uparupa = players[0];
    //     //players[0].name = "PL_uparupa";
    //     PL_uparupa.name = "PL_uparupa";
    //     PL_mendako = Instantiate(playerPrehab, playerPrehab.transform.position, Quaternion.identity);
    //     DontDestroyOnLoad(PL_mendako);
    //     //PL_mendako = players[1];
    //     PL_mendako.name = "PL_mendako";
    //     // チーム設定
    //     PL_uparupa.GetComponent<PlayerState>().getsetTeam = PlayerState.Team.uparupa;
    //     PL_mendako.GetComponent<PlayerState>().getsetTeam = PlayerState.Team.mendako;
    //     // マネージャーを通知
    //     PL_uparupa.GetComponent<PlayerState>().setManager = this.gameObject;
    //     PL_mendako.GetComponent<PlayerState>().setManager = this.gameObject;
    //     // UIを渡す
    //     GameObject[] UIarray = new GameObject[]{UI_message, UI_finishMessage, UI_real, UI_fake};
    //     PL_uparupa.GetComponent<SettingUI>().setUIObject(UIarray);
    //     PL_mendako.GetComponent<SettingUI>().setUIObject(UIarray);
    //     // カメラ位置設定
    //     SetCameraPos(isUparupaTeam);
    //     // ピース配置開始
    //     PL_uparupa.GetComponent<CreatePiece>().CreateInitPieces();
    //     PL_mendako.GetComponent<CreatePiece>().CreateInitPieces();
    //     // グリッド管理オブジェクト生成、プレイヤーオブジェクトを伝達
    //     manageGrid = Instantiate(gridSystemPrehab, gridSystemPrehab.transform.position, Quaternion.identity);
    //     DontDestroyOnLoad(manageGrid);
    //     manageGrid.GetComponent<ManageGrid>().setImUparupa = isUparupaTeam ? true : false;
    //     manageGrid.GetComponent<ManageGrid>().SetPlayers = new GameObject[]{PL_uparupa, PL_mendako}; //グリッド生成
    //     PL_uparupa.GetComponent<PlayerState>().setManageGrid = manageGrid;
    //     PL_mendako.GetComponent<PlayerState>().setManageGrid = manageGrid;

    //     isInitialized = true; // 初期化完了
    // }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized) return; // 初期化が完了していない場合は何もしない
        bool finishSet = false;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (obj.GetComponent<PlayerState>().selectMode == PlayerState.SelectMode.SetAllPieces)
            {
                finishSet = true;
            }
            else
            {
                finishSet = false;
            }
        }
        if (finishSet)
        {
            SceneManager.LoadScene("SC_Game");
        }
    }

    private void SetCameraPos(bool isUparupaTeam)
    {
        if (isUparupaTeam)
        {
            cam.transform.position = new Vector3(0, 10, UPA_CAMERA_POSITION_Z);
            cam.transform.rotation = Quaternion.Euler(48, UPA_CAMERA_ROTATION_Y, 0);
        }
        else
        {
            cam.transform.position = new Vector3(0, 10, MEN_CAMERA_POSITION_Z);
            cam.transform.rotation = Quaternion.Euler(48, MEN_CAMERA_ROTATION_Y, 0);
        }
    }

    public void setTeam(int num)
    { // 陣営決め
        if (num == 1)
        {
            isUparupaTeam = true;
        }
        else if (num == 2)
        {
            isUparupaTeam = false;
        }
    }
    public bool getTeam()
    {
        return isUparupaTeam;
    }
}
