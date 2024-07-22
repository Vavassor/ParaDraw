using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Draw boxes with ShapeDrawer.
    /// </summary>
    public partial class ShapeDrawer
    {
        [Header("Box")]
        public Mesh boxMesh;

        private readonly Vector3[] boxVertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),

            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),

            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),

            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),

            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),

            new Vector3(-0.5f, 0.5f, -0.5f),
        };

        /// <summary>
        /// Draws a solid box.
        /// </summary>
        /// <param name="center">The box center.</param>
        /// <param name="rotation">The box rotation.</param>
        /// <param name="size">The box side lengths.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidBox(Vector3 position, Quaternion rotation, Vector3 size, Color color, float duration = 0.0f)
        {
            DrawSolidMesh(boxMesh, position, rotation, size, color, duration);
        }

        /// <summary>
        /// Draws a solid box collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidBoxCollider(BoxCollider collider, Color color, float duration = 0.0f)
        {
            var colliderTransform = collider.transform;
            DrawSolidBox(colliderTransform.TransformPoint(collider.center), colliderTransform.rotation, Vector3.Scale(colliderTransform.lossyScale, collider.size), color, duration);
        }

        /// <summary>
        /// Draws a wireframe box. (rectangular cuboid)
        /// </summary>
        /// <param name="center">The box center.</param>
        /// <param name="rotation">The box rotation.</param>
        /// <param name="size">The box side lengths.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireBox(Vector3 center, Quaternion rotation, Vector3 size, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawPolyline(boxVertices, center, rotation, size, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a wireframe box collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireBoxCollider(BoxCollider collider, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var colliderTransform = collider.transform;
            DrawWireBox(colliderTransform.TransformPoint(collider.center), colliderTransform.rotation, Vector3.Scale(colliderTransform.lossyScale, collider.size), color, lineWidth, duration);
        }
    }
}
