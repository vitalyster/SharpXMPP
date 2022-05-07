Changelog
=========

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.4.0] - 2022-05-07
### Changed
- [#71](https://github.com/vitalyster/SharpXMPP/issues/71): add DNS server detection for Android, remove hardcoded server
- Get rid of .NET Framework 4.5.1 support, only 4.8 is supported from now
- WebSockets: use HTTP Lookup method for XMPP Websocket URI instead of a DNS-based one

### Fixed
- [#155](https://github.com/vitalyster/SharpXMPP/issues/155): `NullReferenceException` thrown from `InfoHandler::Handle`
- `NullReferenceException` thrown from `XmppConnection::OnIq` if there were IQ queries without defined handlers
- An exception when trying to call `XmppTcpConnection::Dispose`

## [0.3.0] - 2021-09-25
### Added
- Separate build for .NET 5

### Fixed
- Real cancellation for `XmppConnection::ConnectAsync` (including derived classes)

## [0.2.0] - 2020-10-03
### Added
- Allow overriding IQ handlers

## [0.1.1] - 2020-09-24
### Fixed
- Bring back handmade DNS resolver for SRV records (fixes crashes on Xamarin, [#71](https://github.com/vitalyster/SharpXMPP/issues/71))

## [0.1.0] - 2020-09-03
### Added
- Separate package for websocket support
- Support password-protected MUC

## [0.0.3] - 2020-02-23
### Fixed
- Fix parsing for JIDs with at signs ([#27](https://github.com/vitalyster/SharpXMPP/issues/27))

## [0.0.2] - 2019-07-13
### Changed
- Update to .NET Core 2.0

### Fixed
- Fix an endless loop on connection termination

### Added
- Support `DIGEST-MD5` and `PLAIN` authentication schemes
- Enrich the exception information in `ConnectionFailed` event
- Connections are now `IDisposable`

## [0.0.1] - 2017-10-28
Initial release for .NET 4.5.1 and .NET Standard 1.6.

[0.0.1]: https://github.com/vitalyster/SharpXMPP/releases/tag/0.0.1
[0.0.2]: https://github.com/vitalyster/SharpXMPP/compare/0.0.1...0.0.2
[0.0.3]: https://github.com/vitalyster/SharpXMPP/compare/0.0.2...0.0.3
[0.1.0]: https://github.com/vitalyster/SharpXMPP/compare/0.0.3...0.1.0
[0.1.1]: https://github.com/vitalyster/SharpXMPP/compare/0.1.0...0.1.1
[0.2.0]: https://github.com/vitalyster/SharpXMPP/compare/0.1.1...0.2.0
[0.3.0]: https://github.com/vitalyster/SharpXMPP/compare/0.2.0...0.3.0
[0.4.0]: https://github.com/vitalyster/SharpXMPP/compare/0.3.0...0.4.0
[Unreleased]: https://github.com/vitalyster/SharpXMPP/compare/0.4.0...HEAD
