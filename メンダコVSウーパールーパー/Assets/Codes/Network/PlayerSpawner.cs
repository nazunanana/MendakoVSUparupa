using Fusion;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public static event Action OnSpawnComplete;
    public static event Action OnShutDown;
    public static event Action OnFull;
    public GameObject PlayerPrefab;
    private Dictionary<PlayerRef, bool> playerReadyStates = new Dictionary<PlayerRef, bool>();
    [SerializeField]
    private Button backButton;
    private NetworkObject playerObj;
    public NetworkRunner getRunner
    {
        get { return Runner; }
    }
    public NetworkObject getPlayerObj
    {
        get { return playerObj; }
    }
    void Start()
    {
        var backButtonComp = backButton.GetComponent<Button>();
        backButtonComp.onClick.AddListener(backButtonClicked);
        backButton.interactable = false;
    }
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            playerObj = Runner.Spawn(PlayerPrefab);
            if (getPlayerObj != null)
            {
                Debug.Log("プレイヤー" + player.PlayerId + " がスポーンしました。");
            }
            backButton.interactable = true;


            //オブジェクト名設定
            if (player.PlayerId == 1)
            {
                getPlayerObj.gameObject.name = "PL_uparupa";
            }
            else if (player.PlayerId == 2)
            {
                getPlayerObj.gameObject.name = "PL_mendako";
            }
            // runner.ActivePlayers.Countで現在参加しているプレイヤー数が確認できる
            if (Runner.SessionInfo.PlayerCount == 1)
            {
                // プレイヤーがまだ1人だけなら待機
                Debug.Log("プレイヤーを探しています…");
            }
            else if (Runner.SessionInfo.PlayerCount > 2)
            {
                Debug.Log("満員です。");
                OnFull?.Invoke();
                getRunner.Despawn(getPlayerObj);
                getRunner.Shutdown();
                return;
            }

            // コルーチンを開始してプレイヤー数が2人になるのを待つ
            StartCoroutine(WaitForPlayers());

            IEnumerator WaitForPlayers()
            {
                while (Runner.SessionInfo.PlayerCount != 2)
                {
                    yield return null; // 1フレーム待つ
                }

                // プレイヤーが2人集まったら陣営決め
                backButton.interactable = false;
                Debug.Log("マッチ成功！");
                string team = setPlayerState(player);
                Debug.Log("あなたは" + team + "チームです");
                Debug.Log("session: " + Runner.SessionInfo.Name);

                OnSpawnComplete?.Invoke();

                Debug.Log("SC_SetPiecesへ");
                StartCoroutine(WaitLoading());

            }

            IEnumerator WaitLoading()
            {
                // 待つ
                yield return new WaitForSeconds(3);

                // もし切断されたら
                if (Runner.SessionInfo.PlayerCount == 1)
                {
                    OnShutDown?.Invoke();
                    getRunner.Despawn(getPlayerObj);
                    getRunner.Shutdown(shutdownReason: ShutdownReason.Ok);
                    backButton.interactable = true;
                }
                else
                {
                    // 3秒後にシーン遷移
                    SceneManager.LoadScene("SC_SetPieces");
                }
            }

        }
    }

    // プレイヤー陣営決め
    private string setPlayerState(PlayerRef playerRef)
    {
        getPlayerObj.GetComponent<PlayerState>().team = (playerRef.PlayerId == 1) ? PlayerState.Team.uparupa : PlayerState.Team.mendako;

        // PlayerRefとNetworkObjectの関連付け
        Runner.SetPlayerObject(playerRef, getPlayerObj);
        var playerData = getPlayerObj.GetComponent<PlayerState>();


        if (getPlayerObj.GetComponent<PlayerState>().team == PlayerState.Team.uparupa)
        {
            if (playerRef.PlayerId != 1)
            {
                return "[Null]";
            }
            return "[ウーパールーパー]";
        }
        else if (getPlayerObj.GetComponent<PlayerState>().team == PlayerState.Team.mendako)
        {
            return "[メンダコ]";
        }
        else
        {
            return "[Null]";
        }
    }

    void backButtonClicked()
    {
        backButton.interactable = false;
        if (getRunner != null && getRunner.SessionInfo.PlayerCount >= 2)
        {
            Debug.Log("戻れませんでした");
            return;
        }
        if (getRunner != null)
        {
            if (getPlayerObj != null)
            {
                getRunner.Despawn(getPlayerObj);
                Debug.Log("プレイヤーがデスポーンしました");
            }
            getRunner.Shutdown(shutdownReason: ShutdownReason.Ok);
        }
        SceneManager.LoadScene("SC_EnterRoom");
    }

}
