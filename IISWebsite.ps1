Write-Host "Configuring IIS"

$currentDirectory = (Get-Item -Path ".\").FullName

$iisAppName = "passcore"
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
    $iisApp = New-Item $iisAppName -bindings @{protocol="http";bindingInformation="*:9091:"} -physicalPath $directoryPath
    $iisApp | Set-ItemProperty -Name "applicationPool" -Value $iisAppPoolName
    
    Start-Sleep -s 3

    $request = Invoke-WebRequest -Uri "http://localhost:9091"
    if ($request.StatusCode -ne 200) {
        Write-Host "HTTP Error"
        exit 1
    }

    Write-Host "Website request status: $($request.StatusCode)"
}
finally {
    Set-Location $currentDirectory
}
