using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinSession : SimulationBehaviour
//, IPlayerJoined
//INetworkRunnerCallbacks
{

    [SerializeField]
    private TMP_InputField inputText;

    //ルーム番号
    private string roomName;

    [SerializeField]
    private GameObject playerPrefab;

    // [SerializeField]
    // private NetworkRunner runnerPrefab;
    private NetworkRunner runner;

    private PlayerRef playerRef;

    private void Start()
    {
        inputText = inputText.GetComponent<TMP_InputField>();

        //runner = NetworkManager.Instance.Runner;

        // runner = Instantiate(runnerPrefab);

        // runner.ProvideInput = true;
        // DontDestroyOnLoad(runner);
    }

    // private async void InputPassword()
    // {
    //     roomName = inputText.text;
    //     // 入力欄が空欄の時
    //     if (string.IsNullOrEmpty(roomName))
    //     {
    //         Debug.Log("パスワードを入力してください。");
    //         return;
    //     }

    //     Debug.Log("セッションに参加します");

        // if (runner == null)
        // {
        //     Debug.LogError("NetworkRunnerが見つかりません");
        //     return;
        // }

        // StartGameArgsに渡した設定で、セッションに参加する
        // var result = await runner.StartGame(
        //     new StartGameArgs()
        //     {
        //         // セッション名
        //         SessionName = roomName,
        //         // 新規セッションを作成できるか決めるフラグ
        //         EnableClientSessionCreation = true,
        //         // セッションに参加できる最大プレイヤー数
        //         PlayerCount = 2,
        //         GameMode = GameMode.Shared,
        //         SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        //     }
        // );

        // if (result.Ok)
        // {
            // Debug.Log("セッション参加しました。");
            // playerRef = runner.LocalPlayer;

            // runner.ActivePlayers.Countで現在参加しているプレイヤー数が確認できる
            // if (runner.SessionInfo.PlayerCount == 1)
            // {
            //     // プレイヤーがまだ1人だけなら待機
            //     Debug.Log("プレイヤーを探しています…");
            // }

            // // コルーチンを開始してプレイヤー数が2人になるのを待つ
            // StartCoroutine(WaitForPlayers());

        // }
        // else
        // {
        //     if (result.ShutdownReason == ShutdownReason.GameIsFull)
        //     {
        //         Debug.LogError("セッションは満員です。別のパスワードを試してください。");
        //     }
        //     else
        //     {
        //         Debug.LogError("セッション参加に失敗しました");
        //     }
        // }
        // IEnumerator WaitForPlayers()
        // {
        //     while (runner.SessionInfo.PlayerCount != 2)
        //     {
        //         yield return null; // 1フレーム待つ
        //     }

        //     // プレイヤーが2人集まったらシーンを変更する
        //     Debug.Log("マッチ成功！");

        //     //SceneManager.LoadScene("SC_Ready");
        // }
    }
    // public void PlayerJoined(PlayerRef player)
    // {
    //     if (player == Runner.LocalPlayer)
    //     {
    //         var playerObj = Runner.Spawn(playerPrefab);
    //         if (playerObj != null)
    //         {
    //             Debug.Log("プレイヤー" + player.PlayerId + " がスポーンしました。");
    //         }
    //     }

    //     // runner.ActivePlayers.Countで現在参加しているプレイヤー数が確認できる
    //     if (Runner.SessionInfo.PlayerCount == 1)
    //     {
    //         // プレイヤーがまだ1人だけなら待機
    //         Debug.Log("プレイヤーを探しています…");
    //     }
    //     // コルーチンを開始してプレイヤー数が2人になるのを待つ
    //     StartCoroutine(WaitForPlayers());

    //     IEnumerator WaitForPlayers()
    //     {
    //         while (Runner.SessionInfo.PlayerCount != 2)
    //         {
    //             yield return null; // 1フレーム待つ
    //         }

    //         // プレイヤーが2人集まったらシーンを変更する
    //         Debug.Log("マッチ成功！");

    //         //SceneManager.LoadScene("SC_Ready");
    //     }
    // }
    // public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    // {
    //     Debug.Log("ローカルプレイヤー: " + player.PlayerId);

    //     // プレハブが正しく設定されているかチェック
    //     if (playerPrefab == null)
    //     {
    //         Debug.LogError("Player Prefabが設定されていません。インスペクターで設定してください。");
    //         return;
    //     }
    //     Debug.Log("Player Prefabは設定されています。");

    //     try
    //     {
    //         var playerObj = runner.Spawn(playerPrefab);
    //         Debug.Log(player.PlayerId + "人目のプレイヤーをスポーンしました");
    //         if (playerObj == null)
    //         {
    //             Debug.Log("PlayerObjがnullです");
    //         }
    //     }
    //     catch (NetworkObjectSpawnException ex)
    //     {
    //         Debug.LogError("プレイヤーのスポーンに失敗しました: " + ex.Message);
    //         Debug.LogError("詳細: " + ex.ToString());
    //     }

    // }

    // public void OnConnectedToServer(NetworkRunner runner) { }
    // public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    // public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    // public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    // public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    // public void OnInput(NetworkRunner runner, NetworkInput input) { }
    // public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    // public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    // public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    // public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    // public void OnSceneLoadDone(NetworkRunner runner) { }
    // public void OnSceneLoadStart(NetworkRunner runner) { }
    // public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    // public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    // public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    // public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    // public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    // public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

//}
