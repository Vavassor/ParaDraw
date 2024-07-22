using UdonSharp;
using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Draw shapes in 3D space.
    /// </summary>
    [DefaultExecutionOrder(-1)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public partial class ShapeDrawer : UdonSharpBehaviour
    {
        public ParticleSystem pointSystem;

        private ParticleSystem.EmitParams emitParams;

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
                DrawLine(direction + start, direction + end, color, lineWidth, duration);
            }

            DrawCircle(start, axisY, radius, color, lineWidth, duration);
            DrawCircle(end, axisY, radius, color, lineWidth, duration);
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
            var lineRenderer = AllocateLineRenderer();

            if (lineRenderer == null)
            {
                return;
            }

            var positionCount = 16;
            lineRenderer.positionCount = positionCount;

            float tanY = Mathf.Tan(Mathf.Deg2Rad * verticalFieldOfView / 2.0f);
            float nearExtentY = tanY * nearPlane;
            float nearExtentX = nearExtentY * aspectRatio;

            float farExtentY = tanY * farPlane;
            float farExtentX = farExtentY * aspectRatio;

            var right = rotation * Vector3.right;
            var up = rotation * Vector3.up;
            var forward = rotation * Vector3.forward;

            Vector3 fc = farPlane * forward + origin;
            Vector3 nc = nearPlane * forward + origin;

            var c0 = fc + (up * farExtentY) - (right * farExtentX);
            var c1 = fc + (up * farExtentY) + (right * farExtentX);
            var c2 = fc - (up * farExtentY) - (right * farExtentX);
            var c3 = fc - (up * farExtentY) + (right * farExtentX);

            var c4 = nc + (up * nearExtentY) - (right * nearExtentX);
            var c5 = nc + (up * nearExtentY) + (right * nearExtentX);
            var c6 = nc - (up * nearExtentY) - (right * nearExtentX);
            var c7 = nc - (up * nearExtentY) + (right * nearExtentX);

            lineRenderer.SetPosition(0, c0);
            lineRenderer.SetPosition(1, c1);
            lineRenderer.SetPosition(2, c3);
            lineRenderer.SetPosition(3, c2);
            lineRenderer.SetPosition(4, c0);

            lineRenderer.SetPosition(5, c4);
            lineRenderer.SetPosition(6, c5);
            lineRenderer.SetPosition(7, c1);
            lineRenderer.SetPosition(8, c5);

            lineRenderer.SetPosition(9, c7);
            lineRenderer.SetPosition(10, c3);
            lineRenderer.SetPosition(11, c7);

            lineRenderer.SetPosition(12, c6);
            lineRenderer.SetPosition(13, c2);
            lineRenderer.SetPosition(14, c6);

            lineRenderer.SetPosition(15, c4);

            EnableLineRenderer(lineRenderer, color, lineWidth, duration);
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
            var lineRenderer = AllocateLineRenderer();

            if (lineRenderer == null)
            {
                return;
            }

            var capPositionCount = 17;
            var positionCount = 35;
            lineRenderer.positionCount = positionCount;

            var arm = new Vector3(radius, 0.0f, 0.0f);
            var lookRotation = Quaternion.LookRotation(end - start, normal);
            var startPoint = lookRotation * arm + end;
            lineRenderer.SetPosition(0, startPoint);

            var turn = 2.0f * Mathf.PI / (positionCount - 3);

            for (var i = 1; i < capPositionCount; i++)
            {
                var angle = turn * i;
                arm.x = Mathf.Cos(angle) * radius;
                arm.z = Mathf.Sin(angle) * radius;
                lineRenderer.SetPosition(i, lookRotation * arm + end);
            }

            for (var i = capPositionCount; i < positionCount - 1; i++)
            {
                var angle = turn * (i - 1);
                arm.x = Mathf.Cos(angle) * radius;
                arm.z = Mathf.Sin(angle) * radius;
                lineRenderer.SetPosition(i, lookRotation * arm + start);
            }

            lineRenderer.SetPosition(positionCount - 1, startPoint);

            EnableLineRenderer(lineRenderer, color, lineWidth, duration);
        }

        /// <summary>
        /// Get an orthogonal basis given a single vector. Use when any orthogonal basis will do.
        /// </summary>
        private static void GetOrthogonalBasis(Vector3 v, ref Vector3 tangent, ref Vector3 binormal)
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
        private static Vector3 GetOrthogonalVector(Vector3 v)
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

        private void OnDestroy()
        {
            OnDestroyLineDrawer();
        }

        private void Start()
        {
            StartLineDrawer();
            StartMeshDrawer();
            StartTextDrawer();
        }

        private void Update()
        {
            UpdateLineDrawer();
            UpdateMeshDrawer();
            UpdateTextDrawer();
        }
    }
}
