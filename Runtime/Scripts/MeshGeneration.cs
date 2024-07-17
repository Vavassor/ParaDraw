using UnityEngine;

public static class MeshGeneration
{
    public static void CreateEllipsoid(Mesh mesh, Vector3 radii, int meridians, int parallels, Vector2 textureScale, Vector2 textureOffset)
    {
        mesh.Clear();

        // Triangular tegum (3 meridians, 1 parallel) is the minimum shape.
        // 200 max is arbitrary, mainly to prevent slow generation times.
        meridians = Mathf.Clamp(meridians, 3, 200);
        parallels = Mathf.Clamp(parallels, 1, 200);

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

        // Top cap vertices

        for (var j = 0; j < meridians; j++)
        {
            vertices[j] = radii.y * Vector3.up;
            normals[j] = Vector3.up;
            tangents[j] = Vector3.right;
            uv[j] = texelSize * new Vector2(j + 0.5f, rings) + textureOffset;
        }

        // Main surface vertices

        var outBase = meridians;

        for (int i = 0; i < parallels; ++i)
        {
            float step = (i + 1) / ((float)rings);
            float theta = step * Mathf.PI;

            for (int j = 0; j < meridiansPlusSeam; ++j)
            {
                float phi = j / ((float)meridians) * 2.0f * Mathf.PI;
                Vector3 position = new Vector3(
                    radii.x * Mathf.Sin(theta) * Mathf.Cos(phi),
                    radii.y * Mathf.Cos(theta),
                    radii.z * Mathf.Sin(theta) * Mathf.Sin(phi)
                );
                var normal = position.normalized;
                var v = meridiansPlusSeam * i + j + outBase;
                vertices[v] = position;
                normals[v] = normal;
                tangents[v] = Vector3.Cross(normal, Vector3.up).normalized;
                uv[v] = texelSize * new Vector2(j, rings - (i + 1)) + textureOffset;
            }
        }

        // Bottom cap vertices

        outBase += meridiansPlusSeam * parallels;

        for (var j = 0; j < meridians; j++)
        {
            var o = outBase + j;
            vertices[o] = radii.y * Vector3.down;
            normals[o] = Vector3.down;
            tangents[o] = Vector3.left;
            uv[o] = texelSize * new Vector2(j + 0.5f, 0.0f) + textureOffset;
        }

        // Top cap indices

        for (var j = 0; j < meridians; j++)
        {
            triangles[3 * j] = j;
            triangles[3 * j + 1] = meridians + j + 1;
            triangles[3 * j + 2] = meridians + j;
        }

        // Main surface indicies

        var outTriangleBase = 3 * meridians;
        var inTriangleBase = meridians;

        for (int i = 0; i < parallels - 1; ++i)
        {
            for (int j = 0; j < meridians; ++j)
            {
                var o = 6 * (meridians * i + j) + outTriangleBase;
                var k = meridiansPlusSeam * i + j + inTriangleBase;

                triangles[o + 0] = k;
                triangles[o + 1] = k + 1;
                triangles[o + 2] = k + meridiansPlusSeam;

                triangles[o + 3] = k + meridiansPlusSeam;
                triangles[o + 4] = k + 1;
                triangles[o + 5] = k + meridiansPlusSeam + 1;
            }
        }

        // Bottom cap indices

        outTriangleBase += 6 * meridians * (parallels - 1);
        inTriangleBase += meridiansPlusSeam * (parallels - 1);

        for (var j = 0; j < meridians; j++)
        {
            var o = outTriangleBase + 3 * j;
            triangles[o] = inTriangleBase + j;
            triangles[o + 1] = inTriangleBase + j + 1;
            triangles[o + 2] = inTriangleBase + j + meridiansPlusSeam;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.tangents = tangents;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    public static void CreateGrid(Mesh mesh, Vector2 size, int columns, int rows, Vector2 textureScale, Vector2 textureOffset)
    {
        mesh.Clear();

        var verticesPerRow = columns + 1;
        var verticesPerColumn = rows + 1;
        var vertexCount = verticesPerRow * verticesPerColumn;
        var tileCount = columns * rows;
        var indexCount = 6 * tileCount;

        var vertices = new Vector3[vertexCount];
        var normals = new Vector3[vertexCount];
        var tangents = new Vector4[vertexCount];
        var uv = new Vector2[vertexCount];
        var triangles = new int[indexCount];

        var tileSize = new Vector2(size.x / columns, size.y / rows);
        var tileOffset = -0.5f * size;
        var texelSize = new Vector2(textureScale.x / columns, textureScale.y / rows);

        for (var y = 0; y < verticesPerColumn; y++)
        {
            for (var x = 0; x < verticesPerRow; x++)
            {
                var i = verticesPerRow * y + x;
                vertices[i] = new Vector3(tileSize.x * x + tileOffset.x, tileSize.y * y + tileOffset.y, 0.0f);
                uv[i] = texelSize * new Vector2(x, y) + textureOffset;
                normals[i] = Vector3.back;
                tangents[i] = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);
            }
        }

        for (int i = 0; i < tileCount; ++i)
        {
            int o = 6 * i;
            int k = i + i / columns;

            triangles[o + 0] = k + verticesPerRow;
            triangles[o + 1] = k + verticesPerRow + 1;
            triangles[o + 2] = k;

            triangles[o + 3] = k;
            triangles[o + 4] = k + verticesPerRow + 1;
            triangles[o + 5] = k + 1;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.tangents = tangents;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
