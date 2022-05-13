using UnityEngine;
using UnityEngine.Events;

namespace WeaponSystem
{
    [RequireComponent(typeof(WeaponInventory))]
    public class WeaponLooter : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Weapon> onLoot;
        private WeaponInventory _weaponInventory;

        private void Awake()
        {
            _weaponInventory = GetComponent<WeaponInventory>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            var isWeapon = col.GetComponent<WeaponLoot>();
            if (isWeapon)
            {
                _weaponInventory.Add(isWeapon.Loot());
                onLoot?.Invoke(isWeapon.Loot());
            }
        }
    }
}