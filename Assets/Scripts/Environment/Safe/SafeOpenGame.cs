using System.Collections;
using TopDownShooter.Extensions;
using TopDownShooter.Units.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TopDownShooter.Environment
{
    [RequireComponent(typeof(PlayerInputComponent))]
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
        [SerializeField]
        private GameObject _winTextObject;
        [SerializeField]
        private GameObject _failTextObject;
        [SerializeField]
        private Text _attemptsLeftText;
        [SerializeField]
        private PlayerInputComponent _playerInput;
        /// <summary>Центр сектора взлома (в градусах)</summary>
        [SerializeField, ReadOnly]
        private float _passSectorCenter = 180f;
        [SerializeField, ReadOnly]
        private int _attemptsLeft = 5;

        private PlayerControls _playerControls;
        private SafeComponent _safe;
        private UnityAction<SafeLoot> _onGetLoot;

        //region Lifecycle

        private void Awake()
        {
            _passSectorCenter = Random.Range(0f, 360f);
            _attemptsLeft = _maxAttempts;
        }

        private void Start()
        {
            _playerControls = _playerInput.Controls;
        }

        private void Update()
        {
            UpdateMouseRotation();
        }

        private void OnDisable()
        {
            _playerControls.SafeOpen.Disable();
        }

        //endregion

        public void StartGame(SafeComponent safe, UnityAction<SafeLoot> onGetLoot)
        {
            _safe = safe;
            _playerControls.Unit.Disable();
            _playerControls.SafeOpen.Enable();
            _playerControls.SafeOpen.Push.performed += OnPush;
            _gameCanvas.SetActive(true);
            _attemptsLeft = _maxAttempts;
            _onGetLoot = onGetLoot;
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
            var pushAngle = _holeCenter.transform.rotation.eulerAngles.z;
            var passAngleMin = _passSectorCenter - _passSectorAngle * 0.5f;
            var passAngleMax = _passSectorCenter + _passSectorAngle * 0.5f;
            Debug.Log($"PUSHED ANGLE: {pushAngle}\nEXPECTED ANGLES: {passAngleMin}-{passAngleMax}");
            if (pushAngle >= passAngleMin && pushAngle <= passAngleMax)
                OnWin();
            else
                OnFail();
        }

        private void OnWin()
        {
            Debug.Log("WIN SAFE GAME");
            _playerControls.Unit.Enable();
            _playerControls.SafeOpen.Push.performed -= OnPush;
            var loot = _safe?.Open();
            if (loot != null)
                _onGetLoot?.Invoke(loot);
            // TODO: add safe loot
            _failTextObject.SetActive(false);
            StartCoroutine(ShowAndHideAfterDelay(_winTextObject));
            StartCoroutine(ExitGameAfterDelay());
        }

        private void OnFail()
        {
            if (_attemptsLeft > 0)
            {
                _attemptsLeft--;
                _attemptsLeftText.text = _attemptsLeft.ToString();
                StopAllCoroutines();
                StartCoroutine(ShowAndHideAfterDelay(_failTextObject));
                Debug.Log("FAIL: attempts left " + _attemptsLeft.ToString());
            }
            else
            {
                Debug.Log("LOOSE SAFE GAME");
                _playerControls.Unit.Enable();
                _playerControls.SafeOpen.Push.performed -= OnPush;
                StartCoroutine(ShowAndHideAfterDelay(_failTextObject));
                StartCoroutine(ExitGameAfterDelay());
            }
        }

        private IEnumerator ExitGameAfterDelay()
        {
            _playerControls.SafeOpen.Disable();
            yield return new WaitForSeconds(2f);
            _gameCanvas.SetActive(false);
        }

        private IEnumerator ShowAndHideAfterDelay(GameObject go)
        {
            go.SetActive(true);
            yield return new WaitForSeconds(2f);
            go.SetActive(false);
        }

        //endregion
    }
}