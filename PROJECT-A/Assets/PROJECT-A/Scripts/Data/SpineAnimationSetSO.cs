using Spine.Unity;
using UnityEngine;

namespace A
{
    [CreateAssetMenu(menuName = "PROJECT.A/Spine Animation Set", fileName = "SpineAnimationSet")]
    public class SpineAnimationSetSO : ScriptableObject
    {
        [Header("기본상태")]
        public AnimationReferenceAsset Idle;
        public AnimationReferenceAsset Move;
        public AnimationReferenceAsset Attack;
        public AnimationReferenceAsset Hit;
        public AnimationReferenceAsset Dead;

        [Header("추가 상태(옵션")]
        public AnimationReferenceAsset Cast;
        public AnimationReferenceAsset Groggy;
    }
}
