# MIT License
#
# Copyright (c) 2022-2025 Serhii Kokhan
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.

param (
    [Parameter(Mandatory = $true)]
    [string]$V
)

$RootPath = (Resolve-Path -Path "$PSScriptRoot\..").Path
$SrcPath = Join-Path -Path $RootPath -ChildPath "src"
$OutputDir = Join-Path -Path $RootPath -ChildPath "nupkgs"
Set-Variable -Name NuGetSource -Option Constant -Value "C:\NuGet\packages"

if (-not (Test-Path -Path $OutputDir)) {
    New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
}

function Write-Log {
    param (
        [string]$Message,
        [string]$Level = "INFO"
    )
    $Prefix = "[{0}]: {1}" -f $Level.ToUpper(), $Message
    Write-Host $Prefix
}

function Validate-Version {
    param (
        [string]$V
    )
    if (-not $V -or $V -notmatch '^\d+\.\d+\.\d+(-[a-zA-Z0-9]+)?$') {
        Write-Error "Invalid version format. Provide a valid semantic version (e.g., '1.0.0' or '1.0.0-beta1')."
        exit 1
    }
}

function PackAndPublish-Project {
    param (
        [string]$ProjectPath,
        [string]$PackageVersion
    )
    $ProjectName = (Split-Path $ProjectPath -Leaf).Replace(".csproj", "")
    Write-Log "Packing project: $ProjectName" "INFO"
    Write-Host ("-" * 80)

    $PackCommand = "dotnet pack --include-symbols --include-source $ProjectPath -p:PackageVersion=$PackageVersion --output $OutputDir"

    try {
        Invoke-Expression $PackCommand | ForEach-Object {
            $_ -replace "with content hash.*", ""
        }
    } catch {
        Write-Log "Failed to pack project $ProjectName" "ERROR"
        return
    }

    $PackageFile = Join-Path -Path $OutputDir -ChildPath "$ProjectName.$PackageVersion.nupkg"
    if (-Not (Test-Path $PackageFile)) {
        Write-Log "Package file not found for $ProjectName after packing" "ERROR"
        return
    }

    $NuGetAddCommand = "nuget add $PackageFile -source $NuGetSource"
    try {
        Invoke-Expression $NuGetAddCommand | ForEach-Object {
            $_ -replace "with content hash.*", ""
        }
    } catch {
        Write-Log "Failed to add package $ProjectName to NuGet source" "ERROR"
    }

    Write-Host ("=" * 80)
}

function Process-Projects-In-Folder {
    param (
        [string]$BasePath,
        [string]$PackageVersion
    )
    $AllProjects = Get-ChildItem -Path $BasePath -Recurse -Filter *.csproj
    if (-Not $AllProjects) {
        Write-Log "No projects found in folder: $BasePath" "WARNING"
        return
    }

    $GroupedProjects = $AllProjects | Group-Object { (Split-Path $_.DirectoryName -Leaf) }

    foreach ($Group in $GroupedProjects) {
        $CoreProjects = $Group.Group | Where-Object { $_.Name -match "\.Core\.csproj$" }
        $OtherProjects = $Group.Group | Where-Object { $_.Name -notmatch "\.Core\.csproj$" }

        foreach ($Project in $CoreProjects) {
            PackAndPublish-Project -ProjectPath $Project.FullName -PackageVersion $PackageVersion
        }

        foreach ($Project in $OtherProjects) {
            PackAndPublish-Project -ProjectPath $Project.FullName -PackageVersion $PackageVersion
        }
    }
}

Validate-Version -V $V

Write-Log "Processing projects in folder: $SrcPath" "INFO"
Write-Host ("=" * 80)

try {
    Process-Projects-In-Folder -BasePath $SrcPath -PackageVersion $V
    Write-Log "Completed processing of all projects." "INFO"
} catch {
    Write-Log "An error occurred during execution of the script." "ERROR"
    exit 1
}