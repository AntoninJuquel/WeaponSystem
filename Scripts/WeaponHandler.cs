using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponSystem
{
    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(WeaponInventory))]
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] private UnityEvent onShoot;
        private IWeaponUser _weaponUser;
        private WeaponInventory _weaponInventory;

        private ParticleSystem _ps;
        private Weapon CurrentWeapon => _weaponInventory.CurrentWeapon;
        private bool HasAmmoLeft => CurrentWeapon.magazine > 0;
        private bool TriggerPulled => _weaponUser.PullTrigger;

        private void Awake()
        {
            _weaponUser = GetComponentInParent<IWeaponUser>();

            if (_weaponUser == null)
            {
                Debug.LogWarning("IWeaponUser interface is not implemented on gameObject disabling the component", gameObject);
                enabled = false;
                return;
            }

            _ps = GetComponent<ParticleSystem>();
            _weaponInventory = GetComponent<WeaponInventory>();
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => _weaponInventory.HasWeapon);
            yield return StateHandler();
            StartCoroutine(HeatRecovery());

            while (enabled)
            {
                if (TriggerPulled)
                {
                    yield return Charging();

                    yield return Shooting();

                    if (!HasAmmoLeft)
                    {
                        yield return Reloading();
                    }
                    else
                    {
                        yield return RateOfFire();

                        yield return TriggerReady();
                    }

                    CurrentWeapon.state = State.Ready;
                }

                yield return null;
            }
        }

        private IEnumerator StateHandler()
        {
            var decreaseAmount = Mathf.Max(0, CurrentWeapon.TimeElapsed - CurrentWeapon.heatRecoveryDelay) * CurrentWeapon.heatRecoveryRate;
            CurrentWeapon.heat = Mathf.Max(0, CurrentWeapon.heat - (int) decreaseAmount);
            switch (CurrentWeapon.state)
            {
                case State.Ready:
                case State.Charging:
                default:
                    break;
                case State.Shooting:
                case State.Cooling:
                    yield return new WaitForSeconds((60f / CurrentWeapon.fireRate) - CurrentWeapon.TimeElapsed);
                    break;
                case State.WaitingTriggerUp:
                    yield return new WaitWhile(() => TriggerPulled);
                    break;
                case State.Reloading:
                    yield return new WaitForSeconds(CurrentWeapon.reloadTime - CurrentWeapon.TimeElapsed);
                    break;
            }

            CurrentWeapon.state = State.Ready;
        }

        private IEnumerator WaitUntilHeatStabilize()
        {
            while (enabled)
            {
                var previousHeat = CurrentWeapon.heat;
                yield return new WaitForSeconds(CurrentWeapon.heatRecoveryDelay);
                if (previousHeat == CurrentWeapon.heat) yield break;
            }
        }

        private IEnumerator HeatRecovery()
        {
            yield return new WaitUntil(() => CurrentWeapon.heatCapacity != 0);
            while (enabled)
            {
                yield return WaitUntilHeatStabilize();

                while (enabled)
                {
                    var previousHeat = CurrentWeapon.heat;
                    yield return new WaitForSeconds(1f / CurrentWeapon.heatRecoveryRate);
                    if (CurrentWeapon.heat > previousHeat || CurrentWeapon.heat == 0 || CurrentWeapon.state == State.Cooling) break;
                    CurrentWeapon.heat--;
                }
            }
        }

        private IEnumerator Charging()
        {
            CurrentWeapon.state = State.Charging;
            yield return new WaitForSeconds(CurrentWeapon.chargeTime);
        }

        private IEnumerator Shoot()
        {
            foreach (var barrel in CurrentWeapon.barrels)
            {
                Shot(barrel);
            }

            onShoot?.Invoke();
            CurrentWeapon.magazine--;
            CurrentWeapon.heat += CurrentWeapon.heatPerShot;

            if (CurrentWeapon.heatCapacity > 0 && CurrentWeapon.heat >= CurrentWeapon.heatCapacity)
            {
                yield return RecoverHeat();
            }
        }

        private IEnumerator Shooting()
        {
            CurrentWeapon.state = State.Shooting;

            if (0 < CurrentWeapon.CurrentFireMode.roundBurst && HasAmmoLeft)
            {
                yield return Shoot();
            }

            for (var i = 1; i < CurrentWeapon.CurrentFireMode.roundBurst && HasAmmoLeft; i++)
            {
                yield return new WaitForSeconds(60f / CurrentWeapon.burstRate);
                yield return Shoot();
            }
        }

        private IEnumerator Reloading()
        {
            CurrentWeapon.state = State.Reloading;
            yield return new WaitForSeconds(CurrentWeapon.reloadTime);
            CurrentWeapon.magazine = CurrentWeapon.capacity;
        }

        private IEnumerator RateOfFire()
        {
            CurrentWeapon.state = State.Cooling;
            yield return new WaitForSeconds(60f / CurrentWeapon.fireRate);
        }

        private IEnumerator TriggerReady()
        {
            if (CurrentWeapon.CurrentFireMode.automatic || !TriggerPulled) yield break;
            CurrentWeapon.state = State.WaitingTriggerUp;
            yield return new WaitWhile(() => TriggerPulled);
        }

        private IEnumerator RecoverHeat()
        {
            CurrentWeapon.state = State.Cooling;
            yield return new WaitForSeconds(CurrentWeapon.heatCapacity / CurrentWeapon.heatRecoveryRate);
            CurrentWeapon.heat = 0;
        }

        public void OnWeaponChanged()
        {
            StopAllCoroutines();
            StartCoroutine(Start());
        }

        private void Shot(Barrel barrel)
        {
            var main = _ps.main;
            main.startLifetimeMultiplier = CurrentWeapon.ammunition.distance / CurrentWeapon.ammunition.speed;
            main.startSpeedMultiplier = CurrentWeapon.ammunition.speed;
            main.startSizeMultiplier = CurrentWeapon.ammunition.size;
            main.startColor = CurrentWeapon.ammunition.color;
            main.gravityModifierMultiplier = CurrentWeapon.ammunition.gravity;

            var shape = _ps.shape;
            shape.arc = barrel.spread;
            shape.radiusThickness = barrel.thickness;
            shape.arcMode = barrel.arcMode;
            shape.position = barrel.position;
            shape.rotation = Vector3.forward * (90f + (barrel.angle - shape.arc * .5f));
            shape.radius = barrel.radius;

            var textureSheetAnimation = _ps.textureSheetAnimation;
            textureSheetAnimation.SetSprite(0, CurrentWeapon.ammunition.sprite);

            var colorOverLifetime = _ps.colorOverLifetime;
            colorOverLifetime.color = CurrentWeapon.ammunition.colorOverLifetime;

            var limitVelocityOverLifeTime = _ps.limitVelocityOverLifetime;
            limitVelocityOverLifeTime.drag = CurrentWeapon.ammunition.drag;

            var collision = _ps.collision;
            collision.bounceMultiplier = CurrentWeapon.ammunition.bounce;
            collision.lifetimeLossMultiplier = CurrentWeapon.ammunition.lifetimeLoss;
            collision.dampenMultiplier = CurrentWeapon.ammunition.dampen;

            _ps.Emit(barrel.emissionCount);
        }
    }
}