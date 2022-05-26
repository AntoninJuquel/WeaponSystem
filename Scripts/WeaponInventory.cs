using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponSystem
{
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private GameObject weaponHolder;
        [SerializeField] private List<Weapon> weapons;
        public UnityEvent<Weapon, Weapon, Weapon> onWeaponChanged;

        private int _weaponIndex;
        private ISwitchWeapon _switch;

        private Weapon CurrentWeapon => weapons[_weaponIndex];
        private Weapon NextWeapon => weapons[(_weaponIndex + 1) % weapons.Count];
        private Weapon PreviousWeapon => weapons[(_weaponIndex + weapons.Count - 1) % weapons.Count];

        private bool HasWeapon => weapons.Count > 0;

        private void Awake()
        {
            _switch = GetComponent<ISwitchWeapon>();

            for (var i = 0; i < weapons.Count; i++)
            {
                weapons[i] = weapons[i].Clone(gameObject);
            }
        }

        private void Start()
        {
            onWeaponChanged?.Invoke(PreviousWeapon, CurrentWeapon, NextWeapon);
        }

        private void OnEnable()
        {
            if (_switch != null) _switch.OnSwitchWeapon += OnSwitchWeapon;
        }

        private void OnDisable()
        {
            if (_switch != null) _switch.OnSwitchWeapon -= OnSwitchWeapon;
        }

        private void OnSwitchWeapon(object sender, int delta)
        {
            if (!HasWeapon) return;
            CurrentWeapon.SwitchOff();
            _weaponIndex = delta < 0 ? (_weaponIndex + weapons.Count - 1) % weapons.Count : (_weaponIndex + 1) % weapons.Count;
            StopAllCoroutines();
            StartCoroutine(CurrentWeapon.SwitchOn());
            onWeaponChanged?.Invoke(PreviousWeapon, CurrentWeapon, NextWeapon);
        }
    }
}