using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace WeaponSystem
{
    [RequireComponent(typeof(WeaponInventory))]
    [RequireComponent(typeof(WeaponBarrel))]
    public class WeaponCore : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Weapon> onShoot;
        private IWeaponUser _weaponUser;
        private WeaponInventory _weaponInventory;
        private WeaponBarrel _weaponBarrel;

        private Weapon _weapon;
        private bool HasAmmoLeft => _weapon.magazine > 0;
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

            _weaponInventory = GetComponent<WeaponInventory>();
            _weaponBarrel = GetComponent<WeaponBarrel>();
        }

        private void OnEnable()
        {
            _weaponInventory.onWeaponChanged.AddListener(OnWeaponChanged);
        }

        private void OnDisable()
        {
            _weaponInventory.onWeaponChanged.RemoveListener(OnWeaponChanged);
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

                    _weapon.state = State.Ready;
                }

                yield return null;
            }
        }

        private IEnumerator StateHandler()
        {
            var decreaseAmount = Mathf.Max(0, _weapon.TimeElapsed - _weapon.heatRecoveryDelay) * _weapon.heatRecoveryRate;
            _weapon.heat = Mathf.Max(0, _weapon.heat - (int) decreaseAmount);
            switch (_weapon.state)
            {
                case State.Ready:
                case State.Charging:
                default:
                    break;
                case State.Shooting:
                case State.Cooling:
                    yield return new WaitForSeconds((60f / _weapon.fireRate) - _weapon.TimeElapsed);
                    break;
                case State.WaitingTriggerUp:
                    yield return new WaitWhile(() => TriggerPulled);
                    break;
                case State.Reloading:
                    yield return new WaitForSeconds(_weapon.reloadTime - _weapon.TimeElapsed);
                    break;
            }

            _weapon.state = State.Ready;
        }

        private IEnumerator WaitUntilHeatStabilize()
        {
            while (enabled)
            {
                var previousHeat = _weapon.heat;
                yield return new WaitForSeconds(_weapon.heatRecoveryDelay);
                if (previousHeat == _weapon.heat) yield break;
            }
        }

        private IEnumerator HeatRecovery()
        {
            yield return new WaitUntil(() => _weapon.heatCapacity != 0);
            while (enabled)
            {
                yield return WaitUntilHeatStabilize();

                while (enabled)
                {
                    var previousHeat = _weapon.heat;
                    yield return new WaitForSeconds(1f / _weapon.heatRecoveryRate);
                    if (_weapon.heat > previousHeat || _weapon.heat == 0 || _weapon.state == State.Cooling) break;
                    _weapon.heat--;
                }
            }
        }

        private IEnumerator Charging()
        {
            _weapon.state = State.Charging;
            yield return new WaitForSeconds(_weapon.chargeTime);
        }

        private IEnumerator Shoot()
        {
            onShoot?.Invoke(_weapon);
            _weaponBarrel.Shoot(_weapon);
            _weapon.magazine--;
            _weapon.heat += _weapon.heatPerShot;

            if (_weapon.heatCapacity > 0 && _weapon.heat >= _weapon.heatCapacity)
            {
                yield return RecoverHeat();
            }
        }

        private IEnumerator Shooting()
        {
            _weapon.state = State.Shooting;

            if (0 < _weapon.CurrentFireMode.roundBurst && HasAmmoLeft)
            {
                yield return Shoot();
            }

            for (var i = 1; i < _weapon.CurrentFireMode.roundBurst && HasAmmoLeft; i++)
            {
                yield return new WaitForSeconds(60f / _weapon.burstRate);
                yield return Shoot();
            }
        }

        private IEnumerator Reloading()
        {
            _weapon.state = State.Reloading;
            yield return new WaitForSeconds(_weapon.reloadTime);
            _weapon.magazine = _weapon.capacity;
        }

        private IEnumerator RateOfFire()
        {
            _weapon.state = State.Cooling;
            yield return new WaitForSeconds(60f / _weapon.fireRate);
        }

        private IEnumerator TriggerReady()
        {
            if (_weapon.CurrentFireMode.automatic || !TriggerPulled) yield break;
            _weapon.state = State.WaitingTriggerUp;
            yield return new WaitWhile(() => TriggerPulled);
        }

        private IEnumerator RecoverHeat()
        {
            _weapon.state = State.Cooling;
            yield return new WaitForSeconds(_weapon.heatCapacity / _weapon.heatRecoveryRate);
            _weapon.heat = 0;
        }

        private void OnWeaponChanged(Weapon weapon)
        {
            StopAllCoroutines();
            _weapon = weapon;
            StartCoroutine(Start());
        }

        // private void Shot(Barrel barrel)
        // {
        //     var main = _ps.main;
        //     main.startLifetimeMultiplier = _weapon.ammunition.distance / _weapon.ammunition.speed;
        //     main.startSpeedMultiplier = _weapon.ammunition.speed;
        //     main.startSizeMultiplier = _weapon.ammunition.size;
        //     main.startColor = _weapon.ammunition.color;
        //     main.gravityModifierMultiplier = _weapon.ammunition.gravity;
        //
        //     var shape = _ps.shape;
        //     shape.arc = barrel.spread;
        //     shape.radiusThickness = barrel.thickness;
        //     shape.arcMode = barrel.arcMode;
        //     shape.position = barrel.position;
        //     shape.rotation = Vector3.forward * (90f + (barrel.angle - shape.arc * .5f));
        //     shape.radius = barrel.radius;
        //
        //     var textureSheetAnimation = _ps.textureSheetAnimation;
        //     textureSheetAnimation.SetSprite(0, _weapon.ammunition.sprite);
        //
        //     var colorOverLifetime = _ps.colorOverLifetime;
        //     colorOverLifetime.color = _weapon.ammunition.colorOverLifetime;
        //
        //     var limitVelocityOverLifeTime = _ps.limitVelocityOverLifetime;
        //     limitVelocityOverLifeTime.drag = _weapon.ammunition.drag;
        //
        //     var collision = _ps.collision;
        //     collision.bounceMultiplier = _weapon.ammunition.physicsMaterial.bounciness;
        //     collision.lifetimeLossMultiplier = _weapon.ammunition.lifetimeLoss;
        //     collision.dampenMultiplier = _weapon.ammunition.dampen;
        //
        //     _ps.Emit(barrel.emissionCount);
        // }
    }
}