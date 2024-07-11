using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ReadyUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player.GetComponent<PlayerState>().getsetTeam);
        // if(player.GetComponent<PlayerState>().getsetTeam==PlayerState.Team.uparupa){
        //     this.gameObject.GetComponent<TextMeshProUGUI>().text = "あなたはウパルパ陣営です！";
        // }else{
        //     this.gameObject.GetComponent<TextMeshProUGUI>().text = "あなたはメンダコ陣営です！";
        // }
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

