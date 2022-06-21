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
        protected HpBar HpBar;
        [SerializeField]
        private Transform _gunMuzzle;
        [SerializeField]
        private BulletComponent _bulletPrefab;
        [SerializeField]
        private string _weaponName;

        protected IWeapon Weapon = new Rifle1();
        private Animator _animator;
        private UnitInputComponent _input;
        private Queue<GameObject> _bullets = new Queue<GameObject>();

        protected bool _isDead => Stats.HpCurrent <= 0;

        protected float HpFillAmount => Stats.HpCurrent / Stats.HpFull;

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
            switch (_weaponName)
            {
                case "Knife":
                    Weapon = new Knife();
                    break;
                case "Rifle1":
                    Weapon = new Rifle1();
                    break;
                case "Rifle2":
                    Weapon = new Rifle2();
                    break;
                case "Rifle3":
                    Weapon = new Rifle3();
                    break;
                default:
                    break;
            };
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
            if (!_isDead)
            {
                var damage = 0f;
                if (other.gameObject.tag == "Bullet")
                {
                    var bullet = other.gameObject.GetComponent<BulletComponent>();
                    damage = bullet.Damage;
                }
                else if (other.gameObject.tag == "Knife")
                {
                    var knife = other.gameObject.GetComponent<Knife>();
                    damage = knife.Damage;
                }
                if (damage > 0)
                {
                    Stats.HpCurrent -= damage;
                    HpBar.SetFillAmount(HpFillAmount);
                    if (_isDead)
                    {
                        OnDeath();
                        if (Stats.Side == Side.Enemy)
                            GameManager.Instance.OnKillEnemy();
                        else if (Stats.Side == Side.Friend)
                            GameManager.Instance.OnPlayerDead();

                    }
                }
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
            if (attackType == AttackType.Shoot)
            {
                var bullet = Instantiate(_bulletPrefab, _gunMuzzle.position, _gunMuzzle.rotation);
                bullet.Damage = Weapon.Damage;
                _bullets.Enqueue(bullet.gameObject);
                if (_bullets.Count > 50)
                    Destroy(_bullets.Dequeue());
            }
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