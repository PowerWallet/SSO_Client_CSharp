FinApps API - Single Sign On
============================

[REST API] for Single Sign On

[REST API]:https://github.com/PowerWallet/SSO_Client_CSharp/blob/master/API.md


FinApps API - C# API Client
===========================

### Adding FinApps libraries to your .NET project

The best and easiest way to add the FinApps libraries to your .NET project is to use the NuGet package manager.  NuGet is a Visual Studio extension that makes it easy to install and update third-party libraries and tools in Visual Studio.  

NuGet is available for Visual Studio, and you can find instructions for installing the NuGet extension on the NuGet.org website:

[http://docs.nuget.org/docs/start-here/installing-nuget](http://docs.nuget.org/docs/start-here/installing-nuget)

Once you have installed the NuGet extension, you can choose to install the FinApps libraries using either the Package Manager dialog, or using the Package Manager console.

#### Installing via the Package Manager Dialog

To install a FinApps library using the Package Manager dialog, first open the dialog by right-clicking the References folder in your project and selecting the package manager option:

![](https://lh4.googleusercontent.com/f7arKv3rtF3_0x8ckYwDC4d9qr3lfcHcIYROjAAI2h6StebF_szFVy_irxjDuKtUlemg2PC9uWaUKjtSuZfwPh6PatIN76BrksWaL8slscC5yDpxxtQ)

When the package manager dialog opens simply search the online catalog for _‘FinApps’_.  The screen shot below shows the results returned from the NuGet catalog:

![](![](http://i.imgur.com/lQHpfdM.png)

Simply click the Install button next to the FinApps package you want to add to your project and watch as NuGet downloads the FinApps library package (and its dependencies) and adds the proper  references to your project.


#### Installing via the Package Manager Console

To install a FinApp library using the Package Manager console, first open the console, then Use the _Install-Package_ command to install the different FinApps packages:

Install REST API client:

    Install-Package FinApps.ApiClient
    


### Sample Usage

* [net40] - .NET Framework 4.0.0
* [net451] - .NET Framework 4.5.1

[net40]:https://github.com/PowerWallet/SSO_Client_CSharp/blob/master/net40.md
[net451]:https://github.com/PowerWallet/SSO_Client_CSharp/blob/master/net451.md
