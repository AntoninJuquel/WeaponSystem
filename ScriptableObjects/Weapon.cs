using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Weapons/Weapon")]
    public class Weapon : ScriptableObject
    {
        public Ammunition ammunition;
        public float damage;
        public int projectileCount = 1;
        public BarrelSlot[] barrelSlots;
        public FireMode[] fireModes;
        public int fireModeIndex;
        public FireMode FireMode => fireModes[fireModeIndex];
        public Magazine magazine;
        public Cooler cooler;

        public Weapon Clone()
        {
            var clone = Instantiate(this);

            for (var i = 0; i < barrelSlots.Length; i++)
            {
                clone.barrelSlots[i].barrel = Instantiate(barrelSlots[i].barrel);
            }

            for (var i = 0; i < fireModes.Length; i++)
            {
                clone.fireModes[i] = Instantiate(fireModes[i]);
            }

            clone.magazine = Instantiate(magazine);
            clone.cooler = Instantiate(cooler);

            return clone;
        }
    }

    [System.Serializable]
    public struct BarrelSlot
    {
        public Barrel barrel;
        public Vector3 position;
        public float angle;
        public float radius;
    }
}