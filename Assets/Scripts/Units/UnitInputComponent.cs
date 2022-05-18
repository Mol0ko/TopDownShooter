using System;
using UnityEngine;

namespace TopDownShooter.Units
{
    public class UnitInputComponent : MonoBehaviour
    {
        protected  Vector3 Movement;
        public Action<AttackType> OnAttackHandler;

        public ref Vector3 MoveDirection => ref Movement;

        public void CallOnAttackEvent(AttackType attackType) => 
            OnAttackHandler?.Invoke(attackType);
    }
}