using UnityEngine;
using UnityEngine.InputSystem;

namespace TopDownShooter.Environment
{
    public class SafeOpenGame : MonoBehaviour
    {
        [SerializeField]
        private GameObject _gameCanvas;
        [SerializeField]
        private Transform _holeCenter;
        [SerializeField, Tooltip("Угол сектора взлома (в градусах)")]
        private float _passSectorAngle = 25f;
        [SerializeField, Tooltip("Количество попыток для взлома")]
        private int _maxAttempts = 5;

        /// <summary>Центр сектора взлома (в градусах)</summary>
        private float _passSectorCenter = 180f;
        private int _attemptsLeft = 5;
        private PlayerControls _controls;

        //region Lifecycle

        private void Awake()
        {
            _passSectorCenter = Random.Range(0f, 360f);
            _attemptsLeft = _maxAttempts;
            _controls = new PlayerControls();
        }

        private void Update()
        {
            UpdateMouseRotation();
        }

        //endregion

        public void StartGame()
        {
            _controls.Unit.Disable();
            _controls.SafeOpen.Push.performed += OnPush;
            _gameCanvas.SetActive(true);
        }

        //region Private methods

        private void UpdateMouseRotation()
        {
            Vector3 mousePoint = Mouse.current.position.ReadValue();
            var lookPos = mousePoint - _holeCenter.position;
            lookPos.z = 0;
            var rotation = Quaternion.LookRotation(Vector3.forward, lookPos);
            _holeCenter.rotation = Quaternion.Slerp(
                _holeCenter.rotation, rotation,
                Time.deltaTime * 1000f
            );
        }

        private void OnPush(InputAction.CallbackContext obj)
        {
            var pushAngle = _holeCenter.transform.rotation.z;
            var passAngleMin = _passSectorCenter - _passSectorAngle * 0.5f;
            var passAngleMax = _passSectorCenter + _passSectorAngle * 0.5f;
            if (pushAngle >= passAngleMin && pushAngle <= passAngleMax)
                OnWin();
            else
                OnFail();
        }

        private void OnWin()
        {
            Debug.Log("WIN SAFE GAME");
            _gameCanvas.SetActive(false);
            _controls.Unit.Enable();
            _controls.SafeOpen.Push.performed -= OnPush;
            //TODO: game over
        }

        private void OnFail()
        {
            if (_attemptsLeft > 0)
            {
                _attemptsLeft--;
                Debug.Log("FAIL: attempts left " + _attemptsLeft.ToString());
            }
            else
            {
                Debug.Log("LOOSE SAFE GAME");
                _gameCanvas.SetActive(false);
                _controls.Unit.Enable();
                _controls.SafeOpen.Push.performed -= OnPush;
                //TODO: game over
            }
        }

        //endregion
    }
}