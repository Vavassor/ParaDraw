# ParaDraw

Drawing tools for visual debugging in [VRChat](https://vrchat.com) worlds.

Like [Unity Gizmos](https://docs.unity3d.com/ScriptReference/Gizmos.html) or [Debug.DrawLine](https://docs.unity3d.com/ScriptReference/Debug.DrawLine.html) but available at runtime so you can test in VR or on Android.

![Screenshot 2024-05-10 130229](https://github.com/Vavassor/ParaDraw/assets/8423688/f8fff229-9635-456e-8bc8-f2635c408543)

## Features

- Draw lines, rays, boxes, spheres, and cones.
- Control how long shapes are drawn for, to help see momentary events like raycasts.
- Line width and color options, to emphasize or fade away overlapping shapes.

## Getting Started

There's a few options to use the tools.

### Option 1 - Creator Companion

[Get the package for VRChat Creator Companion!](https://vavassor.github.io/OrchidSealVPM)

1. In the VPM repository, click "Add to VCC".
2. In Creator Companion, click the Manage Project button for your project.
3. Under "Manage Packages", find "ParaDraw" and click the add (+) button.

### Option 2 - Manual Install

Download the [latest Unity Package](https://github.com/vavassor/ParaDraw/releases/latest) and follow the directions for [importing local asset packages](https://docs.unity3d.com/2023.1/Documentation/Manual/AssetPackagesImport.html) into your Unity project.

### Using

Drag the ShapeDrawer prefab into your scene. ShapeDrawer is in the folder `Packages/ParaDraw/Runtime/Prefabs`.

Add the ShapeDrawer as a variable to any script you want to debug. And call whatever draw methods you like!

## License

This project is licensed under the terms of the [MIT license](LICENSE.md).
