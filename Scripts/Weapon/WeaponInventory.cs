using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponSystem
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private List<Weapon> weapons;
        public UnityEvent<Weapon> onWeaponChanged;
        private int _weaponIndex;
        private IWeaponUser _weaponUser;
        private Weapon CurrentWeapon => weapons[_weaponIndex];
        public bool HasWeapon => weapons.Count > 0;

        private void Awake()
        {
            _weaponUser = GetComponentInParent<IWeaponUser>();

            if (_weaponUser == null)
            {
                Debug.LogWarning("IWeaponUser interface is not implemented on gameObject disabling the component", gameObject);
                enabled = false;
                return;
            }

            for (var i = 0; i < weapons.Count; i++)
            {
                weapons[i] = Instantiate(weapons[i]);
            }
        }

        private void Start()
        {
            onWeaponChanged?.Invoke(CurrentWeapon);
        }

        private void OnEnable()
        {
            if (_weaponUser == null) return;
            _weaponUser.onSwitchWeapon += OnSwitchWeapon;
        }

        private void OnDisable()
        {
            if (_weaponUser == null) return;
            _weaponUser.onSwitchWeapon -= OnSwitchWeapon;
        }

        private void OnSwitchWeapon(object sender, int delta)
        {
            CurrentWeapon.lastTimeHeld = Time.time;
            _weaponIndex = delta < 0 ? (_weaponIndex + weapons.Count - 1) % weapons.Count : (_weaponIndex + 1) % weapons.Count;
            onWeaponChanged?.Invoke(CurrentWeapon);
        }

        public void Add(Weapon weapon)
        {
            weapons.Add(Instantiate(weapon));
        }
    }
}