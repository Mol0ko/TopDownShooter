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
                var safe = other.gameObject.GetComponent<SafeComponent>();
                _safeGame.StartGame(safe, GetSafeLoot);
            }
        }

        private void GetSafeLoot(SafeLoot loot)
        {
            if (loot.Coins > 0)
                coinsCollected += loot.Coins;

            if (loot.HpRegen > 0)
                Stats.HpCurrent = Mathf.Max(
                    Stats.HpCurrent + loot.HpRegen,
                    Stats.HpFull);

            if (loot.WeaponNames.Length > 0)
            {
                // TODO: add weapons to collection
                Debug.Log($"Weapons obtained: {string.Join(", ", loot.WeaponNames)}");
            }
        }
    }
}