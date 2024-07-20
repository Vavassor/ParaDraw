using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    public static class DiskGeneration
    {
        public static void CreateEllipticRingSection(Mesh mesh, Vector2 outerRadii, float ringWidth, float startAngle, float endAngle, int segments)
        {
            mesh.Clear();

            var dividers = segments + 1;
            var oneSideVertexCount = 2 * dividers;
            var vertexCount = 2 * oneSideVertexCount;
            var indexCount = 12 * segments;

            var vertices = new Vector3[vertexCount];
            var normals = new Vector3[vertexCount];
            var tangents = new Vector4[vertexCount];
            var uv = new Vector2[vertexCount];
            var triangles = new int[indexCount];

            var step = Mathf.Deg2Rad * (endAngle - startAngle) / segments;
            var stepStart = Mathf.Deg2Rad * startAngle;
            var stepU = 1.0f / segments;

            for (var i = 0; i <= segments; i++)
            {
                var angle = step * i + stepStart;
                var outerPosition = outerRadii * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                var innerPosition = outerPosition * (outerPosition.magnitude - ringWidth);
                var tangent = Vector3.Cross(Vector3.back, outerPosition).normalized;

                var o = i + oneSideVertexCount;
                vertices[i] = outerPosition;
                vertices[i + dividers] = innerPosition;
                vertices[o] = innerPosition;
                vertices[o + dividers] = outerPosition;
                tangents[i] = tangent;
                tangents[i + dividers] = tangent;
                tangents[o] = tangent;
                tangents[o + dividers] = tangent;
            }

            for (var i = 0; i < oneSideVertexCount; i++)
            {
                normals[i] = Vector3.back;
            }

            for (var i = oneSideVertexCount; i < vertexCount; i++)
            {
                normals[i] = Vector3.forward;
            }

            for (var i = 0; i <= segments; i++)
            {
                var u = 1.0f - stepU * i;
                var t0 = new Vector2(u, 0.0f);
                var t1 = new Vector2(u, 1.0f);
                uv[i] = t0;
                uv[i + dividers] = t1;
                uv[i + oneSideVertexCount] = t1;
                uv[i + oneSideVertexCount + dividers] = t0;
            }

            IndexGeneration.AddQuadStrip(triangles, 0, 0, segments);
            IndexGeneration.AddQuadStrip(triangles, 6 * segments, oneSideVertexCount, segments);

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.tangents = tangents;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
    }
}
