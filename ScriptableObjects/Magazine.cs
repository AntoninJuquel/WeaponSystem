using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Weapons/Magazine")]
    public class Magazine : ScriptableObject
    {
        public int ammo;
        public int size;
        public float reloadTime;
    }
}