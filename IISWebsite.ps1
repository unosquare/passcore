echo "Configuring IIS"

$iisAppName = "passcore"

$iisAppPoolName = "Passcore Pool PS"
$iisAppPoolDotNetVersion = ""
$iisAppPoolStartMode = "AlwaysRunning"

#the directory where you will be serving the website from
$directoryPath = "C:\passcorePS"

Import-Module WebAdministration

#navigate to the app pools root
cd IIS:\AppPools\

#Get-IISAppPool
#Get-ItemProperty IIS:\AppPools\DefaultAPpPool | select cpu.limitInterval

#check if the app pool exists
if (!(Test-Path $iisAppPoolName -pathType container))
{
    echo "Creating $($iisAppPoolName)"
    #create the app pool
    $appPool = New-Item $iisAppPoolName
    $appPool | Set-ItemProperty -Name "managedRuntimeVersion" -Value $iisAppPoolDotNetVersion
    $appPool | Set-ItemProperty -Name "startMode" -value $iisAppPoolStartMode
} 

#navigate to the sites root
cd IIS:\Sites\

#check if the site exists
if (Test-Path $iisAppName -pathType container)
{
    return echo "The site $($iisAppName) already exist"
}

echo "Creating the $($iisAppName) on IIS"
#create the site
$iisApp = New-Item $iisAppName -bindings @{protocol="http";bindingInformation="*:9091:"} -physicalPath $directoryPath
$iisApp | Set-ItemProperty -Name "applicationPool" -Value $iisAppPoolName