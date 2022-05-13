using System.Collections;
using UnityEngine;

namespace WeaponSystem
{
    public class ProjectileCore : MonoBehaviour
    {
        private ProjectileDisplacement _displacement;
        private ProjectileSprite _sprite;

        private float _timeToDie;

        private Ammunition _ammunition;

        private void Awake()
        {
            _displacement = GetComponent<ProjectileDisplacement>();
            _sprite = GetComponent<ProjectileSprite>();
        }

        public void Initialize(Ammunition ammunition)
        {
            transform.localScale = Vector3.one * ammunition.size;

            _sprite.Initialize(ammunition);
            _displacement.Initialize(ammunition);
            _timeToDie = Time.time + ammunition.distance / ammunition.speed;

            _ammunition = ammunition;

            StartCoroutine(LifeTime());
        }

        private IEnumerator LifeTime()
        {
            yield return new WaitUntil(() => Time.time >= _timeToDie);
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            _timeToDie *= (1 - _ammunition.lifetimeLoss);
        }
    }
}