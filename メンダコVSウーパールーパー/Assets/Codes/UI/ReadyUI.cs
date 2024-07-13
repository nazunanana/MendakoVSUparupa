using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ReadyUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(InvokeAfterDelay(() =>
        {
            // Debug.Logを使ってplayerがnullでないことを確認する
            if (player != null)
            {
                var playerState = player.GetComponent<PlayerState>();
                if (playerState != null)
                {
                    Debug.Log("UIがチームを取得！" + playerState.getsetTeam);
                    // 文字を変える
                    if (player.GetComponent<PlayerState>().getsetTeam == PlayerState.Team.uparupa)
                    {
                        this.gameObject.GetComponent<TextMeshProUGUI>().text = "あなたはウパルパ陣営です！";
                    }
                    else
                    {
                        this.gameObject.GetComponent<TextMeshProUGUI>().text = "あなたはメンダコ陣営です！";
                    }
                }
                else
                {
                    Debug.LogError("PlayerState component is not found on the player object.");
                }
            }
            else
            {
                Debug.LogError("Player object is null in the delayed action.");
            }
        }, 3.5f));
        // Observable.Timer(TimeSpan.FromMilliseconds(100))
        //     .Subscribe(_ => Debug.Log(player.GetComponent<PlayerState>().getsetTeam));

    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

