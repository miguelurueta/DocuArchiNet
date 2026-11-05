param([Parameter(Mandatory=$true)][string]$RepositoryRoot,[Parameter(Mandatory=$true)][string]$TemporaryRoot)
$ErrorActionPreference='Stop'
. (Join-Path $RepositoryRoot 'tools/e2e/scripts/support/doc116-level-permission.ps1')
$script:checks=0
function Check($Value,[string]$Message){if(-not $Value){throw $Message};$script:checks++}
function Reject([scriptblock]$Call,[string]$Pattern){$rejected=$false;try{& $Call}catch{$rejected=$_.Exception.Message -match $Pattern};Check $rejected ('Expected rejection '+$Pattern)}
function Resource {return [pscustomobject]@{userRef='141';companyRef='2';levelRef='7';lotId='1';permissionId='9';environmentHash=('a'*64)}}
function Row {return [pscustomobject]@{id_permisos_niveles='9';ra_pro_niveles_id_nivel='7';remit_dest_interno_id_Remit_Dest_Int='141';estado_permiso=1;agregar_expediente=1;visualizar_archivo=1;doc116_owner='142';doc116_level_active=1;doc116_company='2';doc116_actor_company='2';doc116_actor_active=1;doc116_lot='1'}}
function Fixture([string[]]$Operations) {return [pscustomobject]@{row=(Row);operations=$Operations;next=0;reads=0;writes=(New-Object 'System.Collections.Generic.List[string]');saved=(New-Object 'System.Collections.Generic.List[object]');emitted=(New-Object 'System.Collections.Generic.List[object]');throwAfterRevoke=$false;throwAfterRestore=$false;changeOthers=$false;failRevokeRead=$false;failSave=$false}}
function RunFixture($Fixture,$Previous=$null,[bool]$Recovery=$false) {
    $f=$Fixture;$r=Resource
    $read={
        $f.reads++
        if($f.failRevokeRead -and $f.writes.Count -eq 1 -and $f.reads -eq 2){throw 'SYNTHETIC_READ_FAILURE'}
        return $f.row
    }.GetNewClosure()
    $set={param($expected,$value,$hash)
        Check ([string]$f.row.agregar_expediente -ceq [string]$expected) 'CAS previous value'
        Check ((Get-Doc116LevelHash $f.row) -ceq $hash) 'CAS fields hash'
        $f.writes.Add(([string]$expected+'>'+[string]$value));$f.row.agregar_expediente=$value
        if($expected -eq 1 -and $f.throwAfterRevoke){throw 'SYNTHETIC_REVOKE_RESPONSE_LOSS'}
        if($expected -eq 0 -and $f.throwAfterRestore){throw 'SYNTHETIC_RESTORE_RESPONSE_LOSS'}
    }.GetNewClosure()
    $save={param($record) if($f.failSave){throw 'SYNTHETIC_JOURNAL_FAILURE'};$f.saved.Add(($record|ConvertTo-Json -Compress|ConvertFrom-Json))}.GetNewClosure()
    $next={
        if($f.next -ge $f.operations.Count){return $null};$op=$f.operations[$f.next];$f.next++
        if($f.changeOthers -and $f.next -gt 1){$f.row.visualizar_archivo=0}
        return $op
    }.GetNewClosure()
    $emit={param($value) $f.emitted.Add($value)}.GetNewClosure()
    Invoke-Doc116LevelProtocol $r $f.row $Previous $read $set $save $next $emit $Recovery
}
$f=Fixture @('REVOKE','RESTORE');RunFixture $f
Check ($f.writes.Count -eq 2 -and $f.writes[0] -ceq '1>0' -and $f.writes[1] -ceq '0>1') 'Exact two transitions'
Check ($f.row.agregar_expediente -eq 1 -and $f.row.visualizar_archivo -eq 1) 'Original restored without other changes'
Check ($f.saved[0].stage -ceq 'REVOKE_ATTEMPT' -and $f.saved[0].restored -eq $false -and $f.saved[-1].stage -ceq 'RESTORED' -and $f.saved[-1].restored -eq $true) 'Durable journal ordering'
Check ($f.emitted[-1].stage -ceq 'RESTORED') 'Restoration readback reported'
$f=Fixture @('REVOKE');RunFixture $f
Check ($f.writes.Count -eq 2 -and $f.row.agregar_expediente -eq 1 -and $f.emitted[-1].stage -ceq 'RESTORED_AFTER_EXIT') 'EOF restores in finally'
$f=Fixture @('REVOKE','INVALID');Reject {RunFixture $f} 'OPERATION_INVALID';Check ($f.row.agregar_expediente -eq 1) 'Invalid protocol input restores'
$f=Fixture @('REVOKE');$f.throwAfterRevoke=$true;Reject {RunFixture $f} 'REVOKE_RESPONSE_LOSS';Check ($f.writes.Count -eq 2 -and $f.row.agregar_expediente -eq 1 -and $f.saved[-1].restored) 'Lost revoke response reconciles original value'
$f=Fixture @('REVOKE','RESTORE');$f.throwAfterRestore=$true;Reject {RunFixture $f} 'RESTORE_RESPONSE_LOSS';Check ($f.writes.Count -eq 2 -and $f.row.agregar_expediente -eq 1 -and $f.saved[-1].restored) 'Lost restoration response never re-revokes'
$f=Fixture @('REVOKE');$f.failRevokeRead=$true;Reject {RunFixture $f} 'READ_FAILURE';Check ($f.row.agregar_expediente -eq 1) 'Read failure after mutation restores'
$f=Fixture @('REVOKE');$f.failSave=$true;Reject {RunFixture $f} 'JOURNAL_FAILURE';Check ($f.writes.Count -eq 0 -and $f.row.agregar_expediente -eq 1) 'No mutation without durable intent'
$f=Fixture @('REVOKE','RESTORE');$f.changeOthers=$true;Reject {RunFixture $f} 'OTHER_FIELDS_CHANGED';Check ($f.row.agregar_expediente -eq 0 -and $f.row.visualizar_archivo -eq 0 -and -not $f.saved[-1].restored) 'External change blocks force restoration and retains recovery'
foreach($name in @('doc116_owner','doc116_actor_company','id_permisos_niveles','estado_permiso')){
    $f=Fixture @('REVOKE');$f.row.$name=if($name -ceq 'doc116_owner'){'141'}elseif($name -ceq 'estado_permiso'){'0'}else{'999'}
    Reject {RunFixture $f} 'LEVEL_(OWNER_BYPASS|SCOPE_CHANGED|NOT_EFFECTIVE)';Check ($f.writes.Count -eq 0) 'Wrong owner/scope never writes'
}
$f=Fixture @('REVOKE');$f.row.agregar_expediente=0;Reject {RunFixture $f} 'ORIGINAL_NOT_GRANTED';Check ($f.writes.Count -eq 0) 'Does not grant missing permission'
$f=Fixture @('REVOKE');Reject {RunFixture $f ([pscustomobject]@{restored=$false})} 'PREVIOUS_RECOVERY_REQUIRED';Check ($f.writes.Count -eq 0) 'Unresolved journal blocks a new test'
$r=Resource;Assert-Doc116LevelResources $r
foreach($bad in @('0','01','4294967296','1 OR 1=1')){$r=Resource;$r.permissionId=$bad;Reject {Assert-Doc116LevelResources $r} 'RESOURCE_INVALID'}
$r=Resource;$r|Add-Member password 'rejected-synthetic';Reject {Assert-Doc116LevelResources $r} 'RESOURCE_INVALID'
$f=Fixture @();$f.row.agregar_expediente=0;$r=Resource
$previous=[pscustomobject]@{version=1;originalPermission=1;otherFieldsHash=(Get-Doc116LevelHash $f.row);userRef=$r.userRef;companyRef=$r.companyRef;levelRef=$r.levelRef;lotId=$r.lotId;permissionId=$r.permissionId;environmentHash=$r.environmentHash;restored=$false}
RunFixture $f $previous $true
Check ($f.row.agregar_expediente -eq 1 -and $f.writes.Count -eq 1 -and $f.saved[-1].restored) 'Recovery restores without replaying creation'
$f=Fixture @();$f.row.agregar_expediente=0;$previous.levelRef='8';Reject {RunFixture $f $previous $true} 'RECOVERY_MISMATCH';Check ($f.writes.Count -eq 0) 'Recovery cannot change target'
# AST checks never execute the administrative bridge or its connection resolver.
foreach($rel in @('tools/e2e/scripts/support/doc116-level-permission.ps1','tools/e2e/scripts/support/doc110-permission-db.ps1')){
    $tokens=$null;$errors=$null;[void][Management.Automation.Language.Parser]::ParseFile((Join-Path $RepositoryRoot $rel),[ref]$tokens,[ref]$errors);Check ($errors.Count -eq 0) 'PowerShell parse'
}
$source=Get-Content -LiteralPath (Join-Path $RepositoryRoot 'tools/e2e/scripts/support/doc116-level-permission.ps1') -Raw
Check ($source.Contains('UPDATE ra_pro_permisos_niveles SET agregar_expediente=@value WHERE id_permisos_niveles=@permission')) 'Only native level column can be updated'
Check ($source.Contains('$command.Transaction=$tx') -and $source.Contains('FOR UPDATE') -and $source.Contains('$tx.Commit()') -and $source.Contains('$tx.Rollback()')) 'Parameterized transactional compare-and-set contract'
Check ($source.Contains('Read-Doc116LevelIndependently') -and $source.Contains('DOC116_LEVEL_TRANSACTIONAL_TABLES_REQUIRED')) 'Independent readback and engine precondition'
# Exercise the real SQL writer with command/transaction doubles, not just its text.
function SqlFixture {
    $state=[pscustomobject]@{row=(Row);args=@{};affected=1;commits=0;rollbacks=0;original=1;tx=$null;command=$null}
    $tx=[pscustomobject]@{owner=$state}
    $tx|Add-Member ScriptMethod Commit {$this.owner.commits++}
    $tx|Add-Member ScriptMethod Rollback {$this.owner.rollbacks++;$this.owner.row.agregar_expediente=$this.owner.original}
    $tx|Add-Member ScriptMethod Dispose {}
    $parameters=[pscustomobject]@{owner=$state}
    $parameters|Add-Member ScriptMethod AddWithValue {param($key,$value) $this.owner.args[$key]=$value}
    $command=[pscustomobject]@{owner=$state;CommandText='';CommandTimeout=0;Transaction=$null;Parameters=$parameters}
    $command|Add-Member ScriptMethod ExecuteNonQuery {
        Check ([object]::ReferenceEquals($this.Transaction,$this.owner.tx)) 'Actual writer attaches transaction'
        Check ($this.CommandText -cmatch '^UPDATE ra_pro_permisos_niveles SET agregar_expediente=@value WHERE ') 'Actual writer uses only native column'
        Check ($this.owner.args.Count -eq 5 -and $this.owner.args['@permission'] -ceq '9' -and $this.owner.args['@level'] -ceq '7' -and $this.owner.args['@actor'] -ceq '141') 'Actual writer parameter values'
        if($this.owner.affected -eq 1){$this.owner.row.agregar_expediente=$this.owner.args['@value']};return $this.owner.affected
    }
    $command|Add-Member ScriptMethod Dispose {}
    $connection=[pscustomobject]@{owner=$state}
    $connection|Add-Member ScriptMethod BeginTransaction {$this.owner.original=$this.owner.row.agregar_expediente;return $this.owner.tx}
    $connection|Add-Member ScriptMethod CreateCommand {return $this.owner.command}
    $state.tx=$tx;$state.command=$command;return [pscustomobject]@{state=$state;connection=$connection}
}
function Read-Doc110Rows {param($Sql,$Parameters,$Transaction)
    Check ($Sql.EndsWith('LIMIT 2 FOR UPDATE') -and [object]::ReferenceEquals($Transaction,$script:sqlState.tx)) 'Actual writer locks scoped row'
    Check ($Parameters['@permission'] -ceq '9' -and $Parameters['@level'] -ceq '7' -and $Parameters['@actor'] -ceq '141' -and $Parameters['@lot'] -ceq '1') 'Locked row SELECT parameters'
    return ,@($script:sqlState.row)
}
$fixture=SqlFixture;$script:sqlState=$fixture.state;$script:raConnection=$fixture.connection
Set-Doc116LevelPermission (Resource) 1 0 (Get-Doc116LevelHash $fixture.state.row)
Check ($fixture.state.row.agregar_expediente -eq 0 -and $fixture.state.commits -eq 1 -and $fixture.state.rollbacks -eq 0) 'Actual SQL compare-and-set commit'
Set-Doc116LevelPermission (Resource) 0 1 (Get-Doc116LevelHash $fixture.state.row)
Check ($fixture.state.row.agregar_expediente -eq 1 -and $fixture.state.commits -eq 2) 'Actual SQL restoration commit'
$fixture=SqlFixture;$script:sqlState=$fixture.state;$script:raConnection=$fixture.connection;$fixture.state.affected=0
Reject {Set-Doc116LevelPermission (Resource) 1 0 (Get-Doc116LevelHash $fixture.state.row)} 'LEVEL_CAS_FAILED'
Check ($fixture.state.row.agregar_expediente -eq 1 -and $fixture.state.commits -eq 0 -and $fixture.state.rollbacks -eq 1) 'Zero affected rows roll back'
$fixture=SqlFixture;$script:sqlState=$fixture.state;$script:raConnection=$fixture.connection
Reject {Set-Doc116LevelPermission (Resource) 1 0 ('0'*64)} 'LEVEL_CAS_PRECONDITION_FAILED'
Check ($fixture.state.args.Count -eq 0 -and $fixture.state.rollbacks -eq 1) 'Other-field mismatch cannot reach UPDATE'
# Execute the actual bridge entry/recovery path against memory ports and real local journal files.
# The production connection helper is not loaded; these definitions cannot contact MySQL.
$script:repoRoot=[IO.Path]::GetFullPath($TemporaryRoot);$script:mockRow=Row;$script:mockRow.agregar_expediente=0;$script:mockWrites=0;$script:mockOpens=0
function Open-Doc110Database {$script:mockOpens++;$script:raConnection=New-Object IO.MemoryStream;$script:databaseBuilder=[pscustomobject]@{Server='synthetic';Database='synthetic'};$script:catalogConnection=$null}
function Read-Doc110Rows {param($Sql,$Parameters,$Transaction) if(-not $Sql.Contains('information_schema.TABLES')){throw 'SYNTHETIC_UNEXPECTED_READ'};return ,@('a','b','c','d'|ForEach-Object {[pscustomobject]@{TABLE_NAME=$_;ENGINE=if($script:badEngine){'MyISAM'}else{'InnoDB'}}})}
function Read-Doc116LevelIndependently {param($Resource) Assert-Doc116LevelRow $Resource $script:mockRow;return $script:mockRow}
function Set-Doc116LevelPermission {param($Resource,$Expected,$Value,$OtherHash) Check ([string]$script:mockRow.agregar_expediente -ceq [string]$Expected) 'Recovery CAS expectation';$script:mockRow.agregar_expediente=$Value;$script:mockWrites++}
$r=Resource;$sha=[Security.Cryptography.SHA256]::Create();try{$r.environmentHash=([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes('synthetic|synthetic')))).Replace('-','').ToLowerInvariant()}finally{$sha.Dispose()}
$resourcesFile=Join-Path $TemporaryRoot 'level.json';[IO.File]::WriteAllText($resourcesFile,($r|ConvertTo-Json -Compress))
$artifactDirectory=Join-Path $TemporaryRoot 'tools/e2e/artifacts';[void][IO.Directory]::CreateDirectory($artifactDirectory);$journal=Join-Path $artifactDirectory 'doc116-level-permission-recovery.json'
$record=[pscustomobject]@{version=1;originalPermission=1;otherFieldsHash=(Get-Doc116LevelHash $script:mockRow);userRef=$r.userRef;companyRef=$r.companyRef;levelRef=$r.levelRef;lotId=$r.lotId;permissionId=$r.permissionId;environmentHash=$r.environmentHash;restored=$false}
[IO.File]::WriteAllText($journal,($record|ConvertTo-Json -Compress))
$env:DOC116_AUTH_LEVEL_PERMISSION='true';$env:DOC116_LEVEL_RESOURCES_SHA256=(Get-FileHash -LiteralPath $resourcesFile -Algorithm SHA256).Hash.ToLowerInvariant()
try{
    $output=Invoke-Doc116LevelBridge $resourcesFile $true $false | ConvertFrom-Json
    if($output.ok -ne $true){throw ('Synthetic bridge entry failed: '+$Error[0].Exception.Message)}
    Check ($output.stage -ceq 'RESTORED_AFTER_EXIT') 'Real entry restores with memory ports'
    Check ($script:mockOpens -eq 1 -and $script:mockWrites -eq 1 -and $script:mockRow.agregar_expediente -eq 1) 'One recovery connection and transition'
    $persisted=Get-Content -LiteralPath $journal -Raw|ConvertFrom-Json;Check ($persisted.restored -eq $true -and $persisted.stage -ceq 'RESTORED') 'Atomic journal physically read back'
    $script:badEngine=$true;$failure=Invoke-Doc116LevelBridge $resourcesFile $false $true|ConvertFrom-Json
    Check ($failure.ok -eq $false -and $script:mockWrites -eq 1) 'Nontransactional tables cannot mutate permissions'
    [Environment]::ExitCode=0;$script:badEngine=$false;$opens=$script:mockOpens
    [IO.File]::AppendAllText($resourcesFile,' ');$failure=Invoke-Doc116LevelBridge $resourcesFile $false $true|ConvertFrom-Json
    Check ($failure.ok -eq $false -and $script:mockOpens -eq $opens) 'Resource hash mismatch stops before connection'
    [Environment]::ExitCode=0
}finally{Remove-Item Env:DOC116_AUTH_LEVEL_PERMISSION -ErrorAction SilentlyContinue;Remove-Item Env:DOC116_LEVEL_RESOURCES_SHA256 -ErrorAction SilentlyContinue}
[IO.File]::Delete($journal);[IO.File]::Delete($journal+'.lock');[IO.File]::Delete($resourcesFile)
[IO.Directory]::Delete($artifactDirectory);[IO.Directory]::Delete((Join-Path $TemporaryRoot 'tools/e2e'));[IO.Directory]::Delete((Join-Path $TemporaryRoot 'tools'))
[pscustomobject]@{status='PASS';checks=$script:checks;realDatabase=$false;permissionChanges=$false}|ConvertTo-Json -Compress
