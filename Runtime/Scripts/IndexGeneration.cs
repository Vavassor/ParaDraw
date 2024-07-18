namespace OrchidSeal.ParaDraw
{
    public static class IndexGeneration
    {
        /// <summary>
        /// Add the indices for one row of quads.
        /// </summary>
        public static void AddQuadStrip(int[] triangles, int outTriangleBase, int inTriangleBase, int count)
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
    }
}
