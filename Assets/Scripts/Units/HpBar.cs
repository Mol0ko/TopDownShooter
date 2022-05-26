using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TopDownShooter.Units
{
    public class HpBar : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        private Canvas _canvas;

        private Quaternion _canvasStartRotation = Quaternion.identity;

        private void Awake()
        {
            _image.fillAmount = 1;
        }

        private void Start()
        {
            _canvas = GetComponentInParent<Canvas>();
            _canvasStartRotation = _canvas.transform.rotation;
        }

        private void LateUpdate()
        {
            _canvas.transform.rotation = _canvasStartRotation;
        }

        public void SetFillAmount(float value)
        {
            _image.fillAmount = value;
            if (value <= 0)
                StartCoroutine(HiderRoutine());
        }

        private IEnumerator HiderRoutine() {
            yield return new WaitForSeconds(3f);
            this.gameObject.SetActive(false);
        }
    }
}