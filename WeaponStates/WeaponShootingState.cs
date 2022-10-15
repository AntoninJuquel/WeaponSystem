using System.Collections;
using UnityEngine;

namespace WeaponSystem
{
    public class WeaponShootingState : WeaponState
    {
        private Coroutine _shooting, _heatRecovery;
        private IHandleTrigger _trigger;
        private WaitForSeconds _burstRate, _roundPerMinute;
        private WaitWhile _triggerPulled;

        private bool OverHeat => Weapon.cooler.heatAmount >= Weapon.cooler.heatCapacity;
        private bool HasAmmo => Weapon.magazine.ammo > 0;

        public WeaponShootingState(WeaponStateController controller) : base(controller)
        {
            _trigger = controller.GetComponentInParent<IHandleTrigger>();
            _burstRate = new WaitForSeconds(60f / Weapon.FireMode.burstRate);
            _roundPerMinute = new WaitForSeconds(60f / Weapon.FireMode.roundPerMinute);
            _triggerPulled = new WaitWhile(() => _trigger.TriggerPulled);
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _shooting = Controller.StartCoroutine(Shooting());
        }

        private void OnAmmunitionHit()
        {
        }

        private void OnAmmunitionCollision()
        {
        }

        private void Shoot()
        {
            var barrelSlots = Weapon.barrelSlots;

            foreach (var barrelSlot in barrelSlots)
            {
                var minRadius = (1 - barrelSlot.barrel.thickness) * barrelSlot.radius;
                var maxRadius = barrelSlot.radius;

                var shootAngle = Quaternion.Euler(0, 0, barrelSlot.angle) * Controller.transform.right;
                for (var i = 0; i < Weapon.projectileCount; i++)
                {
                    var shootArc = 0f;
                    switch (barrelSlot.barrel.arcMode)
                    {
                        case ArcMode.Curve:
                            var time = barrelSlot.barrel.spreadCurve.keys[^1].time;
                            shootArc = (barrelSlot.barrel.spreadCurve.Evaluate(barrelSlot.barrel.time) - .5f) * barrelSlot.barrel.arc;
                            barrelSlot.barrel.time = (barrelSlot.barrel.time + time / barrelSlot.barrel.arc) % time;
                            break;
                        case ArcMode.Random:
                            shootArc = Random.Range(-barrelSlot.barrel.arc, barrelSlot.barrel.arc) * .5f;
                            break;
                        case ArcMode.Spread:
                            var div = Weapon.projectileCount % 2 == 0 || Weapon.projectileCount == 1 ? Weapon.projectileCount : Weapon.projectileCount - 1;
                            var step = barrelSlot.barrel.arc / div;
                            var start = -barrelSlot.barrel.arc * .5f + (Weapon.projectileCount % 2 == 0 || Weapon.projectileCount == 1 ? step * .5f : 0);
                            shootArc = i * step + start;
                            break;
                    }

                    var direction = Quaternion.Euler(0, 0, shootArc) * shootAngle;
                    var position = Controller.transform.position + Controller.transform.rotation * barrelSlot.position + direction.normalized * Random.Range(minRadius, maxRadius);

                    var projectileCore = GameObject.Instantiate(Controller.AmmunitionPrefab, position, Quaternion.identity);
                    projectileCore.Initialize(Weapon.ammunition, direction, OnAmmunitionHit, OnAmmunitionCollision);
                }
            }

            Weapon.magazine.ammo--;
            Weapon.cooler.heatAmount += Weapon.cooler.heatPerShot;
            if (_heatRecovery != null) Controller.StopCoroutine(_heatRecovery);
            _heatRecovery = Controller.StartCoroutine(HeatRecovery());
        }

        private IEnumerator Shooting()
        {
            if (_shooting != null)
            {
                yield return _shooting;
                yield break;
            }

            while (_trigger.TriggerPulled && Enabled)
            {
                var chargeTimer = 0f;

                while (chargeTimer < Weapon.FireMode.chargeTime)
                {
                    if (!Enabled)
                    {
                        _shooting = null;
                        yield break;
                    }

                    if (!_trigger.TriggerPulled)
                    {
                        Controller.SwitchState(Controller.Ready);
                        _shooting = null;
                        yield break;
                    }

                    chargeTimer += Time.deltaTime;
                    yield return null;
                }

                if (0 < Weapon.FireMode.burstCount && HasAmmo && !OverHeat)
                {
                    Shoot();
                }

                for (var i = 1; i < Weapon.FireMode.burstCount && HasAmmo && !OverHeat; i++)
                {
                    yield return _burstRate;
                    Shoot();
                }

                if (OverHeat)
                {
                    if (_heatRecovery != null) Controller.StopCoroutine(_heatRecovery);
                    Controller.SwitchState(Controller.OverHeat);
                    _shooting = null;
                    yield break;
                }

                if (!HasAmmo)
                {
                    Controller.SwitchState(Controller.Reloading);
                    _shooting = null;
                    yield break;
                }

                yield return _roundPerMinute;

                if (!Weapon.FireMode.automatic)
                    yield return _triggerPulled;
            }

            if (Enabled && !_trigger.TriggerPulled)
                Controller.SwitchState(Controller.Ready);

            _shooting = null;
        }

        private IEnumerator HeatRecovery()
        {
            yield return new WaitForSeconds(Weapon.cooler.heatRecoveryDelay);

            while (Weapon.cooler.heatAmount > 0)
            {
                Weapon.cooler.heatAmount = Mathf.Max(0, Weapon.cooler.heatAmount - Weapon.cooler.heatRecoveryRate);
                yield return null;
            }
        }
    }
}