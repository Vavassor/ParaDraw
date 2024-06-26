using UdonSharp;
using UnityEngine;

namespace OrchidSeal.ParaDraw.Sample
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class CollisionSample : UdonSharpBehaviour
    {
        public LayerMask raycastLayerMask = (1 << 17); // layer 17 is Walkthrough
        public ShapeDrawer shapeDrawer;

        private void Update()
        {
            shapeDrawer.DrawText("Line of sight", transform.position + new Vector3(0.0f, 0.3f, 0.352f), Color.white);
            
            var p = transform.position + new Vector3(0.0f, 1.0f, -0.5f);

            // Rotate the ray around the surface of a cone.
            var t = Time.time;
            var x = Mathf.Cos(t);
            var y = Mathf.Sin(t);
            var direction = new Vector3(x, y, 3.0f).normalized;

            var maxDistance = 2.0f;

            var isHit = Physics.Raycast(p, direction, out RaycastHit hitInfo, maxDistance, raycastLayerMask);
            if (isHit)
            {
                shapeDrawer.DrawRay(p, hitInfo.point - p, Color.cyan);

                if (hitInfo.collider)
                {
                    shapeDrawer.DrawWireCollider(hitInfo.collider, Color.green);
                    shapeDrawer.DrawLine(hitInfo.point, hitInfo.point + (0.1f * hitInfo.normal), Color.yellow, 0.002f);
                }
            }
            else
            {
                shapeDrawer.DrawRay(p, maxDistance * direction, Color.cyan);
            }
        }
    }
}
