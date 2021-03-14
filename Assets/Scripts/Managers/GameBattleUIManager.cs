using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectDynamax.GameLogic
{
    public class GameBattleUIManager : MonoBehaviour
    {
        [SerializeField] private Image _manaBarFillImage = default;

        [SerializeField] private Button[] _nextTurnSpawnButtons = default;
        [SerializeField] private Button[] _currentTurnSpawnButtons = default;
        [SerializeField] private Button[] _previousTurnSpawnButtons = default;

        [SerializeField] private TMP_Text[] _currentTurnCostTexts = default;

        private GameBattleManager _battleManager;

        #region Unity_Lifecycle
        private void Awake() { }
        private void OnEnable() { }
        private void Start() { }
        private void FixedUpdate() { }
        private void Update() { }
        private void LateUpdate() { }
        private void OnDisable() { }
        private void OnDestroy() { }
        #endregion

        public void InitializeBattleUI(Color[] colors, int[] costs)
        {
            _battleManager = GetComponent<GameBattleManager>();
            if (_battleManager is null) { return; }

            _manaBarFillImage.fillAmount = 1f;

            InitializeButtons(colors, costs);
        }

        private void InitializeButtons(Color[] colors, int[] costs)
        {
            InitializeButtonsListeners();
            InitializeButtonsColor(colors);
            UpdateButtonsCost(costs);
        }

        private void InitializeButtonsListeners()
        {
            foreach (var button in _nextTurnSpawnButtons)
            {
                button.interactable = false;
                button.onClick.RemoveAllListeners();
            }
            foreach (var button in _currentTurnSpawnButtons)
            {
                button.interactable = true;
                button.onClick.RemoveAllListeners();
            }
            foreach (var button in _previousTurnSpawnButtons)
            {
                button.interactable = false;
                button.onClick.RemoveAllListeners();
            }

            for (int i = 0; i < _currentTurnSpawnButtons.Length; i++)
            {
                var index = i;
                _currentTurnSpawnButtons[i].onClick.AddListener(() => _battleManager.OnPressSpawnMinionButton(index));
            }
        }

        private void InitializeButtonsColor(Color[] colors)
        {
            for (int i = 0; i < _previousTurnSpawnButtons.Length; i++)
            { _previousTurnSpawnButtons[i].image.color = Color.black; }

            var index = 0;
            UpdateSingleTurnButtonsColors(_currentTurnSpawnButtons, colors, ref index);
            UpdateSingleTurnButtonsColors(_nextTurnSpawnButtons, colors, ref index);
        }

        public void EnableCurrentTurnSpawnButtons()
        {
            foreach (var button in _currentTurnSpawnButtons)
            { button.interactable = true; }
        }

        public void DisableCurrentTurnSpawnButtons()
        {
            foreach (var button in _currentTurnSpawnButtons)
            { button.interactable = false; }
        }

        public void UpdateButtonsCost(int[] costs)
        {
            for (int i = 0; i < _currentTurnCostTexts.Length; i++)
            {
                _currentTurnCostTexts[i].text = costs[i].ToString();
            }
        }

        public void UpdateButtonsColors(Color[] colors)
        {
            var index = 0;

            UpdateSingleTurnButtonsColors(_previousTurnSpawnButtons, colors, ref index);
            UpdateSingleTurnButtonsColors(_currentTurnSpawnButtons, colors, ref index);
            UpdateSingleTurnButtonsColors(_nextTurnSpawnButtons, colors, ref index);
        }

        private void UpdateSingleTurnButtonsColors(Button[] buttons, Color[] colors, ref int index)
        {
            for (int i = 0; i < buttons.Length; i++)
            { buttons[i].image.color = colors[index + i]; }

            index += buttons.Length;
        }

        public void UpdateManaBar(float fillAmount)
        {
            _manaBarFillImage.fillAmount = fillAmount;
        }
    }
}