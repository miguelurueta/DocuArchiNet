param([Parameter(Mandatory=$true)][ValidatePattern('^[a-f0-9]{32}$')][string]$RunId,[switch]$CurrentWindow,[switch]$IntegrationContinuation)
$ErrorActionPreference='Stop'
$doc118Root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
# Keep this launcher in a supervised exec session. Do not detach its lifetime
# from the visible masked console; no credentials appear in these arguments.
if($CurrentWindow -and $IntegrationContinuation){throw 'DOC118_CONSOLE_AMBIGUOUS_WINDOW'}
$doc118RunFlag=if($IntegrationContinuation){'--integration-continuation'}elseif($CurrentWindow){'--current-window'}else{'--run'}
$doc118Console=Start-Process -FilePath $env:ComSpec -ArgumentList @('/c',('node tools\e2e\scripts\run-doc118-live.cjs '+$doc118RunFlag+' '+$RunId)) -WorkingDirectory $doc118Root -WindowStyle Normal -PassThru
[pscustomobject]@{operation='DOC118_MASKED_CONSOLE';runId=$RunId;consolePid=$doc118Console.Id;supervisorPid=$PID;waitingForExit=$true}|ConvertTo-Json -Compress
$doc118Console.WaitForExit()
exit $doc118Console.ExitCode
