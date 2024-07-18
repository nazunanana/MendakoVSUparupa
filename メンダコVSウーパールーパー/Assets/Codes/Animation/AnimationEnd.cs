using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        player.GetComponent<PlayerState>().canChangeTurn = true;

        // リザルト遷移許可
        if (PlayGame.destroyProcess)
        {
            //Debug.Log("リザルト遷移処理開始");
            player.GetComponent<PlayerState>().canDestroy = true;
        }
        //OnAnimationComplete?.Invoke();
    }
    IEnumerator WaitLoading(float time)
    {
        // 待つ
        yield return new WaitForSeconds(time);
    }
}
