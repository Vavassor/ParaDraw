using UnityEngine;

public static class CapsuleGeneration
{
    private static void AddHemisphereIndices(int[] triangles, int outTriangleBase, int inTriangleBase, int capParallels, int meridians)
    {
        for (int i = 0; i < capParallels; ++i)
        {
            AddQuadRowIndices(triangles, 6 * meridians * i + outTriangleBase, (meridians + 1) * i + inTriangleBase, meridians);
        }
    }

    private static void AddHemisphereVertices(Vector3[] vertices, Vector3[] normals, Vector4[] tangents, Vector2[] uv, int outBase, int stepBase, float radius, Vector3 extentY, int meridians, int capParallels, Vector2 texelSize, Vector2 textureOffset)
    {
        var bothCapRings = 2 * (capParallels + 1);

        for (int i = 0; i <= capParallels; ++i)
        {
            var row = i + stepBase + 1;
            var theta = row / ((float)bothCapRings) * Mathf.PI;

            for (int j = 0; j <= meridians; ++j)
            {
                var phi = j / ((float)meridians) * 2.0f * Mathf.PI;
                Vector3 position = new Vector3(
                    radius * Mathf.Sin(theta) * Mathf.Cos(phi),
                    radius * Mathf.Cos(theta),
                    radius * Mathf.Sin(theta) * Mathf.Sin(phi)
                );
                var normal = position.normalized;
                var v = (meridians + 1) * i + j + outBase;
                vertices[v] = position + extentY;
                normals[v] = normal;
                tangents[v] = Vector3.Cross(normal, Vector3.up).normalized;
                uv[v] = texelSize * new Vector2(j, bothCapRings - row) + textureOffset;
            }
        }
    }

    private static void AddQuadRowIndices(int[] triangles, int outTriangleBase, int inTriangleBase, int meridians)
    {
        for (int j = 0; j < meridians; ++j)
        {
            var o = 6 * j + outTriangleBase;
            var k = j + inTriangleBase;

            triangles[o + 0] = k;
            triangles[o + 1] = k + 1;
            triangles[o + 2] = k + meridians + 1;

            triangles[o + 3] = k + meridians + 1;
            triangles[o + 4] = k + 1;
            triangles[o + 5] = k + meridians + 2;
        }
    }

    public static void CreateCapsuleYAxis(Mesh mesh, float radius, float height, int meridians, int capParallels, Vector2 textureScale, Vector2 textureOffset)
    {
        mesh.Clear();

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

        var extentY = new Vector3(0.0f, 0.5f * height, 0.0f);
        var texelSize = new Vector2(textureScale.x / meridians, textureScale.y / bothCapRings);

        // Top circle vertices

        for (var j = 0; j < meridians; j++)
        {
            var theta = j / ((float)meridians) * 2.0f * Mathf.PI;
            vertices[j] = radius * Vector3.up + extentY;
            normals[j] = Vector3.up;
            tangents[j] = new Vector3(-Mathf.Sin(theta), 0.0f, Mathf.Cos(theta));
            uv[j] = texelSize * new Vector2(j + 0.5f, bothCapRings) + textureOffset;
        }

        var outBase = meridians;

        // Top cap vertices

        AddHemisphereVertices(vertices, normals, tangents, uv, outBase, 0, radius, extentY, meridians, capParallels, texelSize, textureOffset);
        outBase += meridiansPlusSeam * capParallelsPlusSeam;

        // Center surface vertices

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

        AddHemisphereVertices(vertices, normals, tangents, uv, outBase, capParallels, radius, -extentY, meridians, capParallels, texelSize, textureOffset);
        outBase += meridiansPlusSeam * capParallelsPlusSeam;

        // Bottom circle vertices

        for (var j = 0; j < meridians; j++)
        {
            var o = outBase + j;
            var theta = j / ((float)meridians) * 2.0f * Mathf.PI;
            vertices[o] = radius * Vector3.down - extentY;
            normals[o] = Vector3.down;
            tangents[o] = new Vector3(-Mathf.Sin(theta), 0.0f, Mathf.Cos(theta));
            uv[o] = texelSize * new Vector2(j + 0.5f, 0.0f) + textureOffset;
        }

        // Top circle indices

        for (var j = 0; j < meridians; j++)
        {
            triangles[3 * j] = j;
            triangles[3 * j + 1] = meridians + j + 1;
            triangles[3 * j + 2] = meridians + j;
        }

        var outTriangleBase = 3 * meridians;
        var inTriangleBase = meridians;

        // Top cap indices

        AddHemisphereIndices(triangles, outTriangleBase, inTriangleBase, capParallels, meridians);
        outTriangleBase += 6 * meridians * capParallels;
        inTriangleBase += meridiansPlusSeam * capParallelsPlusSeam;

        // Center surface indices

        AddQuadRowIndices(triangles, outTriangleBase, inTriangleBase, meridians);
        outTriangleBase += 6 * meridians;
        inTriangleBase += 2 * meridiansPlusSeam;

        // Bottom cap indices

        AddHemisphereIndices(triangles, outTriangleBase, inTriangleBase, capParallels, meridians);
        outTriangleBase += 6 * meridians * capParallels;
        inTriangleBase += meridiansPlusSeam * capParallels;

        // Bottom circle indices

        for (var j = 0; j < meridians; j++)
        {
            triangles[outTriangleBase + 3 * j] = inTriangleBase + j;
            triangles[outTriangleBase + 3 * j + 1] = inTriangleBase + j + 1;
            triangles[outTriangleBase + 3 * j + 2] = inTriangleBase + j + meridiansPlusSeam;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.tangents = tangents;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
