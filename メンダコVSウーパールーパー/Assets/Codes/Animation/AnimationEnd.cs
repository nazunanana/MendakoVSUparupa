using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// attach to アニメーション用のモデルオブジェクト
/// </summary>
public class AnimationEnd : MonoBehaviour
{
    public static event Action OnAnimationComplete;
    void OnAnimationEnd()
    {
        Debug.Log("アニメーションが終了");
        // アニメーションオブジェクトを廃棄
        Destroy(this.gameObject);
        // ターン遷移許可
        GameObject manager = GameObject.FindGameObjectWithTag("GameManager");
        GameObject player = manager.GetComponent<PlayGame>().myplayer;
        PlayerState playerState = player.GetComponent<PlayerState>();
        playerState.canChangeTurn = true;

        if(player.GetComponent<PlayerState>().isLateAnim){
            manager.GetComponent<PlayGame>().ChangeTurnUI();
        }


        // リザルト遷移許可
        if (PlayGame.destroyProcess)
        {
            //Debug.Log("リザルト遷移処理開始");
            playerState.canDestroy = true;
        }
        //OnAnimationComplete?.Invoke();
    }
    IEnumerator WaitLoading(float time)
    {
        // 待つ
        yield return new WaitForSeconds(time);
    }
}
