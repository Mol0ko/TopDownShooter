using UnityEngine;
using UnityEngine.InputSystem;

namespace TopDownShooter.Units.Player
{
    public class PlayerInputComponent : UnitInputComponent
    {
        public PlayerControls Controls { get; private set; }

        #region Lifecycle

        private void Awake()
        {
            Controls = new PlayerControls();
            Controls.Unit.Attack.performed += OnShoot;
        }

        void Update()
        {
            var direction = Controls.Unit.Move.ReadValue<Vector2>();
            Movement = new Vector3(direction.x, 0f, direction.y);
            UpdateLookAtMouse();
        }

        private void OnEnable()
        {
            Controls.Unit.Enable();
        }

        private void OnDisable()
        {
            Controls.Unit.Disable();
        }
        private void OnDestroy()
        {
            Controls.Unit.Attack.performed -= OnShoot;
            Controls.Dispose();
        }

        #endregion

        private void OnShoot(InputAction.CallbackContext obj) =>
            CallOnAttackEvent(AttackType.Shoot);

        private void UpdateLookAtMouse()
        {
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                var lookPos = hit.point - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1000f);
            }
        }
    }
}