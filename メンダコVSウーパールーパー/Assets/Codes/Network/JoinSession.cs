using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinSession : MonoBehaviour
{

    [SerializeField]
    private TMP_InputField inputText;

    //ルーム番号
    private string roomName;

    private NetworkRunner runner;

    private void Start()
    {
        inputText = inputText.GetComponent<TMP_InputField>();

        runner = NetworkManager.Instance.Runner;
    }

    private async void InputPassword()
    {
        roomName = inputText.text;
        // 入力欄が空欄の時
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.Log("パスワードを入力してください。");
            return;
        }
        
        Debug.Log("セッションに参加します");

        if (runner == null)
        {
            Debug.LogError("NetworkRunnerが見つかりません。NetworkManagerが正しく初期化されているか確認してください。");
            return;
        }

        //runner = gameObject.AddComponent<NetworkRunner>();
        //runner.ProvideInput = true;

        // StartGameArgsに渡した設定で、セッションに参加する
        var result = await runner.StartGame(
            new StartGameArgs()
            {
                // セッション名
                SessionName = roomName,
                // 新規セッションを作成できるか決めるフラグ
                EnableClientSessionCreation = true,
                // セッションに参加できる最大プレイヤー数
                PlayerCount = 2,
                GameMode = GameMode.Shared,
                SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
            }
        );

        if (result.Ok)
        {
            Debug.Log("セッション参加しました。");

            // runner.ActivePlayers.Countで現在参加しているプレイヤー数が確認できる
            if (runner.SessionInfo.PlayerCount == 1)
            {
                // プレイヤーがまだ1人だけなら待機
                Debug.Log("プレイヤーを探しています…");
            }

            // コルーチンを開始してプレイヤー数が2人になるのを待つ
            StartCoroutine(WaitForPlayers());


            //setPlayerState(playerObj, networkRunner.LocalPlayer);
        }
        else
        {
            if (result.ShutdownReason == ShutdownReason.GameIsFull)
            {
                Debug.LogError("セッションは満員です。別のパスワードを試してください。");
            }
            else
            {
                Debug.LogError("セッション参加に失敗しました");
            }
        }
        IEnumerator WaitForPlayers()
        {
            while (runner.SessionInfo.PlayerCount != 2)
            {
                yield return null; // 1フレーム待つ
            }

            // プレイヤーが2人集まったらシーンを変更する
            Debug.Log("マッチ成功！");

            SceneManager.LoadScene("SC_Ready");
        }
    }
    // private IEnumerator SpawnPlayer()
    // {
    //     yield return new WaitUntil(() => runner.SessionInfo.PlayerCount == 2);

    //     PlayerRef player = runner.LocalPlayer;

    //     runner.Spawn(playerPrefab);
    //     Debug.Log(player.PlayerId + "人目のプレイヤーをスポーンしました");
    // }

    // public void PlayerJoined(PlayerRef player)
    // {
    //     Debug.Log("Player." + player.PlayerId + " が参加しました");

    //     networkRunner.Spawn(playerPrefab);
    // }

    // private void setPlayerState(NetworkObject playerObj, PlayerRef playerRef)
    // {
    //     //プレイヤーごとにisUparupaTeam設定
    //     var playerData = playerObj.GetComponent<SettingGame>();

    //     if (playerRef.PlayerId == 1)
    //     {
    //         playerData.isUparupaTeam = true;
    //     }
    //     else if (playerRef.PlayerId == 2)
    //     {
    //         playerData.isUparupaTeam = false;
    //     }
    // }
}
