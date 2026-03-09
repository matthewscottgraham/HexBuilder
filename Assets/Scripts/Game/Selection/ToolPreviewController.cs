using System;
using System.Collections.Generic;
using App.Events;
using Game.Events;
using System.Linq;
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
        private FeatureFactory _featureFactory;

        private Dictionary<string, GameObject> _toolPreviews = new();
        private EventBinding<HoverEvent> _hoverEventBinding;
        private EventBinding<SelectionEvent> _selectionEventBinding;
        private Material _material;
        private Material _errorMaterial;
        private GameObject _preview;
        
        public void Initialize(ToolController toolController, HexController hexController, FeatureFactory featureFactory)
        {
            _toolController = toolController;
            _hexController = hexController;
            _featureFactory = featureFactory;
            _material = _material = Resources.Load<Material>("Materials/mat_preview");
            _errorMaterial = Resources.Load<Material>("Materials/mat_error");

            foreach (var tool in _toolController.Tools)
            {
                if (string.IsNullOrEmpty(tool.PreviewName)) continue;
                var prefab = Resources.Load<GameObject>($"ToolPreviews/{tool.PreviewName}");
                if (prefab != null)
                    _toolPreviews.Add(tool.PreviewName, prefab);
            }
        }

        public void Dispose()
        {
            _hoverEventBinding = null;
        }

        private void OnEnable()
        {
            _hoverEventBinding ??= new EventBinding<HoverEvent>(HandleHoverEvent);
            _selectionEventBinding ??= new EventBinding<SelectionEvent>(HandleSelectionEvent);
            EventBus<HoverEvent>.Register(_hoverEventBinding);
            EventBus<SelectionEvent>.Register(_selectionEventBinding);
        }

        private void OnDisable()
        {
            EventBus<HoverEvent>.Deregister(_hoverEventBinding);
            EventBus<SelectionEvent>.Deregister(_selectionEventBinding);
        }

        private void HandleHoverEvent(HoverEvent evt)
        {
            ReleasePreview();
            var coordinates = evt.HoverSelection.Coordinates;
            var selectionType = evt.HoverSelection.SelectionType;

            if (coordinates.Count == 0) return;
            if (!HexGrid.InBounds(coordinates.FirstOrDefault())) return;
            if (_toolController.GetCurrentToolRadius() > 0) return;
            if (selectionType != SelectionType.Face) return;
            
            GetPreview(coordinates.FirstOrDefault());
        }

        private void HandleSelectionEvent(SelectionEvent evt)
        {
            ReleasePreview();
        }
        
        private void GetPreview(CubicCoordinate coordinate)
        {
            var tool = _toolController.CurrentTool;
            var hex = _hexController.GetHexObject(coordinate);

            if (!string.IsNullOrEmpty(tool.PreviewName) && _toolPreviews.ContainsKey(tool.PreviewName))
            {
                _preview = Instantiate(_toolPreviews[tool.PreviewName], transform);
            }
            else
            {
                var prefabIndex = _featureFactory.GetPrefabIndex(tool.FeatureType);
                _preview = _featureFactory.CreateFeature(tool.FeatureType, prefabIndex);
            }
            
            if (_preview == null) return;
            hex.SetFeatureVisibility(false);
            _preview.transform.position = hex.transform.position + new Vector3(0, hex.Height, 0);
            
            var material = tool.VerifyTileHeight(hex) ? _material : _errorMaterial; 
            var mrs = _preview.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in mrs)
            {
                mr.sharedMaterial = material;
            }
        }

        private void GetToolPreview(GameObject prefab)
        {
            _preview = Instantiate(prefab, transform);
        }

        private void ReleasePreview()
        {
            if (_preview == null) return;
            Destroy(_preview);
            _preview = null;
        }
    }
}
