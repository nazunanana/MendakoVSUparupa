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
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayGame>().canDestroy = true;
        }
        // OnAnimationComplete?.Invoke();
    }
}
