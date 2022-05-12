using System;
using UnityEngine;

namespace WeaponSystem
{
    [RequireComponent(typeof(WeaponInventory))]
    public class WeaponMuzzleFlash : MonoBehaviour
    {
        [SerializeField] private ParticleSystem muzzleFlashParticleSystem;
        private WeaponInventory _weaponInventory;

        private MuzzleFlash CurrentMuzzleFlash => _weaponInventory.CurrentWeapon.muzzleFlash;

        private void Awake()
        {
            _weaponInventory = GetComponent<WeaponInventory>();
            var subEmitters = GetComponent<ParticleSystem>().subEmitters;
            subEmitters.enabled = true;
            subEmitters.AddSubEmitter(muzzleFlashParticleSystem, ParticleSystemSubEmitterType.Birth, ParticleSystemSubEmitterProperties.InheritNothing);
        }

        private void Start()
        {
            SetupMuzzleFlash();
        }

        public void OnWeaponChanged()
        {
            SetupMuzzleFlash();
        }

        private void SetupMuzzleFlash()
        {
            var main = muzzleFlashParticleSystem.main;
            main.startLifetime = CurrentMuzzleFlash.lifetime;
            main.startColor = CurrentMuzzleFlash.color;
            main.startSpeed = CurrentMuzzleFlash.speed;
            main.startSize = CurrentMuzzleFlash.size;

            var shape = muzzleFlashParticleSystem.shape;
            shape.arc = CurrentMuzzleFlash.arc;
            shape.radius = CurrentMuzzleFlash.radius;
            shape.radiusThickness = CurrentMuzzleFlash.radiusThickness;
            shape.arcMode = CurrentMuzzleFlash.arcMode;

            var textureSheetAnimation = muzzleFlashParticleSystem.textureSheetAnimation;
            textureSheetAnimation.SetSprite(0, CurrentMuzzleFlash.sprite);

            var emission = muzzleFlashParticleSystem.emission;
            emission.SetBurst(0, new ParticleSystem.Burst(0, CurrentMuzzleFlash.count));
        }
    }
}