using UnityEngine;

namespace TopDownShooter.Units
{
    [CreateAssetMenu(fileName = "UnitStats", menuName = "ScriptableObjects/UnitStats", order = 1)]
    public class UnitStats : ScriptableObject
    {
        public float MoveSpeed;
        public Side Side;
    }
}