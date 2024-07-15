using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using UnityEngine.Timeline;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class SetAnimation : MonoBehaviour
{
    private Animator anim_Uparurpa;
    private Animator anim_Mendako;
    [SerializeField] private PlayableDirector director_Upa;
    [SerializeField] private PlayableDirector director_Mendako;

    private GameObject piece; //動かす駒オブジェクト
    private PlayerState.Team team;

    public void StartPlay(GameObject target)
    {
        // Trackの状態をResetする
        director_Upa.Stop();
        director_Mendako.Stop();

        var track_uparupa = ((TimelineAsset)director_Upa.playableAsset).GetOutputTracks().First(c => c.name.Equals("GetPiece"));
        var clip_uparupa = (ControlPlayableAsset)track_uparupa.GetClips().First(c => c.displayName == "A_GetUparupa").asset;

        var track_mendako = ((TimelineAsset)director_Mendako.playableAsset).GetOutputTracks().First(c => c.name.Equals("GetPiece"));
        var clip_mendako = (ControlPlayableAsset)track_mendako.GetClips().First(c => c.displayName == "A_GetMendako").asset;

        var exposeName_uparupa = clip_uparupa.sourceGameObject.exposedName;
        var exposeName_mendako = clip_mendako.sourceGameObject.exposedName;

        bool upa = target.GetComponent<PieceState>().team == PlayerState.Team.uparupa;
        (upa ? director_Upa : director_Mendako).SetReferenceValue((upa ? exposeName_uparupa : exposeName_mendako), target);

        // director_Upa.SetPieceBinding(timelineAsset.GetOutputTrack(cameraTrackIndexList[currentTrackIndex]), mainCamera);

        director_Upa.Play();
    }

}
