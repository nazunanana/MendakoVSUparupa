using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 駒の状態
/// </summary>
public class PieceState : MonoBehaviour
{
    // 駒が本物ならtrue,偽物ならfalse
    private bool isReal;
    // 駒
    private enum Pieces{
        uparupa, // ウパルパ駒
        mendako, // メンダコ駒
    }
    private Pieces piece;

    // どのマスにいるか

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool getIsReal(){
        return isReal;
    }

}
