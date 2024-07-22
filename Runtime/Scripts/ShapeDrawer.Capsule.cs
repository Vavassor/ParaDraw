using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Capsule drawing for ShapeDrawer.
    /// </summary>
    public partial class ShapeDrawer
    {
        [Header("Capsule")]
        public Mesh capsuleCapMesh;
        public Mesh capsuleCylinderMesh;

        /// <summary>
        /// Draws a solid capsule.
        /// </summary>
        /// <param name="center">The capsule center</param>
        /// <param name="rotation"></param>
        /// <param name="height">The </param>
        /// <param name="radius">The distance from the center line to the surface.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidCapsule(Vector3 center, Quaternion rotation, float height, float radius, Color color, float duration = 0.0f)
        {
            var scaleY = Mathf.Max(0.5f * height - radius, 0.0001f);
            var extentY = scaleY * (rotation * Vector3.up);
            var scale = radius * Vector3.one;
            DrawSolidMesh(capsuleCapMesh, center + extentY, rotation, scale, color, duration);
            DrawSolidMesh(capsuleCylinderMesh, center, rotation, new Vector3(radius, scaleY, radius), color, duration);
            DrawSolidMesh(capsuleCapMesh, center - extentY, rotation * Quaternion.Euler(180.0f, 0.0f, 0.0f), scale, color, duration);
        }

        /// <summary>
        /// Draws a solid capsule.
        /// </summary>
        /// <param name="start">The start point of the center line.</param>
        /// <param name="end">The end point of the center line.</param>
        /// <param name="radius">The distance from the center line to the surface.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidCapsule(Vector3 start, Vector3 end, float radius, Color color, float duration = 0.0f)
        {
            var direction = end - start;
            DrawSolidCapsule(0.5f * (start + end), Quaternion.LookRotation(Vector3.forward, direction), direction.magnitude + 2.0f * radius, radius, color, duration);
        }

        /// <summary>
        /// Draws a solid capsule collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidCapsuleCollider(CapsuleCollider collider, Color color, float duration = 0.0f)
        {
            Vector3 axis;
            float radius;
            float height;
            var colliderTransform = collider.transform;
            var lossyScale = colliderTransform.lossyScale;
            var unscaledHeight = Mathf.Max(collider.height - 2.0f * collider.radius, 0.0001f);

            switch (collider.direction)
            {
                default:
                case 0:
                    radius = Mathf.Max(lossyScale.y, lossyScale.z) * collider.radius;
                    height = unscaledHeight * lossyScale.x;
                    axis = colliderTransform.TransformDirection(Vector3.right);
                    break;
                case 1:
                    radius = Mathf.Max(lossyScale.x, lossyScale.z) * collider.radius;
                    height = unscaledHeight * lossyScale.y;
                    axis = colliderTransform.TransformDirection(Vector3.up);
                    break;
                case 2:
                    radius = Mathf.Max(lossyScale.x, lossyScale.y) * collider.radius;
                    height = unscaledHeight * lossyScale.z;
                    axis = colliderTransform.TransformDirection(Vector3.forward);
                    break;
            }

            var center = colliderTransform.TransformPoint(collider.center);
            DrawSolidCapsule(center, Quaternion.LookRotation(axis), height, radius, color, duration);
        }

        /// <summary>
        /// Draws a wireframe capsule.
        /// </summary>
        /// <param name="start">The start point of the center line.</param>
        /// <param name="end">The end point of the center line.</param>
        /// <param name="radius">The distance from the center line to the surface.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireCapsule(Vector3 start, Vector3 end, float radius, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var axisY = end - start;
            Vector3 axisX = Vector3.right;
            Vector3 axisZ = Vector3.forward;
            GetOrthogonalBasis(axisY, ref axisX, ref axisZ);
            axisX = radius * axisX.normalized;
            axisZ = radius * axisZ.normalized;

            DrawCircle(start, axisY, radius, color, lineWidth, duration);
            DrawCircle(end, axisY, radius, color, lineWidth, duration);
            DrawWireStadium(start, end, axisX, radius, color, lineWidth, duration);
            DrawWireStadium(start, end, axisZ, radius, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a wireframe capsule collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireCapsuleCollider(CapsuleCollider collider, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            Vector3 axis;
            float radius;
            var colliderTransform = collider.transform;
            var lossyScale = colliderTransform.lossyScale;
            var extent = 0.5f * Mathf.Max(collider.height - 2.0f * collider.radius, 0.0001f);

            switch (collider.direction)
            {
                default:
                case 0:
                    radius = Mathf.Max(lossyScale.y, lossyScale.z) * collider.radius;
                    axis = colliderTransform.TransformDirection(extent * lossyScale.x * Vector3.right);
                    break;
                case 1:
                    radius = Mathf.Max(lossyScale.x, lossyScale.z) * collider.radius;
                    axis = colliderTransform.TransformDirection(extent * lossyScale.y * Vector3.up);
                    break;
                case 2:
                    radius = Mathf.Max(lossyScale.x, lossyScale.y) * collider.radius;
                    axis = colliderTransform.TransformDirection(extent * lossyScale.z * Vector3.forward);
                    break;
            }

            var center = colliderTransform.TransformPoint(collider.center);
            var start = center + axis;
            var end = center - axis;
            DrawWireCapsule(start, end, radius, color, lineWidth, duration);
        }
    }
}
