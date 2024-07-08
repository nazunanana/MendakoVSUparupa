using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance;

    public static NetworkManager Instance
    {
        get { return instance; }
    }

    public NetworkRunner Runner {
        get;
        private set;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 既に別のインスタンスが存在する場合は自身を破棄
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // シーン遷移時に破棄されないようにする

        Runner = gameObject.AddComponent<NetworkRunner>();
        Runner.ProvideInput = true;
    }
}
