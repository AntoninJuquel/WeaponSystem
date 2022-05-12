using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponSystem
{
    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(WeaponInventory))]
    public class WeaponHitRegister : MonoBehaviour
    {
        [SerializeField] private UnityEvent onHit;
        private readonly List<ParticleCollisionEvent> _collisions = new();
        private ParticleSystem _ps;
        private WeaponInventory _weaponInventory;


        private void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
            _weaponInventory = GetComponent<WeaponInventory>();
        }

        private void OnParticleCollision(GameObject other)
        {
            var damageable = other.GetComponent<IDamageable>();
            if (damageable == null) return;
            var events = _ps.GetCollisionEvents(other, _collisions);
            for (var i = 0; i < events; i++)
            {
                Debug.Log("Take this mf");
                damageable.TakeDamage(_weaponInventory.CurrentWeapon.damage);
                onHit?.Invoke();
            }
        }
    }
}