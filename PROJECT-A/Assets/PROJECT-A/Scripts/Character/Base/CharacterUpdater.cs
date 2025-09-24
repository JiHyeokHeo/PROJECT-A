using System.Linq;
using UnityEngine;

namespace Character
{
    public interface ITickable
    {
        void Tick(float dt);
    }

    public interface IPhysicsTickable
    {
        void FixedTick(float fdt);
    }

    [DefaultExecutionOrder(-40)]
    public class CharacterUpdater : MonoBehaviour
    {
        ITickable[] _ticks;
        IPhysicsTickable[] _pticks;
        MonoBehaviour[] _tickBehaviours;
        
        private void Awake()
        {
            _tickBehaviours = GetComponentsInChildren<MonoBehaviour>(true);
            _ticks = _tickBehaviours.OfType<ITickable>().ToArray();
            _pticks = _tickBehaviours.OfType<IPhysicsTickable>().ToArray();
        }

        // Update is called once per frame
        private void Update()
        {
            float dt = Time.deltaTime;
            for (int i = 0; i < _ticks.Length; i++)
            {
                var mb = (MonoBehaviour)_ticks[i];
                if (!mb.isActiveAndEnabled)
                    continue;
                _ticks[i].Tick(dt);
            }
        }

        private void FixedUpdate()
        {
            float fdt = Time.fixedDeltaTime;
            for (int i = 0; i < _pticks.Length; i++)
            {
                var mb = (MonoBehaviour)_pticks[i];
                if (!mb.isActiveAndEnabled) 
                    continue;
                _pticks[i].FixedTick(fdt);
            }
        }
    }
}