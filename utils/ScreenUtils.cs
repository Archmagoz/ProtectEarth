using Godot;

namespace ProtectEarth.Utils
{
    public static class ScreenUtils
    {
        public static Vector2 GetScreenCenter(Node node)
        {
            return node.GetViewport().GetCamera2D().GetScreenCenterPosition();
        }
    }
}