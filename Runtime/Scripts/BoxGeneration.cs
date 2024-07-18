using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    public static class BoxGeneration
    {
        public static void CreateBox(Mesh mesh, Vector3 size, Vector2 textureScale, Vector2 textureOffset)
        {
            mesh.Clear();

            var extents = 0.5f * size;

            mesh.vertices = new Vector3[]
            {
                new Vector3(-extents.x, -extents.y, -extents.z),
                new Vector3(extents.x, -extents.y, -extents.z),
                new Vector3(-extents.x, extents.y, -extents.z),
                new Vector3(extents.x, extents.y, -extents.z),

                new Vector3(extents.x, -extents.y, extents.z),
                new Vector3(-extents.x, -extents.y, extents.z),
                new Vector3(extents.x, extents.y, extents.z),
                new Vector3(-extents.x, extents.y, extents.z),

                new Vector3(-extents.x, -extents.y, extents.z),
                new Vector3(-extents.x, -extents.y, -extents.z),
                new Vector3(-extents.x, extents.y, extents.z),
                new Vector3(-extents.x, extents.y, -extents.z),

                new Vector3(extents.x, -extents.y, -extents.z),
                new Vector3(extents.x, -extents.y, extents.z),
                new Vector3(extents.x, extents.y, -extents.z),
                new Vector3(extents.x, extents.y, extents.z),

                new Vector3(-extents.x, extents.y, -extents.z),
                new Vector3(extents.x, extents.y, -extents.z),
                new Vector3(-extents.x, extents.y, extents.z),
                new Vector3(extents.x, extents.y, extents.z),

                new Vector3(-extents.x, -extents.y, extents.z),
                new Vector3(extents.x, -extents.y, extents.z),
                new Vector3(-extents.x, -extents.y, -extents.z),
                new Vector3(extents.x, -extents.y, -extents.z),
            };

            mesh.normals = new Vector3[]
            {
                Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
                Vector3.back, Vector3.back, Vector3.back, Vector3.back,
                Vector3.left, Vector3.left, Vector3.left, Vector3.left,
                Vector3.right, Vector3.right, Vector3.right, Vector3.right,
                Vector3.up, Vector3.up, Vector3.up, Vector3.up,
                Vector3.down, Vector3.down, Vector3.down, Vector3.down,
            };

            mesh.tangents = new Vector4[]
            {
                Vector3.right, Vector3.right, Vector3.right, Vector3.right,
                Vector3.left, Vector3.left, Vector3.left, Vector3.left,
                Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
                Vector3.back, Vector3.back, Vector3.back, Vector3.back,
                Vector3.right, Vector3.right, Vector3.right, Vector3.right,
                Vector3.right, Vector3.right, Vector3.right, Vector3.right,
            };

            mesh.uv = new Vector2[]
            {
                Vector2.zero + textureOffset,
                textureScale * Vector2.right + textureOffset,
                textureScale * Vector2.up + textureOffset,
                textureScale * Vector2.one + textureOffset,

                Vector2.zero + textureOffset,
                textureScale * Vector2.right + textureOffset,
                textureScale * Vector2.up + textureOffset,
                textureScale * Vector2.one + textureOffset,

                Vector2.zero + textureOffset,
                textureScale * Vector2.right + textureOffset,
                textureScale * Vector2.up + textureOffset,
                textureScale * Vector2.one + textureOffset,

                Vector2.zero + textureOffset,
                textureScale * Vector2.right + textureOffset,
                textureScale * Vector2.up + textureOffset,
                textureScale * Vector2.one + textureOffset,

                Vector2.zero + textureOffset,
                textureScale * Vector2.right + textureOffset,
                textureScale * Vector2.up + textureOffset,
                textureScale * Vector2.one + textureOffset,

                Vector2.zero + textureOffset,
                textureScale * Vector2.right + textureOffset,
                textureScale * Vector2.up + textureOffset,
                textureScale * Vector2.one + textureOffset,
            };

            mesh.triangles = new int[]
            {
                0, 3, 1, 0, 2, 3,
                4, 7, 5, 4, 6, 7,
                8, 11, 9, 8, 10, 11,
                12, 15, 13, 12, 14, 15,
                16, 19, 17, 16, 18, 19,
                20, 23, 21, 20, 22, 23,
            };
        }
    }
}
