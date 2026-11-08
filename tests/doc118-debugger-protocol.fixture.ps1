$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot '../tools/e2e/scripts/support/doc118-debugger-protocol.ps1')
$cases=[Console]::In.ReadToEnd()|ConvertFrom-Json
$results=@()
foreach($case in $cases){try{Confirm-Doc118ObserverCommand -Command $case.command -ExpectedSequence 1;$results+=$true}catch{$results+=$false}}
$doc118Temporary=Join-Path ([IO.Path]::GetTempPath()) ('doc118-status-'+[Guid]::NewGuid().ToString('n')+'.json')
try{
 [Doc118AtomicStatus]::Write($doc118Temporary,'{"status":"STARTING"}')
 [Doc118AtomicStatus]::Write($doc118Temporary,'{"status":"CLOSED","cleanupConfirmed":true}')
 $doc118Saved=Get-Content -LiteralPath $doc118Temporary -Raw|ConvertFrom-Json
 if($doc118Saved.status -ne 'CLOSED' -or -not $doc118Saved.cleanupConfirmed){throw 'DOC118_ATOMIC_STATUS_FAILED'}
}finally{if(Test-Path -LiteralPath $doc118Temporary){Remove-Item -LiteralPath $doc118Temporary -Force}}
ConvertTo-Json -InputObject $results -Compress
