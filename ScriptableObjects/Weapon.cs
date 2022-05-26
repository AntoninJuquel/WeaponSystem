using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Weapons/Weapon")]
    public class Weapon : ScriptableObject
    {
        [field: SerializeField] public State State { get; set; }

        [Header("FireMode")] public FireMode[] fireModes;
        public float roundPerMinute;

        public float chargeTime;
        public float warmTime;

        [Header("Magazine")] public Magazine mag;
        public Ammunition ammunition;
        public int magazine;
        public int capacity;
        public float reloadTime;

        public Heat heatSystem;

        [FormerlySerializedAs("_fireModeIndex")]
        public int fireModeIndex;

        [FormerlySerializedAs("_triggerPulled")]
        public bool triggerPulled;

        [FormerlySerializedAs("_lastTimeHeld")]
        public float lastTimeHeld;

        public bool CanShoot => magazine > 0 && State == State.Ready && triggerPulled;
        public bool CanReload => magazine < capacity && State == State.Ready;
        public bool IsShooting => triggerPulled && State == State.Shooting;
        public FireMode CurrentFireMode => fireModes[fireModeIndex];
        public bool Automatic => CurrentFireMode.automatic;
        public int Burst => CurrentFireMode.roundBurst;
        public float BurstRate => roundPerMinute * CurrentFireMode.coefficient;

        public event EventHandler onShoot;

        public IEnumerator RoundPerMinute()
        {
            yield return new WaitForSeconds(60f / roundPerMinute);
        }

        public IEnumerator Shooting()
        {
            if (!CanShoot) yield break;

            if (0 < Burst && 0 < magazine)
            {
                State = State.Shooting;
                magazine--;
                onShoot?.Invoke(this, default);
            }

            for (var i = 1; i < Burst && 0 < magazine; i++)
            {
                yield return new WaitForSeconds(60f / BurstRate);
                State = State.Shooting;
                magazine--;
                onShoot?.Invoke(this, default);
            }
        }

        public IEnumerator WaitingTriggerUp()
        {
            if (!Automatic)
            {
                State = State.WaitingTriggerUp;
                yield return new WaitUntil(() => !triggerPulled);
            }

            if (State != State.Cooling)
                State = State.Ready;
        }

        public IEnumerator Warming()
        {
            if (!CanShoot) yield break;
            State = State.Warming;
            var timer = 0f;
            while (timer < warmTime && triggerPulled)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            State = State.Ready;
        }

        public IEnumerator Charging()
        {
            if (!CanShoot) yield break;
            State = State.Charging;
            var timer = 0f;
            while (timer < chargeTime && triggerPulled)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            State = State.Ready;
        }

        public void OverHeat()
        {
            State = State.Cooling;
        }

        public IEnumerator Reloading()
        {
            if (!CanReload) yield break;
            State = State.Reloading;
            yield return new WaitForSeconds(reloadTime);
            magazine = capacity;
            State = State.Ready;
        }

        public IEnumerator PullTrigger()
        {
            triggerPulled = true;

            yield return Warming();

            while (CanShoot)
            {
                yield return Charging();

                yield return Shooting();

                yield return RoundPerMinute();

                yield return WaitingTriggerUp();

                if (magazine == 0) yield return Reloading();
            }
        }

        public void ReleaseTrigger()
        {
            triggerPulled = false;
        }

        public void SwitchOff()
        {
            lastTimeHeld = Time.time;
        }

        public IEnumerator SwitchOn()
        {
            switch (State)
            {
                case State.Ready:
                case State.Charging:
                case State.Warming:
                case State.Cooling:
                default:
                    break;
                case State.Shooting:
                case State.Rpm:
                    yield return new WaitForSeconds(60f / roundPerMinute);
                    break;
                case State.WaitingTriggerUp:
                    yield return WaitingTriggerUp();
                    break;
                case State.Reloading:
                    State = State.Ready;
                    yield return Reloading();
                    break;
            }

            State = State.Ready;
        }

        public Weapon Clone(GameObject owner)
        {
            var weapon = Instantiate(this);
            weapon.name = name;

            if (heatSystem)
                weapon.heatSystem = heatSystem.Clone(weapon, owner);
            if (mag)
                weapon.mag = mag.Clone(weapon);

            for (var i = 0; i < fireModes.Length; i++)
            {
                weapon.fireModes[i] = fireModes[i].Clone(weapon);
            }


            return weapon;
        }
    }

    public enum State
    {
        Ready,
        Warming,
        Charging,
        Shooting,
        Rpm,
        Cooling,
        WaitingTriggerUp,
        Reloading
    }
}