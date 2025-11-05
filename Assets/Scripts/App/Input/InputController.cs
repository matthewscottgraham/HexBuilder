using App.Services;
using App.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace App.Input
{
    public class InputController : IDisposable
    {
        private readonly EventSystem _eventSystem;
        private readonly InputSystem_Actions _inputSystem;
        
        public InputController()
        {
            _eventSystem = EventSystem.current;
            _inputSystem = new InputSystem_Actions();
            _inputSystem.Enable();

            _inputSystem.Player.Move.performed += HandleMove;
            _inputSystem.Player.Interact.started += HandleInteract;
            
            ServiceLocator.Instance.Register(new EventBus<MoveEvent>());
            ServiceLocator.Instance.Register(new EventBus<InteractEvent>());
        }

        public void Dispose()
        {
            ServiceLocator.Instance.Deregister(typeof(MoveEvent));
            ServiceLocator.Instance.Deregister(typeof(InteractEvent));
            
            _inputSystem.Player.Move.performed -= HandleMove;
            _inputSystem.Player.Interact.performed -= HandleInteract;
            _inputSystem?.Dispose();
        }

        private bool IsPointerOverUI()
        {
            var pointerEventData = new PointerEventData(_eventSystem)
            {
                position = Pointer.current.position.value
            };
            var results = new List<RaycastResult>();
            _eventSystem.RaycastAll(pointerEventData, results);
            return results.Count > 0;
        }

        private static void HandleMove(InputAction.CallbackContext ctx)
        {
            ServiceLocator.Instance.Get<EventBus<MoveEvent>>().Raise(new MoveEvent(ctx.ReadValue<Vector2>()));
        }

        private void HandleInteract(InputAction.CallbackContext ctx)
        {
            if (IsPointerOverUI()) return;
            ServiceLocator.Instance.Get<EventBus<InteractEvent>>().Raise(new InteractEvent());
        }
    }
}