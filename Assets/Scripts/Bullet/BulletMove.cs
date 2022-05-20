using UnityEngine;

namespace TopDownShooter.Units
{
    public class BulletMove : MonoBehaviour
    {
        [SerializeField, Range(5f, 30f)]
        private float _moveSpeed = 15f;

        void Update()
        {
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        }
    }
}