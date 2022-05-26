using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Weapons/Magazine")]
    public class Magazine : ScriptableObject
    {
        [SerializeField] private Ammunition ammunition;
        [SerializeField] private int ammo;
        [SerializeField] private int capacity;
        [SerializeField] private float reloadTime;

        public Magazine Clone(Weapon weapon)
        {
            return Instantiate(this);
        }
    }
}