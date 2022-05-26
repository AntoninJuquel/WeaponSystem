using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Weapons/FireMode")]
    public class FireMode : ScriptableObject
    {
        public int roundBurst;
        public float coefficient;
        public bool automatic;
        public float roundPerMinute;

        public FireMode Clone(Weapon weapon)
        {
            return Instantiate(this);
        } 
    }
}