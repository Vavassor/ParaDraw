# Changelog

## v0.6.0

- Fixes ShapeDrawer queueing up shapes when it's disabled, causing it to draw them all at once when it's enabled.

## v0.5.0

Released June 16, 2024.

- Added mesh drawing.
- Added collider drawing.
- Changed DrawWireBox to use size instead of extents. It was using size before, just incorrectly labeled as "extents".
- Fixes text rolling with head movement in VR.
- Updates point drawing to draw circles.

## v0.4.0

Released May 14, 2024.

- Added text drawing.
- Added point drawing.

## v0.3.0

Released May 12, 2024.

- Fixed drawings not appearing depending on the script execution order.
- Adds documentation comments to ShapeDrawer.

## v0.2.0

Released May 10, 2024.

- Fixed the cone angle being wrong when using DrawWireCone.
- Added a version of DrawWireEllipsoid to specify the ellipsoid using a rotation.
- Added a version of DrawWireEllipticCone to specify using angles instead of a vector basis.

## v0.1.0

Released May 9, 2024.

- Adds line and wireframe drawing.
