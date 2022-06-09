using UnityEngine;

namespace TopDownShooter.Weapons
{
    public class Knife : MonoBehaviour, IWeapon
    {
        public string Name => "Knife";
        public float Damage => 2f;
    }
}