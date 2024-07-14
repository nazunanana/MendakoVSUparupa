using UnityEngine;

public class SetAnimation : MonoBehaviour
{
    public Vector3 targetPosition; // 固定位置
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartMoveAnimation()
    {
        // 現在の位置を取得
        Vector3 currentPosition = transform.position;

        // アニメーション用のオフセットを計算
        Vector3 offset = targetPosition - currentPosition;

        // アニメーターのパラメーターにオフセットを設定
        animator.SetFloat("OffsetX", offset.x);
        animator.SetFloat("OffsetY", offset.y);
        animator.SetFloat("OffsetZ", offset.z);

        // アニメーションを再生
        animator.Play("MoveToFixedPosition");
    }
}
