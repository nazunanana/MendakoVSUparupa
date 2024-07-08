using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private NetworkPrefabRef playerPrefab;
    private NetworkRunner runner;
    private bool isPlayerSpawned = false;

    private void Start()
    {
        if (!playerPrefab.IsValid)
        {
            Debug.LogError("Player Prefabが設定されていません。インスペクターで設定してください。");
            return;
        }
        runner = NetworkManager.Instance.Runner;

        if (runner == null)
        {
            Debug.LogError("NetworkRunnerが見つかりません。NetworkManagerが正しく初期化されているか確認してください。");
            return;
        }

        Debug.Log("NetworkRunnerが見つかりました");

        runner.AddCallbacks(this);
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (isPlayerSpawned)
        {
            return;
        }

        //PlayerRef player = runner.LocalPlayer;

        Debug.Log("ローカルプレイヤー: " + player.PlayerId);

        try
        {
            // ローカルプレイヤーの場合にのみスポーンする
            runner.Spawn(playerPrefab);
            Debug.Log(player.PlayerId + "人目のプレイヤーをスポーンしました");
            isPlayerSpawned = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("プレイヤーのスポーンに失敗しました: " + ex.Message);
            Debug.LogError("詳細: " + ex.ToString());
        }
    }

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse (NetworkRunner runner, Dictionary< string, object > data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){ }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }

}
