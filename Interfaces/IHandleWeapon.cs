using System;

namespace WeaponSystem
{
    public interface IHandleWeapon
    {
        bool PullTrigger { get; set; }
        public event EventHandler<int> OnSwitchWeapon;
    }
}