using System;

namespace WeaponSystem
{
    public interface IWeaponUser
    {
        bool PullTrigger { get; }
        public event EventHandler<int> onSwitchWeapon;
    }
}