using Godot;

namespace ProtectEarth.Core.Utils
{
    public static class ScreenUtils
    {
        // Retrieves the current screen center position based on the active Camera2D.
        // Requires the provided node to be inside the scene tree and a valid active camera to exist.
        public static Vector2 GetScreenCenter(Node node)
        {
            var camera = node.GetViewport().GetCamera2D();

            // Defensive check — avoids null reference if no camera is active.
            if (camera == null)
                return Vector2.Zero;

            return camera.GetScreenCenterPosition();
        }
    }
}