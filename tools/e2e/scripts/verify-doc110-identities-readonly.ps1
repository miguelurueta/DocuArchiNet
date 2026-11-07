param([uint32]$DraftProfileId=0,[uint32]$PublishedProfileId=0)
$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$catalog=$null;$connection=$null
$checks=New-Object 'System.Collections.Generic.List[object]'
$stage='CONNECT'
function Read-Scalar([string]$Sql) {
    if($Sql -notmatch '\ASELECT\b' -or $Sql.Contains(';') -or $Sql -match '\b(INTO|OUTFILE|DUMPFILE|FOR\s+UPDATE|LOCK|SLEEP|BENCHMARK)\b'){throw 'DOC110_CONTROL_SELECT_REQUIRED'}
    $command=$connection.CreateCommand();$command.CommandText=$Sql;$command.CommandTimeout=10
    try {return $command.ExecuteScalar()}finally{$command.Dispose()}
}
function Check-Zero([string]$Id,[string]$Sql) {
    $script:stage=$Id;$count=[long](Read-Scalar $Sql)
    $checks.Add([pscustomobject]@{id=$Id;count=$count;passed=($count -eq 0)})
    if($count -ne 0){throw 'DOC110_CONTROL_INVARIANT_FAILED'}
}
try {
    if(($DraftProfileId -eq 0) -ne ($PublishedProfileId -eq 0)){throw 'DOC110_ACCEPTANCE_BOTH_PROFILE_IDS_REQUIRED'}
    Add-Type -Path (Join-Path $repoRoot 'bin/MySql.Data.dll')
    $configuration=[xml](Get-Content -LiteralPath (Join-Path $repoRoot 'Web.config') -Raw)
    $catalog=New-Object System.Data.Odbc.OdbcConnection
    $catalog.ConnectionString=($configuration.configuration.connectionStrings.add | Where-Object name -eq 'OdbcServicesGestor').connectionString
    $catalog.Open();$command=$catalog.CreateCommand()
    $command.CommandText="SELECT IP_SERVER_MODULO,DB_NAME_MODULO,USER_DBMS_MODULO,PASW_DBMS_MODULO FROM gestor_modulos WHERE ESTADO_MODULO=1 AND TIPO_MODULO='GESTOR DOCUMENTAL'"
    $reader=$command.ExecuteReader()
    try {
        if(-not $reader.Read()){throw 'DOC110_MODULE_REQUIRED'}
        $builder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder
        $builder.Server=[string]$reader[0];$builder.Database=[string]$reader[1];$builder.UserID=[string]$reader[2];$builder.Password=[string]$reader[3]
        if($reader.Read()){throw 'DOC110_MODULE_AMBIGUOUS'}
    }finally{$reader.Dispose();$command.Dispose();$catalog.Dispose();$catalog=$null}
    $connection=New-Object MySql.Data.MySqlClient.MySqlConnection($builder.ConnectionString)
    $connection.Open()
    $serverVersion=[string](Read-Scalar 'SELECT VERSION()')
    $profileCount=[long](Read-Scalar 'SELECT COUNT(*) FROM ra_integracion_perfil')
    $versionCount=[long](Read-Scalar 'SELECT COUNT(*) FROM ra_integracion_perfil_version')
    $ruleCount=[long](Read-Scalar 'SELECT COUNT(*) FROM ra_integracion_perfil_regla')
    if($profileCount -lt 1 -or $versionCount -lt 1){throw 'DOC110_EXISTING_PROFILE_DATA_REQUIRED'}
    Check-Zero 'PROFILE_INVALID_ID_REVISION' 'SELECT COUNT(*) FROM ra_integracion_perfil WHERE perfil_id=0 OR revision=0'
    Check-Zero 'VERSION_INVALID_ID_REVISION_NUMBER' 'SELECT COUNT(*) FROM ra_integracion_perfil_version WHERE version_id=0 OR perfil_id=0 OR revision=0 OR numero=0'
    Check-Zero 'RULE_INVALID_ID' 'SELECT COUNT(*) FROM ra_integracion_perfil_regla WHERE regla_id=0 OR version_id=0'
    Check-Zero 'DUPLICATE_PROFILE_CODE' 'SELECT COUNT(*) FROM (SELECT ambito_ref,codigo FROM ra_integracion_perfil GROUP BY ambito_ref,codigo HAVING COUNT(*)>1) d'
    Check-Zero 'DUPLICATE_VERSION_NUMBER' 'SELECT COUNT(*) FROM (SELECT perfil_id,numero FROM ra_integracion_perfil_version GROUP BY perfil_id,numero HAVING COUNT(*)>1) d'
    Check-Zero 'DUPLICATE_RULE_DESTINATION' 'SELECT COUNT(*) FROM (SELECT version_id,destino_tipo,destino_ref FROM ra_integracion_perfil_regla GROUP BY version_id,destino_tipo,destino_ref HAVING COUNT(*)>1) d'
    Check-Zero 'VERSION_NUMBER_GAPS' 'SELECT COUNT(*) FROM (SELECT perfil_id FROM ra_integracion_perfil_version GROUP BY perfil_id HAVING MIN(numero)<>1 OR MAX(numero)<>COUNT(*)) d'
    Check-Zero 'ORPHAN_VERSION' 'SELECT COUNT(*) FROM ra_integracion_perfil_version v LEFT JOIN ra_integracion_perfil p ON p.perfil_id=v.perfil_id WHERE p.perfil_id IS NULL'
    Check-Zero 'ORPHAN_RULE' 'SELECT COUNT(*) FROM ra_integracion_perfil_regla r LEFT JOIN ra_integracion_perfil_version v ON v.version_id=r.version_id WHERE v.version_id IS NULL'
    Check-Zero 'INVALID_PUBLISHED_REFERENCE' "SELECT COUNT(*) FROM ra_integracion_perfil p LEFT JOIN ra_integracion_perfil_version v ON v.version_id=p.version_publicada_id WHERE p.version_publicada_id IS NOT NULL AND (v.version_id IS NULL OR v.perfil_id<>p.perfil_id OR v.estado<>'PUBLICADA')"
    Check-Zero 'PUBLISHED_WITHOUT_HASH_OR_AUDIT' "SELECT COUNT(*) FROM ra_integracion_perfil_version WHERE estado='PUBLICADA' AND (config_hash IS NULL OR OCTET_LENGTH(config_hash)<>32 OR publicado_por IS NULL OR publicado_utc IS NULL)"
    $identityDefinitions=@(
        @('ra_integracion_perfil','perfil_id'),
        @('ra_integracion_perfil_version','version_id'),
        @('ra_integracion_perfil_regla','regla_id')
    )
    foreach($definition in $identityDefinitions){
        # Nombres constantes del contrato, nunca entrada del cliente.
        $table=$definition[0];$column=$definition[1];$stage='IDENTITY_SCHEMA_'+$table
        $columns=[long](Read-Scalar "SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='$table' AND COLUMN_NAME='$column' AND COLUMN_TYPE='int(10) unsigned' AND COLUMN_KEY='PRI' AND EXTRA='auto_increment'")
        $next=[long](Read-Scalar "SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='$table' AND ENGINE='InnoDB'")
        $maximum=[long](Read-Scalar "SELECT COALESCE(MAX($column),0) FROM $table")
        $passed=$columns -eq 1 -and $next -gt $maximum
        $checks.Add([pscustomobject]@{id=$stage;passed=$passed;nextGreaterThanMax=($next -gt $maximum)})
        if(-not $passed){throw 'DOC110_INCREMENTAL_IDENTITY_SCHEMA_INVALID'}
    }
    $cases=@()
    if($DraftProfileId -gt 0){
        foreach($entry in @(@('DRAFT',$DraftProfileId,1),@('PUBLISHED',$PublishedProfileId,2))){
            $stage='ACCEPTANCE_'+$entry[0];$id=[uint32]$entry[1];$expected=[long]$entry[2]
            $header=[long](Read-Scalar "SELECT COUNT(*) FROM ra_integracion_perfil WHERE perfil_id=$id AND activo=0 AND codigo LIKE 'E2E-%'")
            $versions=[long](Read-Scalar "SELECT COUNT(*) FROM ra_integracion_perfil_version WHERE perfil_id=$id")
            $rules=[long](Read-Scalar "SELECT COUNT(*) FROM ra_integracion_perfil_regla r INNER JOIN ra_integracion_perfil_version v ON v.version_id=r.version_id WHERE v.perfil_id=$id")
            if($entry[0] -eq 'DRAFT'){
                $states=[long](Read-Scalar "SELECT COUNT(*) FROM ra_integracion_perfil_version WHERE perfil_id=$id AND numero=1 AND estado='BORRADOR'")
            }else{
                $states=[long](Read-Scalar "SELECT COUNT(*) FROM ra_integracion_perfil_version WHERE perfil_id=$id AND estado='PUBLICADA' AND ((numero=1 AND ambito_integridad='EXPEDIENTE') OR (numero=2 AND ambito_integridad='LOTE'))")
            }
            $passed=$header -eq 1 -and $versions -eq $expected -and $states -eq $expected -and $rules -gt 0
            $checks.Add([pscustomobject]@{id=$stage;passed=$passed})
            $cases+=@([pscustomobject]@{kind=$entry[0];profileId=[string]$id;versions=$versions;rules=$rules;expectedStatesMatched=$states;inactive=($header -eq 1);passed=$passed})
            if(-not $passed){throw 'DOC110_ACCEPTANCE_PROFILE_STATE_INVALID'}
        }
    }
    $report=[pscustomobject]@{status='PASS';serverVersion=$serverVersion;profiles=$profileCount;versions=$versionCount;rules=$ruleCount;checks=$checks.ToArray();cases=$cases;finishedUtc=[DateTime]::UtcNow.ToString('o');scope='SELECT only; no new writes or migration'}
    $fileName=if($DraftProfileId -gt 0){'doc110-acceptance-select-result.json'}else{'doc110-identities-select-result.json'}
    $output=Join-Path $repoRoot ('tools/e2e/artifacts/'+$fileName)
    [void][IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($output))
    [IO.File]::WriteAllText($output,($report | ConvertTo-Json -Depth 6),[Text.Encoding]::UTF8)
    $report | Select-Object status,serverVersion,profiles,versions,rules,@{Name='passedChecks';Expression={$checks.Count}},finishedUtc | ConvertTo-Json -Compress
}catch{
    [pscustomobject]@{status='FAIL';stage=$stage;code='DOC110_READONLY_IDENTITY_CONTROL_FAILED'} | ConvertTo-Json -Compress
    exit 1
}finally{
    if($connection){$connection.Dispose()}
    if($catalog){$catalog.Dispose()}
}
