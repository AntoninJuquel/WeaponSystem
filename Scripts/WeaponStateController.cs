using UnityEngine;

namespace WeaponSystem
{
    public class WeaponStateController : MonoBehaviour
    {
        [field: SerializeField] public WeaponInventory WeaponInventory { get; private set; }
        public Weapon Weapon { get; private set; }

        private WeaponState _currentState;

        public WeaponIdleState IdleState { get; private set; }
        public WeaponShootingState ShootingState { get; private set; }
        public WeaponReloadingState ReloadingState { get; private set; }

        private void Awake()
        {
            IdleState = new WeaponIdleState(this);
            ShootingState = new WeaponShootingState(this);
            ReloadingState = new WeaponReloadingState(this);
        }

        private void OnEnable()
        {
            WeaponInventory.onWeaponChanged.AddListener(OnWeaponChanged);
        }

        private void OnDisable()
        {
            WeaponInventory.onWeaponChanged.RemoveListener(OnWeaponChanged);
        }

        private void Start()
        {
            _currentState = IdleState;
            _currentState.OnStateEnter();
        }

        private void OnWeaponChanged(Weapon previousWeapon, Weapon currentWeapon, Weapon nextWeapon)
        {
            Weapon = currentWeapon;
        }

        public void SwitchState(WeaponState state)
        {
            _currentState.OnStateExit();
            _currentState = state;
            _currentState.OnStateEnter();
        }
    }
}