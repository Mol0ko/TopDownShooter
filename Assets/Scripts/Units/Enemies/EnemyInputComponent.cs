using System.Collections;
using UnityEditor;
using UnityEngine;

namespace TopDownShooter.Units.Player
{
    public class EnemyInputComponent : UnitInputComponent
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private Transform[] _routePoints = new Transform[0];

        #region Config

        private AttackType _attackType = AttackType.Shoot;
        private float _seekRadius = 10f;
        private float _attackDelaySeconds = 1f;
        private float _reaction = 10f;
        private float _idleTimeSeconds = 5f;

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
            StartCoroutine(RouteMovingRoutine());
            StartCoroutine(ShootRoutine());
        }

        private void Update()
        {
            UpdateLookAtTarget();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // seek radius
            var radius = 10f;
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, transform.up, radius);
            // rout points
            foreach (var point in _routePoints)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(point.position, 0.1f);
            }
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

        private IEnumerator RouteMovingRoutine()
        {
            var routePointIndex = 0;
            while (true)
            {
                var currentIndex = routePointIndex % _routePoints.Length;
                var destinationPoint = _routePoints[currentIndex];
                var distanceToPoint = Vector3.Distance(transform.position, destinationPoint.position);
                if (distanceToPoint < 0.5f)
                {
                    Movement = Vector3.zero;
                    routePointIndex++;
                    yield return new WaitForSeconds(_idleTimeSeconds);
                }
                else
                {
                    var rotation = Quaternion.LookRotation(destinationPoint.position - transform.position);
                    var shouldRotateToPoint = Vector3.Distance(transform.rotation.eulerAngles, rotation.eulerAngles) > 2;
                    if (shouldRotateToPoint)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _reaction);
                        yield return null;
                    }
                    else
                    {
                        Movement = Vector3.Normalize(destinationPoint.position - transform.position);
                        yield return null;
                    }
                }
            }
        }
    }
}