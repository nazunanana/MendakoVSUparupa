using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEnd : MonoBehaviour
{
    public static event Action OnAnimationComplete;
    void OnAnimationEnd()
    {
        Destroy(this.gameObject);
        if (PlayGame.destroyProcess)
        {
            //PlayGame.canDestroy = true;
            //Debug.Log("終了処理開始&アニメーションが終了");
            GameObject manager = GameObject.FindGameObjectWithTag("GameManager");
            GameObject player = manager.GetComponent<PlayGame>().myplayer;
            //Debug.Log("コンポネントnull確認"+player.GetComponent<PlayerState>());
            //Debug.Log(player.GetComponent<PlayerState>().canDestroy+"はfalse?");
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
