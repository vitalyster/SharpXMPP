Release Instructions
====================

1. Choose the new version according to the [Semantic Versioning][semver] specification.
2. Update the `VersionPrefix` element in the `Directory.Build.props` file.
3. Prepare the fresh release notes in the `CHANGELOG.md` file.
4. Copy-paste the release notes into `PackageReleaseNotes` elements of `SharpXMPP.Shared/SharpXMPP.Shared.csproj` and `SharpXMPP.WebSocket/SharpXMPP.WebSocket.csproj` (replace the Markdown features with plain text).
5. Make and merge a pull request to the main branch.
6. Take the NuGet packages built by the CI, and upload them to https://www.nuget.org/
7. Push a versioned tag in form of `1.0.0`
8. Create a GitHub release from said tag, attach the `.nupkg` files and copy-paste the changelog entry.

[semver]: https://semver.org/spec/v2.0.0.html
