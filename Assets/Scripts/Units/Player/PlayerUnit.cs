using TopDownShooter.Environment;
using TopDownShooter.Weapons;
using UnityEngine;

namespace TopDownShooter.Units
{
    public class PlayerUnit : Unit
    {
        [SerializeField]
        private SafeOpenGame _safeGame;

        protected override void Start()
        {
            _weaponName = GameManager.Instance.SelectedWeapon;
            base.Start();
        }

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
            if (loot.HpRegen > 0)
            {
                Stats.HpCurrent = Mathf.Min(
                    Stats.HpCurrent + loot.HpRegen,
                    Stats.HpFull);
                HpBar.SetFillAmount(HpFillAmount);
            }
        }
    }
}