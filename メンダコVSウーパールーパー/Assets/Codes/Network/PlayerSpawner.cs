using Fusion;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            var playerObj = Runner.Spawn(PlayerPrefab);
            if (playerObj != null)
            {
                Debug.Log("プレイヤー" + player.PlayerId + " がスポーンしました。");
            }
            // runner.ActivePlayers.Countで現在参加しているプレイヤー数が確認できる
            if (Runner.SessionInfo.PlayerCount == 1)
            {
                // プレイヤーがまだ1人だけなら待機
                Debug.Log("プレイヤーを探しています…");
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
                Debug.Log("あなたは"+team+"チームです");

                //SceneManager.LoadScene("SC_Ready");
            }
        }
    }

    private string setPlayerState(NetworkObject playerObj, PlayerRef playerRef)
    {
        //プレイヤーごとにisUparupaTeam設定
        var playerData = playerObj.GetComponent<SettingGame>();

        playerData.setTeam(playerRef.PlayerId);
        if (playerData.getTeam())
        {
            if(playerRef.PlayerId!=1){
                return "[Null]";
            }
            return "[ウーパールーパー]";
        }
        else
        {
            return "[メンダコ]";
        }
    }
}
