using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Weapons/FireMode")]
    public class FireMode : ScriptableObject
    {
        public float warmTime;
        public float chargeTime;
        public int burstCount = 1;
        public float burstRate;
        public float roundPerMinute;
        public bool automatic;
    }
}