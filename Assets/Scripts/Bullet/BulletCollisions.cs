using UnityEngine;

namespace TopDownShooter.Units
{
    public class BulletCollisions : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Bullet triggered with " + other.gameObject.name);
            Destroy(this.gameObject);
        }
    }
}