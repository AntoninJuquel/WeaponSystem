using UnityEngine;
using TMPro;

namespace WeaponSystem
{
    public class WeaponLoot : MonoBehaviour
    {
        [SerializeField] private Weapon[] weapons;
        private Weapon _weapon;

        private void Start()
        {
            _weapon = weapons[Random.Range(0, weapons.Length)];
            GetComponent<TextMeshPro>().text = _weapon.name;
        }

        public Weapon Loot()
        {
            Destroy(gameObject);
            return _weapon;
        }
    }
}