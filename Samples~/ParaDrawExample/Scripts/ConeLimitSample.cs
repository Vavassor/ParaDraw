using UdonSharp;
using UnityEngine;

namespace OrchidSeal.ParaDraw.Sample
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ConeLimitSample : UdonSharpBehaviour
    {
        public Transform pickupTransform;
        public ShapeDrawer shapeDrawer;

        private static Vector2 LimitToEllipse(Vector2 direction, Vector2 radii)
        {
            var a = radii.x;
            var b = radii.y;
            var x0 = direction.x;
            var y0 = direction.y;
            var v = (a * b) / Mathf.Sqrt(a * a * y0 * y0 + b * b * x0 * x0);
            return (v > 1.0f) ? direction : v * direction;
        }

        private static Vector3 LimitToEllipticCone(Vector3 direction, Quaternion coneRotation, Vector2 angles)
        {
            // Project the direction onto a slice of the double cone.
            var coneAxisY = coneRotation * Vector3.up;
            var d = Vector3.Dot(direction, coneAxisY);
            var projection = d / Vector3.Dot(coneAxisY, coneAxisY) * coneAxisY;
            var height = projection.magnitude;
            var radiusX = height * Mathf.Tan(Mathf.Deg2Rad * angles.x);
            var radiusZ = height * Mathf.Tan(Mathf.Deg2Rad * angles.y);
            var directionOs = Quaternion.Inverse(coneRotation) * (direction - projection);

            // Limit within a 2D ellipse slice.
            var limitedDirectionOs = LimitToEllipse(new Vector2(directionOs.x, directionOs.z), new Vector2(radiusX, radiusZ));
            var limitedDirection = coneRotation * new Vector3(limitedDirectionOs.x, 0.0f, limitedDirectionOs.y) + projection;

            // Flip if on the other side of the double cone.
            if (d < 0.0f)
            {
                return Vector3.Reflect(limitedDirection, coneAxisY).normalized;
            }

            return limitedDirection.normalized;
        }

        private void Update()
        {
            shapeDrawer.DrawText("Find the closest direction (cyan) to the pickup within the cone.", transform.position + new Vector3(0.0f, 0.3f, 0.0f), Color.white);

            var coneAxisZ = Quaternion.AngleAxis(360.0f * Time.time * 0.05f, Vector3.up) * -Vector3.one;
            var coneRotation = Quaternion.LookRotation(coneAxisZ, -Vector3.up);
            var conePosition = transform.position + new Vector3(0.0f, 2.0f, 0.0f);
            var coneAngles = new Vector2(25.0f, 45.0f);
            shapeDrawer.DrawWireEllipticCone(conePosition, coneRotation, coneAngles, 0.3f, Color.white);

            var toolPosition = pickupTransform.position;
            var pullColor = new Color(0.0f, 1.0f, 0.0f, 0.1f);
            shapeDrawer.DrawLine(conePosition, toolPosition, pullColor);

            var coneAxisX = coneRotation * Vector3.right;
            var coneAxisY = coneRotation * Vector3.up;
            var boneDirection = toolPosition - conePosition;
            var d = Vector3.Dot(boneDirection, coneAxisY);
            var projection = d / Vector3.Dot(coneAxisY, coneAxisY) * coneAxisY;
            var height = projection.magnitude;

            // Hide when the ellipse gets far away and massive.
            if (height < 1.5f)
            {
                var radiusX = height * Mathf.Tan(Mathf.Deg2Rad * coneAngles.x);
                var radiusZ = height * Mathf.Tan(Mathf.Deg2Rad * coneAngles.y);
                shapeDrawer.DrawEllipse(conePosition + projection, coneAxisX, coneAxisY, new Vector2(radiusX, radiusZ), Color.green);
            }

            var limitedDirection = LimitToEllipticCone(boneDirection, coneRotation, coneAngles);
            var limitedPosition = limitedDirection + conePosition;
            shapeDrawer.DrawLine(conePosition, limitedPosition, Color.cyan, 0.015f);
        }
    }
}
