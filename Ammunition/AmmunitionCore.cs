using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponSystem
{
    public class AmmunitionCore : MonoBehaviour
    {
        private AmmunitionDisplacement _displacement;
        private AmmunitionSprite _sprite;

        private float _timeToDie;

        private Ammunition _ammunition;
        private Action _onHit, _onCollision;

        private Transform _transform;

        private void Awake()
        {
            _displacement = GetComponent<AmmunitionDisplacement>();
            _sprite = GetComponent<AmmunitionSprite>();
            _transform = transform;
        }

        public void Initialize(Ammunition ammunition, Vector3 direction, Action onHit, Action onCollision)
        {
            _transform.right = direction;
            _transform.localScale = Vector3.one * ammunition.size;

            _sprite.Initialize(ammunition);
            _displacement.Initialize(ammunition);
            _timeToDie = Time.time + ammunition.distance / ammunition.speed;

            _ammunition = ammunition;

            _onHit = onHit;
            _onCollision = onCollision;

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

            var takeHit = col.gameObject.GetComponent<ITakeAmmunitionHit>();
            if (takeHit != null)
            {
                takeHit.Hit(_ammunition.damage);
                _onHit?.Invoke();
            }
            else
            {
                _onCollision?.Invoke();
            }
        }
    }
}