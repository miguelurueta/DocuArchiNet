$ErrorActionPreference = "Continue"

# Avoid treating native stderr output as terminating errors when exit code is 0.
# This keeps git warnings (e.g., LF/CRLF) from aborting the script.
try {
  if (Get-Variable -Name PSNativeCommandUseErrorActionPreference -Scope Global -ErrorAction SilentlyContinue) {
    $global:PSNativeCommandUseErrorActionPreference = $false
  }
} catch {
  # Ignore - not available in Windows PowerShell 5.1
}

function LoadDotEnv([string]$path) {
  if (-not (Test-Path -LiteralPath $path)) {
    return $false
  }
  Write-Host "== load env from $path =="
  Get-Content -LiteralPath $path | ForEach-Object {
    $line = $_
    if ($null -eq $line) { return }
    $trimmed = $line.Trim()
    if (-not $trimmed) { return }
    if ($trimmed.StartsWith("#")) { return }
    $pair = $trimmed.Split("=", 2)
    if ($pair.Length -lt 2) { return }
    $key = $pair[0].Trim()
    $value = $pair[1]
    if (-not $key) { return }
    Set-Item -Path ("Env:" + $key) -Value $value
  }
  return $true
}

function Fail([string]$message) {
  Write-Error $message
  exit 1
}

function RunGit {
  param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$Args
  )
  $out = & git @Args 2>&1
  if ($LASTEXITCODE -ne 0) {
    Fail ("git " + ($Args -join " ") + "`n" + ($out | Out-String))
  }
  return ($out | Out-String)
}

Write-Host "== git status -sb =="
RunGit status -sb | Write-Host

$loadedEnv = LoadDotEnv ".env.jira"
if (-not $loadedEnv) {
  LoadDotEnv ".env"
}

$porcelain = RunGit status --porcelain
$lines = $porcelain -split "\r?\n" | ForEach-Object { $_.TrimEnd() } | Where-Object { $_ }

if (-not $lines) {
  Write-Host "Nada que commitear."
  exit 0
}

Write-Host "== git add -A (respeta .gitignore) =="
RunGit add "-A" | Out-Null

Write-Host "== git commit =="
try {
  RunGit commit -m "chore(git): add git:verify command" | Write-Host
} catch {
  # If there's nothing to commit after add, exit cleanly
  if ($_.Exception.Message -match "nothing to commit") {
    Write-Host "Nada que commitear despues de git add."
    exit 0
  }
  throw
}

Write-Host "== git push =="
RunGit push | Write-Host

Write-Host "== create PR (GitHub) =="
try {
  $prUrl = (& node scripts/git-create-pr.js 2>&1 | Out-String).Trim()
  if ($LASTEXITCODE -ne 0) {
    Fail ("No se pudo crear el PR automaticamente.`n" + $prUrl)
  }
  if ($prUrl) {
    Write-Host ("PR: " + $prUrl)
  }
} catch {
  Fail ("No se pudo crear el PR automaticamente.`n" + $_.Exception.Message)
}

Write-Host "OK: cambios subidos a Git."
