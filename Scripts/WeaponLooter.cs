using UnityEngine;
using UnityEngine.Events;

namespace WeaponSystem
{
    public class WeaponLooter : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Weapon> onLoot;

        private void OnTriggerEnter2D(Collider2D col)
        {
            var isWeaponLoot = col.GetComponent<WeaponLoot>();
            if (isWeaponLoot)
            {
                onLoot?.Invoke(isWeaponLoot.Loot());
            }
        }
    }
}