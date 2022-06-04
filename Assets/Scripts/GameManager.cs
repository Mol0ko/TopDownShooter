using TopDonShooter.Dialogs;
using TopDownShooter.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TopDownShooter
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance => _instance;

        //region Serialized Fields

        [SerializeField, ReadOnly]
        private int _currentLevel = 1;
        [SerializeField]
        private int _gameLevelsCount = 5;
        [SerializeField, ReadOnly]
        private int _totalCoins = 0;
        [SerializeField, ReadOnly]
        private int _coinsCollectedOnLevel = 0;
        [SerializeField, ReadOnly]
        private int _safesOpenedOnLevel = 0;
        [SerializeField, ReadOnly]
        private int _enemiesKilledOnLevel = 0;
        [SerializeField]
        private int[] _enemiesToKillOnLevel;
        [SerializeField]
        private string[] _levelSceneNames;
        [SerializeField]
        private Sprite[] _welcomeTourSprites;

        //endregion

        //region Lifecycle

        private void Awake()
        {
            if (_instance != null)
                Destroy(_instance.gameObject);

            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            _currentLevel = PlayerPrefs.GetInt(PlayerPrefsKeys.CURRENT_LEVEL);
            _totalCoins = PlayerPrefs.GetInt(PlayerPrefsKeys.TOTAL_COINS_COUNT);
        }

        private void Start()
        {
            if (PlayerPrefs.GetInt(PlayerPrefsKeys.WELCOME_TOUR_WATCHED) == 0)
            {
                ShowWelcomeTour();
            }
        }

        //endregion

        //region Public methods

        public void OnGetCoins(int count)
        {
            _totalCoins += count;
            PlayerPrefs.SetInt(PlayerPrefsKeys.TOTAL_COINS_COUNT, _totalCoins);
            PlayerPrefs.Save();
            _coinsCollectedOnLevel += count;
        }

        public void OnKillEnemy()
        {
            _enemiesKilledOnLevel++;
            CheckIsLevelCompleted();
        }

        public void OnSafeOpen()
        {
            _safesOpenedOnLevel++;
        }

        //endregion

        //region Private methods

        private void ShowWelcomeTour()
        {
            DialogData dialogData1 = null;
            DialogData dialogData2 = null;

            void ShowTip1() =>
                DialogManager.Instance.ShowDialog(dialogData1);
            void ShowTip2() =>
                DialogManager.Instance.ShowDialog(dialogData2);

            var message1 = "• Пользуйтесь клавишами WASD для перемещения персонажа.\n" +
                "• Двигайте курсор мыши чтобы прицелиться.\n" +
                "• Нажмите ЛКМ чтобы выстрелить.";
            dialogData1 = new DialogData(
                "Управление",
                message1,
                _welcomeTourSprites[0],
                "Далее",
                ShowTip2);

            var message2 = $"• Очистите город от всех врагов, чтобы пройти уровень.\n" +
                "• Собирайте монеты.\n" +
                "• Открывайте сейфы, чтобы получить предметы.";
            dialogData2 = new DialogData(
                "Игровой процесс",
                message2,
                _welcomeTourSprites[1],
                "Играть",
                DialogManager.Instance.CloseCurrentDialog);

            ShowTip1();
            PlayerPrefs.SetInt(PlayerPrefsKeys.WELCOME_TOUR_WATCHED, 1);
            PlayerPrefs.Save();
        }

        private void CheckIsLevelCompleted()
        {
            // Условие прохождения уровня - убийство необходимого количества врагов
            var levelCompleted = _enemiesKilledOnLevel >= _enemiesToKillOnLevel[_currentLevel];
            if (levelCompleted)
            {
                ShowLevelStatsDialog();
                PlayerPrefs.SetInt(PlayerPrefsKeys.CURRENT_LEVEL, _currentLevel + 1);
                PlayerPrefs.Save();
            }
        }

        private void ShowLevelStatsDialog()
        {
            var message = $"• Врагов убито: {_enemiesKilledOnLevel}\n" +
                $"• Монет собрано: {_coinsCollectedOnLevel}\n" +
                $"• Вскрыто сейфов: {_safesOpenedOnLevel}\n";
            var data = new DialogData(
                "Уровень пройден!",
                message,
                null,
                "Следующий уровень",
                GoToNextLevel,
                false);
            DialogManager.Instance.ShowDialog(data);
        }

        private void GoToNextLevel()
        {
            DialogManager.Instance.CloseCurrentDialog();
            Debug.Log($"Level {_currentLevel} completed!");
            if (_currentLevel >= _gameLevelsCount)
                ShowGameCompletedDialog();
            else
            {
                _currentLevel++;
                _coinsCollectedOnLevel = 0;
                _enemiesKilledOnLevel = 0;
                _safesOpenedOnLevel = 0;
                SceneManager.LoadScene(
                    _levelSceneNames[_currentLevel]
                );
            }
            // TODO: go to next level
        }

        private void ShowGameCompletedDialog()
        {
            var message = "Вы прошли игру TopDownShooter.\n\n" +
                $"Автор, гейм-дизайнер и разработчик: Никита Красавин\n" +
                $"Игра написана при поддержке онлайн-школы \"Нетология\"";
            var data = new DialogData(
                "Поздравляем!",
                message,
                null,
                "Продолжить бродить по уровню",
                DialogManager.Instance.CloseCurrentDialog);
            DialogManager.Instance.ShowDialog(data);
        }

        //endregion
    }
}