using Fusion;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    //private Button readyBtn;
    //private Dictionary<PlayerRef, bool> playerReadyStates = new Dictionary<PlayerRef, bool>();
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
            if (!Runner.TryGetPlayerObject(player, out var playerObject))
            {

                var playerObj = Runner.Spawn(PlayerPrefab);
                if (playerObj != null)
                {
                    Debug.Log("プレイヤー" + player.PlayerId + " がスポーンしました。");
                }
                Runner.SetPlayerObject(player, playerObj);

                // シーン遷移してもプレイヤーオブジェクトが消えないようにする
                // DontDestroyOnLoad(playerObj.gameObject);

                // runner.ActivePlayers.Countで現在参加しているプレイヤー数が確認できる
                if (Runner.SessionInfo.PlayerCount == 1)
                {
                    // プレイヤーがまだ1人だけなら待機
                    Debug.Log("プレイヤーを探しています…");
                }

                // コルーチンを開始してプレイヤー数が2人になるのを待つ
                StartCoroutine(WaitForPlayers(playerObj, player));

                // IEnumerator WaitLoading()
                // {
                //     // 3秒間待つ
                //     yield return new WaitForSeconds(3);

                //     // 3秒後にシーン遷移
                //     SceneManager.LoadScene("SC_SetPieces");
                // }

            }
        }
    }
    IEnumerator WaitForPlayers(NetworkObject playerObj, PlayerRef player)
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

        //SceneManager.LoadScene("SC_SetPieces");

        if (Runner.IsSceneAuthority)
        {
            yield return new WaitForSeconds(3);
            Runner.LoadScene("SC_SetPieces", LoadSceneMode.Single);
            Runner.MoveGameObjectToScene(playerObj.gameObject, SceneRef.FromIndex(5));
        }


    }

    // プレイヤー陣営決め
    private string setPlayerState(NetworkObject playerObj, PlayerRef playerRef)
    {
        //プレイヤーごとにisUparupaTeam設定
        // var plSettingGame = playerObj.GetComponent<SettingGame>();

        // plSettingGame.setTeam(playerRef.PlayerId);
        // string team = plSettingGame.getTeam() ? "[ウーパールーパー]" : "[メンダコ]";

        // PlayerRefとNetworkObjectの関連付け
        var playerData = playerObj.GetComponent<PlayerState>();

        playerData.getsetObject = playerObj;

        if(playerRef.PlayerId == 1){
            playerData.getsetTeam = PlayerState.Team.uparupa;
            return "[ウーパールーパー]";
        }else if(playerRef.PlayerId == 2){
            playerData.getsetTeam = PlayerState.Team.mendako;
            return "[メンダコ]";
        }else{
            return null;
        }

        // if (plSettingGame.getTeam())
        // {
        //     if (playerRef.PlayerId != 1)
        //     {
        //         return "[Null]";
        //     }
        //     return "[ウーパールーパー]";
        // }
        // else
        // {
        //     return "[メンダコ]";
        // }

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
