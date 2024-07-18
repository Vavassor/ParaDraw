using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    public static class CapsuleGeneration
    {
        public static void CreateCapsuleYAxis(Mesh mesh, float radius, float height, int meridians, int capParallels, Vector2 textureScale, Vector2 textureOffset)
        {
            mesh.Clear();

            meridians = Mathf.Clamp(meridians, 3, 100);
            capParallels = Mathf.Clamp(capParallels, 0, 100);

            var meridiansPlusSeam = meridians + 1;
            var capParallelsPlusSeam = capParallels + 1;
            var bothCapRings = 2 * capParallelsPlusSeam;
            var vertexCount = 2 * meridiansPlusSeam * capParallelsPlusSeam + 2 * meridiansPlusSeam + 2 * meridians;
            var indexCount = 12 * meridians * capParallels + 12 * meridians;

            var vertices = new Vector3[vertexCount];
            var normals = new Vector3[vertexCount];
            var tangents = new Vector4[vertexCount];
            var uv = new Vector2[vertexCount];
            var triangles = new int[indexCount];

            var radii = radius * Vector3.one;
            var extentY = new Vector3(0.0f, 0.5f * height, 0.0f);
            var texelSize = new Vector2(textureScale.x / meridians, textureScale.y / bothCapRings);

            // Top cap vertices

            EllipsoidGeneration.AddTopPoint(vertices, normals, tangents, uv, extentY, radii, meridians, bothCapRings - 1, texelSize, textureOffset);
            var outBase = meridians;

            EllipsoidGeneration.AddSegmentVertices(vertices, normals, tangents, uv, outBase, radii, extentY, meridians, bothCapRings - 1, 0, capParallels, texelSize, textureOffset);
            outBase += meridiansPlusSeam * capParallelsPlusSeam;

            // Center segment vertices

            for (var j = 0; j < meridiansPlusSeam; j++)
            {
                var o1 = outBase + j;
                var o2 = o1 + meridiansPlusSeam;
                float fraction = j / ((float)meridians);
                float phi = fraction * 2.0f * Mathf.PI;
                var position = new Vector3(radius * Mathf.Cos(phi), 0.0f, radius * Mathf.Sin(phi));
                var normal = position.normalized;
                vertices[o1] = position + extentY;
                vertices[o2] = position - extentY;
                normals[o1] = normal;
                normals[o2] = normal;
                tangents[o1] = Vector3.right;
                tangents[o2] = Vector3.right;
                uv[o1] = textureScale * new Vector2(fraction, 1.0f) + textureOffset;
                uv[o2] = textureScale * new Vector2(fraction, 0.0f) + textureOffset;
            }

            outBase += 2 * meridiansPlusSeam;

            // Bottom cap vertices

            EllipsoidGeneration.AddSegmentVertices(vertices, normals, tangents, uv, outBase, radii, -extentY, meridians, bothCapRings - 1, capParallels, capParallels, texelSize, textureOffset);
            outBase += meridiansPlusSeam * capParallelsPlusSeam;

            EllipsoidGeneration.AddBottomPoint(vertices, normals, tangents, uv, outBase, -extentY, radii, meridians, texelSize, textureOffset);

            // Top cap indices

            EllipsoidGeneration.AddSawtoothUpIndices(triangles, 0, 0, meridians);
            var outTriangleBase = 3 * meridians;
            var inTriangleBase = meridians;

            EllipsoidGeneration.AddSegmentIndices(triangles, outTriangleBase, inTriangleBase, 0, capParallels, meridians);
            outTriangleBase += 6 * meridians * capParallels;
            inTriangleBase += meridiansPlusSeam * capParallelsPlusSeam;

            // Center segment indices

            EllipsoidGeneration.AddQuadStripIndices(triangles, outTriangleBase, inTriangleBase, meridians);
            outTriangleBase += 6 * meridians;
            inTriangleBase += 2 * meridiansPlusSeam;

            // Bottom cap indices

            EllipsoidGeneration.AddSegmentIndices(triangles, outTriangleBase, inTriangleBase, 0, capParallels, meridians);
            outTriangleBase += 6 * meridians * capParallels;
            inTriangleBase += meridiansPlusSeam * capParallels;

            EllipsoidGeneration.AddSawtoothDownIndices(triangles, outTriangleBase, inTriangleBase, meridians);

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.tangents = tangents;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
    }
}
