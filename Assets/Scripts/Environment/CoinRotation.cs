using UnityEngine;

namespace TopDownShooter.Environment
{
    public class CoinRotation : MonoBehaviour
    {
        [SerializeField]
        private float _rotateSpeed = 200f;

        private void Update()
        {
            transform.Rotate(0f, _rotateSpeed * Time.deltaTime, 0f);
        }
    }
}