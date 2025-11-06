using System;
using App;
using App.Events;
using App.Input;
using App.Services;
using Game.Grid;
using UnityEngine;

public class CellSelector : MonoBehaviour
{
    private Camera _camera;
    private Transform _cellHighlighter;
    
    private EventBinding<MoveEvent> _moveEventBinding;
    private EventBinding<InteractEvent> _interactEventBinding;

    private void Start()
    {
        _camera = Camera.main;
        _cellHighlighter = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        _cellHighlighter.SetParent(this.transform);
    }

    private void Awake()
    {
        if (ServiceLocator.Instance == null) return;
        
        _moveEventBinding = new EventBinding<MoveEvent>(HandleMoveEvent);
        _interactEventBinding = new EventBinding<InteractEvent>(HandleInteractEvent);
        
        ServiceLocator.Instance.Get<EventBus<MoveEvent>>().Register(_moveEventBinding);
        ServiceLocator.Instance.Get<EventBus<InteractEvent>>().Register(_interactEventBinding);
    }

    private void OnDestroy()
    {
        if (ServiceLocator.Instance == null) return;
        ServiceLocator.Instance.Get<EventBus<MoveEvent>>().Deregister(_moveEventBinding);
        ServiceLocator.Instance.Get<EventBus<InteractEvent>>().Deregister(_interactEventBinding);
    }

    private void Update()
    {
        if (!InputController.PointerHasMovedThisFrame) return;
        
        Ray ray = _camera.ScreenPointToRay(InputController.PointerPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = ClampPositionToCell(hit.point);
        }
    }

    private void HandleMoveEvent(MoveEvent moveEvent)
    {
        if (moveEvent.Delta.y != 0)
        {
            var nudge = moveEvent.Delta.y > 0 ? 1 : -1;
            transform.position = ClampPositionToCell(new Vector3(
                transform.position.x + moveEvent.Delta.x * 2 + nudge,
                0,
                transform.position.z + moveEvent.Delta.y * 1.5f));
        }
        else
        {
            transform.position = ClampPositionToCell(new Vector3(
                transform.position.x + moveEvent.Delta.x * 2,
                0,
                transform.position.z + moveEvent.Delta.y * 1.5f));
        }
    }

    private void HandleInteractEvent(InteractEvent eventData)
    {
        // get cell at current transform position
    }

    private Vector3 ClampPositionToCell(Vector3 position)
    {
        return ServiceLocator.Instance.Get<HexGrid>().GetClosestHexCenterToPosition(position);
    }
}
