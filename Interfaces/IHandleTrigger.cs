using System;

namespace WeaponSystem
{
    public interface IHandleTrigger
    {
        public event Action OnPullTrigger, OnReleaseTrigger;
        public bool TriggerPulled { get; set; }
    }
}