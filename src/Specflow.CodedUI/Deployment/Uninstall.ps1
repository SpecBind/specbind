param($installPath, $toolsPath, $package, $project)

# Get the Specflow package tools path
$parentDir = (Split-Path -Parent $installPath)
$specFlowDir = (Join-Path $parentDir "SpecFlow.1.9.0")
$specFlowToolsDir = (Join-Path $specFlowDir "tools")

# Get the deployment dll
$assemblyDllPath = (Join-Path $specFlowToolsDir "Specflow.CodedUI.dll")

# Delete the assembly from the SpecFlow tools directory
Remove-Item -Path $assemblyDllPath  -Force
