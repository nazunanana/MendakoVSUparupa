using Fusion;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public static event Action OnSpawnComplete;
    public GameObject PlayerPrefab;
    private Button readyBtn;
    private Dictionary<PlayerRef, bool> playerReadyStates = new Dictionary<PlayerRef, bool>();
    //[Networked, OnChangedRender(nameof(PlayersReady))] public bool isReady { get; set; } = false;

    // public void Start()
    // {
    //     readyBtn = gameObject.GetComponent<Button>();
    //     readyBtn.interactable = false;
    //     readyBtn.onClick.AddListener(OnReadyButtonClicked);
    // }

    public NetworkRunner getRunner{
    get { return Runner; }
}
    public void PlayerJoined(PlayerRef player)
{
    if (player == Runner.LocalPlayer)
    {
        var playerObj = Runner.Spawn(PlayerPrefab);
        if (playerObj != null)
        {
            // OnSpawnComplete?.Invoke();
            Debug.Log("プレイヤー" + player.PlayerId + " がスポーンしました。");
        }

        // シーン遷移してもプレイヤーオブジェクトが消えないようにする
        //DontDestroyOnLoad(playerObj.gameObject);

        //オブジェクト名設定
        if (player.PlayerId == 1)
        {
            playerObj.gameObject.name = "PL_uparupa";
        }
        else if (player.PlayerId == 2)
        {
            playerObj.gameObject.name = "PL_mendako";
        }
        // runner.ActivePlayers.Countで現在参加しているプレイヤー数が確認できる
        if (Runner.SessionInfo.PlayerCount == 1)
        {
            // プレイヤーがまだ1人だけなら待機
            Debug.Log("プレイヤーを探しています…");
        }

        // コルーチンを開始してプレイヤー数が2人になるのを待つ
        StartCoroutine(WaitForPlayers());

        IEnumerator WaitForPlayers()
        {
            while (Runner.SessionInfo.PlayerCount != 2)
            {
                yield return null; // 1フレーム待つ
            }

            // プレイヤーが2人集まったら陣営決め
            Debug.Log("マッチ成功！");
            string team = setPlayerState(playerObj, player);
            Debug.Log("あなたは" + team + "チームです");

            OnSpawnComplete?.Invoke();

            //readyBtn.interactable = true;
            Debug.Log("SC_SetPiecesへ");
            StartCoroutine(WaitLoading());


        }

        IEnumerator WaitLoading()
        {
            // 待つ
            yield return new WaitForSeconds(3);

            // 3秒後にシーン遷移
            SceneManager.LoadScene("SC_SetPieces");
        }

    }
}

// プレイヤー陣営決め
private string setPlayerState(NetworkObject playerObj, PlayerRef playerRef)
{
    //プレイヤーごとにisUparupaTeam設定
    // var plSettingGame = playerObj.GetComponent<SettingGame>();

    // plSettingGame.setTeam(playerRef.PlayerId);

    playerObj.GetComponent<PlayerState>().getsetTeam = (playerRef.PlayerId == 1) ? PlayerState.Team.uparupa : PlayerState.Team.mendako;

    // PlayerRefとNetworkObjectの関連付け
    Runner.SetPlayerObject(playerRef, playerObj);
    var playerData = playerObj.GetComponent<PlayerState>();

    playerData.getsetObject = playerObj;
    // Debug.Log("プレイヤー名" + playerObj.gameObject.name);

    // Debug.Log("ready runner runnning:"+Runner.IsRunning + "方は"+Runner.GetType());

    if (playerObj.GetComponent<PlayerState>().getsetTeam == PlayerState.Team.uparupa)
    {
        if (playerRef.PlayerId != 1)
        {
            return "[Null]";
        }
        return "[ウーパールーパー]";
    }
    else if (playerObj.GetComponent<PlayerState>().getsetTeam == PlayerState.Team.mendako)
    {
        return "[メンダコ]";
    }
    else
    {
        return "[Null]";
    }
}

    // void PlayersReady()
    // {
    //     Debug.Log("変更を検知しました");
    //     if(isReady){
    //         Debug.Log("全プレイヤー準備完了");
    //     }
    // }

    // // プレイヤーがreadyボタンを押したら
    // public void OnReadyButtonClicked()
    // {
    //     //playerReadyStates[Runner.LocalPlayer] = true;
    //     //CheckAllPlayersReady();

    //     isReady = true;
    //     Debug.Log("ボタンを押しました");
    // }

    // private void CheckAllPlayersReady()
    // {
    //     // playerReadyState変数にtrue/falseを順番に格納
    //     foreach (var playerReadyState in playerReadyStates.Values)
    //     {
    //         if (!playerReadyState)
    //         {
    //             return;
    //         }
    //     }

    //     // 全員が準備完了ならシーンをロード
    //     SceneManager.LoadScene("SC_Ready");
    // }

    // public override void FixedUpdateNetwork()
    // {
    //     base.FixedUpdateNetwork();
    //     CheckAllPlayersReady();
    // }
}
