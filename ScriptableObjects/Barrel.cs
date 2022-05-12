using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "WeaponSystem/Barrel")]
    public class Barrel : ScriptableObject
    {
        public Vector2 position;
        public float angle;
        [Range(0f, 1f)] public float thickness;
        [Range(0f, 360f)] public float spread;
        public float radius;
        public ParticleSystemShapeMultiModeValue arcMode;
        public int emissionCount;
    }
}