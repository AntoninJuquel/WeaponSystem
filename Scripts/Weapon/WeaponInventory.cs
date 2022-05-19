using System.Collections;
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
        private IHandleWeapon _handleWeapon;
        private Weapon CurrentWeapon => weapons[_weaponIndex];
        public bool HasWeapon => weapons.Count > 0;

        private void Awake()
        {
            _handleWeapon = GetComponentInParent<IHandleWeapon>();

            for (var i = 0; i < weapons.Count; i++)
            {
                weapons[i] = Instantiate(weapons[i]);
            }

            if (_handleWeapon != null) return;

            Debug.LogWarning("IWeaponUser interface is not implemented on parent disabling the component", gameObject);
            enabled = false;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => HasWeapon);
            onWeaponChanged?.Invoke(CurrentWeapon);
        }

        private void OnEnable()
        {
            if (_handleWeapon == null) return;
            _handleWeapon.OnSwitchWeapon += OnSwitchHandleWeapon;
        }

        private void OnDisable()
        {
            if (_handleWeapon == null) return;
            _handleWeapon.OnSwitchWeapon -= OnSwitchHandleWeapon;
        }

        private void OnSwitchHandleWeapon(object sender, int delta)
        {
            if (!HasWeapon) return;
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