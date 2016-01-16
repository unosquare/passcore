# PassCore
A self-service password change utility for Active Directory

PassCore is a very simple 1-page web application written in <a target="_blank" href="https://github.com/dotnet/roslyn">C#</a>, using <a href="https://github.com/aspnet" target="_blank">ASP.NET 5</a>, <a href="https://github.com/twbs/bootstrap" target="_blank">Bootstrap</a>, <a href="https://github.com/angular" target="_blank">AngularJS</a> and <a href="https://msdn.microsoft.com/en-us/library/system.directoryservices.activedirectory(v=vs.110).aspx" target="_blank">Microsoft Directory Services</a>. It allows users to change their Active Directory password on their own, provided the user is not disabled.

PassCore does not require any configuration, as it obtains the principal context from the current domain. I wrote this because a number of people have requested several features that the <a taget="_blank" href="http://unopasscore.codeplex.com/">original version</a> did not have. There really is no free alternative out there (that I know of) so hopefully this saves someone else some time and money.
