using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

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
        inputText = inputText.GetComponent<TMP_InputField> ();
    }
    public async void InputPassword()
    {
        roomName = inputText.text;
        // 入力欄が空欄の時
        if(roomName == ""){
            Debug.Log("パスワードを入力してください。");
            return;
        }

        // NetworkRunnerを生成する
        networkRunner = Instantiate(networkRunnerPrefab);
        // StartGameArgsに渡した設定で、セッションに参加する
        var result = await networkRunner.StartGame(new StartGameArgs
        {
            // セッション名
            SessionName = roomName,
            // 新規セッションを作成できるか決めるフラグ
            EnableClientSessionCreation = true,
            // セッションに参加できる最大プレイヤー数
            PlayerCount = 2,

            GameMode = GameMode.Shared,
            SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            Debug.Log("セッション参加しました。");
        }
        else
        {
            Debug.Log("セッション参加に失敗しました");
        }
    }
}
