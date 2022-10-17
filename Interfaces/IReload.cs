using System;

namespace WeaponSystem
{
    public interface IReload
    {
        public event Action OnReload;
    }
}