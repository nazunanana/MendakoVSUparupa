using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Debug.Log("ChangeSceneByRemote instance created.");
        }
        else if(Instance != this)
        {
            Destroy(gameObject); // 既にインスタンスが存在する場合は破棄する
            Debug.LogWarning("Duplicate ChangeSceneByRemote instance destroyed.");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcLoadScene(string sceneName)
    {
        if (Instance == null)
        {
            Debug.LogError("ChangeSceneByRemote is not initialized properly.");
            return;
        }

        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
