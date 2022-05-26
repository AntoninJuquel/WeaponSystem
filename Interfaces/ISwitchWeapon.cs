using System;

namespace WeaponSystem
{
    public interface ISwitchWeapon
    {
        public event EventHandler<int> OnSwitchWeapon;
    }
}