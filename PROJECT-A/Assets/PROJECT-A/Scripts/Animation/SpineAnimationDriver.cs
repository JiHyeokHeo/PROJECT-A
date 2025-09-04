using Spine;
using Spine.Unity;

using UnityEngine;

namespace A
{
    public class SpineAnimationDriver : MonoBehaviour
    {
        public SpineAnimationSetSO animSetSO;
        SkeletonAnimation skeleton;
        Spine.AnimationState state;

        private void Awake()
        {
            skeleton = GetComponent<SkeletonAnimation>();
            state = skeleton.AnimationState;
            state.Event += OnSpineEvent;
        }

        public void PlayLoop(AnimationReferenceAsset clip, float mix = 0.1f)
        {
            if (clip == null)
                return;

            state.SetAnimation(0, clip, true).MixDuration = mix;
        }

        public TrackEntry PlayOnce(AnimationReferenceAsset clip, float mix = 0.1f)
        {
            if (clip == null)
                return null;

            var trackentry = state.SetAnimation(0, clip, false);
            trackentry.MixDuration = mix;

            return trackentry;
        }

        public TrackEntry QueueOnce(AnimationReferenceAsset clip, float delay = 0f)
        {
            if (clip == null) 
                return null;

            var te = state.AddAnimation(0, clip, false, delay);
            return te;
        }

        public TrackEntry QueueOnceLoop(AnimationReferenceAsset clip, float delay = 0f)
        {
            if (clip == null)
                return null;

            var te = state.AddAnimation(0, clip, true, delay);
            return te;
        }

        public void Clear(float mixOut)
        {
            state.SetEmptyAnimation(0, mixOut);
        }

        public void ClearAll()
        {
            state.SetEmptyAnimations(0);
        }


        public void PlayIdle() => PlayLoop(animSetSO?.Idle);
        public void PlayMove() => PlayLoop(animSetSO?.Move);
        public TrackEntry PlayAttack() => PlayOnce(animSetSO?.Attack);
        public TrackEntry PlayHit() => PlayOnce(animSetSO?.Hit, 0.05f);
        public TrackEntry PlayDead() => PlayOnce(animSetSO?.Dead, 0.05f);
        public TrackEntry PlayCast() => PlayOnce(animSetSO?.Cast);
        public void PlayGroggy() => PlayLoop(animSetSO?.Groggy);

        #region SpineAnimationEvent
        public System.Action<string, float> OnAnimEvent; // (eventName, float/int as float)
        void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
        {
            OnAnimEvent?.Invoke(e.Data.Name, e.Float);
        }
        #endregion

    }
}
