using System.Collections.Generic;
using TopDownShooter.Extensions;
using UnityEngine;

namespace TopDownShooter.Units
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField]
        private UnitStats _stats;
        [SerializeField]
        private Transform _gunMuzzle;
        [SerializeField]
        private GameObject _bulletPrefab;

        private Animator _animator;
        private UnitInputComponent _input;
        private Queue<GameObject> _bullets = new Queue<GameObject>();

        private bool isDead => _stats.HpCurrent <= 0;

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

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("[TRIGGER] " + gameObject.name + " triggered with " + other.gameObject.name);
            if (other.gameObject.tag == "Bullet")
            {
                _stats.HpCurrent--;
                if (isDead)
                    OnDeath();
            }
        }

        #endregion

        private void OnMoveUpdate()
        {
            if (isDead) return;
            ref var movement = ref _input.MoveDirection;
            var moving = movement.x != 0 || movement.z != 0;
            _animator.SetBool("Moving", moving);
            if (moving)
            {
                var rotatedMovement = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.down) * movement;
                _animator.SetFloat("ForwardMove", rotatedMovement.z);
                _animator.SetFloat("SideMove", rotatedMovement.x);
                transform.position += movement *
                    _stats.MoveSpeed * Time.deltaTime;
            }
        }

        private void OnAttack(AttackType attackType)
        {
            if (isDead) return;
            if (!_animator.GetBool("Moving"))
            {
                var animationName = attackType.ToString();
                _animator.SetTrigger(animationName);
            }
            var bullet = Instantiate(_bulletPrefab, _gunMuzzle.position, _gunMuzzle.rotation);
            _bullets.Enqueue(bullet);
            if (_bullets.Count > 50)
                Destroy(_bullets.Dequeue());
        }

        private void OnDeath()
        {
            _animator.SetTrigger("Death");
        }
    }
}