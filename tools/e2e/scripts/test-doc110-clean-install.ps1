param([switch]$Apply)
$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$target='doc110_instalacion_prueba'
$catalog=$null;$source=$null;$sandbox=$null
$stage='PREFLIGHT';$completed=0
$evidence=[ordered]@{status='RUNNING';database=$target;startedUtc=[DateTime]::UtcNow.ToString('o');ddlStatementsCompleted=0}
function Digest([string]$Text){
    $sha=[Security.Cryptography.SHA256]::Create()
    try{return ([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($Text)))).Replace('-','').ToLowerInvariant()}
    finally{$sha.Dispose()}
}
function Scalar($Connection,[string]$Sql){
    if($Sql -notmatch '\ASELECT\b' -or $Sql.Contains(';')){throw 'SELECT_REQUIRED'}
    $cmd=$Connection.CreateCommand();$cmd.CommandText=$Sql;$cmd.CommandTimeout=10
    try{return $cmd.ExecuteScalar()}finally{$cmd.Dispose()}
}
function Write-Sandbox([string]$Sql){
    if([string](Scalar $sandbox 'SELECT DATABASE()') -cne $target){throw 'SANDBOX_DATABASE_MISMATCH'}
    $cmd=$sandbox.CreateCommand();$cmd.CommandTimeout=15;$cmd.CommandText=$Sql
    try{[void]$cmd.ExecuteNonQuery()}finally{$cmd.Dispose()}
}
function Production-Type([string]$Name){
    $type=$assembly.GetType('GestionDocumental_Docuarchi.net.'+$Name,$false)
    if(-not $type){$type=$assembly.GetType($Name,$false)}
    if(-not $type){throw 'PRODUCTION_TYPE_REQUIRED'}
    return $type
}
try{
    $packagePath=Join-Path $repoRoot 'tools/e2e/artifacts/doc110-instalacion-nueva-preparada.sql'
    $package=[IO.File]::ReadAllText($packagePath)
    $expected='1b46bef87631ab775a5b1be846e71fd41ba9ca2d12a695fa415319d0e42a17a8'
    if((Digest $package) -cne $expected){throw 'APPROVED_PACKAGE_CHANGED'}
    $sql01=[IO.File]::ReadAllText((Join-Path $repoRoot 'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/01-perfil-importacion-mysql51.sql'))
    if((Digest $sql01) -cne '002e34b873b90d0b92d822aba0385e645fbdeaba645c94633eabf4394d7c0461'){throw 'INSTALL_SQL_CHANGED'}
    $statements=@(([regex]::Replace($package,'(?m)^\s*--[^\r\n]*','')).Split(';') | ForEach-Object {$_.Trim()} | Where-Object {$_})
    if($statements.Count -ne 8 -or $statements[0] -cne 'CREATE DATABASE doc110_instalacion_prueba CHARACTER SET ascii COLLATE ascii_bin' -or $statements[1] -cne 'USE doc110_instalacion_prueba'){throw 'PACKAGE_TARGET_INVALID'}
    Add-Type -Path (Join-Path $repoRoot 'bin/MySql.Data.dll')
    Add-Type -Path (Join-Path $repoRoot 'bin/Newtonsoft.Json.dll')
    $assembly=[Reflection.Assembly]::LoadFrom((Join-Path $repoRoot 'bin/GestionDocumental-Docuarchi.net.dll'))
    $repositoryType=Production-Type 'MySqlPerfilImportacionRepository'
    $contextType=Production-Type 'ContextoPerfilImportacion'
    $ruleType=Production-Type 'ReglaPerfilImportacion'
    $configPath=Join-Path $repoRoot 'Web.config';$configHash=(Get-FileHash -LiteralPath $configPath -Algorithm SHA256).Hash
    $config=[xml](Get-Content -LiteralPath $configPath -Raw)
    $catalog=New-Object Data.Odbc.OdbcConnection
    $catalog.ConnectionString=($config.configuration.connectionStrings.add | Where-Object name -eq 'OdbcServicesGestor').connectionString
    $catalog.Open();$cmd=$catalog.CreateCommand()
    $cmd.CommandText="SELECT IP_SERVER_MODULO,DB_NAME_MODULO,USER_DBMS_MODULO,PASW_DBMS_MODULO FROM gestor_modulos WHERE ESTADO_MODULO=1 AND TIPO_MODULO='GESTOR DOCUMENTAL'"
    $reader=$cmd.ExecuteReader()
    try{
        if(-not $reader.Read()){throw 'MODULE_REQUIRED'}
        $builder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder
        $builder.Server=[string]$reader[0];$builder.Database=[string]$reader[1];$builder.UserID=[string]$reader[2];$builder.Password=[string]$reader[3]
        if($reader.Read() -or $builder.Database -ceq $target){throw 'SOURCE_DATABASE_INVALID'}
    }finally{$reader.Dispose();$cmd.Dispose();$catalog.Dispose();$catalog=$null}
    $source=New-Object MySql.Data.MySqlClient.MySqlConnection($builder.ConnectionString);$source.Open()
    if([long](Scalar $source "SELECT COUNT(*) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME='doc110_instalacion_prueba'") -ne 0){throw 'TARGET_ALREADY_EXISTS_NO_RETRY'}
    $before=@([long](Scalar $source 'SELECT COUNT(*) FROM ra_integracion_perfil'),[long](Scalar $source 'SELECT COUNT(*) FROM ra_integracion_perfil_version'),[long](Scalar $source 'SELECT COUNT(*) FROM ra_integracion_perfil_regla'))
    $version=[string](Scalar $source 'SELECT VERSION()')
    if($version -notmatch '^5\.1\.'){throw 'MYSQL51_REQUIRED'}
    if(-not $Apply){
        [pscustomobject]@{status='READY_NOT_APPLIED';packageHash=$expected;statements=$statements.Count;targetAbsent=$true;productionTypesResolved=$true;serverVersion=$version} | ConvertTo-Json -Compress
        return
    }
    $stage='CREATE_ISOLATED_DATABASE'
    $sandbox=New-Object MySql.Data.MySqlClient.MySqlConnection($builder.ConnectionString);$sandbox.Open()
    $cmd=$sandbox.CreateCommand();$cmd.CommandTimeout=15;$cmd.CommandText=$statements[0]
    try{[void]$cmd.ExecuteNonQuery();$completed=1}finally{$cmd.Dispose()}
    $sandbox.ChangeDatabase($target);$completed=2
    foreach($sql in $statements[2..7]){$stage='DDL_'+($completed+1);Write-Sandbox $sql;$completed++}
    $evidence.ddlStatementsCompleted=$completed
    $stage='VERIFY_FRESH_SCHEMA'
    if([long](Scalar $sandbox 'SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE()') -ne 5){throw 'ISOLATED_TABLE_SET_INVALID'}
    $permission=[long](Scalar $sandbox "SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='remit_dest_interno_perfil_produccion' AND COLUMN_NAME='ADMINISTRAR_PERFILES_IMPORTACION' AND COLUMN_DEFAULT='0' AND IS_NULLABLE='NO'")
    if($permission -ne 1){throw 'PERMISSION_DEFAULT_INVALID'}
    foreach($table in @('ra_integracion_perfil','ra_integracion_perfil_version','ra_integracion_perfil_regla','remit_dest_interno_perfil_produccion')){
        if([long](Scalar $sandbox ('SELECT COUNT(*) FROM '+$table)) -ne 0){throw 'FRESH_EMPTY_STATE_REQUIRED'}
    }
    $sandboxBuilder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder($builder.ConnectionString);$sandboxBuilder.Database=$target
    $repository=[Activator]::CreateInstance($repositoryType,[object[]]@($sandboxBuilder.ConnectionString))
    $repository.VerificarEsquema()
    $context=[Activator]::CreateInstance($contextType);$context.UsuarioRef='1';$context.AmbitoRef='1';$context.PuedeAdministrar=$true
    $profiles=@();$versions=@();$rules=@()
    $stage='PRODUCTION_REPOSITORY_INSERTIONS'
    foreach($index in 1..2){
        if([string](Scalar $sandbox 'SELECT DATABASE()') -cne $target){throw 'SANDBOX_DATABASE_MISMATCH'}
        $profile=$repository.Crear($context,('E2E-INSTALL-'+$index),'Prueba sintetica instalacion',$null)
        $rule=[Activator]::CreateInstance($ruleType)
        $rule.DestinoTipo='CAMPO_EXPEDIENTE';$rule.DestinoRef='CODIGO';$rule.FuenteTipo='CONSTANTE';$rule.TipoValor='TEXTO';$rule.ValorConstante='000123'
        $profile.Version.Reglas.Add($rule)
        $saved=$repository.Guardar($context,$profile.PerfilId,$profile.Version)
        $loaded=$repository.Obtener($context,$saved.PerfilId,$saved.Version.VersionId)
        if($loaded.Version.Reglas.Count -ne 1 -or $loaded.Version.Reglas[0].ValorConstante -cne '000123'){throw 'PERSISTENCE_ROUNDTRIP_FAILED'}
        $profiles+=@([string]$loaded.PerfilId);$versions+=@([string]$loaded.Version.VersionId);$rules+=@([string]$loaded.Version.Reglas[0].ReglaId)
        $inactive=$repository.CambiarDisponibilidad($context,$loaded.PerfilId,$loaded.Revision,$false)
        if($inactive.Activo){throw 'DISPOSABLE_PROFILE_NOT_INACTIVE'}
    }
    foreach($ids in @($profiles,$versions,$rules)){
        if(@($ids | Select-Object -Unique).Count -ne 2 -or @($ids | Where-Object {$_ -notmatch '^[1-9][0-9]*$'}).Count -gt 0){throw 'INCREMENTAL_IDS_NOT_UNIQUE'}
    }
    $counts=@([long](Scalar $sandbox 'SELECT COUNT(*) FROM ra_integracion_perfil'),[long](Scalar $sandbox 'SELECT COUNT(*) FROM ra_integracion_perfil_version'),[long](Scalar $sandbox 'SELECT COUNT(*) FROM ra_integracion_perfil_regla'))
    if(($counts -join ',') -cne '2,2,2'){throw 'INSERTION_COUNTS_INVALID'}
    if([long](Scalar $sandbox 'SELECT COUNT(*) FROM ra_integracion_perfil_version v LEFT JOIN ra_integracion_perfil p ON p.perfil_id=v.perfil_id WHERE p.perfil_id IS NULL') -ne 0 -or
       [long](Scalar $sandbox 'SELECT COUNT(*) FROM ra_integracion_perfil_regla r LEFT JOIN ra_integracion_perfil_version v ON v.version_id=r.version_id WHERE v.version_id IS NULL') -ne 0){throw 'ORPHAN_DETECTED'}
    $after=@([long](Scalar $source 'SELECT COUNT(*) FROM ra_integracion_perfil'),[long](Scalar $source 'SELECT COUNT(*) FROM ra_integracion_perfil_version'),[long](Scalar $source 'SELECT COUNT(*) FROM ra_integracion_perfil_regla'))
    if(($before -join ',') -cne ($after -join ',') -or (Get-FileHash -LiteralPath $configPath -Algorithm SHA256).Hash -cne $configHash){throw 'SOURCE_OR_CONFIGURATION_CHANGED'}
    $evidence.status='PASS';$evidence.serverVersion=$version;$evidence.packageSha256=$expected;$evidence.repositorySchemaVerified=$true
    $evidence.permissionDefaultZero=$true;$evidence.profileIds=$profiles;$evidence.versionIds=$versions;$evidence.ruleIds=$rules
    $evidence.counts=$counts;$evidence.orphans=0;$evidence.sourceCountsUnchanged=$true;$evidence.siteConfigUnchanged=$true
    $evidence.activeDllSha256=(Get-FileHash (Join-Path $repoRoot 'bin/GestionDocumental-Docuarchi.net.dll') -Algorithm SHA256).Hash
}catch{
    $evidence.status='FAIL';$evidence.stage=$stage;$evidence.ddlStatementsCompleted=$completed;$evidence.code='DOC110_CLEAN_INSTALL_CHECK_FAILED'
    [pscustomobject]@{status='FAIL';stage=$stage;ddlStatementsCompleted=$completed;code=$evidence.code;automaticCleanup=$false} | ConvertTo-Json -Compress
    [Environment]::ExitCode=1
}finally{
    if($sandbox){$sandbox.Dispose()};if($source){$source.Dispose()};if($catalog){$catalog.Dispose()}
    if($Apply){
        $evidence.finishedUtc=[DateTime]::UtcNow.ToString('o')
        $file=Join-Path $repoRoot 'tools/e2e/artifacts/doc110-clean-install-result.json'
        [IO.File]::WriteAllText($file,($evidence | ConvertTo-Json -Depth 5),[Text.Encoding]::UTF8)
        if($evidence.status -eq 'PASS'){[pscustomobject]$evidence | ConvertTo-Json -Compress -Depth 5}
    }
}
