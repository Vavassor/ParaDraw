using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Circle, ellipse, and disk drawing for ShapeDrawer.
    /// </summary>
    public partial class ShapeDrawer
    {
        [Header("Circle")]
        public Mesh diskMesh;

        /// <summary>
        /// Draws a circular arc.
        /// </summary>
        /// <param name="origin">The circle center.</param>
        /// <param name="axisX">The axis parallel to the circle.</param>
        /// <param name="axisY">The axis of rotation.</param>
        /// <param name="radius">The circle radius.</param>
        /// <param name="startAngle">The angle between the X axis and the start of the arc, in degrees.</param>
        /// <param name="endAngle">The angle between the X axis and the end of the arc, in degrees.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawArc(Vector3 origin, Vector3 axisX, Vector3 axisY, float radius, float startAngle, float endAngle, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawEllipticArc(origin, axisX, axisY, radius * Vector2.one, startAngle, endAngle, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a wireframe circle.
        /// </summary>
        /// <param name="center">The circle center.</param>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="radius">The circle radius.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawCircle(Vector3 center, Vector3 axis, float radius, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawEllipticArc(center, GetOrthogonalVector(axis), axis, Vector2.one * radius, 0.0f, 360.0f, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a disk.
        /// </summary>
        /// <param name="center">The disk center.</param>
        /// <param name="axisY">The axis of rotation.</param>
        /// <param name="radius">The disk radius.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawDisk(Vector3 center, Vector3 axisY, float radius, Color color, float duration = 0.0f)
        {
            DrawEllipticDisk(center, Quaternion.LookRotation(axisY), radius * Vector3.one, color, duration);
        }

        /// <summary>
        /// Draws a disk.
        /// </summary>
        /// <param name="center">The disk center.</param>
        /// <param name="rotation">The disk rotation.</param>
        /// <param name="radius">The disk radius.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawDisk(Vector3 center, Quaternion rotation, float radius, Color color, float duration = 0.0f)
        {
            DrawEllipticDisk(center, rotation, radius * Vector3.one, color, duration);
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="center">The ellipse center.</param>
        /// <param name="axisX">The axis parallel to the ellipse.</param>
        /// <param name="axisY">The axis perpendicular to the ellipse.</param>
        /// <param name="radii">The ellipse radii. X is the semi-major axis and Y is the semi-minor axis.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawEllipse(Vector3 center, Vector3 axisX, Vector3 axisY, Vector2 radii, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawEllipticArc(center, axisX, axisY, radii, 0.0f, 360.0f, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws an elliptic arc.
        /// </summary>
        /// <param name="origin">The ellipse center.</param>
        /// <param name="axisX">The axis parallel to the ellipse.</param>
        /// <param name="axisY">The axis perpendicular to the ellipse.</param>
        /// <param name="radii">The ellipse radii. X is the semi-major axis and Y is the semi-minor axis.</param>
        /// <param name="startAngle">The angle between the X axis and the start of the arc, in degrees.</param>
        /// <param name="endAngle">The angle between the X axis and the end of the arc, in degrees.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawEllipticArc(Vector3 origin, Vector3 axisX, Vector3 axisY, Vector2 radii, float startAngle, float endAngle, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var lineRenderer = AllocateLineRenderer();

            if (lineRenderer == null)
            {
                return;
            }

            var positionCount = 32;
            lineRenderer.positionCount = positionCount;

            var arm = Vector3.zero;
            var axisZ = Vector3.Cross(axisX, axisY);
            var lookRotation = Quaternion.LookRotation(axisZ, axisY);
            var turn = (endAngle - startAngle) * Mathf.Deg2Rad / (positionCount - 1);
            var startTurn = startAngle * Mathf.Deg2Rad;

            for (var i = 0; i < positionCount; i++)
            {
                var angle = turn * i + startTurn;
                arm.x = Mathf.Cos(angle) * radii.x;
                arm.z = Mathf.Sin(angle) * radii.y;
                lineRenderer.SetPosition(i, lookRotation * arm + origin);
            }

            EnableLineRenderer(lineRenderer, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws an elliptic disk.
        /// </summary>
        /// <param name="center">The disk center.</param>
        /// <param name="rotation">The disk rotation.</param>
        /// <param name="radii">The ellipse radii. X is the semi-major axis and Y is the semi-minor axis.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawEllipticDisk(Vector3 center, Quaternion rotation, Vector2 radii, Color color, float duration = 0.0f)
        {
            DrawSolidMesh(diskMesh, center, rotation, new Vector3(radii.x, radii.y, 1.0f), color, duration);
        }

        /// <summary>
        /// Draws an elliptic disk.
        /// </summary>
        /// <param name="center">The disk center.</param>
        /// <param name="axisX">The axis parallel to the ellipse.</param>
        /// <param name="axisY">The axis perpendicular to the ellipse.</param>
        /// <param name="radii">The ellipse radii. X is the semi-major axis and Y is the semi-minor axis.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawEllipticDisk(Vector3 center, Vector3 axisX, Vector3 axisY, Vector2 radii, Color color, float duration = 0.0f)
        {
            var rotation = Quaternion.LookRotation(axisX, axisY);
            DrawEllipticDisk(center, rotation, radii, color, duration);
        }

        /// <summary>
        /// Draws a circular ring.
        /// </summary>
        /// <param name="center">The ring center.</param>
        /// <param name="axisX">The axis parallel to the ring.</param>
        /// <param name="axisY">The axis of rotation.</param>
        /// <param name="width">The ring width.</param>
        /// <param name="radius">The outer radius.</param>
        /// <param name="startAngle">The angle between the X axis and the start of the ring, in degrees.</param>
        /// <param name="endAngle">The angle between the X axis and the end of the ring, in degrees.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawRing(Vector3 center, Vector3 axisX, Vector3 axisY, float radius, float width, float startAngle, float endAngle, Color color, float duration = 0.0f)
        {
            var rotation = Quaternion.LookRotation(-axisY, Vector3.Cross(axisX, axisY));
            DrawEllipticRingSection(center, rotation, radius * Vector2.one, width, startAngle, endAngle, color, duration);
        }

        /// <summary>
        /// Draws a circular sector.
        /// </summary>
        /// <param name="center">The circle center.</param>
        /// <param name="axisX">The axis parallel to the circle.</param>
        /// <param name="axisY">The axis of rotation.</param>
        /// <param name="radius">The circle radius.</param>
        /// <param name="startAngle">The angle between the X axis and the start of the sector, in degrees.</param>
        /// <param name="endAngle">The angle between the X axis and the end of the sector, in degrees.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSector(Vector3 center, Vector3 axisX, Vector3 axisY, float radius, float startAngle, float endAngle, Color color, float duration = 0.0f)
        {
            var rotation = Quaternion.LookRotation(-axisY, Vector3.Cross(axisX, axisY));
            DrawEllipticRingSection(center, rotation, radius * Vector2.one, radius, startAngle, endAngle, color, duration);
        }

        private void DrawEllipticRingSection(Vector3 center, Quaternion rotation, Vector2 radii, float width, float startAngle, float endAngle, Color color, float duration = 0.0f)
        {
            AllocateDynamicMesh(out MeshFilter meshFilter, out MeshRenderer meshRenderer, out MaterialPropertyBlock propertyBlock);

            if (!meshFilter)
            {
                return;
            }

            DiskGeneration.CreateEllipticRingSection(dynamicMeshes[dynamicMeshIndexEnd], radii, width, startAngle, endAngle, 32);
            propertyBlock.SetColor("_SurfaceColor", color);
            var material = color.a >= 0.999f ? solidOpaqueMaterial : solidTransparentMaterial;
            EnableDynamicMesh(meshRenderer, propertyBlock, center, rotation, Vector3.one, material, duration);
        }
    }
}
