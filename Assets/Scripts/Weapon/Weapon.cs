using System;

namespace TopDownShooter.Weapons
{
    public interface IWeapon {
        string Name { get; }
        float Damage { get; }
    }
}