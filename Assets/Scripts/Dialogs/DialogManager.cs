using TopDownShooter.Environment;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TopDonShooter.Dialogs
{
    public class DialogManager : MonoBehaviour
    {
        private static DialogManager _instance;

        public static DialogManager Instance => _instance;

        [SerializeField]
        private GameObject _dialogCanvas;
        [SerializeField]
        private Text _title;
        [SerializeField]
        private Text _message;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Text _actionButtonTitle;
        [SerializeField]
        private Button _actionButton;
        [SerializeField]
        private Button _closeButton;

        private UnityAction _buttonAction;

        private void Awake()
        {
            if (_instance != null)
                Destroy(_instance.gameObject);

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public void ShowDialog(DialogData data)
        {
            _title.text = data.Title;
            _message.text = data.Message;
            _image.gameObject.SetActive(data.imageSprite != null);
            if (data.imageSprite != null)
                _image.sprite = data.imageSprite;
            _actionButtonTitle.text = data.ButtonTitle;
            _buttonAction = data.ButtonAction;
            _dialogCanvas.SetActive(true);
        }

        public void ShowSafeLootDialog(SafeLoot loot)
        {
            var message = "Вы получили:\n";
            if (loot.Coins > 0)
                message += $"\nМонеты: {loot.Coins}";
            if (loot.HpRegen > 0)
                message += $"\nЕду, которая восстановила вам {loot.HpRegen} жизней";
            if (loot.WeaponNames.Length > 0)
                message += $"Оружие: {string.Join(", ", loot.WeaponNames)}";

            var dialogData = new DialogData(
                "Сейф открыт!",
                message,
                null,
                "ОК",
                CloseCurrentDialog);
            
        }

        public void CloseCurrentDialog()
        {
            _dialogCanvas.SetActive(false);
        }
    }
}