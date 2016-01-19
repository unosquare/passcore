# <img src="https://raw.githubusercontent.com/unosquare/passcore/master/src/Unosquare.PassCore.Web/wwwroot/images/passcore-logo.png"></img>
**PassCore: A self-service password change utility for Active Directory [![Analytics](https://ga-beacon.appspot.com/UA-8535255-2/unosquare/passcore/)](https://github.com/igrigorik/ga-beacon)**

*:star:If you like this project please star it*

## Overview

PassCore is a very simple 1-page web application written in <a target="_blank" href="https://github.com/dotnet/roslyn">C#</a>, using <a href="https://github.com/aspnet" target="_blank">ASP.NET 5</a>, <a href="https://github.com/twbs/bootstrap" target="_blank">Bootstrap</a>, <a href="https://github.com/angular" target="_blank">AngularJS</a> and <a href="https://msdn.microsoft.com/en-us/library/system.directoryservices.activedirectory(v=vs.110).aspx" target="_blank">Microsoft Directory Services</a>. It allows users to change their Active Directory password on their own, provided the user is not disabled.

PassCore does not require any configuration, as it obtains the principal context from the current domain. I wrote this because a number of people have requested several features that the <a taget="_blank" href="http://unopasscore.codeplex.com/">original version</a> did not have. The original version of this tool was downloaded around 8000 times in 2.5 years. My hope is that the new version continues to be just as popular. There really is no free alternative out there (that I know of) so hopefully this saves someone else some time and money.

### Features

PassCore has the following features:
- Easily localizable (i.e. you can customize all of the strings in the UI -- see the section on Customization)
- Supports <a href="https://www.google.com/recaptcha/intro/index.html">reCAPTCHA</a>
- Has a built-in password meter (thanks to <a href="https://github.com/subarroca/ng-password-strength">ng-password-strength</a> 
- Responsive design that works on mobiles, tablets, and desktops.

<img align="center" src="https://raw.githubusercontent.com/unosquare/passcore/master/passcore-screenshot-01.png" alt="PassCore Screenshot"></img>

## Installation

_There is no binary release yet because ASP.NET 5 is still in its RC stage_
ASP.NET 5 is still in RC and I'm having trouble creating a release version as the Publish feature is not yet working properly. See: <a href="http://forums.asp.net/p/2082720/6010940.aspx">Publish Dialog not Showing Up</a>. Hopefully someone is able to help out.

## Customization

All server-side settings and client-side settings are stored in the <code>appsettings.json</code> file.
The most relevant configuration entries are shown below. Make sure you make your changes to the <code>appsettings.json</code> file using a regular text editor like <a href="https://atom.io/">Atom</a>.

- To enable reCAPTCHA
  - Find the <code>RecaptchaPrivateKey</code> entry and enter your private key within double quotes (")
  - Find the <code>IsEnabled</code> entry and enter the word <code>true</code> (note this should be done _without_ double quotes
  - Finde the <code>SiteKey</code> entry and enter your Site Key within double quotes (")
- To enable the password meter
  - Find the <code>ShowPasswordMeter</code> entry and set it to <code>true</code> (without quotes)
- To disable the password meter
  - Find the <code>ShowPasswordMeter</code> entry and set it to <code>false</code> (without quotes)
- The rest of the configuration entries are all pretty much all UI strings.
  - Change them to localize or brand this utility to meet your needs

## License

PassCore is open source software and MIT licensed. Please star this project if you like it.
