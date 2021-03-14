using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectDynamax.GameLogic
{
    [System.Serializable]
    public class BattleDeck
    {
        private List<Minion> _currentAllMinions;

        private List<Minion> _previousTurnMinions;
        private List<Minion> _currentTurnMinions;
        private List<Minion> _nextTurnMinions;

        private int _currentMinionIndex;

        public BattleDeck(List<Minion> allMinions)
        {
            _currentAllMinions = new List<Minion>();
            for (int i = 0; i < allMinions.Count; i++) { _currentAllMinions.Add(allMinions[i]); }

            _previousTurnMinions = new List<Minion>();
            _currentTurnMinions = new List<Minion>();
            _nextTurnMinions = new List<Minion>();

            InitializeDeck();
        }

        public void ShuffleDeck()
        {
            for (int n = _currentAllMinions.Count - 1; n > 0; n--)
            {
                var i = Random.Range(0, n + 1);
                var temp = _currentAllMinions[i];
                _currentAllMinions[i] = _currentAllMinions[n];
                _currentAllMinions[n] = temp;
            }
        }

        public void MoveToNextTurn()
        {
            PushCurrentMinionsToPrevious();
            PushNextMinionsToCurrent();
            TryPushMinionsToNext();
        }

        public Minion GetPreviousTurnMinionByIndex(int index) => _previousTurnMinions[index];
        public Minion GetCurrentTurnMinionByIndex(int index) => _currentTurnMinions[index];
        public Minion GetNextTurnMinionByIndex(int index) => _nextTurnMinions[index];

        public Color[] GetMinionsColor()
        {
            var colors = new List<Color>();

            for (int i = 0; i < _previousTurnMinions.Count; i++)
            { colors.Add(_previousTurnMinions[i].MinionColor); }

            for (int i = 0; i < _currentTurnMinions.Count; i++)
            { colors.Add(_currentTurnMinions[i].MinionColor); }

            for (int i = 0; i < _nextTurnMinions.Count; i++)
            { colors.Add(_nextTurnMinions[i].MinionColor); }

            return colors.ToArray();
        }
        public int[] GetCurrentMinionsCost()
        {
            var costs = new int[_currentTurnMinions.Count];

            for (int i = 0; i < _currentTurnMinions.Count; i++)
            {
                costs[i] = _currentTurnMinions[i].ManaCost;
            }

            return costs;
        }

        private void InitializeDeck()
        {
            ShuffleDeck();
            _currentMinionIndex = 0;
            InitializeCurrentList();
            InitializeNextList();
        }
        private void InitializeCurrentList()
        {
            for (_currentMinionIndex = 0; _currentMinionIndex < 3; _currentMinionIndex++)
            { PushMinionsToCurrent(_currentAllMinions[_currentMinionIndex]); }
        }
        private void InitializeNextList()
        {
            for (_currentMinionIndex = 3; _currentMinionIndex < 6; _currentMinionIndex++)
            { PushMinionToNext(_currentAllMinions[_currentMinionIndex]); }
        }

        private void PushCurrentMinionsToPrevious()
        {
            _previousTurnMinions.Clear();

            for (int i = 0; i < _currentTurnMinions.Count; i++)
            { PushMinionToPrevious(_currentTurnMinions[i]); }
        }
        private void PushNextMinionsToCurrent()
        {
            _currentTurnMinions.Clear();

            for (int i = 0; i < _nextTurnMinions.Count; i++)
            { PushMinionsToCurrent(_nextTurnMinions[i]); }
        }
        private void TryPushMinionsToNext()
        {
            if (_currentMinionIndex >= _currentAllMinions.Count)
            {
                ShuffleDeck();
                _currentMinionIndex = 0;
            }

            PushMinionsToNext();
        }
        private void PushMinionsToNext()
        {
            _nextTurnMinions.Clear();

            for (int i = 0; i < 3; i++)
            { PushMinionToNext(_currentAllMinions[_currentMinionIndex + i]); }

            _currentMinionIndex += 3;
        }

        private void PushMinionToPrevious(Minion minion) => _previousTurnMinions.Add(minion);
        private void PushMinionsToCurrent(Minion minion) => _currentTurnMinions.Add(minion);
        private void PushMinionToNext(Minion minion) => _nextTurnMinions.Add(minion);
    }
}