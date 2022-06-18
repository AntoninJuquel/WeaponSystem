using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WeaponSystem
{
    public class WeaponInventory : MonoBehaviour
    {
        private List<WeaponStateController> _weapons = new();
        private int _weaponIndex;

        private WeaponStateController CurrentWeaponController => _weapons[_weaponIndex];
        private ISwitchWeapon _switch;
        private IPickWeapon _pick;
        private IDropWeapon _drop;

        private void Awake()
        {
            _weapons = GetComponentsInChildren<WeaponStateController>().ToList();
            _switch = GetComponent<ISwitchWeapon>();
            _pick = GetComponent<IPickWeapon>();
            _drop = GetComponent<IDropWeapon>();
        }

        private void OnEnable()
        {
            if (_switch != null)
                _switch.OnSwitchWeapon += OnSwitchWeapon;
            if (_pick != null)
                _pick.OnPickWeapon += OnPickWeapon;
            if (_drop != null)
                _drop.OnDropWeapon += OnDropWeapon;
        }

        private void OnDisable()
        {
            if (_switch != null)
                _switch.OnSwitchWeapon -= OnSwitchWeapon;
            if (_pick != null)
                _pick.OnPickWeapon -= OnPickWeapon;
            if (_drop != null)
                _drop.OnDropWeapon -= OnDropWeapon;
        }

        private void Start()
        {
            foreach (var weapon in _weapons)
            {
                weapon.SwitchState(weapon.Stored);
            }

            CurrentWeaponController.SwitchState(CurrentWeaponController.Ready);
        }

        private void OnSwitchWeapon(int delta)
        {
            CurrentWeaponController.Store();
            _weaponIndex = delta < 0 ? (_weaponIndex + _weapons.Count - 1) % _weapons.Count : (_weaponIndex + 1) % _weapons.Count;
            CurrentWeaponController.Hand();
        }

        private void OnPickWeapon()
        {
        }

        private void OnDropWeapon()
        {
        }
    }
}