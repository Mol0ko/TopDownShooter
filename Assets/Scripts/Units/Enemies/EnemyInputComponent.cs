using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TopDownShooter.Units.Player
{
    public class EnemyInputComponent : UnitInputComponent
    {
        [SerializeField]
        private Transform _playerTarget;

        #region Config

        private AttackType _attackType = AttackType.Shoot;
        private float _seekRadius = 10f;

        #endregion

        #region Lifecycle

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


        private void OnShoot(InputAction.CallbackContext obj) =>
            CallOnAttackEvent(_attackType);

        private void UpdateLookAtTarget()
        {
            if (_playerTarget == null)
                return;

            var playerPos = _playerTarget.transform.position;
            var distanceToPlayer = Vector3.Distance(transform.position, playerPos);
            if (distanceToPlayer <= _seekRadius)
            {
                var rotation = Quaternion.LookRotation(playerPos - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1000f);
            }
        }
    }
}