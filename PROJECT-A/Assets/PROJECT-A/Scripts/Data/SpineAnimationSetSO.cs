using Spine.Unity;
using UnityEngine;

namespace A
{
    [CreateAssetMenu(menuName = "PROJECT.A/Spine Animation Set", fileName = "SpineAnimationSet")]
    public class SpineAnimationSetSO : ScriptableObject
    {
        [Header("�⺻����")]
        public AnimationReferenceAsset Idle;
        public AnimationReferenceAsset Move;
        public AnimationReferenceAsset Attack;
        public AnimationReferenceAsset Hit;
        public AnimationReferenceAsset Dead;

        [Header("�߰� ����(�ɼ�")]
        public AnimationReferenceAsset Cast;
        public AnimationReferenceAsset Groggy;
    }
}
