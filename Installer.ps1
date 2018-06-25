Write-Host "Installing Passcore"
Write-Host "Creating Directory"

$directory = "C:\passcore"
New-Item -ItemType directory -Path $directory

Set-Location $directory

# Determining latest release
Write-Host "Determining latest release"
$repo = "unosquare/passcore"
$releasesUrl = "https://api.github.com/repos/$repo/releases"

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$releases = Invoke-WebRequest $releasesUrl -UseBasicParsing

$releasesJson = ($releases.Content | ConvertFrom-Json)[0]
$assetsJson = $releasesJson.assets
$zipName = $assetsJson.name
$zipUrl = $assetsJson.browser_download_url

$zipPath = "$($directory)\$($zipName)"

# Downloading
Write-Host "Downloading $($zipName)"
Invoke-WebRequest -Uri $zipUrl -OutFile $zipPath -UseBasicParsing

# Unzipping
Write-Host "Unzipping"
Expand-Archive $zipPath -dest $directory -force
Remove-Item $zipPath

# IIS setup script
Write-Host "ISS setup runnig"
$iisSetup = (new-object net.webclient).DownloadString('https://raw.githubusercontent.com/unosquare/passcore/master/IISSetup.ps1')

Invoke-Command -ScriptBlock ([scriptblock]::Create($iisSetup)) -ArgumentList $directory

Write-Host "Once you finish configuring appsettings.json, passcore will restart" 
Start-Process notepad .\appsettings.json -NoNewWindow -Wait

Write-Host "Restarting passcore"
Import-Module WebAdministration
Stop-WebSite 'passcore'
Start-WebSite 'passcore'