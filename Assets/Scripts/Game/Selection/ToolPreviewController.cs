using System;
using System.Collections.Generic;
using App.Events;
using Game.Events;
using System.Linq;
using App.Input;
using Game.Grid;
using Game.Hexes;
using Game.Hexes.Features;
using Game.Tools;
using UnityEngine;

namespace Game.Selection
{
    public class ToolPreviewController : MonoBehaviour, IDisposable
    {
        private ToolController _toolController;
        private HexController _hexController;

        private readonly Dictionary<string, Texture2D> _toolCursors = new();
        private EventBinding<HoverEvent> _hoverEventBinding;
        
        private Texture2D _currentToolCursor;
        private readonly Vector2 _cursorHotspot = new Vector2(1f, 1f);
        private Color _errorColour = Color.red;

        private bool _isDefaultCursor = true;
        
        public void Initialize(ToolController toolController, HexController hexController)
        {
            _toolController = toolController;
            _hexController = hexController;

            var cursors = Resources.LoadAll<Texture2D>("ToolPreviews");
            foreach (var cursor in cursors)
            {
                _toolCursors.Add(cursor.name, cursor);
                _toolCursors.Add($"{cursor.name}_error", GenerateErrorCursor(cursor));
            }
        }

        public void Dispose()
        {
            _hoverEventBinding = null;
        }

        private void OnEnable()
        {
            _hoverEventBinding ??= new EventBinding<HoverEvent>(HandleHoverEvent);
            EventBus<HoverEvent>.Register(_hoverEventBinding);
        }

        private void OnDisable()
        {
            EventBus<HoverEvent>.Deregister(_hoverEventBinding);
        }

        private void LateUpdate()
        {
            switch (InputController.PointerIsOverUI)
            {
                case true when !_isDefaultCursor:
                    UseDefaultCursor();
                    break;
                case false when _isDefaultCursor:
                    UseToolCursor();
                    break;
            }
        }

        private Texture2D GenerateErrorCursor(Texture2D cursor)
        {
            var errorCursor = Instantiate(cursor);

            var pixels = errorCursor.GetPixels();

            for (var i = 0; i < pixels.Length; i++)
            {
                pixels[i] *= _errorColour;
            }

            errorCursor.SetPixels(pixels);
            errorCursor.Apply();
            
            return errorCursor;
        }

        private void HandleHoverEvent(HoverEvent evt)
        {
            UseDefaultCursor();
            var coordinates = evt.HoverSelection.Coordinates;
            var selectionType = evt.HoverSelection.SelectionType;

            if (coordinates.Count == 0) return;
            if (!HexGrid.InBounds(coordinates.FirstOrDefault())) return;
            
            SetToolCursor(coordinates.FirstOrDefault());
        }

        private void UseDefaultCursor()
        {
            _isDefaultCursor = true;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        private void UseToolCursor()
        {
            if (!_currentToolCursor)
            {
                UseDefaultCursor();
                return;
            } 
            
            _isDefaultCursor = false;
            Cursor.SetCursor(_currentToolCursor, _cursorHotspot, CursorMode.Auto);
        }
        
        private void SetToolCursor(CubicCoordinate coordinate)
        {
            var tool = _toolController.CurrentTool;
            var hex = _hexController.GetHexObject(coordinate);
            var isError = CheckIfValidHex(tool, hex);
            
            _currentToolCursor = (isError) ? _toolCursors[tool.Icon.name] : _toolCursors[tool.Icon.name + "_error"];
            UseToolCursor();
        }

        private static bool CheckIfValidHex(Tool tool, HexObject hex)
        {
            if (tool.GetType() == typeof(RaiseTerrainTool))
            { 
                return hex.Height != HexFactory.MaxHeight;
            }

            if (tool.GetType() == typeof(LowerTerrainTool))
            {
                return hex.Height != 0;
            }
            
            return tool.VerifyTileHeight(hex);
        }
    }
}
