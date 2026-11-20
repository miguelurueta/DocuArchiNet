param([Parameter(Mandatory=$true)][string]$ResourcesFile,[Parameter(Mandatory=$true)][string]$RuntimeRoot,[switch]$Check,[switch]$Segmented,[switch]$FileReservations,[switch]$InspectFiles,[switch]$FixtureFiles)
$ErrorActionPreference='Stop'
$privateRepository=$Segmented -or $FileReservations
if($Segmented -and $FileReservations){throw 'DOC120_FILES_SCOPE_INVALID'}
if($InspectFiles -and -not $FileReservations){throw 'DOC120_FILES_SCOPE_INVALID'}
if($FixtureFiles -and (-not $FileReservations -or $InspectFiles)){throw 'DOC120_FILES_SCOPE_INVALID'}
if(-not $Check -and (($privateRepository -and $env:DOC120_SEGMENTED_AUTHORIZED -ne 'true') -or (-not $privateRepository -and $env:DOC115_07A_AUTHORIZED -ne 'true'))){throw 'DOC115_REPOSITORY_AUTHORIZATION_REQUIRED'}
. (Join-Path $PSScriptRoot 'doc115-readonly-failure.ps1')
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../../..'))
$scope=Get-Content -LiteralPath $ResourcesFile -Raw|ConvertFrom-Json
if($privateRepository){
    if($scope.runId -notmatch '\A[a-f0-9]{32}\z' -or $scope.userRef -ne '141' -or $scope.companyRef -ne '2' -or $scope.odbcDsn -ne 'MembershipUsers' -or $scope.cabinetRef -ne '10' -or $scope.environmentHash -notmatch '\A[a-f0-9]{64}\z' -or $scope.backendSha256 -notmatch '\A[a-f0-9]{64}\z' -or $scope.files.Count -ne 2 -or $scope.preserveResources -ne $true){throw 'DOC120_SEGMENTED_SCOPE_INVALID'}
    if($FileReservations){
        if($FixtureFiles){
            if($scope.scenario -ne 'DOC120-FILES-FIXTURE' -or $scope.levelRef -ne '7' -or $scope.profileRef -ne '24' -or $scope.versionRef -ne '38' -or ($scope.files -join ',') -cne 'a1,a2' -or $scope.previous -or $scope.inspectOnly -or $scope.baseUrl -ne 'https://localhost/GestionDocumental-Docuarchi.net/' -or $scope.fixtureSha256 -ne (Get-FileHash -LiteralPath (Join-Path $root 'tools/e2e/fixtures/doc111-carga-individual.pdf') -Algorithm SHA256).Hash.ToLowerInvariant()){throw 'DOC120_FILES_FIXTURE_SCOPE_INVALID'}
        }else{
        if($scope.scenario -ne 'DOC120-FILES-REPOSITORY' -or $scope.levelRef -ne '223' -or $scope.profileRef -ne '28' -or $scope.versionRef -ne '45' -or $scope.lotRef -ne '50' -or ($scope.files -join ',') -cne '122,123' -or $scope.previous){throw 'DOC120_FILES_SCOPE_INVALID'}
        if([bool]$scope.inspectOnly -ne [bool]$InspectFiles){throw 'DOC120_FILES_INSPECTION_SCOPE_REQUIRED'}
        }
    }else{
    if($scope.scenario -ne 'DOC120-SEGMENTED-REPOSITORY' -or $scope.levelRef -ne '7' -or $scope.profileRef -ne '24'){throw 'DOC120_SEGMENTED_SCOPE_INVALID'}
    $corpus=[IO.Path]::GetFullPath('D:\Pruebas\Importador\Egresos_20261113_092045_39126bb4')+[IO.Path]::DirectorySeparatorChar
    foreach($file in $scope.files){
        $path=[IO.Path]::GetFullPath([string]$file.path)
        if(-not $path.StartsWith($corpus,[StringComparison]::OrdinalIgnoreCase) -or [IO.Path]::GetExtension($path) -ine '.pdf' -or [IO.Path]::GetFileName($path) -cne $file.name -or [uint64]$file.bytes -lt 1 -or [uint64]$file.bytes -gt 2097152 -or $file.sha256 -notmatch '\A[a-f0-9]{64}\z' -or (Get-Item -LiteralPath $path).Length -ne [uint64]$file.bytes -or (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant() -ne $file.sha256){throw 'DOC120_SEGMENTED_CORPUS_CHANGED'}
    }
    if($scope.files[0].path -eq $scope.files[1].path -or $scope.files[0].name -eq $scope.files[1].name -or @($scope.sources.PSObject.Properties).Count -lt 10){throw 'DOC120_SEGMENTED_CORPUS_DUPLICATE'}
    }
    if(@($scope.sources.PSObject.Properties).Count -lt 10){throw 'DOC120_SEGMENTED_SOURCE_CHANGED'}
    foreach($source in $scope.sources.PSObject.Properties){if((Get-FileHash -LiteralPath (Join-Path $root $source.Name) -Algorithm SHA256).Hash.ToLowerInvariant() -ne $source.Value){throw 'DOC120_SEGMENTED_SOURCE_CHANGED'}}
    if([IO.Path]::GetFullPath($RuntimeRoot) -ne [IO.Path]::GetFullPath($scope.runtimeRoot) -or (Get-FileHash -LiteralPath (Join-Path $RuntimeRoot 'bin/GestionDocumental-Docuarchi.net.dll') -Algorithm SHA256).Hash.ToLowerInvariant() -ne $scope.backendSha256){throw 'DOC120_SEGMENTED_RUNTIME_CHANGED'}
    $segmentedTarget=Resolve-Doc115DsnTarget -Dsn $scope.odbcDsn -ExpectedHash $scope.environmentHash
    if($scope.previous){
        if($scope.previous.runId -notmatch '\A[a-f0-9]{32}\z' -or $scope.previous.journalSha256 -notmatch '\A[a-f0-9]{64}\z'){throw 'DOC120_SEGMENTED_PREVIOUS_INVALID'}
        $previousPath=Join-Path $root ('tools/e2e/artifacts/doc120-segmented-'+$scope.previous.runId+'.json')
        if((Get-FileHash -LiteralPath $previousPath -Algorithm SHA256).Hash.ToLowerInvariant() -ne $scope.previous.journalSha256){throw 'DOC120_SEGMENTED_PREVIOUS_CHANGED'}
        $previous=Get-Content -LiteralPath $previousPath -Raw|ConvertFrom-Json
        if($previous.status -ne 'STOPPED_REVIEW_REQUIRED' -or $previous.phase -ne 'REGISTERED_CONNECTION' -or $previous.failure.code -ne 'DOC115_AUTHORIZED_DATABASE_CHANGED' -or @($previous.references).Count -ne 0 -or $previous.wrapperSelects -ne 29){throw 'DOC120_SEGMENTED_PREVIOUS_EFFECTS_UNREVIEWED'}
        $previousConsolePath=Join-Path $root ('.tmp/doc120-segmented/'+$scope.previous.runId+'/resources.json.console-state.json')
        $previousConsole=Get-Content -LiteralPath $previousConsolePath -Raw|ConvertFrom-Json
        if($previousConsole.status -ne 'STOPPED' -or $previousConsole.credentialsCleared -ne $true){throw 'DOC120_SEGMENTED_PREVIOUS_UNCLOSED'}
    }
    if($Check){[Console]::Out.WriteLine('DOC120_SEGMENTED_CHECK=PASS_NOT_EXECUTED');exit 0}
    if((Get-FileHash -LiteralPath $ResourcesFile -Algorithm SHA256).Hash.ToLowerInvariant() -ne $env:DOC120_SEGMENTED_RESOURCES_SHA256){throw 'DOC120_SEGMENTED_SCOPE_CHANGED'}
}else{
if($scope.scenario -ne 'REPO-07A' -or $scope.preparations -ne 3 -or $scope.maximumCommittedLots -ne 2 -or $scope.receipts -ne 1 -or $scope.documentEffects -ne 0 -or $scope.rootNames.Count -ne 6 -or $scope.odbcDsn -ne 'MembershipUsers' -or $scope.userRef -ne '141' -or $scope.companyRef -ne '2' -or $scope.levelRef -ne '7' -or $scope.versionRef -ne '38' -or $scope.preserveResources -ne $true){throw 'DOC115_REPOSITORY_SCOPE_INVALID'}
if($scope.profileRef -ne '24' -or $scope.cabinetRef -ne '10' -or $scope.pdfFixture -ne 'doc111-carga-individual.pdf' -or $scope.backendSha256 -notmatch '\A[a-f0-9]{64}\z' -or $scope.fixtureSha256 -notmatch '\A[a-f0-9]{64}\z' -or $scope.environmentHash -notmatch '\A[a-f0-9]{64}\z' -or ($scope.rootNames|Select-Object -Unique).Count -ne 6 -or @($scope.rootNames|Where-Object {$_ -notmatch '\ADOC115-[0-9]{8}-07A-0[1-6]\z'}).Count -ne 0){throw 'DOC115_REPOSITORY_SCOPE_INVALID'}
if(-not $Check -and (Get-FileHash -LiteralPath $ResourcesFile -Algorithm SHA256).Hash.ToLowerInvariant() -ne $env:DOC115_07A_RESOURCES_SHA256){throw 'DOC115_REPOSITORY_SCOPE_CHANGED'}
if([IO.Path]::GetFullPath($RuntimeRoot) -ne [IO.Path]::GetFullPath($scope.runtimeRoot)){throw 'DOC115_RUNTIME_CHANGED'}
if((Get-FileHash -LiteralPath (Join-Path $RuntimeRoot 'bin/GestionDocumental-Docuarchi.net.dll') -Algorithm SHA256).Hash.ToLowerInvariant() -ne $scope.backendSha256){throw 'DOC115_BACKEND_CHANGED'}
if($Check){
    if(($scope.rootNames|Select-Object -Unique).Count -ne 6 -or @($scope.rootNames|Where-Object {$_ -notmatch '\ADOC115-[0-9]{8}-07A-0[1-6]\z'}).Count -ne 0){throw 'DOC115_FIXTURE_NAMES_INVALID'}
    if((Get-FileHash -LiteralPath (Join-Path $root ('tools/e2e/fixtures/'+$scope.pdfFixture)) -Algorithm SHA256).Hash.ToLowerInvariant() -ne $scope.fixtureSha256){throw 'DOC115_FIXTURE_CHANGED'}
    [Console]::Out.WriteLine('DOC115_07A_CHECK='+(@{status='PASS';realExecution=$false;preparations=3;maximumCommittedLots=2;receipts=1;injectedRollbacks=4;resourcesSha256=(Get-FileHash -LiteralPath $ResourcesFile -Algorithm SHA256).Hash.ToLowerInvariant()}|ConvertTo-Json -Compress));exit 0
}
}
if($privateRepository){$runId=[string]$scope.runId}else{$runId=$env:DOC115_07A_RUN_ID}
if($runId -notmatch '\A[a-f0-9]{32}\z'){throw 'DOC115_RUN_ID_REQUIRED'}
$journalPath=Join-Path $root ('tools/e2e/artifacts/'+$(if($FixtureFiles){'doc120-files-fixture-'}elseif($InspectFiles){'doc120-files-inspect-'}elseif($FileReservations){'doc120-files-'}elseif($Segmented){'doc120-segmented-'}else{'doc115-07a-'})+$runId+'.json')
if(Test-Path -LiteralPath $journalPath){throw 'DOC115_JOURNAL_ALREADY_EXISTS'}
if($privateRepository){$marker=[IO.File]::Open($journalPath,[IO.FileMode]::CreateNew,[IO.FileAccess]::Write,[IO.FileShare]::None);$marker.Dispose()}
$journal=[ordered]@{issueKey='DOC-115';scenario='REPO-07A';status='STARTING';phase='COMPILE';references=@();resourcesSha256=$env:DOC115_07A_RESOURCES_SHA256;backendSha256=$scope.backendSha256;startedUtc=[DateTime]::UtcNow.ToString('o')}
if($privateRepository){$journal.issueKey='DOC-120';$journal.scenario=$scope.scenario;$journal.resourcesSha256=$env:DOC120_SEGMENTED_RESOURCES_SHA256;$journal.wrapperSelects=0;$journal.wrapperSelectsThisRun=0;$journal.authorized=$true;if($scope.previous){$journal.previous=$scope.previous;$journal.wrapperSelects=29}}
$admission=[Diagnostics.Stopwatch]::StartNew()
function Save-Journal { [IO.File]::WriteAllText($journalPath,($journal|ConvertTo-Json -Depth 15),[Text.UTF8Encoding]::new($false)) }
$control=$null;$catalog=$null;$runLock=$null;$writerBuilder=$null
function Read-Rows([string]$sql,[object[]]$arguments=@()){
    if($sql -notmatch '\ASELECT\b' -or $sql -match ';|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO|LOCK|SLEEP|BENCHMARK)\b' -or ([regex]::Matches($sql,'\?')).Count -ne $arguments.Count){throw 'DOC115_SELECT_REQUIRED'}
    if($privateRepository){if($admission.Elapsed.TotalMinutes -ge 15 -or $journal.wrapperSelects -ge $(if($InspectFiles){12}else{96})){throw 'DOC120_SEGMENTED_WRAPPER_BUDGET'};$journal.wrapperSelects++;$journal.wrapperSelectsThisRun++}
    $journal.readonlyStep='SELECT';$journal.selectSha256=Hash-Text $sql;Save-Journal
    $cmd=$control.CreateCommand();$cmd.CommandText=$sql;$cmd.CommandTimeout=15
    foreach($value in $arguments){$p=$cmd.Parameters.Add('@p'+$cmd.Parameters.Count,[Data.Odbc.OdbcType]::VarChar);$p.Size=2048;$p.Value=if($null -eq $value){[DBNull]::Value}else{[Convert]::ToString($value,[Globalization.CultureInfo]::InvariantCulture)}}
    $reader=$cmd.ExecuteReader();$items=New-Object 'System.Collections.Generic.List[object]'
    try{while($reader.Read()){$row=[ordered]@{};for($i=0;$i -lt $reader.FieldCount;$i++){$row[$reader.GetName($i)]=if($reader.IsDBNull($i)){$null}elseif($reader.GetFieldType($i) -eq [byte[]]){[Convert]::ToBase64String([byte[]]$reader.GetValue($i))}else{[Convert]::ToString($reader.GetValue($i),[Globalization.CultureInfo]::InvariantCulture)}};$items.Add([pscustomobject]$row)}}finally{$reader.Dispose();$cmd.Dispose()}
    return ,$items.ToArray()
}
function Hash-Text([string]$text){$sha=[Security.Cryptography.SHA256]::Create();try{return ([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($text)))).Replace('-','').ToLowerInvariant()}finally{$sha.Dispose()}}
function Row-Hash([string]$sql,[object[]]$arguments=@()){return Hash-Text (ConvertTo-Json -InputObject (Read-Rows $sql $arguments) -Depth 15 -Compress)}
$entities=[ordered]@{plan='plan_id';lote='lote_id';raiz='raiz_id';archivo='archivo_id';intento='intento_id';evento='evento_id'}
if($privateRepository){$entities.preparacion='preparacion_id'}
function Protected-Hashes($maxima){
    $out=[ordered]@{}
    foreach($name in $entities.Keys){
        $filter=''
        if($FileReservations -and -not $FixtureFiles){$filter=switch($name){'lote'{' AND lote_id<>50'};'raiz'{' AND lote_id<>50'};'archivo'{' AND archivo_id NOT IN (122,123)'};default{''}}}
        $out[$name]=Row-Hash ('SELECT * FROM ra_integracion_'+$name+' WHERE '+$entities[$name]+'<=?'+$filter+' ORDER BY '+$entities[$name]) @([string]$maxima[$name])
    }
    if($FileReservations -and -not $FixtureFiles){
        foreach($name in @('lote','raiz','archivo')){
            $filter=switch($name){'lote'{'lote_id=50'};'raiz'{'lote_id=50'};'archivo'{'archivo_id IN (122,123)'}}
            $rows=Read-Rows ('SELECT * FROM ra_integracion_'+$name+' WHERE '+$filter+' ORDER BY '+$entities[$name])
            foreach($row in $rows){foreach($column in @('revision','actualizado_utc')+$(if($name -eq 'archivo'){@('estado_efecto','intento_activo_id','resultado_doc')}else{@('estado')})){$row.PSObject.Properties.Remove($column)}}
            $out[$name+'Originals']=Hash-Text (ConvertTo-Json -InputObject $rows -Depth 15 -Compress)
        }
    }
    $out.profiles=Row-Hash 'SELECT * FROM ra_integracion_perfil ORDER BY perfil_id';$out.versions=Row-Hash 'SELECT * FROM ra_integracion_perfil_version ORDER BY version_id';$out.rules=Row-Hash 'SELECT * FROM ra_integracion_perfil_regla ORDER BY regla_id';$out.permissions=Row-Hash ('SELECT * FROM remit_dest_interno_perfil_produccion ORDER BY '+$script:permissionOrder)
    if($FixtureFiles){$out.expedientes=Row-Hash 'SELECT * FROM expediente_archivo WHERE ID_EXPEDIENTE<=? ORDER BY ID_EXPEDIENTE' @($script:physicalMax.exp);$out.relations=Row-Hash 'SELECT * FROM ra_pro_niveles_has_expediente_archivo WHERE id_registro<=? ORDER BY id_registro' @($script:physicalMax.rel)}else{$out.expedientes=Row-Hash 'SELECT COUNT(*) AS total FROM expediente_archivo'}
    $out.documents=Row-Hash ('SELECT COUNT(*) AS total FROM `'+$script:cabinetTable+'`');$out.inventories=Row-Hash 'SELECT COUNT(*) AS total FROM registro_producion_documental';return $out
}
function Fixture-Hashes($lot,$attemptMax,$eventMax){
    $out=[ordered]@{}
    foreach($name in @('lote','raiz','archivo')){
        $sql=if($name -eq 'archivo'){'SELECT a.* FROM ra_integracion_archivo a INNER JOIN ra_integracion_raiz r ON r.raiz_id=a.raiz_id WHERE r.lote_id=? ORDER BY a.archivo_id'}else{'SELECT * FROM ra_integracion_'+$name+' WHERE lote_id=? ORDER BY '+$entities[$name]}
        $rows=Read-Rows $sql @($lot)
        foreach($row in $rows){foreach($column in @('revision','actualizado_utc')+$(if($name -eq 'archivo'){@('estado_efecto','intento_activo_id','resultado_doc')}else{@('estado')})){$row.PSObject.Properties.Remove($column)}}
        $out[$name]=Hash-Text (ConvertTo-Json -InputObject $rows -Depth 15 -Compress)
    }
    $out.attempts=Row-Hash 'SELECT * FROM ra_integracion_intento WHERE lote_id=? AND intento_id<=? ORDER BY intento_id' @($lot,$attemptMax)
    $out.events=Row-Hash 'SELECT * FROM ra_integracion_evento WHERE lote_id=? AND evento_id<=? ORDER BY evento_id' @($lot,$eventMax)
    return $out
}
try{
    Save-Journal
    $runLock=New-Object IO.FileStream((Join-Path $root '.tmp/doc115-07a.lock'),[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
    Push-Location $root
    try{& node tools/e2e/scripts/compile-doc111-persistence.cjs;if($LASTEXITCODE -ne 0){throw 'DOC115_HARNESS_COMPILE_FAILED'}}finally{Pop-Location}
    Add-Type -Path (Join-Path $root 'bin/MySql.Data.dll');Add-Type -Path (Join-Path $root 'packages/Newtonsoft.Json.13.0.2/lib/net45/Newtonsoft.Json.dll')
    Add-Type -Path (Join-Path $root '.tmp/doc111-persistence-harness/PerfilImportacion.dll')
    $null=[Reflection.Assembly]::LoadFrom((Join-Path $root '.tmp/doc111-persistence-harness/PerfilImportacionTests.exe'))
    $journal.phase='READONLY_PREFLIGHT';Save-Journal
    $journal.readonlyStep='OPEN_CONNECTION';Save-Journal
    $ro=New-Object Data.Odbc.OdbcConnectionStringBuilder;$ro['DSN']=$scope.odbcDsn;$ro['UID']=$env:DOC111_E2E_MYSQL_USER;$ro['PWD']=$env:DOC111_E2E_MYSQL_PASSWORD
    if([string]::IsNullOrWhiteSpace($env:DOC111_E2E_MYSQL_USER) -or [string]::IsNullOrWhiteSpace($env:DOC111_E2E_MYSQL_PASSWORD)){throw 'DOC115_READONLY_CREDENTIALS_REQUIRED'}
    $control=New-Object Data.Odbc.OdbcConnection($ro.ConnectionString);$control.Open()
    if($InspectFiles){
        $journal.phase='READONLY_FILE_INSPECTION';Save-Journal
        $actualDatabase=Read-Rows 'SELECT DATABASE() AS database_ref'
        if($actualDatabase.Count -ne 1 -or $actualDatabase[0].database_ref -cne $segmentedTarget.database){throw 'DOC115_CONTROL_DATABASE_MISMATCH'}
        $lots=Read-Rows 'SELECT l.lote_id,l.plan_id,l.nivel_ref,l.version_id,l.estado,l.detencion_solicitada,l.total_archivos,v.perfil_id FROM ra_integracion_lote l INNER JOIN ra_integracion_perfil_version v ON v.version_id=l.version_id WHERE l.lote_id IN (29,50) AND l.solicitante_ref=141 AND l.ambito_ref=2 ORDER BY l.lote_id'
        if($lots.Count -ne 2){throw 'DOC120_FILES_LOT_CHANGED'}
        $targets=Read-Rows 'SELECT a.archivo_id,r.lote_id,a.estado_recepcion,a.estado_efecto,(a.intento_activo_id IS NOT NULL) AS activo,(a.resultado_doc IS NOT NULL) AS resultado,(a.valores_efectivos_doc IS NOT NULL) AS valores FROM ra_integracion_archivo a INNER JOIN ra_integracion_raiz r ON r.raiz_id=a.raiz_id WHERE r.lote_id=50 AND a.archivo_id IN (122,123) ORDER BY a.archivo_id'
        $candidates=@();$states=@()
        foreach($lot in $lots){
            $group=Read-Rows 'SELECT a.estado_recepcion,a.estado_efecto,(a.valores_efectivos_doc IS NOT NULL) AS valores,COUNT(*) AS total FROM ra_integracion_archivo a INNER JOIN ra_integracion_raiz r ON r.raiz_id=a.raiz_id WHERE r.lote_id=? GROUP BY a.estado_recepcion,a.estado_efecto,a.valores_efectivos_doc IS NOT NULL ORDER BY a.estado_recepcion,a.estado_efecto,valores' @($lot.lote_id)
            foreach($row in $group){$row|Add-Member -NotePropertyName lote_id -NotePropertyValue $lot.lote_id};$states+=@($group)
            $available=Read-Rows "SELECT a.archivo_id,r.lote_id,a.revision FROM ra_integracion_archivo a INNER JOIN ra_integracion_raiz r ON r.raiz_id=a.raiz_id WHERE r.lote_id=? AND a.estado_recepcion='RECIBIDO' AND a.estado_efecto='NO_INICIADO' AND a.resultado_doc IS NULL AND a.intento_activo_id IS NULL AND a.valores_efectivos_doc IS NOT NULL AND r.identidad_expediente_doc IS NOT NULL AND r.intento_activo_id IS NULL ORDER BY a.archivo_id LIMIT 4" @($lot.lote_id)
            $candidates+=@($available)
        }
        $journal.result=@{status='PASS_READONLY_INSPECTION';lots=$lots;targets=$targets;candidates=$candidates;states=$states;mutations=0;writerConnections=0};$journal.status='PASS';$journal.finishedUtc=[DateTime]::UtcNow.ToString('o');Save-Journal
        [Console]::Out.WriteLine('DOC115_07A_RESULT='+($journal.result|ConvertTo-Json -Depth 6 -Compress));return
    }
    if($Segmented -or $FixtureFiles){
        $publication=Read-Rows 'SELECT p.perfil_id,p.version_publicada_id,p.revision,p.activo FROM ra_integracion_perfil p WHERE p.perfil_id=? AND p.ambito_ref=?' @($scope.profileRef,$scope.companyRef)
        if($publication.Count -ne 1 -or $publication[0].activo -ne '1' -or $publication[0].version_publicada_id -notmatch '\A[1-9][0-9]{0,9}\z'){throw 'DOC120_SEGMENTED_EXISTING_PROFILE_UNAVAILABLE'}
        if($FixtureFiles -and $publication[0].version_publicada_id -ne '38'){throw 'DOC120_FILES_PUBLICATION_CHANGED'}
        $scope|Add-Member -NotePropertyName versionRef -NotePropertyValue $publication[0].version_publicada_id -Force
        $journal.publication=@{profileId=$scope.profileRef;versionId=$scope.versionRef;levelId=$scope.levelRef};Save-Journal
    }
    $permissionKeys=Read-Rows "SELECT COLUMN_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=? AND CONSTRAINT_NAME='PRIMARY' ORDER BY ORDINAL_POSITION" @('remit_dest_interno_perfil_produccion')
    if($permissionKeys.Count -lt 1 -or $permissionKeys.Count -gt 8 -or @($permissionKeys|Where-Object {$_.COLUMN_NAME -notmatch '\A[A-Za-z_][A-Za-z0-9_]{0,63}\z'}).Count -ne 0){throw 'DOC115_PERMISSION_KEY_UNAVAILABLE'}
    $script:permissionOrder=(@($permissionKeys|ForEach-Object {'`'+$_.COLUMN_NAME+'`'}) -join ',')
    $version=Read-Rows 'SELECT gabinete_id FROM ra_integracion_perfil_version WHERE version_id=? AND perfil_id=?' @($scope.versionRef,$scope.profileRef)
    if($version.Count -ne 1 -or $version[0].gabinete_id -ne $scope.cabinetRef){throw 'DOC115_CABINET_REFERENCE_CHANGED'}
    $cabinet=Read-Rows 'SELECT NOMBRE FROM system1 WHERE id_gabinete=?' @($scope.cabinetRef)
    if($cabinet.Count -ne 1 -or $cabinet[0].NOMBRE -notmatch '\A[A-Za-z_][A-Za-z0-9_]{0,63}\z'){throw 'DOC115_CABINET_IDENTIFIER_INVALID'}
    $script:cabinetTable=[string]$cabinet[0].NOMBRE
    if(-not $privateRepository){$prior=Read-Rows 'SELECT manifiesto_doc FROM ra_integracion_plan WHERE usuario_ref=? AND ambito_ref=? AND nivel_ref=? AND version_id=? ORDER BY plan_id' @($scope.userRef,$scope.companyRef,$scope.levelRef,$scope.versionRef)
    foreach($row in $prior){$m=ConvertFrom-Json $row.manifiesto_doc;foreach($r in $m.Raices){if($scope.rootNames -contains $r.Nombre){throw 'DOC115_PRIOR_FIXTURE_REQUIRES_REVIEW'}}}}
    $authority=Read-Rows 'SELECT a.IMPORTAR_CARPETAS FROM remit_dest_interno_perfil_produccion a INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=a.remit_dest_interno_idremit_dest_interno WHERE u.id_Remit_Dest_Int=? AND u.Empresa_Gestion_Documental_id_empresa=? AND u.Estado_Usuario=1' @($scope.userRef,$scope.companyRef)
    if($authority.Count -ne 1 -or $authority[0].IMPORTAR_CARPETAS -ne '1'){throw 'DOC115_EXISTING_AUTHORITY_REQUIRED'}
    $level=Read-Rows 'SELECT n.id_nivel FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int WHERE n.id_nivel=? AND n.estado_nivel=1 AND u.Empresa_Gestion_Documental_id_empresa=? AND (u.id_Remit_Dest_Int=? OR EXISTS(SELECT 1 FROM ra_pro_permisos_niveles p WHERE p.ra_pro_niveles_id_nivel=n.id_nivel AND p.remit_dest_interno_id_Remit_Dest_Int=? AND p.estado_permiso=1 AND p.agregar_expediente=1))' @($scope.levelRef,$scope.companyRef,$scope.userRef,$scope.userRef)
    if($level.Count -ne 1){throw 'DOC115_EXISTING_DESTINATION_REQUIRED'}
    $context=New-Object ContextoImportacionCarpetas;$context.UsuarioRef=$scope.userRef;$context.AmbitoRef=$scope.companyRef;$context.NivelRef=$scope.levelRef;$context.RutaCanonica='DOC-115 / fixture de repositorio / nivel '+$scope.levelRef;$context.SelloDestino=Hash-Text ('DOC115|'+$scope.userRef+'|'+$scope.companyRef+'|'+$scope.levelRef+'|'+$(if($privateRepository){$runId}else{$scope.rootNames[0]}));$context.PuedeImportar=$true
    if($FileReservations -and -not $FixtureFiles){
        $existingLot=Read-Rows 'SELECT contexto_doc,plan_id,version_id FROM ra_integracion_lote WHERE lote_id=50 AND solicitante_ref=? AND ambito_ref=? AND nivel_ref=?' @($scope.userRef,$scope.companyRef,$scope.levelRef)
        if($existingLot.Count -ne 1 -or $existingLot[0].plan_id -ne '90' -or $existingLot[0].version_id -ne '45'){throw 'DOC120_FILES_LOT_CHANGED'}
        $context=[Newtonsoft.Json.JsonConvert]::DeserializeObject($existingLot[0].contexto_doc,[ContextoImportacionCarpetas])
        $journal.publication=@{profileId=$scope.profileRef;versionId=$scope.versionRef;levelId=$scope.levelRef;lotId=$scope.lotRef}
    }
    $maxima=[ordered]@{};foreach($name in $entities.Keys){$row=Read-Rows ('SELECT COALESCE(MAX('+$entities[$name]+'),0) AS ultimo FROM ra_integracion_'+$name);$maxima[$name]=[uint64]$row[0].ultimo}
    if($FixtureFiles){$p=Read-Rows 'SELECT (SELECT COALESCE(MAX(ID_EXPEDIENTE),0) FROM expediente_archivo) AS exp,(SELECT COALESCE(MAX(id_registro),0) FROM ra_pro_niveles_has_expediente_archivo) AS rel';$script:physicalMax=$p[0];$journal.physicalMaxima=$script:physicalMax}
    $before=Protected-Hashes $maxima;$journal.protectedBefore=$before;$journal.existingMaxima=$maxima;Save-Journal
    if($privateRepository){
        foreach($partTable in @('preparacion_parte','preparacion_resultado')){$before[$partTable]=Row-Hash ('SELECT * FROM ra_integracion_'+$partTable+' WHERE preparacion_id<=? ORDER BY preparacion_id,tipo,numero') @([string]$maxima.preparacion)}
    }
    # Resolve the registered operational connection in memory, following the
    # existing persistence runner. Never print or persist module credentials.
    $journal.phase='REGISTERED_CONNECTION';Save-Journal
    if($privateRepository){
        if([string]::IsNullOrWhiteSpace($env:DOC120_SEGMENTED_WRITER_USER) -or [string]::IsNullOrWhiteSpace($env:DOC120_SEGMENTED_WRITER_PASSWORD)){throw 'DOC120_SEGMENTED_WRITER_CREDENTIALS_REQUIRED'}
        $writerBuilder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder
        $segmentedTarget=Resolve-Doc115DsnTarget -Dsn $scope.odbcDsn -ExpectedHash $scope.environmentHash
        $writerBuilder.Server=$segmentedTarget.server;$writerBuilder.Database=$segmentedTarget.database
        $writerBuilder.UserID=$env:DOC120_SEGMENTED_WRITER_USER;$writerBuilder.Password=$env:DOC120_SEGMENTED_WRITER_PASSWORD
        $writerBuilder.Pooling=$false;$writerBuilder.ConnectionTimeout=5
    }else{
    $config=[xml](Get-Content -LiteralPath (Join-Path $RuntimeRoot 'Web.config') -Raw)
    $catalog=New-Object Data.Odbc.OdbcConnection;$catalog.ConnectionString=($config.configuration.connectionStrings.add|Where-Object name -eq 'OdbcServicesGestor').connectionString;$catalog.Open()
    $cmd=$catalog.CreateCommand();$cmd.CommandText="SELECT IP_SERVER_MODULO,DB_NAME_MODULO,USER_DBMS_MODULO,PASW_DBMS_MODULO FROM gestor_modulos WHERE ESTADO_MODULO=1 AND TIPO_MODULO='GESTOR DOCUMENTAL'";$cmd.CommandTimeout=15;$reader=$cmd.ExecuteReader()
    try{if(-not $reader.Read()){throw 'DOC115_MODULE_REQUIRED'};$writerBuilder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder;$writerBuilder.Server=[string]$reader[0];$writerBuilder.Database=[string]$reader[1];$writerBuilder.UserID=[string]$reader[2];$writerBuilder.Password=[string]$reader[3];if($reader.Read()){throw 'DOC115_MODULE_AMBIGUOUS'}}finally{$reader.Dispose();$cmd.Dispose();$catalog.Dispose();$catalog=$null}
    }
    if((Hash-Text ($writerBuilder.Server+'|'+$writerBuilder.Database)) -ne $scope.environmentHash){throw 'DOC115_AUTHORIZED_DATABASE_CHANGED'}
    $controlDatabase=Read-Rows 'SELECT DATABASE() AS database_ref'
    if($controlDatabase.Count -ne 1 -or $controlDatabase[0].database_ref -cne $writerBuilder.Database){throw 'DOC115_CONTROL_DATABASE_MISMATCH'}
    if(-not $FileReservations){
    $profileRepo=New-Object MySqlPerfilImportacionRepository($writerBuilder.ConnectionString);$snapshot=$profileRepo.ObtenerPublicacion($context,$scope.versionRef)
    if($snapshot.PerfilId -ne $scope.profileRef -or (-not $privateRepository -and ($snapshot.Configuracion.ModoIntegridad -ne 'COMPLETO_OBLIGATORIO' -or $snapshot.Configuracion.AmbitoIntegridad -ne 'LOTE'))){throw 'DOC115_PUBLICATION_CHANGED'}
    }
    if(-not $privateRepository){$fixture=Join-Path $root ('tools/e2e/fixtures/'+$scope.pdfFixture)
    if((Get-FileHash -LiteralPath $fixture -Algorithm SHA256).Hash.ToLowerInvariant() -ne $scope.fixtureSha256){throw 'DOC115_FIXTURE_CHANGED'}}
    $journal.phase='REPOSITORY_INTEGRATION';$journal.status='RUNNING';Save-Journal
    $journal.codeFiles=@();foreach($relative in @('tests/ImportacionCarpetasBehaviorTests.cs','tests/ImportacionCarpetasRepositoryIntegrationTests.cs','tools/e2e/scripts/run-doc111-persistence.ps1','tools/e2e/scripts/support/doc115-repository-integration.ps1','tools/e2e/scripts/compile-doc111-persistence.cjs','tests/doc110-perfil-importacion-behavior.test.cjs','Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb','Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlPerfilImportacionRepository.vb','Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb')){$journal.codeFiles+=@(@{path=$relative;sha256=(Get-FileHash -LiteralPath (Join-Path $root $relative) -Algorithm SHA256).Hash.ToLowerInvariant()})};Save-Journal
    if($FileReservations){
        foreach($source in $scope.sources.PSObject.Properties){if((Get-FileHash -LiteralPath (Join-Path $root $source.Name) -Algorithm SHA256).Hash.ToLowerInvariant() -ne $source.Value){throw 'DOC120_SEGMENTED_SOURCE_CHANGED'}}
        $reference=[Action[string,string]]{param($kind,$id)if($kind -eq 'phase'){if($id -notmatch '\A[A-Z_]{3,64}\z'){throw 'DOC120_FILES_PHASE_INVALID'};$journal.phase=$id}else{if($kind -ne 'intento' -or $id -notmatch '\A[1-9][0-9]{0,9}\z'){throw 'DOC120_FILES_REFERENCE_INVALID'};$journal.references+=@([ordered]@{kind=$kind;id=$id})};Save-Journal}
        if($admission.Elapsed.TotalMinutes -ge 15){throw 'DOC120_SEGMENTED_WRAPPER_BUDGET'}
        if($FixtureFiles){
            $journal.phase='PREPARE_HTTP_FIXTURE';Save-Journal
            & node (Join-Path $PSScriptRoot '../run-doc115-recepcion-importacion.cjs') --prepare-files-fixture --session $ResourcesFile | ForEach-Object {if($_ -match '\ADOC110_TIMING: [A-Za-z]+ [0-9]+ms\z'){[Console]::Out.WriteLine($_)}}
            $fixtureExit=$LASTEXITCODE;$fixtureState=Get-Content -LiteralPath ($ResourcesFile+'.fixture-state.json') -Raw|ConvertFrom-Json
            $journal.fixture=$fixtureState;$journal.references+=@($fixtureState.references);$journal.wrapperSelects+=[int]$fixtureState.selects;$journal.wrapperSelectsThisRun+=[int]$fixtureState.selects;Save-Journal
            if($fixtureExit -ne 0 -or $fixtureState.status -ne 'PASS_FIXTURE_READY' -or $fixtureState.closed -ne $true){throw 'DOC120_FILES_FIXTURE_STOPPED'}
            $f=$fixtureState.result
            if($f.planId -notmatch '\A[1-9][0-9]{0,9}\z' -or $f.lotId -notmatch '\A[1-9][0-9]{0,9}\z' -or [uint64]$f.planId -le $maxima.plan -or [uint64]$f.lotId -le $maxima.lote -or $f.files.Count -ne 2 -or @($f.files|Where-Object {$_ -notmatch '\A[1-9][0-9]{0,9}\z' -or [uint64]$_ -le $maxima.archivo}).Count -ne 0){throw 'DOC120_FILES_FIXTURE_REFERENCES_INVALID'}
            $actualLot=Read-Rows 'SELECT contexto_doc FROM ra_integracion_lote WHERE lote_id=? AND plan_id=? AND solicitante_ref=141 AND ambito_ref=2 AND nivel_ref=7 AND version_id=38' @($f.lotId,$f.planId)
            if($actualLot.Count -ne 1){throw 'DOC120_FILES_FIXTURE_OWNER_CHANGED'};$context=[Newtonsoft.Json.JsonConvert]::DeserializeObject($actualLot[0].contexto_doc,[ContextoImportacionCarpetas])
            $bounds=Read-Rows 'SELECT (SELECT MAX(intento_id) FROM ra_integracion_intento WHERE lote_id=?) AS intento,(SELECT MAX(evento_id) FROM ra_integracion_evento WHERE lote_id=?) AS evento,(SELECT COUNT(*) FROM ra_integracion_evento WHERE lote_id=?) AS eventos' @($f.lotId,$f.lotId,$f.lotId)
            $fixtureAttemptMax=$bounds[0].intento;$fixtureEventMax=$bounds[0].evento;$fixtureEventCount=[int]$bounds[0].eventos
            $fixtureBefore=Fixture-Hashes $f.lotId $fixtureAttemptMax $fixtureEventMax;$journal.fixtureOriginalsBefore=$fixtureBefore;Save-Journal
            $result=[ImportacionCarpetasBehaviorTests]::RunDoc120FixtureFileRepositoryIntegration($true,$writerBuilder.ConnectionString,$context,$runId,$scope.environmentHash,$f.lotId,[string[]]$f.files,$f.planId,$reference)
        }else{$result=[ImportacionCarpetasBehaviorTests]::RunDoc120FileRepositoryIntegration($true,$writerBuilder.ConnectionString,$context,$runId,$scope.environmentHash,$scope.lotRef,[string[]]$scope.files,$reference)}
    }elseif($Segmented){
        foreach($source in $scope.sources.PSObject.Properties){if((Get-FileHash -LiteralPath (Join-Path $root $source.Name) -Algorithm SHA256).Hash.ToLowerInvariant() -ne $source.Value){throw 'DOC120_SEGMENTED_SOURCE_CHANGED'}}
        $context.RutaCanonica='DOC-120 / E2E existente / nivel '+$scope.levelRef;$context.SelloDestino=Hash-Text ('DOC120|'+$scope.userRef+'|'+$scope.companyRef+'|'+$scope.levelRef+'|'+$runId)
        $plan=New-Object PlanImportacionCarpetas;$plan.EsquemaDoc=1;$plan.Contexto=$context;$plan.Snapshot=$snapshot
        $plan.Manifiesto=New-Object ManifiestoImportacionCarpetas;$plan.Resultado=New-Object ResultadoPreparacionImportacion;$plan.Resultado.PuedeConfirmar=$true;$plan.Resultado.UnidadesElegibles.Add('r1')
        $rootRow=New-Object RaizManifiestoImportacion;$rootRow.IdCliente='r1';$rootRow.SeleccionId='s1';$rootRow.Nombre='DOC120-'+$runId+'-SEG';$plan.Manifiesto.Raices.Add($rootRow)
        for($index=0;$index -lt 2;$index++){$file=New-Object ArchivoManifiestoImportacion;$file.IdCliente='a'+($index+1);$file.RaizIdCliente='r1';$file.Nombre=$scope.files[$index].name;$file.RutaRelativa=$rootRow.Nombre+'/'+$file.Nombre;$file.Bytes=[string]$scope.files[$index].bytes;$file.UltimaModificacionLocal='0';$plan.Manifiesto.Archivos.Add($file)}
        $reference=[Action[string,string]]{param($kind,$id)if($kind -eq 'phase'){if($id -notmatch '\A[A-Z_]{3,64}\z'){throw 'DOC120_SEGMENTED_PHASE_INVALID'};$journal.phase=$id}else{if($kind -notin @('preparacion','plan','lote') -or $id -notmatch '\A[1-9][0-9]{0,9}\z'){throw 'DOC120_SEGMENTED_REFERENCE_INVALID'};$journal.references+=@([ordered]@{kind=$kind;id=$id})};Save-Journal}
        if($admission.Elapsed.TotalMinutes -ge 15){throw 'DOC120_SEGMENTED_WRAPPER_BUDGET'}
        $result=[ImportacionCarpetasBehaviorTests]::RunDoc120SegmentedRepositoryIntegration($true,$writerBuilder.ConnectionString,$plan,$runId,$scope.environmentHash,$reference)
    }else{
        $reference=[Action[string,string]]{param($kind,$id)if($kind -notin @('plan','lote','archivo','intento') -or $id -notmatch '\A[1-9][0-9]{0,9}\z'){throw 'DOC115_REFERENCE_INVALID'};$journal.references+=@([ordered]@{kind=$kind;id=$id});Save-Journal;[Console]::Out.WriteLine('DOC115_07A_REFERENCE='+(@{kind=$kind;id=$id}|ConvertTo-Json -Compress))}
        $result=[ImportacionCarpetasBehaviorTests]::RunDoc115RepositoryIntegration($true,$writerBuilder.ConnectionString,$context,$snapshot,[string[]]$scope.rootNames,$RuntimeRoot,[IO.File]::ReadAllBytes($fixture),$reference)
    }
    $journal.phase='READONLY_FINAL';Save-Journal
    $after=Protected-Hashes $maxima
    if($privateRepository){foreach($partTable in @('preparacion_parte','preparacion_resultado')){$after[$partTable]=Row-Hash ('SELECT * FROM ra_integracion_'+$partTable+' WHERE preparacion_id<=? ORDER BY preparacion_id,tipo,numero') @([string]$maxima.preparacion)}}
    foreach($key in $before.Keys){if($before[$key] -cne $after[$key]){throw 'DOC115_PREEXISTING_DATA_CHANGED'}}
    $journal.protectedAfter=$after
    if($FixtureFiles){
        $fixtureAfter=Fixture-Hashes $f.lotId $fixtureAttemptMax $fixtureEventMax
        foreach($key in $fixtureBefore.Keys){if($fixtureBefore[$key] -cne $fixtureAfter[$key]){throw 'DOC120_FILES_FIXTURE_ORIGINALS_CHANGED'}};$journal.fixtureOriginalsAfter=$fixtureAfter
        $physical=Read-Rows 'SELECT (SELECT COUNT(*) FROM expediente_archivo WHERE ID_EXPEDIENTE>?) AS exp,(SELECT COUNT(*) FROM ra_pro_niveles_has_expediente_archivo WHERE id_registro>?) AS rel' @($script:physicalMax.exp,$script:physicalMax.rel)
        if($physical[0].exp -ne '1' -or $physical[0].rel -ne '1'){throw 'DOC120_FILES_PHYSICAL_COUNTS_UNEXPECTED'}
        $identity=Read-Rows 'SELECT e.ID_EXPEDIENTE,r.id_registro FROM expediente_archivo e INNER JOIN ra_pro_niveles_has_expediente_archivo r ON r.expediente_archivo_ID_EXPEDIENTE=e.ID_EXPEDIENTE WHERE e.ID_EXPEDIENTE=? AND r.id_registro=? AND r.ra_pro_niveles_id_nivel=7' @($f.expedientId,$f.relationId)
        if($identity.Count -ne 1){throw 'DOC120_FILES_PHYSICAL_IDENTITY_CHANGED'}
    }
    foreach($name in $entities.Keys){
        $row=Read-Rows ('SELECT COUNT(*) AS total FROM ra_integracion_'+$name+' WHERE '+$entities[$name]+'>?') @([string]$maxima[$name])
        $expected=if($FixtureFiles){@{plan=1;lote=1;raiz=1;archivo=2;intento=5;preparacion=0}}elseif($FileReservations){@{plan=0;lote=0;raiz=0;archivo=0;intento=2;evento=7;preparacion=0}}elseif($Segmented){@{plan=1;lote=1;raiz=1;archivo=2;intento=0;evento=1;preparacion=1}}else{@{plan=3;lote=2;raiz=4;archivo=4;intento=1;evento=4}}
        if($FileReservations -and $name -eq 'evento'){$newCount=[int]$row[0].total-$(if($FixtureFiles){$fixtureEventCount}else{0});if($newCount -lt 4 -or $newCount -gt 7 -or [int]$row[0].total -gt 40){throw 'DOC120_FILES_EVENTS_UNEXPECTED'}}elseif([uint64]$row[0].total -ne $expected[$name]){throw 'DOC115_NEW_RECORD_COUNTS_UNEXPECTED'}
    }
    if($FileReservations){
        $eventFloor=if($FixtureFiles){$fixtureEventMax}else{[string]$maxima.evento};$eventLot=if($FixtureFiles){$f.lotId}else{'50'}
        $events=Read-Rows 'SELECT accion,recurso_tipo,COUNT(*) AS total FROM ra_integracion_evento WHERE evento_id>? AND lote_id=? AND actor_ref=141 GROUP BY accion,recurso_tipo ORDER BY accion,recurso_tipo' @($eventFloor,$eventLot)
        $sum=0;$reserves=0;$incidents=0
        foreach($event in $events){$sum+=[int]$event.total;if($event.accion -eq 'RESERVAR_INTENTO' -and $event.recurso_tipo -eq 'ARCHIVO'){$reserves+=[int]$event.total}elseif($event.accion -eq 'ORIGINAL_NO_DISPONIBLE' -and $event.recurso_tipo -eq 'ARCHIVO'){$incidents+=[int]$event.total}elseif($event.accion -ne 'CONSOLIDAR_ESTADO' -or $event.recurso_tipo -notin @('LOTE','RAIZ')){throw 'DOC120_FILES_EVENTS_UNEXPECTED'}}
        $newEvents=Read-Rows 'SELECT COUNT(*) AS total FROM ra_integracion_evento WHERE evento_id>?' @($eventFloor)
        if($sum -ne [int]$newEvents[0].total -or $reserves -ne 2 -or $incidents -ne 2){throw 'DOC120_FILES_EVENTS_UNEXPECTED'}
        $result['newEvents']=$sum
        if($FixtureFiles){$result['fixture']=$fixtureState.result;$result['fixtureHttp']=$fixtureState.http;$result['fixtureSelects']=$fixtureState.selects;$result['fixtureOriginalsUnchanged']=$true;$result['priorExpedientsUnchanged']=$true;$result['fixtureEventCount']=$fixtureEventCount}
    }
    if((Get-FileHash -LiteralPath (Join-Path $RuntimeRoot 'bin/GestionDocumental-Docuarchi.net.dll') -Algorithm SHA256).Hash.ToLowerInvariant() -ne $scope.backendSha256){throw 'DOC115_BACKEND_CHANGED'}
    if($privateRepository){foreach($source in $scope.sources.PSObject.Properties){if((Get-FileHash -LiteralPath (Join-Path $root $source.Name) -Algorithm SHA256).Hash.ToLowerInvariant() -ne $source.Value){throw 'DOC120_SEGMENTED_SOURCE_CHANGED'}}}
    $result['preexistingDataUnchanged']=$true;$result['readOnlyControls']=$true;$journal.result=$result;$journal.status='PASS';$journal.finishedUtc=[DateTime]::UtcNow.ToString('o');Save-Journal
    [Console]::Out.WriteLine('DOC115_07A_RESULT='+($result|ConvertTo-Json -Depth 8 -Compress))
}catch{
    $odbc=$null;$candidate=$_.Exception;while($candidate){if($candidate -is [Data.Odbc.OdbcException]){$odbc=$candidate;break};$candidate=$candidate.InnerException}
    $e=$_.Exception;while($e.InnerException){$e=$e.InnerException};$code=if($e.Message -match '\A(?:DOC115|DOC120_SEGMENTED|DOC120_FILES)_[A-Z0-9_]{3,80}\z'){$e.Message}else{'DOC115_REPOSITORY_INTEGRATION_FAILED'}
    if($code -eq 'DOC115_REPOSITORY_INTEGRATION_FAILED' -and $journal.phase -in @('READONLY_PREFLIGHT','READONLY_FINAL')){$states=@();$native=@();if($odbc){$states=@($odbc.Errors|ForEach-Object {$_.SQLState});$native=@($odbc.Errors|ForEach-Object {$_.NativeError})};$code=Get-Doc115ReadOnlyFailure -Step $journal.readonlyStep -States $states -Native $native}
    $journal.status='STOPPED_REVIEW_REQUIRED';$journal.failure=@{phase=$journal.phase;code=$code};if($journal.phase -in @('READONLY_PREFLIGHT','READONLY_FINAL')){$journal.failure.step=$journal.readonlyStep};$journal.finishedUtc=[DateTime]::UtcNow.ToString('o');Save-Journal
    [Console]::Out.WriteLine('DOC115_07A_FAILURE='+($journal.failure|ConvertTo-Json -Compress));exit 1
}finally{if($control){$control.Dispose()};if($catalog){$catalog.Dispose()};if($runLock){$runLock.Dispose()};$writerBuilder=$null;$config=$null;$snapshot=$null;$ro=$null}
