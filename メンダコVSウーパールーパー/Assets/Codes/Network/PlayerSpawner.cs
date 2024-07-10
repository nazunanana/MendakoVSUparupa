using Fusion;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    private Button readyBtn;
    private Dictionary<PlayerRef, bool> playerReadyStates = new Dictionary<PlayerRef, bool>();
    //[Networked, OnChangedRender(nameof(PlayersReady))] public bool isReady { get; set; } = false;

    // public void Start()
    // {
    //     readyBtn = gameObject.GetComponent<Button>();
    //     readyBtn.interactable = false;
    //     readyBtn.onClick.AddListener(OnReadyButtonClicked);
    // }

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            var playerObj = Runner.Spawn(PlayerPrefab);
            if (playerObj != null)
            {
                Debug.Log("プレイヤー" + player.PlayerId + " がスポーンしました。");
            }

            // シーン遷移してもプレイヤーオブジェクトが消えないようにする
            DontDestroyOnLoad(playerObj.gameObject);

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
                Debug.Log("あなたは" + team + "チームです");

                //readyBtn.interactable = true;
                //StartCoroutine(WaitLoading());
                
                SceneManager.LoadScene("SC_SetPieces");


            }

            // IEnumerator WaitLoading()
            // {
            //     // 3秒間待つ
            //     yield return new WaitForSeconds(3);

            //     // 3秒後にシーン遷移
            //     SceneManager.LoadScene("SC_SetPieces");
            // }

        }
    }

    // プレイヤー陣営決め
    private string setPlayerState(NetworkObject playerObj, PlayerRef playerRef)
    {
        //プレイヤーごとにisUparupaTeam設定
        var plSettingGame = playerObj.GetComponent<SettingGame>();

        plSettingGame.setTeam(playerRef.PlayerId);
        if (plSettingGame.getTeam())
        {
            if (playerRef.PlayerId != 1)
            {
                return "[Null]";
            }
            return "[ウーパールーパー]";
        }
        else
        {
            return "[メンダコ]";
        }

        // PlayerRefとNetworkObjectの関連付け
        Runner.SetPlayerObject(playerRef, playerObj);
        var playerData = playerObj.GetComponent<PlayerState>();

        playerData.getsetObject = playerObj;
    }

    // void PlayersReady()
    // {
    //     Debug.Log("変更を検知しました");
    //     if(isReady){
    //         Debug.Log("全プレイヤー準備完了");
    //     }
    // }

    // // プレイヤーがreadyボタンを押したら
    // public void OnReadyButtonClicked()
    // {
    //     //playerReadyStates[Runner.LocalPlayer] = true;
    //     //CheckAllPlayersReady();

    //     isReady = true;
    //     Debug.Log("ボタンを押しました");
    // }

    // private void CheckAllPlayersReady()
    // {
    //     // playerReadyState変数にtrue/falseを順番に格納
    //     foreach (var playerReadyState in playerReadyStates.Values)
    //     {
    //         if (!playerReadyState)
    //         {
    //             return;
    //         }
    //     }

    //     // 全員が準備完了ならシーンをロード
    //     SceneManager.LoadScene("SC_Ready");
    // }

    // public override void FixedUpdateNetwork()
    // {
    //     base.FixedUpdateNetwork();
    //     CheckAllPlayersReady();
    // }
}
