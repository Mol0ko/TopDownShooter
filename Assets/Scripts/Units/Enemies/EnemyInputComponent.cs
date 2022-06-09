using System.Collections;
using TopDownShooter.Extensions;
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
        [SerializeField]
        private EnemyParametersPreset _parametersPreset;
        [SerializeField, ReadOnly]
        private EnemyParameters _parameters;

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
                    result = distanceToPlayer <= _parameters.SeekRadius;
                }
                return result;
            }
        }

        #region Lifecycle

        private void Awake()
        {
            _parameters = new EnemyParameters(_parametersPreset);
        }

        private void OnValidate()
        {
            _parameters = new EnemyParameters(_parametersPreset);
        }

        private void Start()
        {
            StartCoroutine(RouteMovingRoutine());
            StartCoroutine(AttackRoutine());
        }

        private void Update()
        {
            UpdateLookAtTarget();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // seek radius
            var radius = 12f;
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
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _parameters.Reaction);
            }
        }

        private IEnumerator AttackRoutine()
        {
            while (true)
            {
                var distanceToTarget = Vector3.Distance(transform.position, _target.position);
                if (_isTargetCaught &&
                    (_parameters.AttackType == AttackType.Shoot ||
                        (_parameters.AttackType == AttackType.Hit && distanceToTarget < 1.2f)))
                {
                    if (_parameters.AttackType == AttackType.Shoot)
                        yield return new WaitForSeconds(_parameters.AttackDelaySeconds);
                    CallOnAttackEvent(_parameters.AttackType);
                    if (_parameters.AttackType == AttackType.Hit)
                        yield return new WaitForSeconds(_parameters.AttackDelaySeconds);
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
                if (_parameters.AttackType == AttackType.Shoot && _isTargetCaught)
                {
                    Movement = Vector3.zero;
                    yield return null;
                }
                else
                {
                    var currentIndex = routePointIndex % _routePoints.Length;
                    Transform destinationPoint;
                    if (_parameters.AttackType == AttackType.Hit && _isTargetCaught)
                        destinationPoint = _target;
                    else
                        destinationPoint = _routePoints[currentIndex];

                    var distanceToPoint = Vector3.Distance(transform.position, destinationPoint.position);

                    if (_parameters.AttackType == AttackType.Hit && _isTargetCaught && distanceToPoint < 1f)
                    {
                        Movement = Vector3.zero;
                        yield return null;
                    }
                    else if (distanceToPoint < 0.5f)
                    {
                        Movement = Vector3.zero;
                        routePointIndex++;
                        yield return new WaitForSeconds(_parameters.IdleTimeSeconds);
                    }
                    else
                    {
                        var rotation = Quaternion.LookRotation(destinationPoint.position - transform.position);
                        var shouldRotateToPoint = Vector3.Distance(transform.rotation.eulerAngles, rotation.eulerAngles) > 2;
                        if (shouldRotateToPoint)
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _parameters.Reaction);
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
}