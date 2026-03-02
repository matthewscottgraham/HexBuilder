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

    public struct SelectTool : IEvent
    {
        public SelectTool(Tool tool)
        {
            Tool = tool;
        }
        public Tool Tool;
    }

    public struct SetCameraModeEvent : IEvent
    {
        public SetCameraModeEvent(CameraMode cameraMode)
        {
            CameraMode = cameraMode;
        }
        public CameraMode CameraMode;
    }
    public struct SetDofEvent : IEvent
    {
        public SetDofEvent(float dof)
        {
            Dof = dof;
        }
        public float Dof;
    }
    public struct SetFovEvent : IEvent
    {
        public SetFovEvent(float fov)
        {
            Fov = fov;
        }
        public float Fov;
    }
    public struct SetTimeEvent : IEvent
    {
        public SetTimeEvent(float time)
        {
            Time = time;
        }
        public float Time;
    }
}