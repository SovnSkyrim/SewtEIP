param (
    [string]$version
)

if (-not $version) {
    Write-Host "Usage: .\build.ps1 <version>"
    exit 1
}

$repo = "EpitechPromo2026/G-EIP-700-lil-7-1-eip-ulysse.decottignies"
$artifact = "SewtApp"

# 1. Build the project
Write-Host "Building the project..."
dotnet publish -c Release -r win-x64 --self-contained

# 2. Tag the current commit with the version
Write-Host "Tagging the commit with version $version..."
git tag -a "v$version" -m "Release $version"
git push origin "v$version"

# 3. Create the GitHub release
Write-Host "Creating a GitHub release..."
gh release create "v$version" --title "Release $version" --notes "Release notes for version $version" ".\bin\Release\net8.0\win-x64\publish\$artifact.exe"
