# SharpXMPP
[![Appveyor build status](https://ci.appveyor.com/api/projects/status/rxg50qdn6gcxknav/branch/master?svg=true)](https://ci.appveyor.com/project/vitalyster/sharpxmpp/branch/master)
[![Travis build Status](https://travis-ci.org/vitalyster/SharpXMPP.svg?branch=master)](https://travis-ci.org/vitalyster/SharpXMPP)
[![NuGet](https://img.shields.io/nuget/v/SharpXMPP.Shared.svg)](https://www.nuget.org/packages/SharpXMPP.Shared/)

XMPP library for .NET/Xamarin/.NET Core

License: [LGPLv3](LICENSE.md)

Publishing
----------

If you want to publish the package to NuGet, run the following:

```console
$ dotnet pack SharpXMPP.Shared -c Release
```

After that publish the artifact from `SharpXMPP.Shared/bin/Release/*.nupkg`
