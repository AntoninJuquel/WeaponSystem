using System.Collections;
using UnityEngine;

namespace WeaponSystem
{
    public class WeaponStateController : MonoBehaviour
    {
        [field: SerializeField] public AmmunitionCore AmmunitionPrefab { get; private set; }
        [field: SerializeField] public Weapon Weapon { get; private set; }
        public WeaponFloorState Floor { get; private set; }
        public WeaponReadyState Ready { get; private set; }
        public WeaponWarmingState Warming { get; private set; }
        public WeaponShootingState Shooting { get; private set; }
        public WeaponReloadingState Reloading { get; private set; }
        public WeaponOverHeatState OverHeat { get; private set; }
        public WeaponStoredState Stored { get; private set; }
        public WeaponState CurrentState { get; private set; }

        public string State;

        private void Awake()
        {
            Weapon = Weapon.Clone();

            Floor = new WeaponFloorState(this);
            Ready = new WeaponReadyState(this);
            Warming = new WeaponWarmingState(this);
            Shooting = new WeaponShootingState(this);
            Reloading = new WeaponReloadingState(this);
            OverHeat = new WeaponOverHeatState(this);
            Stored = new WeaponStoredState(this);
        }

        public void SwitchState(WeaponState state)
        {
            CurrentState?.OnStateExit();
            CurrentState = state;
            State = state.ToString();
            CurrentState.OnStateEnter();
        }

        public void Pick()
        {
        }

        public void Drop()
        {
            transform.SetParent(null);
            SwitchState(Floor);
        }

        public void Hand()
        {
            SwitchState(Ready);
        }

        public void Store()
        {
            SwitchState(Stored);
        }
    }
}