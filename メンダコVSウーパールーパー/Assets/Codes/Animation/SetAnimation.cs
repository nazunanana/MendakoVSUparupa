using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SetAnimation : MonoBehaviour
{
    // private Animator anim_Uparurpa;
    // private Animator anim_Mendako;
    // [SerializeField] private PlayableDirector director_Upa;
    // [SerializeField] private PlayableDirector director_Mendako;

    // private GameObject piece; //動かす駒オブジェクト
    // private PlayerState.Team team;
    // void Start()
    // {
    //     TimelineAsset tlAsset_uparupa = (TimelineAsset)director_Upa.playableAsset;
    //     TimelineAsset tlAsset_mendako = (TimelineAsset)director_Mendako.playableAsset;
    //     director_Upa.SetPieceBinding(timelineAsset.GetOutputTrack(cameraTrackIndexList[currentTrackIndex]), mainCamera);
    //     // Trackの状態をResetする
    //     director_Upa.Stop();
    //     director_Upa.Play();
    // }

    // public void SetPieceBinding()
    // {
    //     int index = 0;

    //     // 1 ～ (Listの数 - 1)の範囲
    //     int i = Random.Range(1, cameraTrackIndexList.Count);
    //     // 現在のindex + i がリストの数以上の場合、0に戻って余剰分を足す
    //     if (cameraTrackIndexList.Count <= currentTrackIndex + i)
    //     {
    //         index = (currentTrackIndex + i) - cameraTrackIndexList.Count;
    //     }
    //     else index = currentTrackIndex + i;

    //     TimelineAsset timelineAsset = director_Upa.playableAsset as TimelineAsset;

    //     // 現在カメラが設定されているTrackのBindingをリセット
    //     director_Upa.ClearGenericBinding(timelineAsset.GetOutputTrack(piece));

    //     // 新しいTrackのBindingに設定
    //     director_Upa.SetGenericBinding(timelineAsset.GetOutputTrack(newpiece), piece);
    //     // CinemachineTrackの状態をリセット
    //     director_Upa.Stop();
    //     director_Upa.Play();
    // }

}
