Write-Host "Configuring IIS"

$currentDirectory = (Get-Item -Path ".\").FullName

$iisAppName = "passcore"
$iisAppPoolName = "Passcore Pool PS"
$iisAppPoolDotNetVersion = ""
$iisAppPoolStartMode = "AlwaysRunning"
#the directory where you will be serving the website from
$directoryPath = $args[0]

try {
    Import-Module WebAdministration

    #navigate to the app pools root
    cd IIS:\AppPools\
    
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
    
    $request = Invoke-WebRequest -Uri "http://localhost"
    if ($request.StatusCode -ne 200) {
        Write-Host "HTTP Error"
        exit 1
    }
}
finally {
    Set-Location $currentDirectory
}
