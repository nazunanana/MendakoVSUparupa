using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinSession : MonoBehaviour
{
    [SerializeField]
    private NetworkRunner networkRunnerPrefab;

    [SerializeField]
    private NetworkPrefabRef playerPrefab;

    [SerializeField]
    private TMP_InputField inputText;

    private string roomName;

    private NetworkRunner networkRunner;

    private void Start()
    {
        inputText = inputText.GetComponent<TMP_InputField>();
    }

    public async void InputPassword()
    {
        roomName = inputText.text;
        // 入力欄が空欄の時
        if (roomName == "")
        {
            Debug.Log("パスワードを入力してください。");
            return;
        }

        // NetworkRunnerを生成する
        networkRunner = Instantiate(networkRunnerPrefab);

        // StartGameArgsに渡した設定で、セッションに参加する
        var result = await networkRunner.StartGame(
            new StartGameArgs
            {
                // セッション名
                SessionName = roomName,
                // 新規セッションを作成できるか決めるフラグ
                EnableClientSessionCreation = true,
                // セッションに参加できる最大プレイヤー数
                PlayerCount = 2,
                GameMode = GameMode.Shared,
                SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>()
            }
        );

        if (result.Ok)
        {
            Debug.Log("セッション参加しました。");
            // runner.ActivePlayers.Countで現在参加しているプレイヤー数が確認できる

            if (networkRunner.SessionInfo.PlayerCount == 1)
            {
                // プレイヤーがまだ1人だけだったら
                Debug.Log("プレイヤーを探しています…");
            }

            // コルーチンを開始してプレイヤー数が2人になるのを待つ
            StartCoroutine(WaitForPlayers());

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
            while (networkRunner.SessionInfo.PlayerCount != 2)
            {
                yield return null; // 1フレーム待つ
            }

            // プレイヤーが2人集まったらシーンを変更する
            Debug.Log("マッチ成功！");

            var playerObj = networkRunner.SpawnAsync(playerPrefab);
            // var playerSta = plObject.getComponent<PlayerState>;
            // setPlayerState(playerObj, plData);

            // SceneManager.LoadScene("SC_Ready");
        }
    }

    //private void setPlayerState()

}
