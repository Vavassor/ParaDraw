using UnityEngine;

namespace OrchidSeal.ParaDraw
{
    /// <summary>
    /// Point drawing for ShapeDrawer.
    /// </summary>
    public partial class ShapeDrawer
    {
        [Header("Point")]
        public ParticleSystem pointSystem;

        private ParticleSystem.EmitParams emitParams;

        /// <summary>
        /// Draws a point.
        /// </summary>
        /// <param name="point">The point position.</param>
        /// <param name="color">The point color.</param>
        /// <param name="radius">The point radius.</param>
        /// <param name="duration">The number of seconds the point should be visible for.</param>
        public void DrawPoint(Vector3 point, Color color, float radius = 0.02f, float duration = 0.0f)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            emitParams.position = point;
            emitParams.startColor = color;
            emitParams.startSize = radius;
            emitParams.startLifetime = Mathf.Max(duration, Time.deltaTime + 1e-4f);
            pointSystem.Emit(emitParams, 1);
        }
    }
}
