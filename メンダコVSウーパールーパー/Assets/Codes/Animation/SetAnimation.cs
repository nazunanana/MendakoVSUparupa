using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using UnityEngine.Timeline;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class SetAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim_Uparurpa;
    [SerializeField] private Animator anim_Mendako;
    [SerializeField] private RuntimeAnimatorController ac_Uparupa;
    [SerializeField] private RuntimeAnimatorController ac_Mendako;

    [SerializeField] private GameObject realUparupaPhb;
    [SerializeField] private GameObject fakeUparupaPhb;
    [SerializeField] private GameObject realMendakoPhb;
    [SerializeField] private GameObject fakeMendakoPhb;
    private GameObject piece; //動かす駒オブジェクト
    private PlayerState.Team team;

    public void StartPlay(GameObject target, bool real, bool Isuparupa)
    {
        // インスタンスを登場させる
        GameObject model;

        if (real)
        {
            if (Isuparupa) model = realUparupaPhb;
            else model = realMendakoPhb;
        }
        else
        {
            if (Isuparupa) model = fakeUparupaPhb;
            else model = fakeMendakoPhb;
        }
        GameObject obj = Instantiate(model, new Vector3(0, 0, 0), Quaternion.identity);
        // Animatorコンポネントをアタッチ
        Animator animator = obj.AddComponent<Animator>();
        animator.runtimeAnimatorController = Isuparupa ? ac_Uparupa : ac_Mendako;
        animator.Play(Isuparupa ? "A_GetUparupa" : "A_GetMendako");
    }

}
