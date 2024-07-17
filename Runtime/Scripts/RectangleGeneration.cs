using UnityEngine;

public static class RectangleGeneration
{
    public static void CreateRectangle(Mesh mesh, Vector2 size, Vector2 textureScale, Vector2 textureOffset)
    {
        mesh.Clear();

        var extents = 0.5f * size;

        mesh.vertices = new Vector3[]
        {
            new Vector3(-extents.x, -extents.y, 0.0f),
            new Vector3(extents.x, -extents.y, 0.0f),
            new Vector3(-extents.x, extents.y, 0.0f),
            new Vector3(extents.x, extents.y, 0.0f)
        };

        mesh.normals = new Vector3[]
        {
            Vector3.back, Vector3.back, Vector3.back, Vector3.back
        };

        mesh.tangents = new Vector4[]
        {
            new Vector4(1.0f, 0.0f, 0.0f, -1.0f),
            new Vector4(1.0f, 0.0f, 0.0f, -1.0f),
            new Vector4(1.0f, 0.0f, 0.0f, -1.0f),
            new Vector4(1.0f, 0.0f, 0.0f, -1.0f)
        };

        mesh.uv = new Vector2[]
        {
            Vector2.zero + textureOffset,
            textureScale * Vector2.right + textureOffset,
            textureScale * Vector2.up + textureOffset,
            textureScale * Vector2.one + textureOffset
        };

        mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
    }
}
