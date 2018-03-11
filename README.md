[![Build status](https://ci.appveyor.com/api/projects/status/76nxqw893mk7xfb9/branch/master?svg=true)](https://ci.appveyor.com/project/geoperez/passcore/branch/master)
[![Github All Releases](https://img.shields.io/github/downloads/unosquare/passcore/total.svg)](https://github.com/unosquare/passcore/releases)
[![Analytics](https://ga-beacon.appspot.com/UA-8535255-2/unosquare/passcore/)](https://github.com/igrigorik/ga-beacon)

# <img src="https://github.com/unosquare/passcore/raw/master/src/Unosquare.PassCore.Web/ClientApp/assets/images/passcore-logo.png"></img>

**PassCore: A self-service password change utility for Active Directory**

*:star:Please star this project if you find it useful!*

## Overview

PassCore is a very simple 1-page web application written in <a target="_blank" href="https://github.com/dotnet/roslyn">C#</a>, using <a href="https://github.com/aspnet" target="_blank">ASP.NET 5</a>, <a href="https://github.com/angular/material2" target="_blank">Angular Material</a>, <a href="https://github.com/angular/angular" target="_blank">Angular</a> and <a href="https://msdn.microsoft.com/en-us/library/system.directoryservices.activedirectory(v=vs.110).aspx" target="_blank">Microsoft Directory Services</a>. It allows users to change their Active Directory password on their own, provided the user is not disabled.

PassCore does not require any configuration, as it obtains the principal context from the current domain. I wrote this because a number of people have requested several features that the <a taget="_blank" href="http://unopasscore.codeplex.com/">original version</a> did not have. The original version of this tool was downloaded around 8000 times in 2.5 years. My hope is that the new version continues to be just as popular. There really is no free alternative out there (that I know of) so hopefully this saves someone else some time and money.

### Features

PassCore has the following features:
- Easily localizable (i.e. you can customize all of the strings in the UI -- see the section on Customization)
- Supports [reCAPTCHA](https://www.google.com/recaptcha/intro/index.html)
- Has a built-in password meter
- Responsive design that works on mobiles, tablets, and desktops.

<img align="center" src="https://github.com/unosquare/passcore/raw/master/preview.png"></img>

## Installation on IIS

1. Ensure the server running IIS is domain-joined. To determine if the computer is domain-joined:
* Go to the *Start* menu, right click on *Computer*, then select *Properties*
* Make sure the *Domain* field contains the correct setting.
2. You need a Passcore copy to continue. We recommed to download the latest binary release of [PassCore](https://github.com/unosquare/passcore/releases/download/3.0.1/Passcore-3.0.1.zip). 
   * **NOTE:** Before extracting the contents of the file, please right click on it, select Properties and make sure the file is Unblocked (Click on the Unblock button at the bottom of the dialog if it is available). Then, extract the contents of the zip file to the directory where you will be serving the website from.
  * If you download the source code you need to run the following command via an Command Prompt. Make sure you start the Command Prompt with the Administrator option.
      * `dotnet publish --framework net461 --output "<path>" --configuration Release`
      * The `<path>` is the directory where you will be serving the website from.
3. Install the [.NET Core Windows Server Hosting bundle](https://docs.microsoft.com/en-us/aspnet/core/publishing/iis?tabs=aspnetcore2x#install-the-net-core-windows-server-hosting-bundle).
4. Go to your *IIS Manager*, Right click on *Application Pools* and select *Add Application Pool*.
5. A dialog appears. Under Name enter **PassCore Application Pool**, Under .NET CLR Version select **No Managed Code** and finally, under Managed pipeline mode select **Integrated**. Click OK after all fields have been set.
6. Now, right click on the application pool you just created in the previous step and select *Advanced Settings ...*. Change the *Start Mode* to **AlwaysRunning**, and the *Idle Time-out (minutes)* to **0**. Click on *OK*. This will ensure PassCore stays responsive even after long periods of inactivity.
7. Back on your *IIS Manager*, right click on *Sites* and select *Add Website*
8. A dialog appears. Under *Site name*, enter **PassCore Website**. Under *Application pool* click on *Select* and ensure you select **PassCore Application Pool**. Under *Physical path*, click on the ellispsis *(...)*, navigate to the folder where you extracted PassCore. 
* **Important:** Make sure the Physical path points to the *parent* folder which is the one containing the files, *logs* and *wwwroot* folders.  
* **NOTE:** If the folder <code>logs</code> is not there you can created. To enable the logs you need to change `stdoutLogEnabled` to `true` in the `web.config` file.
9. Under the *Binding section* of the same dialog, configure the *Type* to be **https**, set *IP Address* to **All Unassigned**, the *Port* to **443** and the *Host name* to something like **password.yourdomain.com**. Under *SSL Certificate* select a certificate that matches the Host name you provided above. If you don't know how to install a certificate, please refer to <a href="https://www.digicert.com/ssl-certificate-installation-microsoft-iis-8.htm">SSL Certificate Install on IIS 8</a> or <a href="https://www.digicert.com/csr-creation-ssl-installation-iis-10.htm">SSL Certificate Install on IIS 10</a> in order to install a proper certificate. **Important:** Do not serve this website without an SSL certificate because requests and responses will be transmitted in cleartext and an attacker could easily retrieve these messages and collect usernames and passwords.
10. Click *OK* and navigate to https://password.yourdomain.com (the host name you previously set). If all is set then you should be able to see the PassCore tool show up in your browser.

**NOTE:** If you have a previous version, you **can not** use the same `appsettings.json` file. Please update your settings manually editing the new file.

## Customization and Configuration

All server-side settings and client-side settings are stored in the <code>/appsettings.json</code> file.
The most relevant configuration entries are shown below. Make sure you make your changes to the <code>appsettings.json</code> file using a regular text editor like <a href="https://code.visualstudio.com">Visual Studio Code</a>.

- To enable reCAPTCHA
  - reCaptcha is enable in testing mode, please change the keys to use it with your application
  - Find the <code>RecaptchaPrivateKey</code> entry and enter your private key within double quotes (<code>"</code>)
  - Find the <code>SiteKey</code> entry and enter your Site Key within double quotes (<code>"</code>)
- To change the language of the reCAPTCHA widget
  - Find the <code>LanguageCode</code> entry and enter <a href="https://developers.google.com/recaptcha/docs/language">one of the options listed here</a>. By default this is set to <code>en</code>
- To enable the password meter
  - Find the <code>ShowPasswordMeter</code> entry and set it to <code>true</code> (without quotes)
- To disable the password meter
  - Find the <code>ShowPasswordMeter</code> entry and set it to <code>false</code> (without quotes)
- To enable restricted group checking
  - Find the <code>CheckRestrictedAdGroups</code> entry and set it to <code>true</code> (without quotes)
  - Find the <code>RestrictedADGroups</code> entry and add any groups that are sensitive.  Accounts in these groups (directly or inherited) will not be able to change their password.
- To enable Let's Encrypt support
  - Find the <code>EnableLetsEncrypt</code> entry and set it to <code>true</code> (without quotes)
  - Create .well-known/acme-challenge folders in wwwroot directory
- To provide an optional paramerter to the URL to set the username text box automatically
  - <code>http://mypasscore.com/?userName=someusername</code> 
  - This helps the user incase they forgot thier username and, also comes in handy when sending a link to the application or having it embeded into another application were the user is all ready signed in.  
- The rest of the configuration entries are all pretty much all UI strings.
  - Change them to localize or brand this utility to meet your needs

## Running as a sub application
To run as a sub application you need to modify the `base href="/"` value in the `wwwroot/index.html` file to be the base url for PassCore. For example you might have PassCore setup at /PassCore so you would put
```
<base href="/PassCore/" />
```
## Troubleshooting
- If you find a HTTP Error 502.5
  - Ensure you have installed [.NET Framework 4.6.1](https://www.microsoft.com/en-us/download/details.aspx?id=49982) or better
- If you find a [HTTP Error 500](https://stackoverflow.com/questions/45415832/http-error-500-19-in-iis-10-and-visual-studio-2017) you can try
  - Press Win Key+R to Open Run Window
  - in the Run Window, enter "OptionalFeatures.exe"
  - in the features window, Click: "Internet Information Services"
  - Click: "World Wide Web Services"
  - Click: "Application Development Features"
  - Check the features.
- If your users are having trouble changing passwords as in issues **<a href="https://github.com/unosquare/passcore/issues/8">#8</a>** or **<a href="https://github.com/unosquare/passcore/issues/9">#9</a>** then try configuring the section <code>PasswordChangeOptions</code> in the <code>/appsettings.json</code> file. Here are some guidelines:
  - Ensure <code>UseAutomaticContext</code> is set to <code>false</code>
  - Ensure <code>LdapUsername</code> is set to an AD user with enough permissions to reset user passwords
  - Ensure <code>LdapPassword</code> is set to the correct password for the admin user mentioned above
  - User @gadams65 suggests the following: Use the FQDN of your LDAP host. Enter the LDAP username without any other prefix or suffix such as <code>domain\\</code> or <code>@domain</code>. Only the username.

## License

PassCore is open source software and MIT licensed. Please star this project if you like it.
