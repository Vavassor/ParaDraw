# Contributing

If you have questions or ideas, feel free to open an issue, or join our [Discord server](https://discord.gg/XJYmP5dFJs).

If you'd like to contribute a change or new feature, please open an [issue](https://github.com/Vavassor/ParaDraw/issues) to discuss. We want to save you from doing work and making a pull request if it may not be accepted.

Bug fixes are welcome and don't need prior discussion or a Github issue.

## Reporting Bugs

When submitting a bug report, please search for an existing or similar [issue](https://github.com/Vavassor/ParaDraw/issues) first.

If your problem seems unique, create a new issue! It's helpful to explain the problem you see, what you were expecting instead, and steps you took when the problem occurred. (actual behavior, expected behavior, steps to reproduce) Photos or video also help a lot!

## Development

First [create a new VRChat world project](https://creators.vrchat.com/sdk/) to use for development.

[Fork](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/working-with-forks/fork-a-repo) the git repository and [clone](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository) it into the Packages folder of your project. Rename the repository folder to "com.orchidseal.paradraw".

Create a new git branch for your changes. Development for ParaDraw happens on the `main` branch. So use it as the start point of your branch.

In Unity, create a scene to test your changes. Or, import the sample scene, found under the Samples tab in the [package manager](https://docs.unity3d.com/Manual/upm-ui.html).

Make your changes and test them.

Commit to your git branch and push the changes to your forked repository.

[Open a pull request](https://github.com/Vavassor/ParaDraw/pulls) in Github!

## Samples

When changing samples, make sure the files are copied to the folder for that sample under `Packages/com.orchidseal.paradraw/Samples~`. If [adding a sample](https://docs.unity3d.com/Manual/cus-samples.html), remember to add it to the package manifest (package.json).

## Licensing

ParaDraw is licensed under the [MIT License](LICENSE.md). By contributing, you agree to license your changes under the same license.
