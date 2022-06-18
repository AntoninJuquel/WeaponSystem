using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Weapons/Cooler")]
    public class Cooler : ScriptableObject
    {
        public float heatAmount;
        public float heatCapacity;
        public float heatPerShot;
        public float heatRecoveryRate;
        public float heatRecoveryDelay;
    }
}