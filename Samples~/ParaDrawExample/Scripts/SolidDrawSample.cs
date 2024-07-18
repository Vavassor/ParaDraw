using UdonSharp;
using UnityEngine;

namespace OrchidSeal.ParaDraw.Sample
{
    public class SolidDrawSample : UdonSharpBehaviour
    {
        public ShapeDrawer shapeDrawer;
        public MeshCollider bottleMesh;

        private void Update()
        {
            var p = transform.position;
            var t = 0.25f * Time.time;

            shapeDrawer.DrawSolidRectangle(p, Quaternion.identity, 0.4f * Vector2.one, Color.red);
            p += Vector3.left;

            var ellipsoidScale = new Vector3(Wave(0.2f, 0.5f, t), 0.2f, Wave(0.1f, 0.5f, t + 0.5f));
            shapeDrawer.DrawSolidEllipsoid(p, Quaternion.Euler(25.0f, 72.0f, 0.0f), ellipsoidScale, Color.green);
            p += Vector3.left;

            shapeDrawer.DrawSolidBox(p, Quaternion.identity, new Vector3(0.4f, 0.2f, 0.8f), new Color(1.0f, 0.0f, 1.0f, 0.04f));
            p += Vector3.left;

            var bottleColor = ColorWave(new Color(0.0f, 0.0f, 1.0f, 0.13f), Color.cyan, t);
            shapeDrawer.DrawSolidMeshCollider(bottleMesh, bottleColor);
            p += Vector3.left;

            var capsuleStart = p + Mathf.Sin(2.0f * Mathf.PI * t) * new Vector3(0.04f, 0.1f, 0.0f);
            var capsuleEnd = p + Mathf.Sin(0.71f * 2.0f * Mathf.PI * t) * new Vector3(0.04f, 0.0f, 0.2f) + new Vector3(0.3f, 0.2f, -0.4f);
            var capsuleRadius = Wave(0.03f, 0.16f, t);
            shapeDrawer.DrawSolidCapsule(capsuleStart, capsuleEnd, capsuleRadius, Color.white);
        }

        private Color ColorWave(Color a, Color b, float t)
        {
            var s = 0.5f * Mathf.Sin(2.0f * Mathf.PI * t) + 0.5f;
            return Color.Lerp(a, b, s);
        }

        private float Wave(float min, float max, float t)
        {
            var s = 0.5f * Mathf.Sin(2.0f * Mathf.PI * t) + 0.5f;
            return Mathf.Lerp(min, max, s);
        }
    }
}
