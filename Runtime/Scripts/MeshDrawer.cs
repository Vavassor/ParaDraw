using UdonSharp;
using UnityEngine;

#if UNITY_ANDROID
using VRC.SDK3.Data;
#endif // UNITY_ANDROID

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Draw meshes in 3D space.
    /// </summary>
    [DefaultExecutionOrder(-1)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MeshDrawer : UdonSharpBehaviour
    {
        public GameObject[] meshObjects = new GameObject[4];
        public GameObject meshPrefab;
        public GameObject meshesGroup;
        public Material androidWireMaterial;
        public Material solidOpaqueMaterial;
        public Material solidTransparentMaterial;
        public Material wireMaterial;

        [Header("Base Meshes")]
        public Mesh boxMesh;
        public Mesh capsuleCapMesh;
        public Mesh capsuleCylinderMesh;
        public Mesh rectangleMesh;
        public Mesh sphereMesh;

#if UNITY_ANDROID
        private int cachedMeshIndex;
        private readonly Mesh[] cachedMeshes = new Mesh[16];
        private readonly DataDictionary meshCacheIndicesById = new DataDictionary();
#endif // UNITY_ANDROID

        private float[] meshDurations = new float[4];
        private int meshIndexEnd;
        private MeshFilter[] meshFilters = new MeshFilter[4];
        private MeshRenderer[] meshRenderers = new MeshRenderer[4];
        private MaterialPropertyBlock[] propertyBlocks = new MaterialPropertyBlock[4];

        public void DrawSolidBox(Vector3 position, Quaternion rotation, Vector3 size, Color color, float duration = 0.0f)
        {
            DrawSolidMesh(boxMesh, position, rotation, size, color, duration);
        }

        public void DrawSolidCapsule(Vector3 center, Quaternion rotation, float height, float radius, Color color, float duration = 0.0f)
        {
            var extentY = 0.5f * height * (rotation * Vector3.up);
            var scale = radius * Vector3.one;
            DrawSolidMesh(capsuleCapMesh, center + extentY, rotation, scale, color, duration);
            DrawSolidMesh(capsuleCylinderMesh, center, rotation, new Vector3(radius, 0.5f * height, radius), color, duration);
            DrawSolidMesh(capsuleCapMesh, center - extentY, rotation * Quaternion.Euler(180.0f, 0.0f, 0.0f), scale, color, duration);
        }

        public void DrawSolidMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration = 0.0f)
        {
            AllocateMesh(out MeshFilter meshFilter, out MeshRenderer meshRenderer, out MaterialPropertyBlock propertyBlock);

            if (!meshFilter)
            {
                return;
            }

            meshFilter.sharedMesh = mesh;
            propertyBlock.SetColor("_SurfaceColor", color);
            var material = color.a >= 0.999f ? solidOpaqueMaterial : solidTransparentMaterial;
            EnableMesh(meshRenderer, propertyBlock, position, rotation, scale, material, duration);
        }

        public void DrawSolidRectangle(Vector3 position, Quaternion rotation, Vector2 size, Color color, float duration = 0.0f)
        {
            DrawSolidMesh(rectangleMesh, position, rotation, new Vector3(size.x, size.y, 1.0f), color, duration);
        }

        public void DrawSolidSphere(Vector3 center, float radius, Color color, float duration = 0.0f)
        {
            DrawSolidMesh(sphereMesh, center, Quaternion.identity, radius * Vector3.one, color, duration);
        }

        public void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float lineWidth = 0.005f, float duration = 0.0f, bool shouldCache = true)
        {
            AllocateMesh(out MeshFilter meshFilter, out MeshRenderer meshRenderer, out MaterialPropertyBlock propertyBlock);

            if (!meshFilter)
            {
                return;
            }

            var meshObject = meshObjects[meshIndexEnd];

#if UNITY_ANDROID
            // Quest 2 headset doesn't support geometry shaders. So render wireframes based on 
            // barycentric coordinates. This requires a mesh with split triangles. Splitting is
            // intensive, so split the mesh the first time and cache it for later.
            if (!mesh.isReadable)
            {
                // Using the original non-split mesh will appear as a broken wireframe. But at least
                // it'll be visible.
                meshFilter.mesh = mesh;
            }
            else if (shouldCache)
            {
                meshFilter.mesh = GetWireMeshCached(mesh);
            }
            else
            {
                meshFilter.mesh = GetWireMeshUncached(mesh);
            }

            if (meshRenderer.sharedMaterial != androidWireMaterial)
            {
                var materials = meshRenderer.materials;

                for (var i = 0; i < materials.Length; i++)
                {
                    materials[i] = androidWireMaterial;
                }

                meshRenderer.materials = materials;
            }
#else
            meshFilter.mesh = mesh;
#endif // UNITY_ANDROID

            propertyBlock.SetColor("_WireColor", color);
            propertyBlock.SetFloat("_WireThickness", lineWidth * 20000.0f);
            EnableMesh(meshRenderer, propertyBlock, position, rotation, scale, wireMaterial, duration);
        }

        private void EnableMesh(MeshRenderer meshRenderer, MaterialPropertyBlock propertyBlock, Vector3 position, Quaternion rotation, Vector3 scale, Material material, float duration)
        {
            var materials = meshRenderer.materials;

            for (var i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }

            meshRenderer.materials = materials;

            meshRenderer.SetPropertyBlock(propertyBlock);
            meshRenderer.enabled = true;
            meshObjects[meshIndexEnd].transform.SetPositionAndRotation(position, rotation);
            meshObjects[meshIndexEnd].transform.localScale = scale;
            meshDurations[meshIndexEnd] = duration;
            meshIndexEnd += 1;
        }

        private void AllocateMesh(out MeshFilter meshFilter, out MeshRenderer meshRenderer, out MaterialPropertyBlock propertyBlock)
        {
            if (!gameObject.activeInHierarchy)
            {
                meshFilter = null;
                meshRenderer = null;
                propertyBlock = null;
                return;
            }

            if (meshIndexEnd > meshFilters.Length - 1)
            {
                var meshCount = meshFilters.Length;
                var newMeshCount = 2 * meshCount;
                var newMeshDurations = new float[newMeshCount];
                var newMeshObjects = new GameObject[newMeshCount];
                var newMeshFilters = new MeshFilter[newMeshCount];
                var newMeshRenderers = new MeshRenderer[newMeshCount];
                var newPropertyBlocks = new MaterialPropertyBlock[newMeshCount];
                System.Array.Copy(meshDurations, newMeshDurations, meshCount);
                System.Array.Copy(meshObjects, newMeshObjects, meshCount);
                System.Array.Copy(meshFilters, newMeshFilters, meshCount);
                System.Array.Copy(meshRenderers, newMeshRenderers, meshCount);
                System.Array.Copy(propertyBlocks, newPropertyBlocks, meshCount);

                for (var i = meshCount; i < newMeshCount; i++)
                {
                    newMeshObjects[i] = Instantiate(meshPrefab, meshesGroup.transform);
                    newMeshFilters[i] = newMeshObjects[i].GetComponentInChildren<MeshFilter>();
                    newMeshRenderers[i] = newMeshObjects[i].GetComponentInChildren<MeshRenderer>();
                    newPropertyBlocks[i] = new MaterialPropertyBlock();
                }

                meshDurations = newMeshDurations;
                meshObjects = newMeshObjects;
                meshFilters = newMeshFilters;
                meshRenderers = newMeshRenderers;
                propertyBlocks = newPropertyBlocks;
            }

            meshFilter = meshFilters[meshIndexEnd];
            meshRenderer = meshRenderers[meshIndexEnd];
            propertyBlock = propertyBlocks[meshIndexEnd];
        }

        private void DeallocateMesh(int index)
        {
            var lastMeshIndex = meshIndexEnd - 1;

            meshRenderers[index].enabled = false;
            meshIndexEnd -= 1;

            if (index == lastMeshIndex)
            {
                return;
            }

            meshDurations[index] = meshDurations[lastMeshIndex];

            var tempMesh = meshRenderers[index];
            meshRenderers[index] = meshRenderers[lastMeshIndex];
            meshRenderers[lastMeshIndex] = tempMesh;

            var tempMeshFilter = meshFilters[index];
            meshFilters[index] = meshFilters[lastMeshIndex];
            meshFilters[lastMeshIndex] = tempMeshFilter;

            var tempMeshObject = meshObjects[index];
            meshObjects[index] = meshObjects[lastMeshIndex];
            meshObjects[lastMeshIndex] = tempMeshObject;

            var tempBlock = propertyBlocks[index];
            propertyBlocks[index] = propertyBlocks[lastMeshIndex];
            propertyBlocks[lastMeshIndex] = tempBlock;
        }

