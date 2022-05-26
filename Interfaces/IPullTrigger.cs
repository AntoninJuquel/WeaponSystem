using System;

namespace WeaponSystem
{
    public interface IPullTrigger
    {
        public event EventHandler OnPullTrigger;
        public event EventHandler OnReleaseTrigger;
    }
}