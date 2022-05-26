using UnityEngine;

namespace TopDownShooter.Units
{
    public class BulletCollisions : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Destroy(this.gameObject);
        }
    }
}