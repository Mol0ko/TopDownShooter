using TopDownShooter.Environment;
using UnityEngine;

namespace TopDonShooter.DialogManager
{
    public class DialogManager : MonoBehaviour
    {

        private static DialogManager _instance;

        public static DialogManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null)
                Destroy(_instance.gameObject);

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public void ShowSafeLootDialog(SafeLoot loot) {
            // TODO: implement
            Debug.Log($"Safe loot to show: {loot}");
        }
    }
}