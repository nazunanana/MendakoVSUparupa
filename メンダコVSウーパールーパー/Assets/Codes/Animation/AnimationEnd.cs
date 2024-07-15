using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEnd : MonoBehaviour
{
    void OnAnimationEnd(){
        Destroy(this.gameObject);
    }
}
