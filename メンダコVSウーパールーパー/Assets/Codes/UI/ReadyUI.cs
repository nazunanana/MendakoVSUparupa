using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ReadyUI : MonoBehaviour
{
    [SerializeField] private GameObject PrototypeRunner;
    void Awake()
    {
        // SceneManager.sceneLoaded += OnSceneLoaded;
        PlayerSpawner.OnSpawnComplete += CompleteSpawn;
        PlayerSpawner.OnShutDown += ShutDown;
        PlayerSpawner.OnFull += Full;
    }
    void OnDestroy()
    {
        // SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerSpawner.OnSpawnComplete -= CompleteSpawn; // イベントから登録解除
        PlayerSpawner.OnShutDown -= ShutDown;
        PlayerSpawner.OnFull -= Full;

    }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    void CompleteSpawn()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(InvokeAfterDelay(() =>
        {
            // Debug.Logを使ってplayerがnullでないことを確認する
            if (player != null)
            {
                var playerState = player.GetComponent<PlayerState>();
                Debug.Log("UIがチームを取得" + playerState.team);
                // 文字を変える
                if (player.GetComponent<PlayerState>().team == PlayerState.Team.uparupa)
                {
                    this.gameObject.GetComponent<TextMeshProUGUI>().text = "マッチ完了！\nあなたはウパルパ陣営です！";
                }
                else
                {
                    this.gameObject.GetComponent<TextMeshProUGUI>().text = "マッチ完了！\nあなたはメンダコ陣営です！";
                }
            }
            else
            {
                Debug.LogError("Player objectがnullです");
            }
        }, 0f));
        // Observable.Timer(TimeSpan.FromMilliseconds(100))
        //     .Subscribe(_ => Debug.Log(player.GetComponent<PlayerState>().team));

    }

    void ShutDown()
    {
        this.gameObject.GetComponent<TextMeshProUGUI>().text = "通信が切断されました";
    }

    void Full()
    {
        this.gameObject.GetComponent<TextMeshProUGUI>().text = "この部屋は満員です";

    }

    private IEnumerator InvokeAfterDelay(System.Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

