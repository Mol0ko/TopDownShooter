using System.Collections.Generic;
using TopDownShooter.Extensions;
using TopDownShooter.Weapons;
using UnityEngine;

namespace TopDownShooter.Units
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField]
        private UnitStatsPreset _statsPreset;
        [SerializeField, ReadOnly]
        protected UnitStats Stats;
        [SerializeField]
        private HpBar _hpBar;
        [SerializeField]
        private Transform _gunMuzzle;
        [SerializeField]
        private BulletComponent _bulletPrefab;

        protected IWeapon Weapon = new Rifle1();
        private Animator _animator;
        private UnitInputComponent _input;
        private Queue<GameObject> _bullets = new Queue<GameObject>();

        protected bool _isDead => Stats.HpCurrent <= 0;

        private float _hpFillAmount => Stats.HpCurrent / Stats.HpFull;

        #region Lifecycle

        private void Awake()
        {
            Stats = new UnitStats(_statsPreset);
        }

        private void OnValidate()
        {
            Stats = new UnitStats(_statsPreset);
        }

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

        protected void OnTriggerEnter(Collider other)
        {
            Debug.Log("[TRIGGER] " + gameObject.name + " triggered with " + other.gameObject.name);
            if (other.gameObject.tag == "Bullet" && !_isDead)
            {
                var bullet = other.gameObject.GetComponent<BulletComponent>();
                Stats.HpCurrent -= bullet.Damage;
                _hpBar.SetFillAmount(_hpFillAmount);
                if (_isDead)
                    OnDeath();
            }
        }

        #endregion

        private void OnMoveUpdate()
        {
            if (_isDead) return;
            ref var movement = ref _input.MoveDirection;
            var moving = movement.x != 0 || movement.z != 0;
            _animator.SetBool("Moving", moving);
            if (moving)
            {
                var rotatedMovement = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.down) * movement;
                _animator.SetFloat("ForwardMove", rotatedMovement.z);
                _animator.SetFloat("SideMove", rotatedMovement.x);
                transform.position += movement *
                    _statsPreset.MoveSpeed * Time.deltaTime;
            }
        }

        private void OnAttack(AttackType attackType)
        {
            if (_isDead) return;
            if (!_animator.GetBool("Moving"))
            {
                var animationName = attackType.ToString();
                _animator.SetTrigger(animationName);
            }
            var bullet = Instantiate(_bulletPrefab, _gunMuzzle.position, _gunMuzzle.rotation);
            bullet.Damage = Weapon.Damage;
            _bullets.Enqueue(bullet.gameObject);
            if (_bullets.Count > 50)
                Destroy(_bullets.Dequeue());
        }

        private void OnDeath()
        {
            Debug.Log($"{this.name} is dead");
            _input.StopAllCoroutines();
            _input.enabled = false;
            _animator.SetTrigger("Death");
        }
    }
}