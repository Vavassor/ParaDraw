using UdonSharp;
using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Draw meshes in 3D space.
    /// </summary>
    [DefaultExecutionOrder(-1)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MeshDrawer : UdonSharpBehaviour
    {
        public GameObject[] meshObjects = new GameObject[4];
        public GameObject meshPrefab;
        public GameObject meshesGroup;

        private float[] meshDurations = new float[4];
        private int meshIndexEnd;
        private MeshFilter[] meshFilters = new MeshFilter[4];
        private MeshRenderer[] meshRenderers = new MeshRenderer[4];

        public void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float lineWidth = 0.005f, float duration = 0.0f)
        {
            AllocateMesh(out MeshFilter meshFilter, out MeshRenderer meshRenderer);

            if (!meshFilter)
            {
                return;
            }

            var meshObject = meshObjects[meshIndexEnd];
            meshFilter.mesh = mesh;
            meshRenderer.material.SetColor("_WireColor", color);
            meshRenderer.material.SetFloat("_WireThickness", lineWidth * 20000.0f);
            meshRenderer.enabled = true;
            meshObject.transform.SetPositionAndRotation(position, rotation);
            meshObject.transform.localScale = scale;
            meshDurations[meshIndexEnd] = duration;
            meshIndexEnd += 1;
        }

        private void AllocateMesh(out MeshFilter meshFilter, out MeshRenderer meshRenderer)
        {
            if (!gameObject.activeInHierarchy)
            {
                meshFilter = null;
                meshRenderer = null;
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
                System.Array.Copy(meshDurations, newMeshDurations, meshCount);
                System.Array.Copy(meshObjects, newMeshObjects, meshCount);
                System.Array.Copy(meshFilters, newMeshFilters, meshCount);
                System.Array.Copy(meshRenderers, newMeshRenderers, meshCount);

                for (var i = meshCount; i < newMeshCount; i++)
                {
                    newMeshObjects[i] = Instantiate(meshPrefab, meshesGroup.transform);
                    newMeshFilters[i] = newMeshObjects[i].GetComponentInChildren<MeshFilter>();
                    newMeshRenderers[i] = newMeshObjects[i].GetComponentInChildren<MeshRenderer>();
                }

                meshDurations = newMeshDurations;
                meshObjects = newMeshObjects;
                meshFilters = newMeshFilters;
                meshRenderers = newMeshRenderers;
            }

            meshFilter = meshFilters[meshIndexEnd];
            meshRenderer = meshRenderers[meshIndexEnd];
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
        }

        private void Start()
        {
            for (var i = 0; i < meshObjects.Length; i++)
            {
                meshFilters[i] = meshObjects[i].GetComponentInChildren<MeshFilter>();
                meshRenderers[i] = meshObjects[i].GetComponentInChildren<MeshRenderer>();
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
