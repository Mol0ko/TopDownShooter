using TopDownShooter.Extensions;
using UnityEngine;

namespace TopDownShooter.Units
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField]
        private UnitStats _stats;
        private Animator _animator;
        private UnitInputComponent _input;

        #region Lifecycle

        protected virtual void Start()
        {
            _animator = this.FindComponent<Animator>();
            _input = this.FindComponent<UnitInputComponent>();

            if (_input != null)
            {
                _input.OnAttackHandler += OnAttack;
            }
        }

        protected virtual void Update()
        {
            OnMoveUpdate();
        }

        private void OnDestroy()
        {
            if (_input != null)
            {
                _input.OnAttackHandler -= OnAttack;
            }
        }

        #endregion

        private void OnMoveUpdate()
        {
            ref var movement = ref _input.MoveDirection;
            _animator.SetFloat("ForwardMove", movement.z);
            _animator.SetFloat("SideMove", movement.x);
            var moving = movement.x != 0 || movement.z != 0;
            _animator.SetBool("Moving", moving);
            if (moving)
                transform.position += transform.TransformVector(movement) *
                    _stats.MoveSpeed * Time.deltaTime;
        }

        private void OnAttack(AttackType attackType)
        {
            var animationName = attackType.ToString();
            _animator.SetTrigger(animationName);
        }
    }
}