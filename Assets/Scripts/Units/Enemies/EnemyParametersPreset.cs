using System;
using UnityEngine;

namespace TopDownShooter.Units
{
    [CreateAssetMenu(fileName = "EnemyParametersPreset", menuName = "ScriptableObjects/EnemyParametersPreset", order = 1)]
    public class EnemyParametersPreset : ScriptableObject
    {
        public AttackType AttackType = AttackType.Shoot;
        public float SeekRadius = 12f;
        public float AttackDelaySeconds = 0.85f;
        public float Reaction = 10f;
        public float IdleTimeSeconds = 5f;
    }

    [Serializable]
    public struct EnemyParameters
    {
        public AttackType AttackType;
        public float SeekRadius;
        public float AttackDelaySeconds;
        public float Reaction;
        public float IdleTimeSeconds;

        public EnemyParameters(EnemyParametersPreset preset)
        {
            AttackType = preset.AttackType;
            SeekRadius = preset.SeekRadius;
            AttackDelaySeconds = preset.AttackDelaySeconds;
            Reaction = preset.Reaction;
            IdleTimeSeconds = preset.IdleTimeSeconds;
        }
    }
}