using UdonSharp;
using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Draw shapes in 3D space.
    /// </summary>
    [DefaultExecutionOrder(-1)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ShapeDrawer : UdonSharpBehaviour
    {
        public LineDrawer lineDrawer;
        public MeshDrawer meshDrawer;
        public ParticleSystem pointSystem;
        public TextDrawer textDrawer;

        private ParticleSystem.EmitParams emitParams;

        /// <summary>
        /// Draws a circular arc.
        /// </summary>
        /// <param name="origin">The circle center.</param>
        /// <param name="axisX">The axis parallel to the circle. </param>
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
        /// Draws 3D axes representing a given transform.
        /// </summary>
        /// <param name="origin">The transform translation.</param>
        /// <param name="rotation">The transform rotation.</param>
        /// <param name="scale">The transform scale.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawAxes(Vector3 origin, Quaternion rotation, Vector3 scale, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawRay(origin, rotation * (scale.x * Vector3.right), Color.red, lineWidth, duration);
            DrawRay(origin, rotation * (scale.y * Vector3.up), Color.green, lineWidth, duration);
            DrawRay(origin, rotation * (scale.z * Vector3.forward), Color.blue, lineWidth, duration);
        }

        /// <summary>
        /// Draws 3D axes representing a given transform.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawAxes(Transform t, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawAxes(t.position, t.rotation, t.lossyScale, lineWidth, duration);
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
        /// Draws an ellipse.
        /// </summary>
        /// <param name="center">The ellipse center.</param>
        /// <param name="axisX">The axis parallel to the ellipse. </param>
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
        /// <param name="axisX">The axis parallel to the ellipse. </param>
        /// <param name="axisY">The axis perpendicular to the ellipse.</param>
        /// <param name="radii">The ellipse radii. X is the semi-major axis and Y is the semi-minor axis.</param>
        /// <param name="startAngle">The angle between the X axis and the start of the arc, in degrees.</param>
        /// <param name="endAngle">The angle between the X axis and the end of the arc, in degrees.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawEllipticArc(Vector3 origin, Vector3 axisX, Vector3 axisY, Vector2 radii, float startAngle, float endAngle, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            lineDrawer.DrawEllipticArc(origin, axisX, axisY, radii, startAngle, endAngle, 32, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a line segment between the given start and end points.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawLine(Vector3 start, Vector3 end, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            lineDrawer.DrawLine(start, end, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a point.
        /// </summary>
        /// <param name="point">The point position.</param>
        /// <param name="color">The point color.</param>
        /// <param name="radius">The point radius.</param>
        /// <param name="duration">The number of seconds the point should be visible for.</param>
        public void DrawPoint(Vector3 point, Color color, float radius = 0.02f, float duration = 0.0f)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            emitParams.position = point;
            emitParams.startColor = color;
            emitParams.startSize = radius;
            emitParams.startLifetime = Mathf.Max(duration, Time.deltaTime + 1e-4f);
            pointSystem.Emit(emitParams, 1);
        }

        /// <summary>
        /// Draws a polyline between the given points. A polyline is a connected series of line segments.
        /// </summary>
        /// <param name="vertices">The points in the polyline.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawPolyline(Vector3[] vertices, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            lineDrawer.DrawPolyline(vertices, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a polyline with a given transform. A polyline is a connected series of line segments.
        /// </summary>
        /// <param name="vertices">The points in the polyline.</param>
        /// <param name="position">The transform translation.</param>
        /// <param name="rotation">The transform rotation.</param>
        /// <param name="scale">The transform scale.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawPolyline(Vector3[] vertices, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            lineDrawer.DrawPolyline(vertices, position, rotation, scale, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a ray.
        /// </summary>
        /// <param name="origin">The ray origin.</param>
        /// <param name="direction">The ray direciton.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawRay(Vector3 origin, Vector3 direction, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            lineDrawer.DrawRay(origin, direction, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a solid box.
        /// </summary>
        /// <param name="center">The box center.</param>
        /// <param name="rotation">The box rotation.</param>
        /// <param name="size">The box side lengths.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidBox(Vector3 center, Quaternion rotation, Vector3 size, Color color, float duration = 0.0f)
        {
            meshDrawer.DrawSolidBox(center, rotation, size, color, duration);
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
            meshDrawer.DrawSolidCapsule(0.5f * (start + end), Quaternion.LookRotation(Vector3.forward, direction), direction.magnitude, radius, color, duration);
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
            meshDrawer.DrawSolidCapsule(center, Quaternion.LookRotation(axis), height, radius, color, duration);
        }

        /// <summary>
        /// Draws a solid collider.
        /// 
        /// Supports BoxCollider, CapsuleCollider, MeshCollider, and SphereCollider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidCollider(Collider collider, Color color, float duration = 0.0f)
        {
            var foundType = collider.GetType();

            if (foundType == typeof(BoxCollider))
            {
                DrawSolidBoxCollider((BoxCollider)collider, color, duration);
            }
            else if (foundType == typeof(CapsuleCollider))
            {
                DrawSolidCapsuleCollider((CapsuleCollider)collider, color, duration);
            }
            else if (foundType == typeof(MeshCollider))
            {
                DrawSolidMeshCollider((MeshCollider)collider, color, duration);
            }
            else if (foundType == typeof(SphereCollider))
            {
                DrawSolidSphereCollider((SphereCollider)collider, color, duration);
            }
        }

        /// <summary>
        /// Draws a solid mesh with a given transform.
        /// </summary>
        /// <param name="position">The transform translation.</param>
        /// <param name="rotation">The transform rotation.</param>
        /// <param name="scale">The transform scale.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0.0f)
        {
            meshDrawer.DrawSolidMesh(mesh, position, rotation, scale, color, duration);
        }

        /// <summary>
        /// Draws a solid mesh collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <param name="color">The surface color.</param>
        /// <param name="duration">The number of seconds the surface should be visible for.</param>
        public void DrawSolidMeshCollider(MeshCollider collider, Color color, float duration = 0.0f)
        {
            var t = collider.transform;
            meshDrawer.DrawSolidMesh(collider.sharedMesh, t.position, t.rotation, t.lossyScale, color, duration);
        }

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
            meshDrawer.DrawSolidRectangle(center, rotation, size, color, duration);
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
            meshDrawer.DrawSolidRectangle(center, Quaternion.LookRotation(axis), size, color, duration);
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
            meshDrawer.DrawSolidSphere(center, radius, color, duration);
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
            meshDrawer.DrawSolidSphere(center, radius, color, duration);
        }

        /// <summary>
        /// Draws text at a position in 3D space. Text always faces the camera.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="position">The text center.</param>
        /// <param name="color">The font color.</param>
        /// <param name="yOffset">The text center height above the given position.</param>
        /// <param name="duration">The number of seconds the text should be visible for.</param>
        public void DrawText(string text, Vector3 position, Color color, float yOffset = 0.0f, float duration = 0.0f)
        {
            textDrawer.DrawText(text, position, Vector3.one, color, yOffset, duration);
        }

        /// <summary>
        /// Draws text at a position in 3D space with a given scale. Text always faces the camera.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="position">The text center.</param>
        /// <param name="scale">The text scale.</param>
        /// <param name="color">The font color.</param>
        /// <param name="yOffset">The text center height above the given position.</param>
        /// <param name="duration">The number of seconds the text should be visible for.</param>
        public void DrawText(string text, Vector3 position, Vector3 scale, Color color, float yOffset = 0.0f, float duration = 0.0f)
        {
            textDrawer.DrawText(text, position, scale, color, yOffset, duration);
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
            lineDrawer.DrawWireBox(center, rotation, size, color, lineWidth, duration);
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

        /// <summary>
        /// Draws a camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireCamera(Camera camera, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var t = camera.transform;
            DrawWireFrustum(t.position, t.rotation, camera.fieldOfView, camera.nearClipPlane, camera.farClipPlane, camera.aspect, color, lineWidth, duration);
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

        /// <summary>
        /// Draws a wireframe collider.
        /// 
        /// Supports BoxCollider, CapsuleCollider, MeshCollider, and SphereCollider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireCollider(Collider collider, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var foundType = collider.GetType();

            if (foundType == typeof(BoxCollider))
            {
                DrawWireBoxCollider((BoxCollider)collider, color, lineWidth, duration);
            }
            else if (foundType == typeof(CapsuleCollider))
            {
                DrawWireCapsuleCollider((CapsuleCollider)collider, color, lineWidth, duration);
            }
            else if (foundType == typeof(MeshCollider))
            {
                DrawWireMeshCollider((MeshCollider)collider, color, lineWidth, duration);
            }
            else if (foundType == typeof(SphereCollider))
            {
                DrawWireSphereCollider((SphereCollider)collider, color, lineWidth, duration);
            }
        }

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
        /// Draws a wireframe cylinder.
        /// </summary>
        /// <param name="start">The center of the first base.</param>
        /// <param name="end">The center of the second base.</param>
        /// <param name="radius">The cylinder radius.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireCylinder(Vector3 start, Vector3 end, float radius, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var axisY = end - start;
            var axisX = GetOrthogonalVector(axisY);
            var turn = 0.5f * Mathf.PI;
            var arm = Vector3.zero;
            var lookRotation = Quaternion.LookRotation(axisX, axisY);

            for (var i = 0; i < 4; i++)
            {
                var angle = turn * i;
                arm.x = Mathf.Cos(angle) * radius;
                arm.z = Mathf.Sin(angle) * radius;
                var direction = lookRotation * arm;
                lineDrawer.DrawLine(direction + start, direction + end, color, lineWidth, duration);
            }

            DrawCircle(start, axisY, radius, color, lineWidth, duration);
            DrawCircle(end, axisY, radius, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a wireframe ellipsoid with a given rotation and lengths of semi-axes.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="scale">The lengths of the semi-axes.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireEllipsoid(Vector3 center, Quaternion rotation, Vector3 scale, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var axisX = rotation * (scale.x * Vector3.right);
            var axisY = rotation * (scale.y * Vector3.up);
            var axisZ = rotation * (scale.z * Vector3.forward);
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
            lineDrawer.DrawWireEllipticCone(origin, axisX, axisY, axisZ, color, lineWidth, duration);
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
        /// Draws a wireframe rectangular frustum. A frustum is a pyramid with the tip cut off.
        /// </summary>
        /// <param name="origin">The apex point.</param>
        /// <param name="rotation">The frustum rotation.</param>
        /// <param name="verticalFieldOfView">The angle in degrees between the lateral surfaces in the vertical plane.</param>
        /// <param name="nearPlane">The distance from the apex to the near plane.</param>
        /// <param name="farPlane">The distance from the apex to the far plane.</param>
        /// <param name="aspectRatio">The ratio of the width and height of the rectangle.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireFrustum(Vector3 origin, Quaternion rotation, float verticalFieldOfView, float nearPlane, float farPlane, float aspectRatio, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            lineDrawer.DrawWireFrustum(origin, rotation, verticalFieldOfView, nearPlane, farPlane, aspectRatio, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a mesh with a given transform.
        /// </summary>
        /// <param name="position">The transform translation.</param>
        /// <param name="rotation">The transform rotation.</param>
        /// <param name="scale">The transform scale.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            meshDrawer.DrawWireMesh(mesh, position, rotation, scale, color, lineWidth, duration);
        }

        /// <summary>
        /// Draws a mesh collider.
        /// </summary>
        /// <param name="collider">The collider.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the mesh should be visible for.</param>
        public void DrawWireMeshCollider(MeshCollider collider, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var colliderTransform = collider.transform;
            meshDrawer.DrawWireMesh(collider.sharedMesh, colliderTransform.position, colliderTransform.rotation, colliderTransform.lossyScale, color, lineWidth, duration);
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
            lineDrawer.DrawWireRectangle(center, Quaternion.LookRotation(axis), size, color, lineWidth, duration);
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
            lineDrawer.DrawWireRectangle(center, rotation, size, color, lineWidth, duration);
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

        /// <summary>
        /// Draws a wireframe stadium. A stadium is a 2D pill shape.
        /// </summary>
        /// <param name="start">The start point of the center line.</param>
        /// <param name="end">The end point of the center line.</param>
        /// <param name="normal">The axis perpendicular to the stadium.</param>
        /// <param name="radius">The distance from the center line to the perimeter.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
        public void DrawWireStadium(Vector3 start, Vector3 end, Vector3 normal, float radius, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            lineDrawer.DrawWireStadium(start, end, normal, radius, color, lineWidth, duration);
        }

        /// <summary>
        /// Get an orthogonal basis given a single vector. Use when any orthogonal basis will do.
        /// </summary>
        private void GetOrthogonalBasis(Vector3 v, ref Vector3 tangent, ref Vector3 binormal)
        {
            float l = (v.x * v.x) + (v.y * v.y);

            if (!Mathf.Approximately(l, 0.0f))
            {
                float d = Mathf.Sqrt(l);
                var n = new Vector3(v.y / d, -v.x / d, 0.0f);
                tangent = n;
                binormal.x = (-v.z * n.y);
                binormal.y = (v.z * n.x);
                binormal.z = (v.x * n.y) - (v.y * n.x);
            }
            else
            {
                tangent.x = (v.z < 0.0f) ? -1.0f : 1.0f;
                tangent.y = 0.0f;
                tangent.z = 0.0f;
                binormal = Vector3.up;
            }
        }

        /// <summary>
        /// Get a vector orthogonal to the given vector. Use when any orthogonal vector will do.
        /// </summary>
        private Vector3 GetOrthogonalVector(Vector3 v)
        {
            float l = (v.x * v.x) + (v.y * v.y);

            if (!Mathf.Approximately(l, 0.0f))
            {
                float d = Mathf.Sqrt(l);
                return new Vector3(v.y / d, -v.x / d, 0.0f);
            }
            else
            {
                float x = v.z < 0.0f ? -1.0f : 1.0f;
                return new Vector3(x, 0.0f, 0.0f);
            }
        }
    }
}
