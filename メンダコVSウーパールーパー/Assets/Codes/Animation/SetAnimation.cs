using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using UnityEngine.Timeline;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
public class SetAnimation : MonoBehaviour
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
        GameObject model;
        if (real)
        {
            if (Isuparupa) model = Instantiate(realUparupaPhb, new Vector3(0, 0, 0), realUparupaPhb.transform.rotation);
            else model = Instantiate(realMendakoPhb, new Vector3(0, 0, 0), realMendakoPhb.transform.rotation);
        }
        else
        {
            if (Isuparupa) model = Instantiate(fakeUparupaPhb, new Vector3(0, 0, 0), fakeUparupaPhb.transform.rotation);
            else model = Instantiate(fakeMendakoPhb, new Vector3(0, 0, 0), fakeMendakoPhb.transform.rotation);
        }
    }

}
