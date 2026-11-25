param([Parameter(Mandatory=$true)][string]$StatePath,
 [Parameter(Mandatory=$true)][string]$OutputPath,
 [Parameter(Mandatory=$true)][string]$StopPath,
 [Parameter(Mandatory=$true)][int]$BudgetMs)
$ErrorActionPreference='Stop'
# Reutiliza el muestreo Get-Process de las ventanas DOC125. Sin credenciales/SQL.
$doc127Root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../../..'))
$doc127Allowed=Join-Path $doc127Root '.tmp/doc127-windows'
foreach($doc127Path in @($StatePath,$OutputPath,$StopPath)){
 if(-not [IO.Path]::GetFullPath($doc127Path).StartsWith($doc127Allowed+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'DOC127_MEMORY_PATH_INVALID'}
}
if($BudgetMs -lt 60000 -or $BudgetMs -gt 900000){throw 'DOC127_MEMORY_BUDGET_INVALID'}
$doc127Samples=New-Object 'System.Collections.Generic.List[object]'
$doc127Clock=[Diagnostics.Stopwatch]::StartNew()
do{
 $doc127State=Get-Content -LiteralPath $StatePath -Raw|ConvertFrom-Json
 if($doc127State.phase -notmatch '\A(?:BASELINE|AUTH|PREPARE|CONFIRM|VERIFY|FINISHED)\z'){throw 'DOC127_MEMORY_PHASE_INVALID'}
 $doc127Workers=@(Get-Process -Name w3wp,iisexpress -ErrorAction SilentlyContinue|ForEach-Object {
  [pscustomobject]@{pid=$_.Id;process=$_.ProcessName;workingSetBytes=$_.WorkingSet64;privateBytes=$_.PrivateMemorySize64;cpuSeconds=$_.CPU}
 })
 $doc127Samples.Add([pscustomobject]@{utc=[DateTime]::UtcNow.ToString('o');phase=$doc127State.phase;workers=$doc127Workers})
 $doc127Temporary=$OutputPath+'.writing'
 [IO.File]::WriteAllText($doc127Temporary,(ConvertTo-Json -InputObject $doc127Samples.ToArray() -Depth 6),[Text.UTF8Encoding]::new($false))
 Move-Item -LiteralPath $doc127Temporary -Destination $OutputPath -Force
 if(Test-Path -LiteralPath $StopPath){break}
 Start-Sleep -Milliseconds 2000
}while($doc127Clock.ElapsedMilliseconds -lt $BudgetMs)
