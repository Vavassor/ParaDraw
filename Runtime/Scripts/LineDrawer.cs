using UdonSharp;
using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Draw lines in 3D space.
    /// </summary>
    [DefaultExecutionOrder(-1)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class LineDrawer : UdonSharpBehaviour
    {
        public GameObject linePrefab;
        public LineRenderer[] lineRenderers = new LineRenderer[16];
        public GameObject linesGroup;

        private float[] lineDurations = new float[16];
        private int lineIndexEnd;
        private Material[] lineMaterials = new Material[16];
        private GameObject[] lineObjects = new GameObject[16];

        private readonly Vector3[] arrowheadVertices = new Vector3[]
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

        private readonly Vector3[] rectangleVertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, 0.5f, 0.0f),
            new Vector3(-0.5f, 0.5f, 0.0f),

            new Vector3(-0.5f, -0.5f, 0.0f),
        };

        public void DrawEllipse(Vector3 center, Vector3 axisX, Vector3 axisY, Vector2 radii, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawEllipticArc(center, axisX, axisY, radii, 0.0f, 360.0f, 32, color, lineWidth, duration);
        }

        public void DrawEllipticArc(Vector3 origin, Vector3 axisX, Vector3 axisY, Vector2 radii, float startAngle, float endAngle, int segmentCount, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var lineRenderer = AllocateLineRenderer();

            if (lineRenderer == null)
            {
                return;
            }

            var positionCount = segmentCount + 1;
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

        public void DrawRay(Vector3 origin, Vector3 direction, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            var end = origin + direction;
            DrawLine(origin, end, color, lineWidth, duration);

            var arrowheadScale = (0.1f * Mathf.Min(direction.magnitude, 0.2f)) * Vector3.one;
            DrawPolyline(arrowheadVertices, end, Quaternion.LookRotation(direction), arrowheadScale, color, lineWidth, duration);
        }

        public void DrawWireBox(Vector3 center, Quaternion rotation, Vector3 size, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawPolyline(boxVertices, center, rotation, size, color, lineWidth, duration);
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

            DrawEllipse(origin + axisY, axisX, axisY, radii, color, lineWidth, duration);
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

        public void DrawWireRectangle(Vector3 center, Quaternion rotation, Vector2 size, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            DrawPolyline(rectangleVertices, center, rotation, new Vector3(size.x, 1.0f, size.y), color, lineWidth, duration);
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
            if (!gameObject.activeInHierarchy)
            {
                return null;
            }

            if (lineIndexEnd > lineRenderers.Length - 1)
            {
                var lineCount = lineRenderers.Length;
                var newLineCount = 2 * lineCount;
                var newLineDurations = new float[newLineCount];
                var newLineObjects = new GameObject[newLineCount];
                var newLineRenderers = new LineRenderer[newLineCount];
                var newLineMaterials = new Material[newLineCount];
                System.Array.Copy(lineDurations, newLineDurations, lineCount);
                System.Array.Copy(lineObjects, newLineObjects, lineCount);
                System.Array.Copy(lineRenderers, newLineRenderers, lineCount);
                System.Array.Copy(lineMaterials, newLineMaterials, lineCount);

                for (var i = lineCount; i < newLineCount; i++)
                {
                    newLineObjects[i] = Instantiate(linePrefab, linesGroup.transform);
                    newLineRenderers[i] = newLineObjects[i].GetComponent<LineRenderer>();
                    newLineMaterials[i] = newLineRenderers[i].material;
                }

                lineDurations = newLineDurations;
                lineObjects = newLineObjects;
                lineRenderers = newLineRenderers;
                lineMaterials = newLineMaterials;
            }

            return lineRenderers[lineIndexEnd];
        }

        private void DeallocateLineRenderer(int index)
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

            var tempMaterial = lineMaterials[index];
            lineMaterials[index] = lineMaterials[lastLineIndex];
            lineMaterials[lastLineIndex] = tempMaterial;
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

        private void OnDestroy()
        {
            for (var i = 0; i < lineMaterials.Length; i++)
            {
                Destroy(lineMaterials[i]);
            }
        }

        private void Update()
        {
            var i = 0;

            while (i < lineIndexEnd)
            {
                lineDurations[i] -= Time.deltaTime;

                if (lineDurations[i] <= 0.0f)
                {
                    DeallocateLineRenderer(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}