using Fusion;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public static event Action OnSpawnComplete;
    public GameObject PlayerPrefab;
    private Button readyBtn;
    private Dictionary<PlayerRef, bool> playerReadyStates = new Dictionary<PlayerRef, bool>();

    public NetworkRunner getRunner
    {
        get { return Runner; }
    }
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            var playerObj = Runner.Spawn(PlayerPrefab);
            if (playerObj != null)
            {
                Debug.Log("プレイヤー" + player.PlayerId + " がスポーンしました。");
            }


            //オブジェクト名設定
            if (player.PlayerId == 1)
            {
                playerObj.gameObject.name = "PL_uparupa";
            }
            else if (player.PlayerId == 2)
            {
                playerObj.gameObject.name = "PL_mendako";
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
                Runner.Shutdown();
                SceneManager.LoadScene("SC_Start");
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
                Debug.Log("マッチ成功！");
                string team = setPlayerState(playerObj, player);
                Debug.Log("あなたは" + team + "チームです");

                OnSpawnComplete?.Invoke();

                Debug.Log("SC_SetPiecesへ");
                StartCoroutine(WaitLoading());


            }

            IEnumerator WaitLoading()
            {
                // 待つ
                yield return new WaitForSeconds(3);

                // 3秒後にシーン遷移
                SceneManager.LoadScene("SC_SetPieces");
            }

        }
    }

    // プレイヤー陣営決め
    private string setPlayerState(NetworkObject playerObj, PlayerRef playerRef)
    {
        playerObj.GetComponent<PlayerState>().team = (playerRef.PlayerId == 1) ? PlayerState.Team.uparupa : PlayerState.Team.mendako;

        // PlayerRefとNetworkObjectの関連付け
        Runner.SetPlayerObject(playerRef, playerObj);
        var playerData = playerObj.GetComponent<PlayerState>();


        if (playerObj.GetComponent<PlayerState>().team == PlayerState.Team.uparupa)
        {
            if (playerRef.PlayerId != 1)
            {
                return "[Null]";
            }
            return "[ウーパールーパー]";
        }
        else if (playerObj.GetComponent<PlayerState>().team == PlayerState.Team.mendako)
        {
            return "[メンダコ]";
        }
        else
        {
            return "[Null]";
        }
    }

}
