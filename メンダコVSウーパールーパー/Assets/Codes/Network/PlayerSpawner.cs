using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using System;

public class PlayerSpawner : SimulationBehaviour
{
    [SerializeField]
    private NetworkPrefabRef playerPrefab;
    private NetworkRunner runner;

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

        PlayerRef player = runner.LocalPlayer;

        Debug.Log("ローカルプレイヤー: " + player.PlayerId);

        // ローカルプレイヤーの場合にのみスポーンする
        //runner.Spawn(playerPrefab);
        //Debug.Log(player.PlayerId + "人目のプレイヤーをスポーンしました");

        try
        {
            //runner.Spawn(playerPrefab);
            Debug.Log(player.PlayerId + "人目のプレイヤーをスポーンしました");
        }
        catch (Exception ex)
        {
            Debug.LogError("プレイヤーのスポーンに失敗しました: " + ex.Message);
            Debug.LogError("詳細: " + ex.ToString());
        }
    }
}
