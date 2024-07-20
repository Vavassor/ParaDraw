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
            p += Vector3.left;

            shapeDrawer.DrawEllipticDisk(p, Quaternion.Euler(50.0f * t, 0.0f, 0.0f), new Vector2(0.1f, 0.3f), Color.cyan);
            p += Vector3.up;

            shapeDrawer.DrawSector(p, Vector3.up, Vector3.back, 0.4f, 0.0f, Wave(0.0f, 360.0f, t), Color.red);
            p += Vector3.right;

            shapeDrawer.DrawRing(p, Vector3.right, Vector3.back, 0.4f, 0.1f, 0.0f, Wave(0.0f, 360.0f, t + 0.5f), Color.yellow);
            shapeDrawer.DrawArc(p, Vector3.right, Vector3.back, 0.46f, 0.0f, Wave(0.0f, 360.0f, t + 0.5f), Color.white, 0.008f);
            p += Vector3.right;

            var arm = 0.5f * (Quaternion.AngleAxis(40.0f * t, new Vector3(0.2f, Wave(-0.5f, 0.5f, t), -0.1f)) * Vector3.right);
            shapeDrawer.DrawRay(p, arm, Color.white);
            DrawSectorBetweenVectors(p, arm, Vector3.right, 0.4f, Color.red);
            DrawSectorBetweenVectors(p, arm, Vector3.up, 0.4f, Color.green);
            DrawSectorBetweenVectors(p, arm, Vector3.forward, 0.4f, Color.blue);
        }

        private void DrawSectorBetweenVectors(Vector3 origin, Vector3 a, Vector3 b, float radius, Color color)
        {
            var axis = Vector3.Cross(b, a).normalized;
            var right = Vector3.Cross(axis, b.normalized);
            var forward = Vector3.Cross(right, axis);
            var v = a.normalized;
            var signedAngle = Mathf.Rad2Deg * Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, forward));
            var angle = signedAngle < 0.0f ? signedAngle + 360.0f : signedAngle;
            shapeDrawer.DrawSector(origin, a, axis, radius, 0.0f, angle, color);
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
