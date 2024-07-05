using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameLauncher : MonoBehaviour
{
    [SerializeField]
    private NetworkRunner networkRunnerPrefab;
    [SerializeField]
   private NetworkPrefabRef playerPrefab;

    private NetworkRunner networkRunner;

    private async void Start()
    {
        // NetworkRunnerを生成する
        networkRunner = Instantiate(networkRunnerPrefab);
        // StartGameArgsに渡した設定で、セッションに参加する
        var result = await networkRunner.StartGame(new StartGameArgs
        {
            // セッション名（ランダムな文字列のパスワード）
             SessionName = "aaaaa",
            // 新規セッションを作成できるか決めるフラグ
            EnableClientSessionCreation = true,
            // セッションに参加できる最大プレイヤー数
            PlayerCount = 2,

            GameMode = GameMode.Shared,
            SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log("成功！");
        }
        else
        {
            Debug.Log("失敗！");
        }

    }
}
