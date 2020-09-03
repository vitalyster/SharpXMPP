How to Pack
===========

To pack the SharpXMPP and publish it to NuGet, use the following command:

```console
$ rm -r SharpXMPP.Shared/bin/Release
$ dotnet pack --configuration Release SharpXMPP.Shared
$ dotnet nuget push SharpXMPP.Shared/bin/Release/SharpXMPP.<version>.nupkg -k <NuGet API key> -s https://www.nuget.org/api/v2/package

$ rm -r SharpXMPP.WebSocket/bin/Release
$ dotnet pack --configuration Release SharpXMPP.WebSocket
$ dotnet nuget push SharpXMPP.WebSocket/bin/Release/SharpXMPP.WebSocket.<version>.nupkg -k <NuGet API key> -s https://www.nuget.org/api/v2/package
```

Remember to set the right version in `Directory.Build.props`.
