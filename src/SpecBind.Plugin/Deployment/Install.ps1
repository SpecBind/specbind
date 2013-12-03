param($installPath, $toolsPath, $package, $project)

# Log the current path to try to figure everything out
$currentLocation = Get-Location

# Update current location to project path
$projectPath = (Split-Path ($project.FullName))
if ($projectPath)
{
	Set-Location $projectPath
}

# Get the plugin dll path
$relativeToolsPath = (Resolve-Path -Path $toolsPath -Relative)

# Revert location back
Set-Location $currentLocation

# Open the App.config file and replace the plugin token
$configFile = $project.ProjectItems.Item("App.config")
if ($configFile)
{
    Write-Host "Replacing plugin path token in web.config with value:" $relativeToolsPath
	$configFile.Open()
     
    $filePath = $configFile.Document.FullName
	(Get-Content $filePath) | Foreach-Object {$_ -replace "\@specBindDllPathChangeMe\@", $relativeToolsPath} | Set-Content $filePath
}
