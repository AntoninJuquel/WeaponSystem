﻿using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(menuName = "Weapons/Barrel")]
    public class Barrel : ScriptableObject
    {
        [Range(0f, 1f)] public float thickness;
        [Range(0f, 360f)] public float arc;
        public ArcMode arcMode;
        public AnimationCurve spreadCurve;
        public float time;
    }

    public enum ArcMode
    {
        Random,
        Spread,
        Curve
    }
}