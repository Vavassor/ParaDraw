using UnityEngine;
using VRC.SDKBase;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Draw lines in 3D space.
    /// </summary>
    public partial class ShapeDrawer
    {
        [Header("Line")]
        public GameObject linePrefab;
        public LineRenderer[] lineRenderers = new LineRenderer[16];
        public GameObject linesGroup;

        private VRCPlayerApi _localPlayer;
        
        private float[] lineDurations = new float[16];
        private int lineIndexEnd;
        private Material[] lineMaterials = new Material[16];
        private GameObject[] lineObjects = new GameObject[16];

        private readonly Vector3[] arrowheadVertices = new Vector3[]
        {
            new Vector3(0.0f, 0.0f, -1.4142f),
            new Vector3(-1.0f, 0.0f, -1.4142f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 0.0f, -1.4142f),
            new Vector3(0.0f, 0.0f, -1.4142f),
        };

        private void StartLineDrawer()
        {
            _localPlayer = Networking.LocalPlayer;
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

        /// <summary>
        /// Draws a polyline between the given points. A polyline is a connected series of line segments.
        /// </summary>
        /// <param name="vertices">The points in the polyline.</param>
        /// <param name="color">The line color.</param>
        /// <param name="lineWidth">The line width.</param>
        /// <param name="duration">The number of seconds the line should be visible for.</param>
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
            var arrowheadScale = 0.1f * Mathf.Min(direction.magnitude, 0.2f);

            var end = origin + direction;
            DrawLine(origin, end - direction.normalized * (arrowheadScale * 1.4142f), color, lineWidth, duration);

            var headData = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            var headPos = headData.position;
            var right = Vector3.Cross(direction, headPos - end).normalized;
            var forward = Vector3.Cross(right, direction).normalized;
            DrawPolyline(arrowheadVertices, end, Quaternion.LookRotation(direction, forward), arrowheadScale * Vector3.one, color, lineWidth, duration);
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

        private void OnDestroyLineDrawer()
        {
            for (var i = 0; i < lineMaterials.Length; i++)
            {
                Destroy(lineMaterials[i]);
            }
        }

        private void UpdateLineDrawer()
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