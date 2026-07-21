# Changelog

## [2.1.0](https://github.com/MarcelRoozekrans/Owasp.Analyzers/compare/v2.0.0...v2.1.0) (2026-07-21)


### Features

* add A01 broken access control analyzers (OWASPA01001..005) ([39db2da](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/39db2da31e44480207e8a7583f236d3f97ed8b66))
* add A02 cryptographic failure analyzers (OWASPA02001..008) ([b086f2e](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/b086f2e550f9bd2e5f748510ceae0ac9550711c0))
* add A03 injection analyzers using taint engine (OWASPA03001..006) ([bd477fb](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/bd477fb8110f2f0ccb0730e8ddfb47e544f0ef7b))
* add A04 insecure design and A05 misconfiguration analyzers ([3a7ca62](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/3a7ca624164d372e33b100455ad1da54018f29ec))
* add A06 vulnerable components MSBuild target (OWASP-A06-001..002) ([9c238c2](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/9c238c218c3f80ccdf1ac16dc9853fafb7a346d1))
* add A07 authentication failure analyzers (OWASPA07001..005) ([0cd1006](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/0cd1006cd85272d40b11fdca2098157917d4547c))
* add A08 data integrity analyzers (OWASPA08001..004) ([b0ed534](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/b0ed534f76c38c55631fec813dcdb4c2e002e07e))
* add A09 logging failure analyzers (OWASPA09001..004) ([3b1aae6](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/3b1aae60fdfdf7228f23e7d8ef2f7378f628934e))
* add A10 SSRF analyzers using taint engine (OWASPA10001..003) ([6fb25b2](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/6fb25b2bc85e7eb2993eec0df81510ac44f6417a))
* add Directory.Build.props, icon and readme to NuGet package ([ddbc8e7](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/ddbc8e724e89a279e1a5587ceea0d55b0ddd0782))
* add OWASP red shield+checkmark package icon ([1f28b62](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/1f28b6242989e1a1b9cc24fd00856b0628b59e1e))
* add taint sources and sinks definitions ([c138720](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/c138720a926229e220c84b87767302bf19106a14))
* implement intra-method taint engine ([2e7bbd0](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/2e7bbd0faaa8a195f4fd0c18140da3c350571066))
* scaffold Docusaurus website ([cb0ec3b](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/cb0ec3bc445f1a85b0598705593c12b0591cb317))
* update to OWASP Top 10:2025 ([#46](https://github.com/MarcelRoozekrans/Owasp.Analyzers/issues/46)) ([de8bc7c](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/de8bc7cd02104e995b38063d900a6739f8baa3f8))


### Bug Fixes

* A01 attribute name matching, remove dead semantic call, suppress RS2008 ([0f74418](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/0f744180dcc284a34e49f7594995c63a1c32fd5e))
* A02 TLS flag detection, cert bypass precision, iv word boundary, Random severity ([f08f471](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/f08f47137126982f8c1dac5ea74d0649505a601b))
* A06 targets IgnoreExitCode and separate deprecated check ([5ee8bfb](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/5ee8bfbe4c9ecbb5d8b337070a027de4a0e2bf3a))
* A07 SecurityAlgorithms qualified name check, add missing negative test ([17634ee](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/17634ee8528085786d3b11ec8ff315ed4b1b87c1))
* A08 TypeNameHandling false positive and add LosFormatter/ObjectStateFormatter tests ([119b417](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/119b4177c8614c4e05e9776228031ab7cc6c2f4f))
* A09 Rule004 keyword message and add log injection taint test ([fc89fb6](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/fc89fb65d274395d59366638164a167d0b440796))
* A10 SsrfAnalyzer documentation and expanded test coverage ([6f070b9](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/6f070b90f894a4fc5901c92e2a62e243231e8e77))
* correct docs URL in README ([15144cc](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/15144cc654151c31f100f9912ae1429462bf146a))
* guard pack metadata in Directory.Build.props, explicit PackagePath ([e933884](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/e9338844c743dd828ca1c2e181df9c37f9460b04))
* handle local re-assignment taint propagation and improve invocation analysis ([b45f4b3](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/b45f4b3ef4760154390ce8e7ade208d07051393e))
* improve A04/A05 code quality ([093809b](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/093809bf52b2f33cfb0778fee5452425e9c3ff3a))
* improve taint engine precision and lookup performance ([4d74c7d](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/4d74c7d05474e4c6ef25ea009e71cbb94ab095c1))
* improve test helper async pattern and NuGet packaging config ([2318504](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/23185041249309f7ec84f1599206df1a8930e260))
* repair release-please manifest mode and resync version ([#52](https://github.com/MarcelRoozekrans/Owasp.Analyzers/issues/52)) ([ded7fa7](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/ded7fa7452e5ca43992dc19bf350a3e5c70ca68a))
* scope taint engine to method boundaries, add LDAP/XPath/XSS injection tests ([865d366](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/865d3667e413ff770d267d04911caf86105ad339))
* use $PSScriptRoot in generate-logo.ps1 for path robustness ([384c3f5](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/384c3f54e516f1322fbdc89813cc5eada69fff45))
* use GraphicsPath+Bezier in generate-logo.ps1 to match SVG curves ([53de70d](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/53de70d6e8aaf76b52dcda9e28e03b64472f6e0f))
* use markup prism language and add redirect index page ([116fe2b](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/116fe2b152a2ee8556ea7585e332ad9fdb0f0025))
* use useBaseUrl in redirect to respect baseUrl config ([5b3f115](https://github.com/MarcelRoozekrans/Owasp.Analyzers/commit/5b3f1158e1a864ef95199e469a8d2bdcc8ef0bf9))
