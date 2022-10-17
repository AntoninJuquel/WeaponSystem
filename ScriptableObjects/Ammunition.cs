using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Weapons/Ammunition")]
    public class Ammunition : ScriptableObject
    {
        public float damage;
        public float speed;
        public float distance;
        public float size;
        public float gravity;
        public float drag;
        [Range(0f, 1f)] public float lifetimeLoss;
        [Range(0f, 1f)] public float dampen;
        public Sprite sprite;
        public Color color;
        public Gradient colorOverLifetime;
        public PhysicsMaterial2D physicsMaterial;
    }
}