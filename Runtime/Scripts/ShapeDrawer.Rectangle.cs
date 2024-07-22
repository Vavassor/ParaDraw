using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Rectangle drawing for ShapeDrawer.
    /// </summary>
    public partial class ShapeDrawer
    {
        [Header("Rectangle")]
        public Mesh rectangleMesh;

        private readonly Vector3[] rectangleVertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, 0.5f, 0.0f),
            new Vector3(-0.5f, 0.5f, 0.0f),

            new Vector3(-0.5f, -0.5f, 0.0f),
        };

        /// <summary>
        /// Draws a solid rectangle.
        /// </summary>
        /// <param name="center">The rectangle center.</param>
        /// <param name="rotation">The rectangle rotation.</param>
        /// <param name="size">The rectangle side lengths.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidRectangle(Vector3 center, Quaternion rotation, Vector2 size, Color color, float duration = 0.0f)
        {
            DrawSolidMesh(rectangleMesh, center, rotation, new Vector3(size.x, size.y, 1.0f), color, duration);
        }

        /// <summary>
        /// Draws a solid rectangle.
        /// </summary>
        /// <param name="center">The rectangle center.</param>
        /// <param name="axis">The axis perpendicular to the rectangle.</param>
        /// <param name="size">The rectangle side lengths.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidRectangle(Vector3 center, Vector3 axis, Vector2 size, Color color, float duration = 0.0f)
        {
            DrawSolidRectangle(center, Quaternion.LookRotation(axis), size, color, duration);
        }

        /// <summary>
        /// Draws a wireframe rectangle.
        /// </summary>
        /// <param name="center">The rectangle center.</param>
        /// <param name="rotation">The rectangle rotation.</param>
        /// <param name="size">The rectangle side lengths.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the mesh should be visible for.</param>
        public void DrawWireRectangle(Vector3 center, Quaternion rotation, Vector2 size, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawPolyline(rectangleVertices, center, rotation, new Vector3(size.x, 1.0f, size.y), color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a wireframe rectangle.
        /// </summary>
        /// <param name="center">The rectangle center.</param>
        /// <param name="axis">The axis perpendicular to the rectangle.</param>
        /// <param name="size">The rectangle side lengths.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the mesh should be visible for.</param>
        public void DrawWireRectangle(Vector3 center, Vector3 axis, Vector2 size, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawWireRectangle(center, Quaternion.LookRotation(axis), size, color, lineWidth, duration);
        }
    }
}
