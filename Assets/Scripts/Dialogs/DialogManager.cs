using System;
using System.Linq;
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
        private ToggleGroup _weaponSelector;
        [SerializeField]
        private Toggle[] _weaponTogglePool;
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
                Destroy(this.gameObject);
            else
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
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
            _closeButton.gameObject.SetActive(data.ShowCloseButton);
            _dialogCanvas.SetActive(true);
        }

        public void ShowSafeLootDialog(SafeLoot loot)
        {
            var message = "Вы получили:\n";
            if (loot.Coins > 0)
                message += $"\n• Монеты: {loot.Coins}";
            if (loot.HpRegen > 0)
                message += $"\n• Еду, которая восстановила вам {loot.HpRegen} жизней";
            if (loot.WeaponNames.Length > 0)
                message += $"\n• Оружие: {string.Join(", ", loot.WeaponNames)}";

            var dialogData = new DialogData(
                "Сейф открыт!",
                message,
                null,
                "ОК",
                CloseCurrentDialog);
            ShowDialog(dialogData);
        }

        public void ShowWeaponSelectionDialog(
            string[] weapons,
            string currentWeapon,
            UnityAction<string> onSelectWeapon)
        {
            _weaponSelector.gameObject.SetActive(true);
            foreach (var toggle in _weaponTogglePool)
                toggle.gameObject.SetActive(false);
            for (var i = 0; i < weapons.Length; i++)
            {
                _weaponTogglePool[i].gameObject.SetActive(true);
                _weaponTogglePool[i].isOn = weapons[i] == currentWeapon;
                _weaponTogglePool[i].GetComponentInChildren<Text>().text = weapons[i];
            }
            var dialogData = new DialogData(
                "Выбор оружия",
                "",
                null,
                "ОК",
                () =>
                {
                    var selectedToggle = _weaponTogglePool.FirstOrDefault(toggle => toggle.isOn);
                    if (selectedToggle != null)
                    {
                        var selectedWeaponIndex = Array.IndexOf(_weaponTogglePool, selectedToggle);
                        var selectedWeapon = weapons[selectedWeaponIndex];
                        onSelectWeapon.Invoke(selectedWeapon);
                        _weaponSelector.gameObject.SetActive(false);
                    }
                    CloseCurrentDialog();
                },
                false);
            ShowDialog(dialogData);
        }

        public void CloseCurrentDialog() =>
            _dialogCanvas.SetActive(false);

        public void InvokeButtonAction() =>
            _buttonAction?.Invoke();
    }
}