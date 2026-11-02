param([switch]$AuthorizedPersistence,[switch]$AuthorizedTemporaryProfile24,[ValidateSet('Create','Rollback','PublicationRead')][string]$Mode='Create')
$ErrorActionPreference='Stop'
if(-not $AuthorizedPersistence -or -not $AuthorizedTemporaryProfile24){throw 'DOC111_PERSISTENCE_AUTHORIZATION_REQUIRED'}
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$catalog=$null;$connection=$null;$runLock=$null;$profileRepo=$null;$adminContext=$null
$activationAttempted=$false;$initialProfile=$null;$initialHashes=$null;$phase='COMPILE'
$report=[ordered]@{status='FAIL';mode=$Mode;authorization='User approved persistence fixtures and temporary activation/restoration of E2E profile 24/version 38, then continued rollback verification';scope='Repository integration only; no HTTP login, document effects, permission grants or configuration content edits; profile 24 availability restored in finally';startedUtc=[DateTime]::UtcNow.ToString('o')}
$resultName=if($Mode -eq 'Rollback'){'doc111-rollback-result.json'}elseif($Mode -eq 'PublicationRead'){'doc111-publication-integration-result.json'}else{'doc111-persistence-result.json'}
function Rows([string]$sql) {
    if($sql -notmatch '\ASELECT\b' -or $sql.Contains(';')){throw 'DOC111_SELECT_REQUIRED'}
    $cmd=$connection.CreateCommand();$cmd.CommandText=$sql;$cmd.CommandTimeout=15
    $reader=$cmd.ExecuteReader();$items=New-Object 'System.Collections.Generic.List[object]'
    try{while($reader.Read()){$row=[ordered]@{};for($i=0;$i -lt $reader.FieldCount;$i++){$row[$reader.GetName($i)]=if($reader.IsDBNull($i)){$null}else{$reader.GetValue($i)}};$items.Add([pscustomobject]$row)}}finally{$reader.Dispose();$cmd.Dispose()}
    return ,$items.ToArray()
}
function Hash-Text([string]$text) {
    $sha=[Security.Cryptography.SHA256]::Create()
    try{return ([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($text)))).Replace('-','').ToLowerInvariant()}finally{$sha.Dispose()}
}
function Hash-Rows([string]$sql) {return Hash-Text (ConvertTo-Json -InputObject (Rows $sql) -Depth 30 -Compress)}
function Content-Hashes {
    return [ordered]@{
        profile=Hash-Rows 'SELECT perfil_id,ambito_ref,codigo,nombre,descripcion,version_publicada_id,creado_por,creado_utc FROM ra_integracion_perfil WHERE perfil_id=24'
        versions=Hash-Rows 'SELECT * FROM ra_integracion_perfil_version WHERE perfil_id=24 ORDER BY version_id'
        rules=Hash-Rows 'SELECT * FROM ra_integracion_perfil_regla WHERE version_id IN (SELECT version_id FROM ra_integracion_perfil_version WHERE perfil_id=24) ORDER BY regla_id'
        otherProfiles=Hash-Rows 'SELECT * FROM ra_integracion_perfil WHERE perfil_id<>24 ORDER BY perfil_id'
        permissions=Hash-Rows 'SELECT * FROM remit_dest_interno_perfil_produccion WHERE remit_dest_interno_idremit_dest_interno=141'
    }
}
function Operational-Counts {
    return ,(Rows "SELECT 'plan' AS entity,COUNT(*) AS total FROM ra_integracion_plan UNION ALL SELECT 'lote',COUNT(*) FROM ra_integracion_lote UNION ALL SELECT 'raiz',COUNT(*) FROM ra_integracion_raiz UNION ALL SELECT 'archivo',COUNT(*) FROM ra_integracion_archivo UNION ALL SELECT 'intento',COUNT(*) FROM ra_integracion_intento UNION ALL SELECT 'evento',COUNT(*) FROM ra_integracion_evento")
}
function Operational-Hashes {
    $hashes=[ordered]@{}
    foreach($entity in @('plan','lote','raiz','archivo','intento','evento')){$hashes[$entity]=Hash-Rows ('SELECT * FROM ra_integracion_'+$entity+' ORDER BY '+$entity+'_id')}
    return $hashes
}
function Allocation-Metadata {
    return ,(Rows "SELECT TABLE_NAME AS entity,AUTO_INCREMENT AS nextIdentity FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN ('ra_integracion_plan','ra_integracion_lote','ra_integracion_raiz','ra_integracion_archivo','ra_integracion_intento','ra_integracion_evento') ORDER BY TABLE_NAME")
}
function Error-Codes($errorRecord) {
    $codes=@();$exception=$errorRecord.Exception
    while($exception){
        $codes+=$exception.GetType().Name
        if($exception.PSObject.Properties.Name -contains 'Codigo'){$codes+=[string]$exception.Codigo}
        if($exception.Message -match '\ADOC111[_ A-Za-z0-9:.-]+\z'){$codes+=$exception.Message}
        $exception=$exception.InnerException
    }
    return ,$codes
}
try {
    $runLock=New-Object IO.FileStream((Join-Path $root '.tmp/doc111-persistence.lock'),[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
    Push-Location $root
    try {
        & node tools/e2e/scripts/compile-doc111-persistence.cjs
        if($LASTEXITCODE -ne 0){throw 'DOC111_HARNESS_COMPILE_FAILED'}
    }finally{Pop-Location}
    Add-Type -Path (Join-Path $root 'bin/MySql.Data.dll')
    Add-Type -Path (Join-Path $root 'packages/Newtonsoft.Json.13.0.2/lib/net45/Newtonsoft.Json.dll')
    Add-Type -Path (Join-Path $root '.tmp/doc111-persistence-harness/PerfilImportacion.dll')
    $null=[Reflection.Assembly]::LoadFrom((Join-Path $root '.tmp/doc111-persistence-harness/PerfilImportacionTests.exe'))
    $phase='CONNECTION_RESOLUTION'
    $config=[xml](Get-Content -LiteralPath (Join-Path $root 'Web.config') -Raw)
    $catalog=New-Object Data.Odbc.OdbcConnection
    $catalog.ConnectionString=($config.configuration.connectionStrings.add|Where-Object name -eq 'OdbcServicesGestor').connectionString
    $catalog.Open();$cmd=$catalog.CreateCommand()
    $cmd.CommandText="SELECT IP_SERVER_MODULO,DB_NAME_MODULO,USER_DBMS_MODULO,PASW_DBMS_MODULO FROM gestor_modulos WHERE ESTADO_MODULO=1 AND TIPO_MODULO='GESTOR DOCUMENTAL'"
    $reader=$cmd.ExecuteReader()
    try {
        if(-not $reader.Read()){throw 'DOC111_MODULE_REQUIRED'}
        $builder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder
        $builder.Server=[string]$reader[0];$builder.Database=[string]$reader[1];$builder.UserID=[string]$reader[2];$builder.Password=[string]$reader[3]
        if($reader.Read()){throw 'DOC111_MODULE_AMBIGUOUS'}
    }finally{$reader.Dispose();$cmd.Dispose();$catalog.Dispose();$catalog=$null}
    $report.environmentHash=Hash-Text ($builder.Server+'|'+$builder.Database)
    if($report.environmentHash -ne '24cce9f4ae03e1b92b7f0cba1e89f395e9b4434f4c108f5ee571ae9515387b91'){throw 'DOC111_AUTHORIZED_ENVIRONMENT_CHANGED'}
    $connection=New-Object MySql.Data.MySqlClient.MySqlConnection($builder.ConnectionString);$connection.Open()
    $phase='FIXTURE_PREFLIGHT'
    $report.beforeCounts=Operational-Counts
    if($Mode -ne 'Create'){
        $prior=Get-Content -LiteralPath (Join-Path $root 'tools/e2e/artifacts/doc111-persistence-result.json') -Raw|ConvertFrom-Json
        if($prior.status -ne 'PASS' -or $prior.fixtureIds.planId -ne '1' -or $prior.fixtureIds.secondPlanId -ne '2' -or $prior.fixtureIds.lotId -ne '1' -or -not $prior.profileRestoredInactive){throw 'DOC111_OWNED_PERSISTENCE_FIXTURE_REQUIRED'}
        $required=@{plan=2;lote=1;raiz=2;archivo=2;intento=0;evento=1}
        foreach($row in $report.beforeCounts){if([uint64]$row.total -ne $required[[string]$row.entity]){throw 'DOC111_EXISTING_OPERATIONAL_RECORDS_REVIEW_REQUIRED'}}
        $report.beforeOperationalHashes=Operational-Hashes
        $report.beforeAllocations=Allocation-Metadata
    }elseif(@($report.beforeCounts|Where-Object {[uint64]$_.total -ne 0}).Count -ne 0){throw 'DOC111_EXISTING_OPERATIONAL_RECORDS_REVIEW_REQUIRED'}
    $authority=Rows 'SELECT a.ADMINISTRAR_PERFILES_IMPORTACION FROM remit_dest_interno_perfil_produccion a INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=a.remit_dest_interno_idremit_dest_interno WHERE u.id_Remit_Dest_Int=141 AND u.Empresa_Gestion_Documental_id_empresa=2 AND u.Estado_Usuario=1'
    if($authority.Count -ne 1 -or [string]$authority[0].ADMINISTRAR_PERFILES_IMPORTACION -ne '1'){throw 'DOC111_EXISTING_ADMINISTRATIVE_AUTHORITY_REQUIRED'}
    $adminContext=New-Object ContextoPerfilImportacion
    $adminContext.UsuarioRef='141';$adminContext.AmbitoRef='2';$adminContext.PuedeAdministrar=$true
    $profileRepo=New-Object MySqlPerfilImportacionRepository($builder.ConnectionString)
    $profileRepo.VerificarEsquema()
    $initialProfile=$profileRepo.Obtener($adminContext,'24','38')
    $requiredProfileRevision=if($Mode -eq 'Rollback'){7}elseif($Mode -eq 'PublicationRead'){9}else{5}
    if($initialProfile.Activo -or $initialProfile.Revision -ne $requiredProfileRevision -or $initialProfile.VersionPublicadaId -ne '38' -or $initialProfile.Version.Estado -ne 'PUBLICADA' -or -not $initialProfile.Codigo.StartsWith('E2E',[StringComparison]::Ordinal) -or $initialProfile.Version.Reglas.Count -ne 5 -or @($initialProfile.Version.Reglas|Where-Object FuenteTipo -eq 'PROVEEDOR').Count -ne 0){throw 'DOC111_AUTHORIZED_PROFILE_CHANGED'}
    $snapshot=New-Object SnapshotPerfilImportacionOperativa
    $snapshot.PerfilId='24';$snapshot.VersionId='38';$snapshot.NumeroVersion=$initialProfile.Version.Numero
    $snapshot.Configuracion=$initialProfile.Version.Configuracion;$snapshot.Reglas=$initialProfile.Version.Reglas
    $snapshot.Huella=([BitConverter]::ToString([PerfilImportacionJson]::Huella($initialProfile.Version))).Replace('-','').ToLowerInvariant()
    $publishedHash=Rows 'SELECT LOWER(HEX(config_hash)) AS hash FROM ra_integracion_perfil_version WHERE version_id=38 AND perfil_id=24'
    if($publishedHash.Count -ne 1 -or $publishedHash[0].hash -cne $snapshot.Huella){throw 'DOC111_PUBLISHED_SNAPSHOT_HASH_MISMATCH'}
    $levels=Rows 'SELECT n.id_nivel FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int WHERE n.estado_nivel=1 AND u.Empresa_Gestion_Documental_id_empresa=2 AND (u.id_Remit_Dest_Int=141 OR EXISTS(SELECT 1 FROM ra_pro_permisos_niveles p WHERE p.ra_pro_niveles_id_nivel=n.id_nivel AND p.remit_dest_interno_id_Remit_Dest_Int=141 AND p.estado_permiso=1 AND p.agregar_expediente=1)) ORDER BY n.id_nivel LIMIT 1'
    if($levels.Count -ne 1){throw 'DOC111_EXISTING_DESTINATION_REQUIRED'}
    $levelId=[uint32]$levels[0].id_nivel;$currentId=$levelId
    $seen=New-Object 'System.Collections.Generic.HashSet[uint32]';$ancestors=New-Object 'System.Collections.Generic.List[object]'
    while($currentId -ne 0){
        if(-not $seen.Add($currentId) -or $seen.Count -gt 128){throw 'DOC111_DESTINATION_CHAIN_INVALID'}
        $nodes=Rows ('SELECT n.id_nivel,n.id_nivel_padre,n.nombre_nivel,n.estado_nivel,n.remit_dest_interno_id_Remit_Dest_Int FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int WHERE u.Empresa_Gestion_Documental_id_empresa=2 AND n.id_nivel='+$currentId.ToString([Globalization.CultureInfo]::InvariantCulture))
        if($nodes.Count -ne 1 -or [string]$nodes[0].estado_nivel -ne '1'){throw 'DOC111_DESTINATION_CHAIN_INVALID'}
        $ancestors.Add($nodes[0]);$currentId=[uint32]$nodes[0].id_nivel_padre
    }
    $context=New-Object ContextoImportacionCarpetas
    $context.UsuarioRef='141';$context.AmbitoRef='2';$context.NivelRef=$levelId.ToString([Globalization.CultureInfo]::InvariantCulture)
    $names=@($ancestors|ForEach-Object {[string]$_.nombre_nivel});[Array]::Reverse($names)
    $context.RutaCanonica=$names -join ' / ';$context.SelloDestino=Hash-Text (ConvertTo-Json -InputObject $ancestors.ToArray() -Depth 8 -Compress)
    # Contexto de fixture para probar el repositorio directamente; no concede ni
    # acredita IMPORTAR_CARPETAS en HTTP. La vía operativa/guard siguen pendientes.
    $context.PuedeImportar=$true
    if($Mode -ne 'Create' -and $context.NivelRef -ne '7'){throw 'DOC111_ORIGINAL_DESTINATION_CHANGED'}
    if($Mode -eq 'PublicationRead'){
        $versions=Rows "SELECT version_id,numero FROM ra_integracion_perfil_version WHERE perfil_id=24 AND estado='PUBLICADA' ORDER BY numero"
        if($versions.Count -ne 2 -or [string]$versions[0].version_id -ne '37' -or [string]$versions[1].version_id -ne '38'){throw 'DOC111_PUBLICATION_IDENTITIES_CHANGED'}
    }
    $initialHashes=Content-Hashes
    $report.profileId=24;$report.versionId=38;$report.levelId=$context.NivelRef;$report.profileRevisionBefore=$initialProfile.Revision
    $report.beforeHashes=$initialHashes
    $report.code=@{}
    foreach($relative in @('Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb','Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlPerfilImportacionRepository.vb','Services/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionJson.vb','tests/ImportacionCarpetasBehaviorTests.cs','tools/e2e/scripts/run-doc111-persistence.ps1')){$report.code[$relative]=(Get-FileHash -LiteralPath (Join-Path $root $relative) -Algorithm SHA256).Hash.ToLowerInvariant()}
    $phase='TEMPORARY_ACTIVATION';$activationAttempted=$true
    $enabled=$profileRepo.CambiarDisponibilidad($adminContext,'24',$initialProfile.Revision,$true)
    if(-not $enabled.Activo -or $enabled.Revision -ne ($requiredProfileRevision+1)){throw 'DOC111_TEMPORARY_ACTIVATION_NOT_CONFIRMED'}
    $report.activationConfirmed=$true
    if($Mode -eq 'Rollback'){
        $phase='ROLLBACK_INTEGRATION'
        $report.rollbackStages=@([ImportacionCarpetasBehaviorTests]::RunRollbackIntegration($true,$builder.ConnectionString,$context,'2','1'))
        $report.fixtureIds=[ordered]@{planId='1';secondPlanId='2';lotId='1'}
        $report.checks=@('PROVISIONAL_INVENTORY_OBSERVED_IN_REAL_TRANSACTION','FAILURE_BEFORE_EVENT_ROLLED_BACK','INSERTED_EVENT_ROLLED_BACK','CONFIRMED_PLAN_UPDATE_ROLLED_BACK','ORIGINAL_CONFIRMED_LOT_REPLAY_UNCHANGED')
    }elseif($Mode -eq 'PublicationRead'){
        $phase='PUBLICATION_READ_INTEGRATION'
        $report.publicationsRead=@([ImportacionCarpetasBehaviorTests]::RunPublicationIntegration($true,$builder.ConnectionString,$context))
        $report.fixtureIds=[ordered]@{planId='1';secondPlanId='2';lotId='1'}
        $report.checks=@('CURRENT_PUBLICATION_READ','HISTORICAL_VERSION_NOT_SUBSTITUTED','PUBLISHED_CURRENT_VERSION_LISTED','CALLER_MUTATION_DOES_NOT_CHANGE_DATABASE','UNKNOWN_VERSION_DENIED','ADMINISTRATIVE_GETTER_STILL_DENIED')
    }else{
        $phase='PERSISTENCE_INTEGRATION'
        $ids=[ImportacionCarpetasBehaviorTests]::RunPersistenceIntegration($true,$builder.ConnectionString,$context,$snapshot)
        $report.fixtureIds=[ordered]@{planId=$ids[0];secondPlanId=$ids[1];lotId=$ids[2]}
        $report.checks=@('PLAN_DATABASE_ROUNDTRIP','CONCURRENT_CONFIRMATION_ONE_LOT','SAME_KEY_REPLAY','SAME_PLAN_OTHER_KEY_REPLAY','CONFIRMED_REPLAY_AFTER_TTL','KEY_REUSE_OTHER_PLAN_DENIED','UNCONFIRMED_EXPIRED_PLAN_DENIED','INVENTORY_AND_EVENT_COUNTS','NO_DOCUMENT_ATTEMPTS')
    }
    $report.status='PASS'
}catch{
    $report.failedPhase=$phase;$report.failureCodes=Error-Codes $_
}finally{
    if($activationAttempted -and $profileRepo -and $initialProfile){
        try {
            $latest=$profileRepo.Obtener($adminContext,'24','38')
            if($latest.Activo){
                if($latest.Revision -ne ($initialProfile.Revision+1)){throw 'DOC111_PROFILE_CONCURRENT_CHANGE_REVIEW_REQUIRED'}
                $latest=$profileRepo.CambiarDisponibilidad($adminContext,'24',$latest.Revision,$false)
            }
            if($latest.Activo){throw 'DOC111_PROFILE_RESTORATION_NOT_CONFIRMED'}
            if($report.Contains('activationConfirmed') -and $latest.Revision -ne ($initialProfile.Revision+2)){throw 'DOC111_PROFILE_RESTORED_REVISION_UNEXPECTED'}
            $report.profileRestoredInactive=$true;$report.profileRevisionAfter=$latest.Revision
            $report.afterHashes=Content-Hashes
            $report.profileContentAndPermissionsUnchanged=$true
            foreach($key in $initialHashes.Keys){if($initialHashes[$key] -cne $report.afterHashes[$key]){$report.profileContentAndPermissionsUnchanged=$false}}
            if(-not $report.profileContentAndPermissionsUnchanged){$report.status='FAIL';$report.preservationFailure=$true}
        }catch{$report.status='RECOVERY_REQUIRED';$report.restorationFailureCodes=Error-Codes $_}
    }
    if($connection){
        try {
            $report.afterCounts=Operational-Counts
            if($Mode -ne 'Create'){
                $report.afterOperationalHashes=Operational-Hashes
                $report.afterAllocations=Allocation-Metadata
                $report.operationalDataUnchanged=$true
                if($report.Contains('beforeOperationalHashes')){
                    foreach($entity in $report.beforeOperationalHashes.Keys){if($report.beforeOperationalHashes[$entity] -cne $report.afterOperationalHashes[$entity]){$report.operationalDataUnchanged=$false}}
                    if(-not $report.operationalDataUnchanged){$report.status='FAIL'}
                }
            }
            $planRows=Rows 'SELECT plan_id FROM ra_integracion_plan WHERE version_id=38 AND usuario_ref=141 AND ambito_ref=2 ORDER BY plan_id'
            $report.disposablePlanIds=@($planRows|ForEach-Object {[string]$_.plan_id})
            if($report.status -eq 'PASS'){
                $expected=@{plan=2;lote=1;raiz=2;archivo=2;intento=0;evento=1}
                foreach($row in $report.afterCounts){if([uint64]$row.total -ne $expected[[string]$row.entity]){$report.status='FAIL';$report.countFailure=$true}}
            }
        }catch{$report.status='FAIL';$report.finalSelectFailureCodes=Error-Codes $_}
        $connection.Dispose()
    }
    if($catalog){$catalog.Dispose()}
    $report.finishedUtc=[DateTime]::UtcNow.ToString('o')
    [IO.File]::WriteAllText((Join-Path $root ('tools/e2e/artifacts/'+$resultName)),($report|ConvertTo-Json -Depth 12),(New-Object Text.UTF8Encoding($false)))
    if($runLock){$runLock.Dispose()}
}
$report|ConvertTo-Json -Depth 12 -Compress
if($report.status -ne 'PASS'){exit 1}
