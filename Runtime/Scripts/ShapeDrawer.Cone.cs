using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Cone drawing for ShapeDrawer.
    /// </summary>
    public partial class ShapeDrawer
    {
        /// <summary>
        /// Draws a wireframe circular cone.
        /// </summary>
        /// <param name="origin">The apex point.</param>
        /// <param name="axisY">The axis of rotation.</param>
        /// <param name="angle">The angle between the axis and the lateral surface, in degrees.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireCone(Vector3 origin, Vector3 axisY, float angle, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            Vector3 axisX = Vector3.right;
            Vector3 axisZ = Vector3.forward;
            GetOrthogonalBasis(axisY, ref axisX, ref axisZ);

            var radius = axisY.magnitude * Mathf.Tan(Mathf.Deg2Rad * angle);
            axisX = radius * axisX.normalized;
            axisZ = radius * axisZ.normalized;

            DrawWireEllipticCone(origin, axisX, axisY, axisZ, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a wireframe elliptic cone with a given rotation and angles.
        /// </summary>
        /// <param name="origin">The apex point.</param>
        /// <param name="rotation">The cone rotation.</param>
        /// <param name="angles">The angles between the axis and the lateral surface along each of the semi-axes.</param>
        /// <param name="height">The cone height.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireEllipticCone(Vector3 origin, Quaternion rotation, Vector2 angles, float height, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var radiusX = height * Mathf.Tan(Mathf.Deg2Rad * angles.x);
            var radiusZ = height * Mathf.Tan(Mathf.Deg2Rad * angles.y);
            var axisX = rotation * (radiusX * Vector3.right);
            var axisY = rotation * (height * Vector3.up);
            var axisZ = rotation * (radiusZ * Vector3.forward);
            DrawWireEllipticCone(origin, axisX, axisY, axisZ, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a wireframe elliptic cone with a given basis.
        /// </summary>
        /// <param name="origin">The apex point.</param>
        /// <param name="axisX">The semi-major axis.</param>
        /// <param name="axisY">The axis of rotation, whose length is the cone height.</param>
        /// <param name="axisZ">The semi-minor axis.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireEllipticCone(Vector3 origin, Vector3 axisX, Vector3 axisY, Vector3 axisZ, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var lineRenderer = AllocateLineRenderer();

            if (lineRenderer == null)
            {
                return;
            }

            var positionCount = 16;
            lineRenderer.positionCount = positionCount;

            var arm = Vector3.zero;
            var lookRotation = Quaternion.LookRotation(axisZ, axisY);
            var radii = new Vector2(axisX.magnitude, axisZ.magnitude);
            var turn = 2.0f * Mathf.PI / positionCount;

            for (var i = 0; i < positionCount; i += 2)
            {
                var angle = turn * i;
                arm.x = Mathf.Cos(angle) * radii.x;
                arm.z = Mathf.Sin(angle) * radii.y;
                lineRenderer.SetPosition(i, origin);
                lineRenderer.SetPosition(i + 1, lookRotation * arm + axisY + origin);
            }

            EnableLineRenderer(lineRenderer, color, lineWidth, duration);

            DrawEllipse(origin + axisY, axisX, axisY, radii, color, lineWidth, duration);
        }
    }
}
