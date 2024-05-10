using UdonSharp;
using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    public class ShapeDrawer : UdonSharpBehaviour
    {
        public GameObject linePrefab;
        public LineRenderer[] lineRenderers = new LineRenderer[16];
        public GameObject linesGroup;

        private float[] lineDurations = new float[16];
        private int lineIndexEnd;
        private GameObject[] lineObjects = new GameObject[16];

        private Vector3[] arrowheadVertices = new Vector3[]
        {
            new Vector3(-1.0f, -1.0f, -1.4142f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(-1.0f, -1.0f, -1.4142f),

            new Vector3(1.0f, -1.0f, -1.4142f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(1.0f, -1.0f, -1.4142f),

            new Vector3(1.0f, 1.0f, -1.4142f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 1.0f, -1.4142f),

            new Vector3(-1.0f, 1.0f, -1.4142f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(-1.0f, 1.0f, -1.4142f),

            new Vector3(-1.0f, -1.0f, -1.4142f),
        };

        private Vector3[] boxVertices = new Vector3[]
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

        public void DrawArc(Vector3 origin, Vector3 axisX, Vector3 axisY, float radius, float startAngle, float endAngle, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawEllipticArc(origin, axisX, axisY, radius * Vector2.one, startAngle, endAngle, color, lineWidth, duration);
        }

        public void DrawAxes(Vector3 origin, Quaternion rotation, Vector3 scale, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawRay(origin, rotation * (scale.x * Vector3.right), Color.red, lineWidth, duration);
            DrawRay(origin, rotation * (scale.y * Vector3.up), Color.green, lineWidth, duration);
            DrawRay(origin, rotation * (scale.z * Vector3.forward), Color.blue, lineWidth, duration);
        }

        public void DrawEllipticArc(Vector3 origin, Vector3 axisX, Vector3 axisY, Vector2 radii, float startAngle, float endAngle, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var lineRenderer = AllocateLineRenderer();

            if (lineRenderer == null)
            {
                return;
            }

            var positionCount = 33;
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

        public void DrawLine(Vector3 start, Vector3 end, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var lineRenderer = AllocateLineRenderer();

            if (lineRenderer == null)
            {
                return;
            }

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            EnableLineRenderer(lineRenderer, color, lineWidth, duration);
        }

        public void DrawPolyline(Vector3[] vertices, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var lineRenderer = AllocateLineRenderer();

            if (lineRenderer == null)
            {
                return;
            }

            lineRenderer.positionCount = vertices.Length;
            lineRenderer.SetPositions(vertices);

            EnableLineRenderer(lineRenderer, color, lineWidth, duration);
        }

        public void DrawPolyline(Vector3[] vertices, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var lineRenderer = AllocateLineRenderer();

            if (lineRenderer == null)
            {
                return;
            }

            lineRenderer.positionCount = vertices.Length;

            for (var i = 0; i < vertices.Length; i++)
            {
                lineRenderer.SetPosition(i, rotation * Vector3.Scale(scale, vertices[i]) + position);
            }

            EnableLineRenderer(lineRenderer, color, lineWidth, duration);
        }

        public void DrawRay(Vector3 start, Vector3 direction, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var end = start + direction;
            DrawLine(start, end, color, lineWidth, duration);
            DrawPolyline(arrowheadVertices, end, Quaternion.LookRotation(direction), 0.02f * Vector3.one, color, lineWidth, duration);
        }

        public void DrawWireBox(Vector3 center, Quaternion rotation, Vector3 scale, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawPolyline(boxVertices, center, rotation, scale, color, lineWidth, duration);
        }

        public void DrawWireCapsule(Vector3 start, Vector3 end, float radius, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var axisY = end - start;
            Vector3 axisX = Vector3.right;
            Vector3 axisZ = Vector3.forward;
            GetOrthogonalBasis(axisY, ref axisX, ref axisZ);
            axisX = radius * axisX.normalized;
            axisZ = radius * axisZ.normalized;

            DrawWireCircle(start, axisY, radius, color, lineWidth, duration);
            DrawWireCircle(end, axisY, radius, color, lineWidth, duration);
            DrawWireStadium(start, end, axisX, radius, color, lineWidth, duration);
            DrawWireStadium(start, end, axisZ, radius, color, lineWidth, duration);
        }

        public void DrawWireCircle(Vector3 center, Vector3 axis, float radius, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawEllipticArc(center, GetOrthogonalVector(axis), axis, Vector2.one * radius, 0.0f, 360.0f, color, lineWidth, duration);
        }

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

        public void DrawWireEllipse(Vector3 center, Vector3 axisX, Vector3 axisY, Vector2 radii, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawEllipticArc(center, axisX, axisY, radii, 0.0f, 360.0f, color, lineWidth, duration);
        }

        public void DrawWireEllipsoid(Vector3 center, Quaternion rotation, Vector3 scale, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var axisX = rotation * (scale.x * Vector3.right);
            var axisY = rotation * (scale.y * Vector3.up);
            var axisZ = rotation * (scale.z * Vector3.forward);
            DrawWireEllipsoid(center, axisX, axisY, axisZ, color, lineWidth, duration);
        }

        public void DrawWireEllipsoid(Vector3 center, Vector3 axisX, Vector3 axisY, Vector3 axisZ, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var x = axisX.magnitude;
            var y = axisY.magnitude;
            var z = axisZ.magnitude;
            DrawWireEllipse(center, axisY, axisX, new Vector2(y, z), color, lineWidth, duration);
            DrawWireEllipse(center, axisX, axisY, new Vector2(x, z), color, lineWidth, duration);
            DrawWireEllipse(center, axisX, axisZ, new Vector2(x, y), color, lineWidth, duration);
        }

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

            DrawWireEllipse(origin + axisY, axisX, axisY, radii, color, lineWidth, duration);
        }

        public void DrawWireEllipticCone(Vector3 origin, Quaternion rotation, Vector2 angles, float height, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var radiusX = height * Mathf.Tan(Mathf.Deg2Rad * angles.x);
            var radiusZ = height * Mathf.Tan(Mathf.Deg2Rad * angles.y);
            var axisX = rotation * (radiusX * Vector3.right);
            var axisY = rotation * (height * Vector3.up);
            var axisZ = rotation * (radiusZ * Vector3.forward);
            DrawWireEllipticCone(origin, axisX, axisY, axisZ, color, lineWidth, duration);
        }

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

        public void DrawWireSphere(Vector3 center, float radius, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawWireEllipsoid(center, radius * Vector3.right, radius * Vector3.up, radius * Vector3.forward, color, lineWidth, duration);
        }

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

        private LineRenderer AllocateLineRenderer()
        {
            if (lineIndexEnd > lineRenderers.Length - 1)
            {
                var lineCount = lineRenderers.Length;
                var newLineCount = 2 * lineCount;
                var newLineDurations = new float[newLineCount];
                var newLineObjects = new GameObject[newLineCount];
                var newLineRenderers = new LineRenderer[newLineCount];
                System.Array.Copy(lineDurations, newLineDurations, lineCount);
                System.Array.Copy(lineObjects, newLineObjects, lineCount);
                System.Array.Copy(lineRenderers, newLineRenderers, lineCount);

                for (var i = lineCount; i < newLineCount; i++)
                {
                    newLineObjects[i] = Instantiate(linePrefab, linesGroup.transform);
                    newLineRenderers[i] = newLineObjects[i].GetComponent<LineRenderer>();
                }

                lineDurations = newLineDurations;
                lineObjects = newLineObjects;
                lineRenderers = newLineRenderers;
            }

            return lineRenderers[lineIndexEnd];
        }

        private void EnableLineRenderer(LineRenderer lineRenderer, Color color, float lineWidth, float duration)
        {
            lineRenderer.widthMultiplier = lineWidth;
            lineRenderer.endColor = color;
            lineRenderer.startColor = color;
            lineRenderer.enabled = true;
            lineDurations[lineIndexEnd] = duration;
            lineIndexEnd += 1;
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


        private void RemoveLineSegment(int index)
        {
            var lastLineIndex = lineIndexEnd - 1;

            lineRenderers[index].enabled = false;
            lineIndexEnd -= 1;

            if (index == lastLineIndex)
            {
                return;
            }

            lineDurations[index] = lineDurations[lastLineIndex];

            var tempRenderer = lineRenderers[index];
            lineRenderers[index] = lineRenderers[lastLineIndex];
            lineRenderers[lastLineIndex] = tempRenderer;
        }

        private void Update()
        {
            UpdateLines();
        }

        private void UpdateLines()
        {
            var i = 0;

            while (i < lineIndexEnd)
            {
                lineDurations[i] -= Time.deltaTime;

                if (lineDurations[i] <= 0.0f)
                {
                    RemoveLineSegment(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
