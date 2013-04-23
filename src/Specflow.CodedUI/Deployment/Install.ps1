param($installPath, $toolsPath, $package, $project)

# Get the deployment dll
$assemblyDllPath = (Join-Path $toolsPath "Specflow.CodedUI.dll")

# Get the Specflow package tools path
$parentDir = (Split-Path -Parent $installPath)
$specFlowDir = (Join-Path $parentDir "SpecFlow.1.9.0")
$specFlowToolsDir = (Join-Path $specFlowDir "tools")

# Copy the assembly into the SpecFlow tools directory
Copy-Item -Path $assemblyDllPath -Destination $specFlowToolsDir -Force
