$iis = Get-Service W3svc -ErrorAction Ignore
if($iis){
    if($iis.Status -eq "Running") {
        Write-Host "World Wide Web Publishing is running"
    }
    else {
        Write-Host "World Wide Web Publishing is not running"
        exit 1
    }
}
else {
    Write-Host "World Wide Web Publishing not installed"
    exit 1
}

Write-Host "Configuring IIS"

$currentDirectory = (Get-Item -Path ".\").FullName

$iisAppName = "passcore"
$iisAppPort = "8080"
$iisAppPoolName = "Passcore Pool PS"
$iisAppPoolDotNetVersion = ""
$iisAppPoolStartMode = "AlwaysRunning"
#the directory where you will be serving the website from
$directoryPath = (Get-Item -Path $args[0]).FullName

try {
    Import-Module WebAdministration

    #navigate to the app pools root
    Set-Location IIS:\AppPools\
    
    #check if the app pool exists
    if (!(Test-Path $iisAppPoolName -pathType container))
    {
        Write-Host "Creating $($iisAppPoolName)"
        #create the app pool
        $appPool = New-Item $iisAppPoolName
        $appPool | Set-ItemProperty -Name "managedRuntimeVersion" -Value $iisAppPoolDotNetVersion
        $appPool | Set-ItemProperty -Name "startMode" -value $iisAppPoolStartMode
    } 
    
    #navigate to the sites root
    Set-Location IIS:\Sites\
    
    #check if the site exists
    if (Test-Path $iisAppName -pathType container)
    {
        Write-Host "The site $($iisAppName) already exist"
        exit 1
    }
    
    Write-Host "Creating $($iisAppName) on IIS"
    #create the site
    $iisApp = New-Item $iisAppName -bindings @{protocol="http";bindingInformation="*:$($iisAppPort):"} -physicalPath $directoryPath
    $iisApp | Set-ItemProperty -Name "applicationPool" -Value $iisAppPoolName
    
    $site = Get-Website $iisAppName
    Get-IISAppPool $iisAppPoolName
    $site
    
    if(!$site) {
        Write-Host "No site was created"
        exit 1
    }

    Start-WebAppPool -Name $iisAppPoolName
    Start-Website -Name $iisAppName

    $request = Invoke-WebRequest -Uri "http://localhost:$($iisAppPort)"
    if ($request.StatusCode -ne 200) {
        Write-Host "HTTP Error"
        exit 1
    }

    Write-Host "Website request status: $($request.StatusCode)"
}
finally {
    Set-Location $currentDirectory
}
