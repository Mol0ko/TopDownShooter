using TopDownShooter.Environment;
using TopDownShooter.Extensions;
using UnityEngine;

namespace TopDownShooter.Units
{
    public class PlayerUnit : Unit
    {
        [SerializeField]
        private SafeOpenGame _safeGame;
        [SerializeField, ReadOnly]
        private int coinsCollected = 0;

        protected new void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (other.gameObject.tag == "Coin" && !_isDead)
            {
                Destroy(other.gameObject);
                coinsCollected++;
            }
            else if (other.gameObject.tag == "Safe" && !_isDead)
            {
                _safeGame.StartGame();
            }
        }
    }
}