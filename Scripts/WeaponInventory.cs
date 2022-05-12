using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private List<Weapon> weapons;
        [SerializeField] private UnityEvent onWeaponChanged;
        private int _weaponIndex;
        private IWeaponUser _weaponUser;
        public Weapon CurrentWeapon => weapons[_weaponIndex];
        public bool HasWeapon => weapons.Count > 0;
        private SpriteRenderer _sr;

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

            _sr = GetComponent<SpriteRenderer>();
            _sr.sprite = CurrentWeapon.sprite;
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
            _sr.sprite = CurrentWeapon.sprite;
            onWeaponChanged?.Invoke();
        }

        public void Add(Weapon weapon)
        {
            weapons.Add(Instantiate(weapon));
        }
    }
}