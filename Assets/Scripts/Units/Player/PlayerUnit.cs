using TopDownShooter.Environment;
using UnityEngine;

namespace TopDownShooter.Units
{
    public class PlayerUnit : Unit
    {
        [SerializeField]
        private SafeOpenGame _safeGame;

        protected new void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (other.gameObject.tag == "Coin" && !_isDead)
            {
                Destroy(other.gameObject);
                GameManager.Instance.OnGetCoins(1);
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
                GameManager.Instance.OnGetCoins(loot.Coins);

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