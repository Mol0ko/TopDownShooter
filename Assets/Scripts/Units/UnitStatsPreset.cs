using System;
using UnityEngine;

namespace TopDownShooter.Units
{
    [CreateAssetMenu(fileName = "UnitStatsPreset", menuName = "ScriptableObjects/UnitStatsPreset", order = 1)]
    public class UnitStatsPreset : ScriptableObject
    {
        public float MoveSpeed;
        public Side Side;
        public float Hp;
    }

    [Serializable]
    public struct UnitStats
    {
        public float MoveSpeed;
        public Side Side;
        public float HpFull;
        public float HpCurrent;

        public UnitStats(UnitStatsPreset preset)
        {
            MoveSpeed = preset.MoveSpeed;
            Side = preset.Side;
            HpFull = preset.Hp;
            HpCurrent = preset.Hp;
        }
    }
}