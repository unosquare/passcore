[![Github All Releases](https://img.shields.io/github/downloads/unosquare/passcore/total.svg)](https://github.com/unosquare/passcore/releases)
![Buils status](https://github.com/unosquare/passcore/workflows/ASP.NET%20Core%20CI/badge.svg)

![Passcore Logo](https://github.com/unosquare/passcore/raw/master/src/Unosquare.PassCore.Web/ClientApp/assets/images/passcore-logo.png)
# PassCore: A self-service password change utility for Active Directory

*:star: Please star this project if you find it useful!*

- [Overview](#overview)
  - [Features](#features)
- [Installation on IIS](#installation-on-iis)
- [PowerShell Installer](#powershell-installer)
- [Docker](#docker)
- [Linux](#linux)
- [LDAP Provider](#ldap-provider)
- [Pwned Password Support](#pwned-password-support)
- [Customization and Configuration](#customization-and-configuration)
  - [Running as a sub application](#running-as-a-sub-application)
- [Troubleshooting](#troubleshooting)
  - [LDAP Support](#ldap-support)
- [License](#license)
- [passcorepro](#passcorepro)

## Overview

PassCore is a very simple 1-page web application written in [C#](https://docs.microsoft.com/en-us/dotnet/csharp/), using [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/getting-started/), [Material UI (React Components)](https://material-ui.com/), and [Microsoft Directory Services](https://docs.microsoft.com/en-us/dotnet/api/system.directoryservices) (Default provider). 

It allows users to change their Active Directory/LDAP password on their own, provided the user is not disabled.

PassCore does not require any configuration, as it obtains the principal context from the current domain. I wrote this because a number of people have requested several features that the [original version](http://unopasscore.codeplex.com/) did not have. The original version of this tool was downloaded around 8000 times in 2.5 years. My hope is that the new version continues to be just as popular. There really is no free alternative out there (that I know of) so hopefully this saves someone else some time and money.

You can check [the wiki section](https://github.com/unosquare/passcore/wiki) for additional content related to development of this project.

### Features

PassCore has the following features:

- Easily localizable (i.e. you can customize all of the strings in the UI -- see the section on Customization)
- Supports [reCAPTCHA](https://www.google.com/recaptcha/intro/index.html)
- Has a built-in password meter
- Has a password generator
- Has a server-side password entropy meter

- Responsive design that works on mobiles, tablets, and desktops.
- Works with Windows/Linux servers.

<img align="center" src="https://user-images.githubusercontent.com/25519413/63782596-39713d80-c8b1-11e9-84f0-eef7a06b447b.png"></img>

## Installation on IIS

*You can easily install using Powershell. Check the next section to know how.*

1. Ensure the server running IIS is domain-joined. To determine if the computer is domain-joined:
    - Go to the *Start* menu, right-click on *Computer*, then select *Properties*
    - Make sure the *Domain* field contains the correct setting.
1. You need a Passcore copy to continue. We recommend to download the latest binary release of [PassCore](https://github.com/unosquare/passcore/releases/download/4.2.4/PassCore424.zip).
1. **NOTE:** Before extracting the contents of the file, please right-click on it, select Properties and make sure the file is Unblocked (Click on the Unblock button at the bottom of the dialog if it is available). Then, extract the contents of the zip file to the directory where you will be serving the website from.
    - If you download the source code you need to run the following command via an Command Prompt. Make sure you start the Command Prompt with the Administrator option.
    - `dotnet publish --configuration Release --runtime win-x64 --output "<path>"`
    - The `<path>` is the directory where you will be serving the website from.
1. Install the [.NET Core 5.0.1 Windows Server Hosting bundle](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-5.0.1-windows-hosting-bundle-installer).
1. Go to your *IIS Manager*, Right-click on *Application Pools* and select *Add Application Pool*.
1. A dialog appears. Under Name enter **PassCore Application Pool**, Under .NET CLR Version select **No Managed Code** and finally, under Managed pipeline mode select **Integrated**. Click OK after all fields have been set.
1. Now, right-click on the application pool you just created in the previous step and select *Advanced Settings ...*. Change the *Start Mode* to **AlwaysRunning**, and the *Idle Time-out (minutes)* to **0**. Click on *OK*. This will ensure PassCore stays responsive even after long periods of inactivity.
1. Back on your *IIS Manager*, right-click on *Sites* and select *Add Website*
1. A dialog appears. Under *Site name*, enter **PassCore Website**. Under *Application pool* click on *Select* and ensure you select **PassCore Application Pool**. Under *Physical path*, click on the ellipsis *(...)*, navigate to the folder where you extracted PassCore.
    - **Important:** Make sure the Physical path points to the *parent* folder which is the one containing the files, *logs* and *wwwroot* folders.
    - **NOTE:** If the folder `logs` is not there you can create it. To enable the logs you need to change `stdoutLogEnabled` to `true` in the `web.config` file. You need to add *Full Control* permissions to your IIS Application Pool account (see Troubleshooting).
1. Under the *Binding section* of the same dialog, configure the *Type* to be **https**, set *IP Address* to **All Unassigned**, the *Port* to **443** and the *Hostname* to something like **password.yourdomain.com**. Under *SSL Certificate* select a certificate that matches the Hostname you provided above. If you don't know how to install a certificate, please refer to [SSL Certificate Install on IIS 8](https://www.digicert.com/ssl-certificate-installation-microsoft-iis-8.htm) or [SSL Certificate Install on IIS 10](https://www.digicert.com/csr-creation-ssl-installation-iis-10.htm) , in order to install a proper certificate.
    - **Important:** Do not serve this website without an SSL certificate because requests and responses will be transmitted in cleartext and an attacker could easily retrieve these messages and collect usernames and passwords.
1. Click *OK* and navigate to `https://password.yourdomain.com` (the hostname you previously set). If all is set then you should be able to see the PassCore tool show up in your browser.

**NOTE:** If you have a previous version, you **can not** use the same `appsettings.json` file. Please update your settings manually editing the new file.

## PowerShell Installer

Use PowerShell to download and setup Passcore using the following command line, just make sure you have installed the [.NET Core 5.0.1 Windows Server Hosting bundle](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-5.0.1-windows-hosting-bundle-installer) and enabled World Wide Web publishing service:

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/unosquare/passcore/master/Installer.ps1'))
```

Using the command shown above will install to the folder `C:\passcore` and using the HTTP Port 8080 with the default (localhost) binding. 

If you want to customize your installation please download the [installer script](https://raw.githubusercontent.com/unosquare/passcore/master/Installer.ps1) and 
the [IIS setup script](https://raw.githubusercontent.com/unosquare/passcore/master/IISSetup.ps1).

**NOTE:** You need [PowerShell version 5 or better](https://docs.microsoft.com/en-us/powershell/scripting/setup/windows-powershell-system-requirements?view=powershell-6) 
to execute the script.

## Linux
We recommend use the docker image and redirect the traffic to nginx.

## Docker

You can use the Alpine Docker Builder image and then copy the assets over to an Alpine container. You can pass environment attributes directly into docker without modifying the appsettings.json

```
docker build --rm -t passcore .
docker run \
-e AppSettings__LdapHostnames__0='ad001.example.com' \
-e AppSettings__LdapHostnames__1='ad002.example.com' \
-e AppSettings__LdapPort='636' \
-e AppSettings__LdapUsername='CN=First Last,OU=Users,DC=example,DC=com' \
-it \
-p 80:80 \
passcore:latest
```

**NOTE:** Docker image contains a build using the LDAP Provider (see below).

## LDAP Provider

PassCore was created to use the Microsoft Active Directory Services provided by .NET Framework, but a new Provider using [Novell LDAP Client](https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard) can be used instead. This provider is the default when PassCore is running at Linux or macOS since Microsoft AD Services are NOT available.

The configuration of the LDAP Provider is slightly different. for example, the AutomaticContext is not available and you need to supply credentials.

*WIP*

## Pwned Password Support
Sometimes a simple set of checks and some custom logic is enough to rule out non-secure trivial passwords. Those checks are always performed locally. There are, however, many more unsafe passwords that cannot be ruled out programatically. For those cases there are no simple set of rules that could be used to check those passwords that should never be used: You either need a local DB with a list of banned passwords or use an external API service.

Here is where Pwned Password API comes into play. Pwned Passwords are more than half a billion passwords which have previously been exposed in different data breaches along the years. The use of this service is free and secure. You can read more about this service in [Pwned Passwords overview](https://haveibeenpwned.com/API/v2#PwnedPasswords)

## Customization and Configuration

All server-side settings and client-side settings are stored in the `/appsettings.json` file.
The most relevant configuration entries are shown below. Make sure you make your changes to the `appsettings.json` file using a regular text editor like [Visual Studio Code](https://code.visualstudio.com)

- To enable reCAPTCHA
  1. Find the `PrivateKey` entry and enter your private key within double quotes (`"`)
  2. Find the `SiteKey` entry and enter your Site Key within double quotes (`"`)
- To change the language of the reCAPTCHA widget
  - Find the `LanguageCode` entry and enter [one of the options listed here](https://developers.google.com/recaptcha/docs/language). By default this is set to `en`
- To enable/disable the password meter
  - Find the `ShowPasswordMeter` entry and set it to `true` or `false` (without quotes)
- To enable enable/disable the password generator
  - Find the `UsePasswordGeneration` entry and set it to `true` or `false` (without quotes)
  - Find the `PasswordEntropy` entry and set it to a numeric value (without quotes) to set the entropy of the generated password
- To enable server-side password entropy meter
  - Find the `MinimumScore` entry and set it to a numeric value (without quotes) between 1 and 4, where 1 is a bit secure and 4 is the most secure. Set to 0, for deactivate the validation.
- To enable restricted group checking
  1. Find the `RestrictedADGroups` entry and add any groups that are sensitive.  Accounts in these groups (directly or inherited) will not be able to change their password.
- Find the `DefaultDomain` entry and set it to your default Active Directory domain. This should eliminate confusion about using e-mail domains / internal domain names. **NOTE:** if you are using a subdomain, and you have errors, please try using your top-level domain. 
- To provide an optional parameter to the URL to set the username text box automatically
  1. `http://mypasscore.com/?userName=someusername`
  2. This helps the user in case they forgot their username and, also comes in handy when sending a link to the application or having it embedded into another application where the user is already signed in.
- To specify which (DC) attribute is used to search for the specific user.
  - With the `IdTypeForUser` it is possible to select one of six Attributes that will be used to search for the specifiv user.
  - The possible values are:
    - `DistinguishedName` or `DN`
    - `GloballyUniqueIdentifier` or `GUID`
    - `Name`
    - `SamAccountName` or `SAM`
    - `SecurityIdentifier` or `SID`
    - `UserPrincipalName` or `UPN`
- The rest of the configuration entries are all pretty much all UI strings. Change them to localize, or to brand this utility, to meet your needs.

### Running as a sub-application

To run as a sub-application you need to modify the `base href="/"` value in the `wwwroot/index.html` file to be the base URL for PassCore. For example you might have PassCore setup at /PassCore so you would put

```html
<base href="/PassCore/" />
```

## Troubleshooting

- At first run if you find an error (e.g. **HTTP Error 502.5**) first ensure you have installed [.NET Core 3.1.0 Windows Server Hosting bundle](https://dotnet.microsoft.com/download/thank-you/dotnet-runtime-3.1.0-windows-hosting-bundle-installer), or better.
- If you find an [HTTP Error 500](https://stackoverflow.com/questions/45415832/http-error-500-19-in-iis-10-and-visual-studio-2017) you can try
  1. Press Win Key+R to Open Run Window
  1. in the Run Window, enter "OptionalFeatures.exe"
  1. in the features window, Click: "Internet Information Services"
  1. Click: "World Wide Web Services"
  1. Click: "Application Development Features"
  1. Check the features.
- If you / your user's current password never seems to be accepted for reset; the affected person may need to use a domain-connected PC to log in and reset their password on it first. Updated group policy settings could be blocking user changes, until a local login is completed.
- You can add permissions to your log folder using [icacls](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/icacls)
```
icacls "<logfolder>/" /grant "IIS AppPool\<passcoreAppPoolAccount>:M" /t
```
- If you find [Exception from HRESULT: 0x800708C5 .The password does not meet the password policy requirements](http://blog.cionsystems.com/?p=907) trying to change a password. Set 'Minimum password age' to 0 at 'Default Domain Policy'.

### LDAP Support

- If your users are having trouble changing passwords as in issues #8 or #9 : try configuring the section `PasswordChangeOptions` in the `/appsettings.json` file. Here are some guidelines:
  1. Ensure `UseAutomaticContext` is set to `false`
  1. Ensure `LdapUsername` is set to an AD user with enough permissions to reset user passwords
  1. Ensure `LdapPassword` is set to the correct password for the admin user mentioned above
  1. User @gadams65 suggests the following: Use the FQDN of your LDAP host. Enter the LDAP username without any other prefix or suffix such as `domain\\` or `@domain`. Only the username.
- You can also opt to use the Linux or macOS version of PassCore. This version includes a LDAP Provider based on Novell. The same provider can be used with Windows, you must build it by yourself.

## License

PassCore is open source software and MIT licensed. Please star this project if you like it.

## passcorepro

PassCore is free and will continue to be free forever.
However, you can access a complete, brand new version with new features and tools.

Introducing passcorepro.
This new, enhanced version of our self-service password manager comes with new features such as:

*	Display and manage your Active Directory information with our user profile system.
*	Search for any staff member with the new Directory grid.
*	Forgot your password? We help you reset it via Email or SMS (via [Twillio Verify API](https://www.twilio.com/docs/verify/api) or custom SMS Gateway).
*	Administrate your AD using our new Dashboard tool.
*	Parlez-vous français? You can now add any language to PassCorePro!

Go to our store and download a free trial: https://store.unosquare.com/PasscorePro
