using System;

namespace WeaponSystem
{
    public interface ISwitchWeapon
    {
        public event Action<int> OnSwitchWeapon;
    }
}