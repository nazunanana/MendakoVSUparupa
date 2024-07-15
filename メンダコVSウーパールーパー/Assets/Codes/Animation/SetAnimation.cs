using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using UnityEngine.Timeline;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using Fusion;
public class SetAnimation : NetworkBehaviour
{
    [SerializeField] private RuntimeAnimatorController ac_Uparupa;
    [SerializeField] private RuntimeAnimatorController ac_Mendako;

    [SerializeField] private GameObject realUparupaPhb;
    [SerializeField] private GameObject fakeUparupaPhb;
    [SerializeField] private GameObject realMendakoPhb;
    [SerializeField] private GameObject fakeMendakoPhb;
    private GameObject piece; //動かす駒オブジェクト
    private PlayerState.Team team;

    public void StartPlay(bool real, bool Isuparupa)
    {
        Debug.Log("アニメーション！");
        // インスタンスを登場させる
        NetworkObject model;
        NetworkRunner runner = this.gameObject.GetComponent<PlayGame>().runner;
        if (real)
        {
            if (Isuparupa) model = runner.Spawn(realUparupaPhb, new Vector3(0, 0, 0), realUparupaPhb.transform.rotation);
            else model = runner.Spawn(realMendakoPhb, new Vector3(0, 0, 0), realMendakoPhb.transform.rotation);
        }
        else
        {
            if (Isuparupa) model = runner.Spawn(fakeUparupaPhb, new Vector3(0, 0, 0), fakeUparupaPhb.transform.rotation);
            else model = runner.Spawn(fakeMendakoPhb, new Vector3(0, 0, 0), fakeMendakoPhb.transform.rotation);

        }
        // Animatorコンポネントをアタッチ
        Animator animator = model.gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = Isuparupa ? ac_Uparupa : ac_Mendako;
        animator.Play(Isuparupa ? "A_GetUparupa" : "A_GetMendako");
    }

}
