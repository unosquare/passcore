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
$releases = Invoke-WebRequest $releasesUrl

$releasesJson = ($releases.Content | ConvertFrom-Json)[0]
$assetsJson = $releasesJson.assets
$zipName = $assetsJson.name
$zipUrl = $assetsJson.browser_download_url

$zipPath = "$($directory)\$($zipName)"

# Downloading
Write-Host "Downloading $($zipName)"
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Write-Host Downloading zip
Invoke-WebRequest -Uri $zipUrl -OutFile $zipPath

# Unzipping
Write-Host "Unzipping"
Expand-Archive $zipPath -dest $directory -force

# IIS setup script
Write-Host "ISS setup runnig"
$iisSetup = (new-object net.webclient).DownloadString('https://raw.githubusercontent.com/unosquare/passcore/Issue141-AppVeyorIntegrationTest/IISSetup.ps1')

Invoke-Command -ScriptBlock ([scriptblock]::Create($iisSetup)) -ArgumentList $directory

## TODO: Open appsettings.json in notepad to let the users set their configuration