; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md
;
; A03 (Software Supply Chain Failures, OWASP-A03-001/OWASP-A03-002) is intentionally excluded:
; it's implemented as an MSBuild <Warning> in build/Owasp.Analyzers.targets, not a Roslyn
; DiagnosticAnalyzer, so it isn't a "rule" this release-tracking mechanism can track.

## Release 2.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
OWASPA01001 | OWASP.A01 | Warning | Controller action missing authorization attribute
OWASPA01002 | OWASP.A01 | Warning | Hardcoded role string in [Authorize]
OWASPA01003 | OWASP.A01 | Warning | IsInRole called with hardcoded string
OWASPA01004 | OWASP.A01 | Warning | CORS AllowAnyOrigin (wildcard)
OWASPA01005 | OWASP.A01 | Warning | POST/PUT/DELETE action missing antiforgery token
OWASPA01006 | OWASP.A01 | Error | SSRF via HttpClient (taint analysis)
OWASPA01007 | OWASP.A01 | Error | SSRF via WebClient (taint analysis)
OWASPA01008 | OWASP.A01 | Warning | AllowAutoRedirect without URL validation
OWASPA02001 | OWASP.A02 | Warning | Developer exception page enabled unconditionally
OWASPA02002 | OWASP.A02 | Warning | Missing HTTPS redirection
OWASPA02003 | OWASP.A02 | Warning | Directory browsing enabled
OWASPA02004 | OWASP.A02 | Warning | Error details exposed to client
OWASPA02005 | OWASP.A02 | Warning | Antiforgery services not configured
OWASPA02006 | OWASP.A02 | Error | Hardcoded credential in source code
OWASPA04001 | OWASP.A04 | Warning | Weak hashing algorithm (MD5 / SHA1)
OWASPA04002 | OWASP.A04 | Warning | ECB cipher mode
OWASPA04003 | OWASP.A04 | Info | System.Random used (not cryptographically secure)
OWASPA04004 | OWASP.A04 | Error | Hardcoded cryptographic key or IV
OWASPA04005 | OWASP.A04 | Warning | Legacy TLS protocol (SSL2/3, TLS 1.0/1.1)
OWASPA04006 | OWASP.A04 | Error | Certificate validation disabled
OWASPA04007 | OWASP.A04 | Warning | HTTP URL used (not HTTPS)
OWASPA04008 | OWASP.A04 | Warning | HSTS not configured alongside HTTPS redirection
OWASPA05001 | OWASP.A05 | Error | SQL injection (taint analysis)
OWASPA05002 | OWASP.A05 | Error | OS command injection (taint analysis)
OWASPA05003 | OWASP.A05 | Error | Path traversal (taint analysis)
OWASPA05004 | OWASP.A05 | Error | LDAP injection (taint analysis)
OWASPA05005 | OWASP.A05 | Error | XPath injection (taint analysis)
OWASPA05006 | OWASP.A05 | Error | XSS via unencoded output (taint analysis)
OWASPA06001 | OWASP.A06 | Warning | Missing rate limiting on authentication endpoints
OWASPA07001 | OWASP.A07 | Error | JWT signed with SecurityAlgorithms.None
OWASPA07002 | OWASP.A07 | Warning | JWT lifetime validation disabled
OWASPA07003 | OWASP.A07 | Error | JWT signing key validation disabled
OWASPA07004 | OWASP.A07 | Warning | Cookie missing HttpOnly or Secure flag
OWASPA07005 | OWASP.A07 | Warning | Cookie SameSite=None without Secure
OWASPA08001 | OWASP.A08 | Error | BinaryFormatter usage
OWASPA08002 | OWASP.A08 | Error | NetDataContractSerializer / SoapFormatter usage
OWASPA08003 | OWASP.A08 | Error | TypeNameHandling not None in Newtonsoft.Json
OWASPA08004 | OWASP.A08 | Error | JavaScriptSerializer with SimpleTypeResolver
OWASPA09001 | OWASP.A09 | Warning | Log injection via user-controlled input (taint analysis)
OWASPA09002 | OWASP.A09 | Warning | Sensitive data keyword in log message
OWASPA10001 | OWASP.A10 | Warning | Empty catch block (swallowed exception)
OWASPA10002 | OWASP.A10 | Warning | Catch block without logging
