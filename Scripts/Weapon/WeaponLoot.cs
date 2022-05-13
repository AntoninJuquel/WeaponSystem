using UnityEngine;

namespace WeaponSystem
{
    public class WeaponLoot : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;

        public Weapon Loot()
        {
            Destroy(gameObject);
            return weapon;
        }
    }
}