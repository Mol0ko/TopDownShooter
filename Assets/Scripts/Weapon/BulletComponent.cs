using TopDownShooter.Extensions;
using UnityEngine;

namespace TopDownShooter.Weapons
{
    public class BulletComponent : MonoBehaviour
    {
        [SerializeField, Range(5f, 30f)]
        private float _moveSpeed = 15f;

        [ReadOnly]
        public float Damage = 1;

        void Update()
        {
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            Destroy(this.gameObject);
        }
    }
}