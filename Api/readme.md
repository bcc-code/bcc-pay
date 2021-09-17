# BCC Pay API overview
  This repository contains an example project that is defined using BccPay.Core NuGet packages; Each library except BccPay.Core.Sample folder are published to nuget.org;
	
## Preparation
  Before working with sample project:
   1. IDEA: Ensure that you have installed [Visual Studio](https://visualstudio.microsoft.com/) or Rider with [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) SDK.
   2. Data Base: [register](https://cloud.ravendb.net/user/register) RavenDB account and generate certificates
>NOTE: After getting the certificate you need to [install](https://ravendb.net/docs/article-page/4.2/csharp/server/security/authentication/client-certificate-usage) them and convert into Base64
string. You can use following [guide](https://blog.techfabric.com/convert-pfx-certificate-to-base64-string/) on Windows
   
   3. Currency exchange service: [Fixer](https://fixer.io/documentation) API-key with at least [Basic plan](https://fixer.io/product), to be able to use currency exchange service.
   4. Payment Provider: [Nets](https://developers.nets.eu/en-EU/) test API-key and checkout key, to be able to use Nets payment provider with the hosted checkout page. Admin panel: [URL](https://portal.dibspayment.eu/).
   5. Payment Provider: [Mollie](https://www.mollie.com/en) test API-key, to be able to use Mollie payment provider. Admin panel: [URL](https://www.mollie.com/dashboard/login?lang=en). 

## Setup project
  1. Clone git repository [BccPay](https://github.com/bcc-code/bcc-pay);
  2. Open API folder and run BccPay.Core.sln file;
  3. Setup .NET secrets (instructions below)

## Setup .NET secrets (Visual Studio)
  In solution explorer find and right-click on "BccPay.Core.Sample.API", select "Manage User Secret" either you can put values into appsettings.json.  

  ### For RavenDB 
  Setup RavenSettings with the following key-value pairs CertFilePath (as base64 string), CertPassword, Urls, and DatabaseName;  

  ### For other third party clients
  NetsSecretKey; MollieSecretKey; FixerApiKey.  

>Warning: each field is required!  

As a result, it will look like this:
```
  "RavenSettings": {
     "CertFilePath": "base64stringvalue"
     "CertPassword": "value",
     "Urls": ["https://cluster"],
     "DatabaseName": "Database-test"
  },
  "FixerApiKey": "api-key",
  "NetsSecretKey": "test-secret-key-*",
  "MollieSecretKey": "test_*" 
```
