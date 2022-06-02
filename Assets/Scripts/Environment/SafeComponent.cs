using System.Linq;
using UnityEngine;

namespace TopDownShooter.Environment
{
    public class SafeComponent : MonoBehaviour
    {
        [SerializeField, Range(1f, 5f)]
        private float _shineSpeed = 2.5f;

        private Material _shineMaterial;
        public bool Opened { get; private set; } = false;

        private bool _shineGrows = false;

        public void Open()
        {
            Opened = true;
            _shineMaterial.color = new Color(
                _shineMaterial.color.r,
                _shineMaterial.color.g,
                _shineMaterial.color.b,
                0f
            );
        }

        private void Awake()
        {
            _shineMaterial = GetComponent<Renderer>().materials
                .FirstOrDefault(material => material.name.Contains("Shine"));
        }

        private void Update()
        {
            if (_shineMaterial != null && !Opened)
                UpdateShineAnimation();
        }

        private void UpdateShineAnimation()
        {
            var currentAlpha = _shineMaterial.color.a;
            var newAlpha = currentAlpha;
            var alphaChange = Time.deltaTime * _shineSpeed;

            if (currentAlpha >= 0.5f)
                _shineGrows = false;
            if (currentAlpha <= 0f)
                _shineGrows = true;

            if (_shineGrows)
                newAlpha += alphaChange;
            else
                newAlpha -= alphaChange;

            _shineMaterial.color = new Color(
                _shineMaterial.color.r,
                _shineMaterial.color.g,
                _shineMaterial.color.b,
                newAlpha
            );
        }
    }
}