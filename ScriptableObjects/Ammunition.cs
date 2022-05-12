using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "WeaponSystem/Ammunition")]
    public class Ammunition : ScriptableObject
    {
        public float speed;
        public float distance;
        public float size;
        public float gravity;
        public float drag;
        public float bounce;
        public float lifetimeLoss;
        public float dampen;
        public Sprite sprite;
        public Color color;
        public Gradient colorOverLifetime;
    }
}