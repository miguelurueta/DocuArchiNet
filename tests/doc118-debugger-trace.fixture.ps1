$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot '../tools/e2e/scripts/support/doc118-debugger-protocol.ps1')
$doc118Ast=[Management.Automation.Language.Parser]::ParseFile((Join-Path $PSScriptRoot '../tools/e2e/scripts/support/doc118-debugger.ps1'),[ref]$null,[ref]$null)
$doc118Function=$doc118Ast.Find({param($node) $node -is [Management.Automation.Language.FunctionDefinitionAst] -and $node.Name -eq 'Save-Observer'},$true)
Invoke-Expression $doc118Function.Extent.Text
$doc118TempRoot=[IO.Path]::GetFullPath([IO.Path]::GetTempPath()).TrimEnd('\')+'\'
$doc118Directory=[IO.Path]::GetFullPath((Join-Path $doc118TempRoot ('doc118-trace-'+[Guid]::NewGuid().ToString('N'))))
if(-not $doc118Directory.StartsWith($doc118TempRoot,[StringComparison]::OrdinalIgnoreCase)){throw 'TEMP_SCOPE_INVALID'}
New-Item -ItemType Directory -Path $doc118Directory|Out-Null
try{
 $doc118Status=Join-Path $doc118Directory 'status.json';$doc118LastTrace='';$doc118TraceEvents=0;$doc118Sequence=0;$doc118Failure=$null;$doc118CleanupConfirmed=$false;$doc118Owner=[pscustomobject]@{Stage='INITIALIZED'};$doc118Created=[DateTime]::UtcNow.ToString('o');$RunId='aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa';$doc118Hashes=@{}
 Save-Observer 'ATTACHED' ([pscustomobject]@{frozen=$false;entriesA=0})
 Save-Observer 'ATTACHED' ([pscustomobject]@{frozen=$false;entriesA=0})
 $doc118Sequence=1;Save-Observer 'INSIDE_FROZEN' ([pscustomobject]@{frozen=$true;entriesA=1})
 $doc118Sequence=2;Save-Observer 'INSPECT' ([pscustomobject]@{frozen=$true;entriesA=1})
 $doc118Sequence=3;$doc118CleanupConfirmed=$true;Save-Observer 'CLOSED' $null
 $rows=@(Get-Content -LiteralPath (Join-Path $doc118Directory 'observation-history.jsonl')|ForEach-Object {$_|ConvertFrom-Json})
 if($rows.Count -ne 4 -or $rows[1].value.status -ne 'INSIDE_FROZEN' -or $rows[2].value.observation.entriesA -ne 1 -or -not $rows[3].value.cleanupConfirmed){throw 'TRACE_PROOF_LOST'}
 if((Get-Content -LiteralPath $doc118Status -Raw|ConvertFrom-Json).status -ne 'CLOSED'){throw 'STATUS_NOT_CLOSED'}
 [pscustomobject]@{events=4;bodyRetained=$true;closeRetained=$true;heartbeatDeduplicated=$true}|ConvertTo-Json -Compress
}finally{Remove-Item -LiteralPath $doc118Directory -Recurse -Force}
