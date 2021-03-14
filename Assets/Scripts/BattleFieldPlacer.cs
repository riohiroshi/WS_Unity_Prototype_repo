using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectDynamax.GameLogic
{
    public class BattleFieldPlacer : MonoBehaviour
    {
        private const int BATTLE_FIELD_WIDTH = 16;
        private const int BATTLE_FIELD_HEIGHT = 12;

        private Vector3 _battleFieldSize = new Vector3(BATTLE_FIELD_WIDTH, 1f, BATTLE_FIELD_HEIGHT);

        private Cell[] _cellList;
        private float _cellSize = 4f;
        private int _cellCountX;
        private int _cellCountY;

        #region Unity_Lifecycle
        private void Awake() { }
        private void OnEnable() { }
        private void Start() { InitializeCells(); }
        private void FixedUpdate() { }
        private void Update() { }
        private void OnDrawGizmos() { DrawGizmos(); }
        private void LateUpdate() { }
        private void OnDisable() { }
        private void OnDestroy() { }
        #endregion


        public Vector3 OccupyNextAvailablePosition()
        {
            var cell = NextAvailableCell();

            if (cell is null) { return Vector3.zero; }

            cell.IsOccupied = true;

            return cell.PositionInWorld;
        }


        private void InitializeCells()
        {
            var div = _battleFieldSize / _cellSize;
            _cellCountX = Mathf.CeilToInt(div.x);
            _cellCountY = Mathf.CeilToInt(div.z);

            _cellList = new Cell[_cellCountX * _cellCountY];

            var offset = new Vector3(_battleFieldSize.x / 2, 0f, /*_battleFieldSize.z / 2*/0f);
            var size = Vector3.one * _cellSize;

            for (int y = 0; y < _cellCountY; y++)
            {
                for (int x = 0; x < _cellCountX; x++)
                {
                    var pos = new Vector3(x, 25, y) * _cellSize - offset + size / 2;
                    _cellList[y * _cellCountX + x] = new Cell(x, y, pos);
                }
            }
        }

        private Cell GetCellByPos(in Vector3 pos)
        {
            var offset = _battleFieldSize / 2;
            var p = (pos + offset) / _cellSize;
            return GetCell(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.z));
        }
        private Cell GetCell(int x, int y)
        {
            if (x < 0 || x >= _cellCountX) { return null; }
            if (y < 0 || y >= _cellCountY) { return null; }

            return _cellList[y * _cellCountX + x];
        }

        private Cell NextAvailableCell()
        {
            for (int i = 0; i < _cellList.Length; i++)
            {
                if (_cellList[i].IsOccupied) { continue; }

                return _cellList[i];
            }

            return null;
        }

        private void DrawGizmos()
        {
            if (_cellList is null) { return; }

            var offset = new Vector3(_battleFieldSize.x / 2, 0f, _battleFieldSize.z / 2);
            var size = Vector3.one * _cellSize;

            for (int i = 0; i < _cellList.Length; i++)
            {
                Gizmos.color = _cellList[i].IsOccupied ? Color.red : Color.white;

                //var pos = new Vector3(_cellList[i].PositionInMap.XPos, 20, _cellList[i].PositionInMap.YPos) * _cellSize - offset + size / 2;
                var pos = _cellList[i].PositionInWorld;
                Gizmos.DrawWireCube(pos, size);
#if UNITY_EDITOR
                UnityEditor.Handles.Label(pos, $"{_cellList[i].PositionInMap.XPos}, {_cellList[i].PositionInMap.YPos}: {_cellList[i].IsOccupied}");
#endif
            }
        }

        private struct MapPosition
        {
            int _xPos;
            int _yPos;

            public int XPos { get => _xPos; }
            public int YPos { get => _yPos; }

            public MapPosition(int xPos, int yPos)
            {
                _xPos = xPos;
                _yPos = yPos;
            }

            public bool Equals(MapPosition other) => _xPos == other.XPos && _yPos == other.YPos;

            public override string ToString() => string.Concat(_xPos, ", ", _yPos);

            public static MapPosition operator +(MapPosition a, MapPosition b) => new MapPosition(a.XPos + b.XPos, a.YPos + b.YPos);
            public static MapPosition operator -(MapPosition a, MapPosition b) => new MapPosition(a.XPos - b.XPos, a.YPos - b.YPos);
        }
        private class Cell
        {
            MapPosition _positionInMap;
            Vector3 _positionInWorld;
            bool _isOccupied;

            public MapPosition PositionInMap { get => _positionInMap; }
            public Vector3 PositionInWorld { get => _positionInWorld; }
            public bool IsOccupied { get => _isOccupied; set => _isOccupied = value; }

            public Cell(MapPosition mapPosition, bool isOccupied = false)
            {
                _positionInMap = mapPosition;
                _positionInWorld = Vector3.zero;
                _isOccupied = isOccupied;
            }

            public Cell(int xPos, int yPos, bool isOccupied = false) => new Cell(xPos, yPos, Vector3.zero, isOccupied);

            public Cell(int xPos, int yPos, Vector3 position, bool isOccupied = false)
            {
                _positionInMap = new MapPosition(xPos, yPos);
                _positionInWorld = position;
                _isOccupied = isOccupied;
            }
        }
    }
}