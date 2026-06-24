param(
    [Parameter(Mandatory = $false)]
    [string]$ApiKey
)

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    $ApiKey = Read-Host "Enter OpenAI API key"
}

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    Write-Error "API key is required."
    exit 1
}

[Environment]::SetEnvironmentVariable("OpenAI__ApiKey", $ApiKey.Trim(), "User")
$env:OpenAI__ApiKey = $ApiKey.Trim()

Write-Host "OpenAI__ApiKey saved to User environment variables."
Write-Host "Restart Visual Studio / Cursor terminals for changes to take effect in new processes."
