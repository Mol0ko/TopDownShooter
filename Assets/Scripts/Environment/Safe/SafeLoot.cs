using UnityEngine;

namespace TopDownShooter.Environment
{
    [CreateAssetMenu(fileName = "SafeLoot", menuName = "ScriptableObjects/SafeLoot", order = 1)]
    public class SafeLoot : ScriptableObject
    {
        public int Coins;
        public int HpRegen;
        public string[] WeaponNames;
    }
}