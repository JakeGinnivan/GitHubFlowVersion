param($installPath, $toolsPath, $package, $project)

$ErrorActionPreference = "Stop"
$gitDir = $null
Write-Host "Looking for .git directory"
while ($true)
{
    $possibleGitDir = Join-Path $workingDirectory.FullName ".git"
    if (Test-Path $possibleGitDir)
    {
        $gitDir = $possibleGitDir
        Break
    }
    $parent = $workingDirectory.parent
    if ($parent -eq $null)
    {
        Break
    }
    $workingDirectory = $parent;
}

if ($gitDir -ne $null)
{
    Write-Host "Found git directory for project at $gitDir"
    $solutionDir = (get-item $gitDir -Force).Parent.FullName
    $gitHubFlowToolsDir = Join-Path $solutionDir "tools\GitHubFlowVersion"
    if (Test-Path $gitHubFlowToolsDir -PathType Container)
    {
        Write-Host "Creating directory $gitHubFlowToolsDir"
    }
    
    Write-Host "GitHubFlowVersion tools installed to $gitHubFlowToolsDir"
    Copy-Item $toolsPath –destination $gitHubFlowToolsDir -recurse -container -force
}
