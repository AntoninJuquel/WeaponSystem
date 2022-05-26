using UnityEngine;

namespace WeaponSystem
{
    public abstract class WeaponState
    {
        protected WeaponStateController StateController;
        protected Weapon Weapon => StateController.Weapon;

        public WeaponState(WeaponStateController stateController)
        {
            StateController = stateController;
        }

        public abstract void OnStateEnter();
        public abstract void OnStateUpdate();
        public abstract void OnStateExit();
    }
}