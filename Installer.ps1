Write-Host "Installing Passcore"
Write-Host "Creating Directory"

$directory = "C:\passcore"
New-Item -ItemType directory -Path $directory

Set-Location $directory

# Determining latest release
Write-Host "Determining latest release"
$releasesUrl = "https://api.github.com/repos/unosquare/passcore/releases"

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$releases = Invoke-WebRequest $releasesUrl -UseBasicParsing

$releasesJson = ($releases.Content | ConvertFrom-Json)[0]
$zipName = "passcore.zip"
$zipUrl = $releasesJson.assets.browser_download_url | where-object {$_ -NotLike "*mac*" -and $_ -NotLike "*linux*"}

$zipPath = "$($directory)\$($zipName)"

# Downloading
Write-Host "Downloading $($zipName)"
Invoke-WebRequest -Uri $zipUrl -OutFile $zipPath -UseBasicParsing

# Unzipping
Write-Host "Unzipping"
Expand-Archive $zipPath -dest $directory -force
Remove-Item $zipPath

# Checkin for Net Core Host
$netCoreHost = Get-wmiobject -class win32_product | Where-Object {$_.Name -match "Microsoft .NET Core Host - 2.2.3" }
if([string]::IsNullOrEmpty($netCoreHost)) {
    Write-Host "Please install the hosting bundle and then restart the installation"
    Start-Process "https://dotnet.microsoft.com/download/thank-you/dotnet-runtime-2.2.3-windows-hosting-bundle-installer"
    exit 1
}

# IIS setup script
# Comment or delete the follow lines if you are making a custom installation and setup
Write-Host "ISS setup runnig"
$iisSetup = (new-object net.webclient).DownloadString('https://raw.githubusercontent.com/unosquare/passcore/master/IISSetup.ps1')

Invoke-Command -ScriptBlock ([scriptblock]::Create($iisSetup)) -ArgumentList $directory

Write-Host "Once you finish configuring appsettings.json, passcore will restart" 
Start-Process notepad .\appsettings.json -NoNewWindow -Wait

Write-Host "Restarting passcore"
Import-Module WebAdministration
Stop-WebSite 'passcore'
Start-WebSite 'passcore'
