using UnityEngine;

public static class EllipsoidGeneration
{
    /// <summary>
    /// Add indices for a row of triangles that are connected by their top corners. ▽▽▽▽▽
    /// </summary>
    public static void AddSawtoothDownIndices(int[] triangles, int outTriangleBase, int inTriangleBase, int meridians)
    {
        for (var j = 0; j < meridians; j++)
        {
            var o = outTriangleBase + 3 * j;
            var k = inTriangleBase + j;
            triangles[o] = k;
            triangles[o + 1] = k + 1;
            triangles[o + 2] = k + meridians + 1;
        }
    }

    /// <summary>
    /// Add indices for a row of triangles that are connected by their bottom corners. △△△△△
    /// </summary>
    public static void AddSawtoothUpIndices(int[] triangles, int outTriangleBase, int inTriangleBase, int meridians)
    {
        for (var j = 0; j < meridians; j++)
        {
            var o = outTriangleBase + 3 * j;
            var k = inTriangleBase + j;
            triangles[o] = k;
            triangles[o + 1] = k + meridians + 1;
            triangles[o + 2] = k + meridians;
        }
    }

    /// <summary>
    /// Add the bottom point of an ellipsoid. This is multiple vertices to fan out the UVs.
    /// </summary>
    public static void AddBottomPoint(Vector3[] vertices, Vector3[] normals, Vector4[] tangents, Vector2[] uv, int outBase, Vector3 center, Vector3 radii, int meridians, Vector2 texelSize, Vector2 textureOffset)
    {
        for (var j = 0; j < meridians; j++)
        {
            var o = outBase + j;
            var theta = j / ((float)meridians) * 2.0f * Mathf.PI;
            vertices[o] = radii.y * Vector3.down + center;
            normals[o] = Vector3.down;
            tangents[o] = new Vector3(-Mathf.Sin(theta), 0.0f, Mathf.Cos(theta));
            uv[o] = texelSize * new Vector2(j + 0.5f, 0.0f) + textureOffset;
        }
    }

    /// <summary>
    /// Add the indices for one row of quads.
    /// </summary>
    public static void AddQuadStripIndices(int[] triangles, int outTriangleBase, int inTriangleBase, int count)
    {
        for (int j = 0; j < count; ++j)
        {
            var o = 6 * j + outTriangleBase;
            var k = j + inTriangleBase;

            triangles[o + 0] = k;
            triangles[o + 1] = k + 1;
            triangles[o + 2] = k + count + 1;

            triangles[o + 3] = k + count + 1;
            triangles[o + 4] = k + 1;
            triangles[o + 5] = k + count + 2;
        }
    }

    /// <summary>
    /// Add the indices for an ellipsoidal segment.
    /// </summary>
    public static void AddSegmentIndices(int[] triangles, int outTriangleBase, int inTriangleBase, int parallelStart, int parallelEnd, int meridians)
    {
        for (int i = parallelStart; i < parallelEnd; ++i)
        {
            AddQuadStripIndices(triangles, 6 * meridians * i + outTriangleBase, (meridians + 1) * i + inTriangleBase, meridians);
        }
    }

    /// <summary>
    /// Add the vertices for an ellipsoidal segment.
    /// </summary>
    public static void AddSegmentVertices(Vector3[] vertices, Vector3[] normals, Vector4[] tangents, Vector2[] uv, int outBase, Vector3 radii, Vector3 center, int meridians, int parallels, int parallelStart, int parallelCount, Vector2 texelSize, Vector2 textureOffset)
    {
        var rings = parallels + 1;

        for (int i = 0; i <= parallelCount; ++i)
        {
            var row = i + parallelStart + 1;
            var theta = row / ((float)rings) * Mathf.PI;

            for (int j = 0; j <= meridians; ++j)
            {
                var phi = j / ((float)meridians) * 2.0f * Mathf.PI;
                Vector3 position = new Vector3(
                    radii.x * Mathf.Sin(theta) * Mathf.Cos(phi),
                    radii.y * Mathf.Cos(theta),
                    radii.z * Mathf.Sin(theta) * Mathf.Sin(phi)
                );
                var normal = position.normalized;
                var v = (meridians + 1) * i + j + outBase;
                vertices[v] = position + center;
                normals[v] = normal;
                tangents[v] = Vector3.Cross(normal, Vector3.up).normalized;
                uv[v] = texelSize * new Vector2(j, rings - row) + textureOffset;
            }
        }
    }

    /// <summary>
    /// Add the top point of an ellipsoid. This is multiple vertices to fan out the UVs.
    /// </summary>
    public static void AddTopPoint(Vector3[] vertices, Vector3[] normals, Vector4[] tangents, Vector2[] uv, Vector3 center, Vector3 radii, int meridians, int parallels, Vector2 texelSize, Vector2 textureOffset)
    {
        var rings = parallels + 1;

        for (var j = 0; j < meridians; j++)
        {
            var theta = j / ((float)meridians) * 2.0f * Mathf.PI;
            vertices[j] = radii.y * Vector3.up + center;
            normals[j] = Vector3.up;
            tangents[j] = new Vector3(-Mathf.Sin(theta), 0.0f, Mathf.Cos(theta));
            uv[j] = texelSize * new Vector2(j + 0.5f, rings) + textureOffset;
        }
    }

    public static void CreateEllipsoid(Mesh mesh, Vector3 radii, int meridians, int parallels, Vector2 textureScale, Vector2 textureOffset)
    {
        mesh.Clear();

        // Triangular tegum (3 meridians, 1 parallel) is the minimum shape.
        // 100 max is arbitrary, mainly to prevent slow generation times.
        meridians = Mathf.Clamp(meridians, 3, 100);
        parallels = Mathf.Clamp(parallels, 1, 100);

        var meridiansPlusSeam = meridians + 1;
        var rings = parallels + 1;
        var vertexCount = meridiansPlusSeam * parallels + 2 * meridians;
        var indexCount = 6 * meridians * parallels;

        var vertices = new Vector3[vertexCount];
        var normals = new Vector3[vertexCount];
        var tangents = new Vector4[vertexCount];
        var uv = new Vector2[vertexCount];
        var triangles = new int[indexCount];

        var texelSize = new Vector2(textureScale.x / meridians, textureScale.y / rings);

        AddTopPoint(vertices, normals, tangents, uv, Vector3.zero, radii, meridians, parallels, texelSize, textureOffset);
        var outBase = meridians;

        AddSegmentVertices(vertices, normals, tangents, uv, outBase, radii, Vector3.zero, meridians, parallels, 0, parallels - 1, texelSize, textureOffset);
        outBase += meridiansPlusSeam * parallels;

        AddBottomPoint(vertices, normals, tangents, uv, outBase, Vector3.zero, radii, meridians, texelSize, textureOffset);

        AddSawtoothUpIndices(triangles, 0, 0, meridians);
        var outTriangleBase = 3 * meridians;
        var inTriangleBase = meridians;

        AddSegmentIndices(triangles, outTriangleBase, inTriangleBase, 0, parallels - 1, meridians);
        outTriangleBase += 6 * meridians * (parallels - 1);
        inTriangleBase += meridiansPlusSeam * (parallels - 1);

        AddSawtoothDownIndices(triangles, outTriangleBase, inTriangleBase, meridians);

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.tangents = tangents;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