#if UNITY_ANDROID
        private Mesh GetWireMeshUncached(Mesh sourceMesh)
        {
            var triangles = sourceMesh.triangles;
            var vertexCount = triangles.Length;
            var vertices = sourceMesh.vertices;

            // Split all triangles in the mesh.
            var newVertices = new Vector3[vertexCount];
            for (var i = 0; i < vertexCount; i++)
            {
                newVertices[i] = vertices[triangles[i]];
            }

            var newTriangles = new int[vertexCount];
            for (var i = 0; i < vertexCount; i++)
            {
                newTriangles[i] = i;
            }

            Mesh wireMesh = new Mesh();

            // 65536 is 2^16. So 65535 is the largest number that fits in a 16-bit index.
            if (vertexCount >= 65536)
            {
                wireMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }
            
            wireMesh.vertices = newVertices;
            wireMesh.triangles = newTriangles;

            return wireMesh;
        }

        private Mesh GetWireMeshCached(Mesh sourceMesh)
        {
            var sourceMeshId = sourceMesh.GetInstanceID();

            if (meshCacheIndicesById.TryGetValue(sourceMeshId, out DataToken cacheIndex))
            {
                return cachedMeshes[cacheIndex.Int];
            }

            var wireMesh = GetWireMeshUncached(sourceMesh);

            var removedMesh = cachedMeshes[cachedMeshIndex];
            if (removedMesh)
            {
                meshCacheIndicesById.Remove(removedMesh.GetInstanceID());
            }

            meshCacheIndicesById.Add(sourceMeshId, cachedMeshIndex);
            cachedMeshes[cachedMeshIndex] = wireMesh;
            cachedMeshIndex = (cachedMeshIndex + 1) % cachedMeshes.Length;

            return wireMesh;
        }
#endif // UNITY_ANDROID

        private void Start()
        {
            for (var i = 0; i < meshObjects.Length; i++)
            {
                meshFilters[i] = meshObjects[i].GetComponentInChildren<MeshFilter>();
                meshRenderers[i] = meshObjects[i].GetComponentInChildren<MeshRenderer>();
                propertyBlocks[i] = new MaterialPropertyBlock();
            }
        }

        private void Update()
        {
            var i = 0;

            while (i < meshIndexEnd)
            {
                meshDurations[i] -= Time.deltaTime;

                if (meshDurations[i] <= 0.0f)
                {
                    DeallocateMesh(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
