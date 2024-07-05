using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGame : MonoBehaviour
{
    private GameObject PL_uparupa;
    private GameObject PL_mendako;
    public GameObject playerPrehab;
    private GameObject manageGrid;
    public GameObject gridSystemPrehab;
    void Start()
    {
        // プレイヤーオブジェクトを作成
        PL_uparupa = Instantiate(playerPrehab, playerPrehab.transform.position, Quaternion.identity);
        PL_mendako = Instantiate(playerPrehab, playerPrehab.transform.position, Quaternion.identity);
        PL_uparupa.GetComponent<PlayerState>().getsetTeam = PlayerState.Team.uparupa;
        PL_mendako.GetComponent<PlayerState>().getsetTeam = PlayerState.Team.mendako;
        // 管理オブジェクト生成
        manageGrid = Instantiate(gridSystemPrehab, gridSystemPrehab.transform.position, Quaternion.identity);
        manageGrid.GetComponent<ManageGrid>().SetPlayers = new GameObject[]{PL_uparupa, PL_mendako};
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
