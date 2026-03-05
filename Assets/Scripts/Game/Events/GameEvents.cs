using App.Events;
using Game.Cameras;
using Game.Selection;
using Game.Tools;

namespace Game.Events
{
    public struct SelectionEvent : IEvent
    {
        public SelectionEvent(SelectionContext selection)
        {
            Selection = selection;
        }
        
        public SelectionContext Selection;
    }

    public struct HoverEvent : IEvent
    {
        public HoverEvent(SelectionContext hoverSelection)
        {
            HoverSelection = hoverSelection;
        }
        
        public SelectionContext HoverSelection;
    }

    public struct SelectToolEvent : IEvent
    {
        public SelectToolEvent(Tool tool)
        {
            Tool = tool;
        }
        public readonly Tool Tool;
    }

    public struct SetCameraModeEvent : IEvent
    {
        public SetCameraModeEvent(CameraMode cameraMode)
        {
            CameraMode = cameraMode;
        }
        public readonly CameraMode CameraMode;
    }
    public struct SetDofEvent : IEvent
    {
        public SetDofEvent(float dof)
        {
            Dof = dof;
        }
        public readonly float Dof;
    }
    public struct SetFovEvent : IEvent
    {
        public SetFovEvent(float fov)
        {
            Fov = fov;
        }
        public readonly float Fov;
    }
    public struct SetTimeEvent : IEvent
    {
        public SetTimeEvent(float time)
        {
            Time = time;
        }
        public readonly float Time;
    }

    public struct SetWhiteBalanceEvent : IEvent
    {
        public SetWhiteBalanceEvent(float temperature)
        {
            Temperature = temperature;
        }
        public readonly float Temperature;
    }
}