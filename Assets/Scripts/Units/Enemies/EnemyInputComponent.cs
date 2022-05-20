using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TopDownShooter.Units.Player
{
    public class EnemyInputComponent : UnitInputComponent
    {
        [SerializeField]
        private Transform _target;

        #region Config

        private AttackType _attackType = AttackType.Shoot;
        private float _seekRadius = 10f;
        private float _attackDelaySeconds = 1f;
        private float _reaction = 10f;

        private bool _isTargetCaught
        {
            get
            {
                var result = false;
                if (_target != null)
                {
                    var distanceToPlayer = Vector3.Distance(
                        transform.position,
                        _target.transform.position
                    );
                    result = distanceToPlayer <= _seekRadius;
                }
                return result;
            }
        }

        #endregion

        #region Lifecycle

        private void Start()
        {
            StartCoroutine(ShootRoutine());
        }

        private void Update()
        {
            UpdateLookAtTarget();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var radius = 10f;
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, transform.up, radius);
        }
#endif

        #endregion

        private void UpdateLookAtTarget()
        {
            if (_isTargetCaught)
            {
                var rotation = Quaternion.LookRotation(_target.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _reaction);
            }
        }

        private IEnumerator ShootRoutine()
        {
            while (true)
            {
                if (_isTargetCaught)
                {
                    yield return new WaitForSeconds(_attackDelaySeconds);
                    CallOnAttackEvent(_attackType);
                }
                else
                    yield return null;
            }
        }
    }
}