using UnityEngine;
using UdonSharp;

namespace OrchidSeal.ParaDraw.Sample
{
    [DefaultExecutionOrder(1)]
    public class DynamicSample : UdonSharpBehaviour
    {
        public ShapeDrawer shapeDrawer;

        private void Update()
        {
            var p = transform.position;
            var t = 0.25f * Time.time;

            var sphereRadius = Wave(0.2f, 0.5f, t);
            shapeDrawer.DrawWireSphere(p, sphereRadius, Color.yellow);
            shapeDrawer.DrawRay(p, sphereRadius * Vector3.up, Color.red);
            p += Vector3.right;

            var angle = Wave(0.0f, 360.0f, 0.2f * t);
            var arcAxis = new Vector3(1.0f, 1.0f, 0.0f);
            var arcRotation = Quaternion.LookRotation(arcAxis);
            var arcRight = arcRotation * arcAxis;
            shapeDrawer.DrawWireCircle(p, arcAxis, 0.1f, Color.white);
            shapeDrawer.DrawArc(p, arcRight, arcAxis, 0.11f, angle, angle + 60.0f, Color.red, 0.02f);
            shapeDrawer.DrawWireEllipse(p, arcRight, arcAxis, new Vector2(0.2f, 0.5f), Color.white);
            shapeDrawer.DrawEllipticArc(p, arcRight, arcAxis, new Vector2(0.21f, 0.51f), angle, angle + 60.0f, Color.red, 0.03f);
            p += Vector3.right;

            shapeDrawer.DrawAxes(p, Quaternion.AngleAxis(angle, new Vector3(4.0f, -1.0f, -3.0f)), 0.4f * Vector3.one);
            p += Vector3.right;

            var boxRotation = Quaternion.LookRotation(new Vector3(-1.0f, 2.0f, 0.0f));
            var boxScale = new Vector3(Wave(0.2f, 0.5f, t), Wave(0.5f, 0.8f, t), 0.3f);
            shapeDrawer.DrawWireBox(p, boxRotation, boxScale, Color.white);
            shapeDrawer.DrawAxes(p, boxRotation, 0.5f * boxScale);
            p += Vector3.right;

            var capsuleEnd = p + new Vector3(0.5f, 0.15f, Wave(-0.8f, 0.8f, t));
            shapeDrawer.DrawWireCapsule(p, capsuleEnd, 0.1f, Color.cyan);
            p += Vector3.right;

            var ellipsoidRotation = Quaternion.LookRotation(Vector3.one);
            var ellipsoidX = ellipsoidRotation * (0.1f * Vector3.right);
            var ellipsoidY = ellipsoidRotation * (0.5f * Vector3.up);
            var ellipsoidZ = ellipsoidRotation * (0.2f * Vector3.forward);
            shapeDrawer.DrawWireEllipsoid(p, ellipsoidX, ellipsoidY, ellipsoidZ, Color.green);
            p += Vector3.right;

            shapeDrawer.DrawWireEllipticCone(p, ellipsoidX, ellipsoidY, ellipsoidZ, Color.white);
            p += Vector3.right;

            shapeDrawer.DrawWireCone(p, -0.2f * Vector3.one, Wave(10.0f, 40.0f, t), Color.yellow);
            p += Vector3.right;

            var frustumRotation = Quaternion.AngleAxis(Wave(-45.0f, 45.0f, t), Vector3.up);
            shapeDrawer.DrawWireFrustum(p, frustumRotation, 60.0f, 0.01f, 0.3f, 1.7777f, Color.cyan);
            p += Vector3.right;

            shapeDrawer.DrawLine(p, p + Vector3.forward, Color.red);
        }

        private float Wave(float min, float max, float t)
        {
            var s = 0.5f * Mathf.Sin(2.0f * Mathf.PI * t) + 0.5f;
            return Mathf.Lerp(min, max, s);
        }
    }
}
