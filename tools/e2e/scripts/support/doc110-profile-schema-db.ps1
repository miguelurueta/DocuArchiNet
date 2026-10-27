param([switch]$Inspect,[switch]$Recover,[switch]$ProbeSandbox)
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../../..'))
$connection=$null;$catalog=$null;$restoreRequired=$false
$names=@('ra_integracion_perfil','ra_integracion_perfil_version','ra_integracion_perfil_regla')
$temporary=@('doc110_p75_perfil','doc110_p75_version','doc110_p75_regla')
$keys=@('perfil_id','version_id','regla_id')
$recordFile=Join-Path $root ('tools/e2e/artifacts/doc110-schema-recovery'+$(if($ProbeSandbox){'-sandbox'}else{''})+'.json')
function Hash-Value($Value){
    $sha=[Security.Cryptography.SHA256]::Create()
    try{return ([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes((ConvertTo-Json -InputObject $Value -Compress -Depth 12))))).Replace('-','').ToLowerInvariant()}
    finally{$sha.Dispose()}
}
function Open-Database {
    Add-Type -Path (Join-Path $root 'bin/MySql.Data.dll')
    $config=[xml](Get-Content -LiteralPath (Join-Path $root 'Web.config') -Raw)
    $script:catalog=New-Object Data.Odbc.OdbcConnection
    $script:catalog.ConnectionString=($config.configuration.connectionStrings.add | Where-Object name -eq 'OdbcServicesGestor').connectionString
    $script:catalog.Open();$cmd=$script:catalog.CreateCommand()
    $cmd.CommandText="SELECT IP_SERVER_MODULO,DB_NAME_MODULO,USER_DBMS_MODULO,PASW_DBMS_MODULO FROM gestor_modulos WHERE ESTADO_MODULO=1 AND TIPO_MODULO='GESTOR DOCUMENTAL'"
    $reader=$cmd.ExecuteReader()
    try{
        if(-not $reader.Read()){throw 'MODULE_REQUIRED'}
        $script:builder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder
        $script:builder.Server=[string]$reader[0];$script:builder.Database=[string]$reader[1];$script:builder.UserID=[string]$reader[2];$script:builder.Password=[string]$reader[3]
        if($reader.Read()){throw 'MODULE_AMBIGUOUS'}
        if($ProbeSandbox){$script:builder.Database='doc110_instalacion_prueba'}
    }finally{$reader.Dispose();$cmd.Dispose();$script:catalog.Dispose();$script:catalog=$null}
    $script:connection=New-Object MySql.Data.MySqlClient.MySqlConnection($script:builder.ConnectionString);$script:connection.Open()
    $script:environmentHash=Hash-Value ($script:builder.Server+'|'+$script:builder.Database)
}
function Rows([string]$Sql,[hashtable]$Parameters=@{}){
    if($Sql -notmatch '\ASELECT\b' -or $Sql.Contains(';')){throw 'SELECT_REQUIRED'}
    $script:queryKind=if($Sql.Contains('information_schema.KEY_COLUMN_USAGE') -or $Sql.Contains('information_schema.REFERENTIAL_CONSTRAINTS')){'FOREIGN_KEY_METADATA'}elseif($Sql.Contains('information_schema.COLUMNS')){'COLUMN_METADATA'}elseif($Sql.Contains('information_schema.STATISTICS')){'INDEX_METADATA'}elseif($Sql.Contains('information_schema.TABLES')){'TABLE_METADATA'}else{'DATA_SNAPSHOT'}
    $cmd=$script:connection.CreateCommand();$cmd.CommandText=$Sql;$cmd.CommandTimeout=10
    foreach($key in $Parameters.Keys){[void]$cmd.Parameters.AddWithValue($key,$Parameters[$key])}
    $watch=[Diagnostics.Stopwatch]::StartNew();$reader=$null;$rows=New-Object 'System.Collections.Generic.List[object]'
    try{
        $reader=$cmd.ExecuteReader()
        while($reader.Read()){
            if($rows.Count -ge 50000){throw 'ROW_LIMIT'}
            $row=[ordered]@{}
            for($i=0;$i -lt $reader.FieldCount;$i++){$row[$reader.GetName($i)]=if($reader.IsDBNull($i)){$null}else{$reader.GetValue($i)}}
            $rows.Add([pscustomobject]$row)
        }
    }finally{
        if($reader){$reader.Dispose()};$cmd.Dispose();$watch.Stop()
        if(-not $script:slowestQuery -or $watch.ElapsedMilliseconds -gt $script:slowestQuery.milliseconds){$script:slowestQuery=[pscustomobject]@{kind=$script:queryKind;milliseconds=$watch.ElapsedMilliseconds}}
    }
    return ,$rows.ToArray()
}
function Table-Exists([string]$Name){
    # Solo nombres constantes del script.
    $rows=Rows "SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='$Name' AND ENGINE='InnoDB'"
    return $rows.Count -eq 1
}
function Layout {
    return [pscustomobject]@{original=@($names | Where-Object {Table-Exists $_}).Count;temporary=@($temporary | Where-Object {Table-Exists $_}).Count}
}
function Snapshot {
    $snapshot=[ordered]@{}
    for($i=0;$i -lt 3;$i++){
        $table=$names[$i];$key=$keys[$i]
        $data=Rows "SELECT * FROM $table ORDER BY $key"
        $columns=Rows "SELECT COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,COLUMN_DEFAULT,EXTRA,COLLATION_NAME FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='$table' ORDER BY ORDINAL_POSITION"
        $indexes=Rows "SELECT INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME,SUB_PART FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='$table' ORDER BY INDEX_NAME,SEQ_IN_INDEX"
        $foreignKeys=Rows "SELECT CONSTRAINT_NAME,COLUMN_NAME,REFERENCED_TABLE_NAME,REFERENCED_COLUMN_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='$table' AND REFERENCED_TABLE_NAME IS NOT NULL ORDER BY CONSTRAINT_NAME,ORDINAL_POSITION"
        $next=Rows "SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='$table'"
        $snapshot[$table]=[pscustomobject]@{count=$data.Count;dataHash=(Hash-Value $data);columnsHash=(Hash-Value $columns);indexesHash=(Hash-Value $indexes);foreignKeysHash=(Hash-Value $foreignKeys);nextId=[string]$next[0].AUTO_INCREMENT}
    }
    return [pscustomobject]$snapshot
}
function Same-Snapshot($Expected,$Actual){
    foreach($table in $names){
        foreach($field in @('count','dataHash','columnsHash','indexesHash','foreignKeysHash','nextId')){
            if([string]$Expected.$table.$field -cne [string]$Actual.$table.$field){throw 'PROFILE_SNAPSHOT_CHANGED'}
        }
    }
}
function Save-Recovery([string]$Stage,[bool]$Restored){
    $record=[ordered]@{environmentHash=$script:environmentHash;stage=$Stage;restored=$Restored;snapshot=$script:baseline;updatedUtc=[DateTime]::UtcNow.ToString('o')}
    [void][IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($recordFile))
    [IO.File]::WriteAllText($recordFile,($record | ConvertTo-Json -Depth 8),[Text.Encoding]::UTF8)
}
function Rename-Profiles([bool]$Back){
    $parts=@()
    for($i=0;$i -lt 3;$i++){
        if($Back){$parts+=('`'+$temporary[$i]+'` TO `'+$names[$i]+'`')}
        else{$parts+=('`'+$names[$i]+'` TO `'+$temporary[$i]+'`')}
    }
    $cmd=$script:connection.CreateCommand();$cmd.CommandTimeout=15;$cmd.CommandText='RENAME TABLE '+($parts -join ', ')
    try{[void]$cmd.ExecuteNonQuery()}finally{$cmd.Dispose()}
}
function Restore-Profiles {
    if(-not $script:restoreRequired){return}
    try{$layout=Layout}catch{
        $script:connection.Dispose();$script:connection=New-Object MySql.Data.MySqlClient.MySqlConnection($script:builder.ConnectionString);$script:connection.Open();$layout=Layout
    }
    if($layout.original -eq 0 -and $layout.temporary -eq 3){Rename-Profiles $true}
    elseif($layout.original -ne 3 -or $layout.temporary -ne 0){throw 'TABLE_LAYOUT_REQUIRES_MANUAL_RECONCILIATION'}
    Same-Snapshot $script:baseline (Snapshot)
    $script:restoreRequired=$false;Save-Recovery 'RESTORED' $true
}
try{
    Open-Database
    if($Recover){
        $record=Get-Content -LiteralPath $recordFile -Raw | ConvertFrom-Json
        if($record.environmentHash -cne $script:environmentHash){throw 'RECOVERY_ENVIRONMENT_MISMATCH'}
        $script:baseline=$record.snapshot;$script:restoreRequired=$true;Restore-Profiles
        [pscustomobject]@{ok=$true;stage='RESTORED';profileDataAndSchemaUnchanged=$true} | ConvertTo-Json -Compress
    }else{
        $layout=Layout
        if($layout.original -ne 3 -or $layout.temporary -ne 0){throw 'INITIAL_LAYOUT_INVALID'}
        # MySQL 5.1 necesita filtro explícito del esquema hijo para no abrir de
        # una vez todos los catálogos en una consulta cercana al timeout.
        $schemas=Rows "SELECT SCHEMA_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME NOT IN ('information_schema','mysql','performance_schema','sys') ORDER BY SCHEMA_NAME"
        if($schemas.Count -gt 64){throw 'SCHEMA_INSPECTION_LIMIT'}
        foreach($schema in $schemas){
            $external=Rows "SELECT COUNT(*) AS total FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=@childSchema AND UNIQUE_CONSTRAINT_SCHEMA=DATABASE() AND REFERENCED_TABLE_NAME IN ('ra_integracion_perfil','ra_integracion_perfil_version','ra_integracion_perfil_regla') AND (CONSTRAINT_SCHEMA<>DATABASE() OR TABLE_NAME NOT IN ('ra_integracion_perfil','ra_integracion_perfil_version','ra_integracion_perfil_regla'))" @{'@childSchema'=[string]$schema.SCHEMA_NAME}
            if([long]$external[0].total -ne 0){throw 'EXTERNAL_FOREIGN_KEYS_PRESENT'}
        }
        $script:baseline=Snapshot
        if($Inspect){
            [pscustomobject]@{ok=$true;stage='INSPECT';originalTables=3;temporaryTables=0;externalForeignKeys=0;profiles=$script:baseline.ra_integracion_perfil.count;versions=$script:baseline.ra_integracion_perfil_version.count;rules=$script:baseline.ra_integracion_perfil_regla.count;slowestQuery=$script:slowestQuery} | ConvertTo-Json -Compress
        }else{
            if((Test-Path -LiteralPath $recordFile) -and -not (Get-Content -LiteralPath $recordFile -Raw | ConvertFrom-Json).restored){throw 'PREVIOUS_RECOVERY_REQUIRED'}
            if($ProbeSandbox){
                Save-Recovery 'HIDE_ATTEMPT' $false;$script:restoreRequired=$true;Rename-Profiles $false
                $hidden=Layout;if($hidden.original -ne 0 -or $hidden.temporary -ne 3){throw 'HIDE_UNCONFIRMED'}
                Restore-Profiles
                [pscustomobject]@{ok=$true;stage='SANDBOX_RENAME_RESTORE_PASS';profileDataAndSchemaUnchanged=$true} | ConvertTo-Json -Compress
            }else{
                [pscustomobject]@{ok=$true;stage='READY';slowestQuery=$script:slowestQuery} | ConvertTo-Json -Compress
                while($true){
                    $task=[Console]::In.ReadLineAsync();if(-not $task.Wait(300000)){break}
                    $line=$task.Result;if($null -eq $line){break}
                    $request=$line | ConvertFrom-Json
                    switch($request.operation){
                        'HIDE' {
                            if($script:restoreRequired){throw 'ALREADY_HIDDEN'}
                            Same-Snapshot $script:baseline (Snapshot)
                            Save-Recovery 'HIDE_ATTEMPT' $false;$script:restoreRequired=$true;Rename-Profiles $false
                            $hidden=Layout;if($hidden.original -ne 0 -or $hidden.temporary -ne 3){throw 'HIDE_UNCONFIRMED'}
                            Save-Recovery 'HIDDEN' $false
                            [pscustomobject]@{ok=$true;stage='HIDDEN';missingOriginalTables=3} | ConvertTo-Json -Compress
                        }
                        'LEGACY_COUNT' {
                            $legacy=Rows 'SELECT COUNT(*) AS total FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=467'
                            [pscustomobject]@{ok=$true;stage='LEGACY_COUNT';count=[long]$legacy[0].total} | ConvertTo-Json -Compress
                        }
                        'RESTORE' {
                            Restore-Profiles
                            [pscustomobject]@{ok=$true;stage='RESTORED';profileDataAndSchemaUnchanged=$true} | ConvertTo-Json -Compress
                        }
                        default {throw 'OPERATION_INVALID'}
                    }
                }
            }
        }
    }
}catch{
    $safeCode=if($_.Exception.Message -cmatch '^[A-Z_]{3,80}$'){$_.Exception.Message}else{'DOC110_SCHEMA_OPERATION_FAILED'}
    $baseError=$_.Exception.GetBaseException()
    $mysqlNumber=if($baseError -is [MySql.Data.MySqlClient.MySqlException]){$baseError.Number}else{$null}
    [pscustomobject]@{ok=$false;code=$safeCode;errorType=$baseError.GetType().Name;mysqlNumber=$mysqlNumber;queryKind=$script:queryKind;line=$_.InvocationInfo.ScriptLineNumber} | ConvertTo-Json -Compress
    exit 1
}finally{
    if($restoreRequired){
        try{Restore-Profiles;[pscustomobject]@{ok=$true;stage='RESTORED_AFTER_EXIT';profileDataAndSchemaUnchanged=$true} | ConvertTo-Json -Compress}
        catch{[pscustomobject]@{ok=$false;code='DOC110_SCHEMA_RECOVERY_REQUIRED'} | ConvertTo-Json -Compress;exit 1}
    }
    if($connection){$connection.Dispose()};if($catalog){$catalog.Dispose()}
}
