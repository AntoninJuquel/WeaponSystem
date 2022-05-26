using System;

namespace WeaponSystem
{
    public interface ISwitchFireMode
    {
        public event EventHandler<int> OnSwitchFireMode;
    }
}