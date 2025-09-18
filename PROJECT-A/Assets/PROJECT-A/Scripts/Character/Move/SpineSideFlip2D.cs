using UnityEngine;
using Character;
using Spine.Unity;

[DisallowMultipleComponent]
public class SpineSideFlip2D : MonoBehaviour
{
    [SerializeField] SkeletonRenderer skeleton;
    //방향이 오른쪽 기본이면 true
    [SerializeField] bool facesRightByDefault = true;

    // 값 amount
    [SerializeField] float minXSpeedToFlip = 0.01f;
    // 스킬/평타 중에는 잠금
    [SerializeField] bool freezeWhileLocked = true;

    private ActionLock actionLock;
    private Transform self;
    private int facing = +1;

    bool CanFlip() => !(freezeWhileLocked && actionLock && actionLock.IsLocked);

    private void Awake()
    {
        self = transform;
        skeleton = GetComponent<SkeletonRenderer>();
        actionLock = GetComponent<ActionLock>();
    }

    public void FaceByVelocity(Vector2 vel)
    {
        if (!CanFlip()) 
            return;
        if (Mathf.Abs(vel.x) < minXSpeedToFlip)
            return;
        SetFacing(vel.x >= 0f ? +1 : -1);
    }

    public void FaceByPoint(Vector2 worldPoint)
    {
        if (!CanFlip())
            return;
        float dx = worldPoint.x - self.position.x;
        if (Mathf.Abs(dx) < 1e-6f)
            return;
        SetFacing(dx >= 0f ? +1 : -1);
    }


    public void SnapLeft() => SetFacing(-1);
    public void SnapRight() => SetFacing(+1);

    private void SetFacing(int dir)
    {
        if (skeleton == null || facing == dir)
            return;
        facing = dir;

        var skel = skeleton.Skeleton;
        float baseSign = facesRightByDefault ? 1f : -1f;
        float abs = Mathf.Abs(skel.ScaleX);
        if (abs < 1e-6f)
            abs = 1f;
        skel.ScaleX = (dir > 0 ? 1f : -1f) * baseSign * abs;
    }

}