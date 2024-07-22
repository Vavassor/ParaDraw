using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Sphere drawing for ShapeDrawer.
    /// </summary>
    public partial class ShapeDrawer
    {
        [Header("Sphere")]
        public Mesh sphereMesh;

        /// <summary>
        /// Draws a solid ellipsoid with a given rotation and lengths of semi-axes.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="radii">The lengths of the semi-axes.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidEllipsoid(Vector3 center, Quaternion rotation, Vector3 radii, Color color, float duration = 0.0f)
        {
            DrawSolidMesh(sphereMesh, center, rotation, radii, color, duration);
        }

        /// <summary>
        /// Draws a solid sphere.
        /// </summary>
        /// <param name="center">The sphere center.</param>
        /// <param name="radius">The sphere radius.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidSphere(Vector3 center, float radius, Color color, float duration = 0.0f)
        {
            DrawSolidEllipsoid(center, Quaternion.identity, radius * Vector3.one, color, duration);
        }

        /// <summary>
        /// Draws a solid sphere collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidSphereCollider(SphereCollider collider, Color color, float duration = 0.0f)
        {
            var colliderTransform = collider.transform;
            var lossyScale = colliderTransform.lossyScale;
            var center = colliderTransform.TransformPoint(collider.center);
            var radius = collider.radius * Mathf.Max(lossyScale.x, lossyScale.y, lossyScale.z);
            DrawSolidEllipsoid(center, Quaternion.identity, radius * Vector3.one, color, duration);
        }

        /// <summary>
        /// Draws a wireframe ellipsoid with a given rotation and lengths of semi-axes.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="radii">The lengths of the semi-axes.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireEllipsoid(Vector3 center, Quaternion rotation, Vector3 radii, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var axisX = rotation * (radii.x * Vector3.right);
            var axisY = rotation * (radii.y * Vector3.up);
            var axisZ = rotation * (radii.z * Vector3.forward);
            DrawWireEllipsoid(center, axisX, axisY, axisZ, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a wireframe ellipsoid with given semi-axes.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="axisX">The X semi-axis.</param>
        /// <param name="axisY">The Y semi-axis.</param>
        /// <param name="axisZ">The Z semi-axis.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireEllipsoid(Vector3 center, Vector3 axisX, Vector3 axisY, Vector3 axisZ, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var x = axisX.magnitude;
            var y = axisY.magnitude;
            var z = axisZ.magnitude;
            DrawEllipse(center, axisY, axisX, new Vector2(y, z), color, lineWidth, duration);
            DrawEllipse(center, axisX, axisY, new Vector2(x, z), color, lineWidth, duration);
            DrawEllipse(center, axisX, axisZ, new Vector2(x, y), color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a wireframe sphere.
        /// </summary>
        /// <param name="center">The sphere center.</param>
        /// <param name="radius">The sphere radius.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireSphere(Vector3 center, float radius, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawWireEllipsoid(center, radius * Vector3.right, radius * Vector3.up, radius * Vector3.forward, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a wireframe sphere collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireSphereCollider(SphereCollider collider, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var colliderTransform = collider.transform;
            var lossyScale = colliderTransform.lossyScale;
            var center = colliderTransform.TransformPoint(collider.center);
            var radius = collider.radius * Mathf.Max(lossyScale.x, lossyScale.y, lossyScale.z);
            DrawWireSphere(center, radius, color, lineWidth, duration);
        }
    }
}
