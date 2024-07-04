using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

/// <summary>
/// シーン遷移を両プレイヤーに対して実行されるようにする(処理を共有)
/// </summary>

public class ChangeSceneByRemote : NetworkBehaviour
{
    public static ChangeSceneByRemote Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されないようにする
        }
        else
        {
           Destroy(gameObject); // 既にインスタンスが存在する場合は破棄する
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcLoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
