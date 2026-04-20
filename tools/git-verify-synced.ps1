$ErrorActionPreference = "Stop"

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

RunGit update-index -q --refresh | Out-Null

$branch = (RunGit rev-parse --abbrev-ref HEAD).Trim()
if (-not $branch -or $branch -eq "HEAD") {
  Fail "No se pudo determinar la rama actual (HEAD detached)."
}

$status = RunGit status --porcelain
if ($status.Trim()) {
  Fail ("El repo tiene cambios locales sin guardar en Git (working tree/index).`n`n" +
    "Solucion:`n- git status`n- git add -A`n- git commit -m `"...`"`n- git push`n`n" +
    "Status:`n" + $status.TrimEnd())
}

$upstream = & git rev-parse --abbrev-ref --symbolic-full-name "@{upstream}" 2>$null
if ($LASTEXITCODE -ne 0 -or -not $upstream) {
  Fail ("La rama '$branch' no tiene upstream configurado.`n" +
    "Solucion:`n- git push -u origin $branch")
}

$counts = (RunGit rev-list --left-right --count "@{upstream}...HEAD").Trim() -split "\s+"
if ($counts.Length -lt 2) {
  Fail ("Salida inesperada de rev-list: " + ($counts -join " "))
}
$behind = [int]$counts[0]
$ahead = [int]$counts[1]

if ($behind -ne 0 -or $ahead -ne 0) {
  Fail ("La rama no esta sincronizada con upstream ($($upstream.Trim())).`n" +
    "- Behind: $behind`n- Ahead: $ahead`n`n" +
    "Solucion:`n- Si Ahead > 0: git push`n- Si Behind > 0: git pull --rebase")
}

Write-Host "OK: repo limpio y sincronizado con upstream ($($upstream.Trim())). Rama: $branch"
