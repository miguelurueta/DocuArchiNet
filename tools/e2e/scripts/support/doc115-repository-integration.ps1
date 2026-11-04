param([Parameter(Mandatory=$true)][string]$ResourcesFile,[Parameter(Mandatory=$true)][string]$RuntimeRoot,[switch]$Check)
$ErrorActionPreference='Stop'
if(-not $Check -and $env:DOC115_07A_AUTHORIZED -ne 'true'){throw 'DOC115_REPOSITORY_AUTHORIZATION_REQUIRED'}
. (Join-Path $PSScriptRoot 'doc115-readonly-failure.ps1')
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../../..'))
$scope=Get-Content -LiteralPath $ResourcesFile -Raw|ConvertFrom-Json
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
if($env:DOC115_07A_RUN_ID -notmatch '\A[a-f0-9]{32}\z'){throw 'DOC115_RUN_ID_REQUIRED'}
$journalPath=Join-Path $root ('tools/e2e/artifacts/doc115-07a-'+$env:DOC115_07A_RUN_ID+'.json')
if(Test-Path -LiteralPath $journalPath){throw 'DOC115_JOURNAL_ALREADY_EXISTS'}
$journal=[ordered]@{issueKey='DOC-115';scenario='REPO-07A';status='STARTING';phase='COMPILE';references=@();resourcesSha256=$env:DOC115_07A_RESOURCES_SHA256;backendSha256=$scope.backendSha256;startedUtc=[DateTime]::UtcNow.ToString('o')}
function Save-Journal { [IO.File]::WriteAllText($journalPath,($journal|ConvertTo-Json -Depth 15),[Text.UTF8Encoding]::new($false)) }
$control=$null;$catalog=$null;$runLock=$null;$writerBuilder=$null
function Read-Rows([string]$sql,[object[]]$arguments=@()){
    if($sql -notmatch '\ASELECT\b' -or $sql -match ';|\b(?:INSERT|UPDATE|DELETE|CALL|EXEC|DROP|ALTER|CREATE|REPLACE|TRUNCATE|GRANT|REVOKE|SET|USE|LOAD|OUTFILE|INTO|LOCK|SLEEP|BENCHMARK)\b' -or ([regex]::Matches($sql,'\?')).Count -ne $arguments.Count){throw 'DOC115_SELECT_REQUIRED'}
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
function Protected-Hashes($maxima){$out=[ordered]@{};foreach($name in $entities.Keys){$out[$name]=Row-Hash ('SELECT * FROM ra_integracion_'+$name+' WHERE '+$entities[$name]+'<=? ORDER BY '+$entities[$name]) @([string]$maxima[$name])};$out.profiles=Row-Hash 'SELECT * FROM ra_integracion_perfil ORDER BY perfil_id';$out.versions=Row-Hash 'SELECT * FROM ra_integracion_perfil_version ORDER BY version_id';$out.rules=Row-Hash 'SELECT * FROM ra_integracion_perfil_regla ORDER BY regla_id';$out.permissions=Row-Hash ('SELECT * FROM remit_dest_interno_perfil_produccion ORDER BY '+$script:permissionOrder);$out.expedientes=Row-Hash 'SELECT COUNT(*) AS total FROM expediente_archivo';$out.documents=Row-Hash ('SELECT COUNT(*) AS total FROM `'+$script:cabinetTable+'`');$out.inventories=Row-Hash 'SELECT COUNT(*) AS total FROM registro_producion_documental';return $out}
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
    $permissionKeys=Read-Rows "SELECT COLUMN_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=? AND CONSTRAINT_NAME='PRIMARY' ORDER BY ORDINAL_POSITION" @('remit_dest_interno_perfil_produccion')
    if($permissionKeys.Count -lt 1 -or $permissionKeys.Count -gt 8 -or @($permissionKeys|Where-Object {$_.COLUMN_NAME -notmatch '\A[A-Za-z_][A-Za-z0-9_]{0,63}\z'}).Count -ne 0){throw 'DOC115_PERMISSION_KEY_UNAVAILABLE'}
    $script:permissionOrder=(@($permissionKeys|ForEach-Object {'`'+$_.COLUMN_NAME+'`'}) -join ',')
    $version=Read-Rows 'SELECT gabinete_id FROM ra_integracion_perfil_version WHERE version_id=? AND perfil_id=?' @($scope.versionRef,$scope.profileRef)
    if($version.Count -ne 1 -or $version[0].gabinete_id -ne $scope.cabinetRef){throw 'DOC115_CABINET_REFERENCE_CHANGED'}
    $cabinet=Read-Rows 'SELECT NOMBRE FROM system1 WHERE id_gabinete=?' @($scope.cabinetRef)
    if($cabinet.Count -ne 1 -or $cabinet[0].NOMBRE -notmatch '\A[A-Za-z_][A-Za-z0-9_]{0,63}\z'){throw 'DOC115_CABINET_IDENTIFIER_INVALID'}
    $script:cabinetTable=[string]$cabinet[0].NOMBRE
    $prior=Read-Rows 'SELECT manifiesto_doc FROM ra_integracion_plan WHERE usuario_ref=? AND ambito_ref=? AND nivel_ref=? AND version_id=? ORDER BY plan_id' @($scope.userRef,$scope.companyRef,$scope.levelRef,$scope.versionRef)
    foreach($row in $prior){$m=ConvertFrom-Json $row.manifiesto_doc;foreach($r in $m.Raices){if($scope.rootNames -contains $r.Nombre){throw 'DOC115_PRIOR_FIXTURE_REQUIRES_REVIEW'}}}
    $authority=Read-Rows 'SELECT a.IMPORTAR_CARPETAS FROM remit_dest_interno_perfil_produccion a INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=a.remit_dest_interno_idremit_dest_interno WHERE u.id_Remit_Dest_Int=? AND u.Empresa_Gestion_Documental_id_empresa=? AND u.Estado_Usuario=1' @($scope.userRef,$scope.companyRef)
    if($authority.Count -ne 1 -or $authority[0].IMPORTAR_CARPETAS -ne '1'){throw 'DOC115_EXISTING_AUTHORITY_REQUIRED'}
    $level=Read-Rows 'SELECT n.id_nivel FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int WHERE n.id_nivel=? AND n.estado_nivel=1 AND u.Empresa_Gestion_Documental_id_empresa=? AND (u.id_Remit_Dest_Int=? OR EXISTS(SELECT 1 FROM ra_pro_permisos_niveles p WHERE p.ra_pro_niveles_id_nivel=n.id_nivel AND p.remit_dest_interno_id_Remit_Dest_Int=? AND p.estado_permiso=1 AND p.agregar_expediente=1))' @($scope.levelRef,$scope.companyRef,$scope.userRef,$scope.userRef)
    if($level.Count -ne 1){throw 'DOC115_EXISTING_DESTINATION_REQUIRED'}
    $context=New-Object ContextoImportacionCarpetas;$context.UsuarioRef=$scope.userRef;$context.AmbitoRef=$scope.companyRef;$context.NivelRef=$scope.levelRef;$context.RutaCanonica='DOC-115 / fixture de repositorio / nivel '+$scope.levelRef;$context.SelloDestino=Hash-Text ('DOC115|'+$scope.userRef+'|'+$scope.companyRef+'|'+$scope.levelRef+'|'+$scope.rootNames[0]);$context.PuedeImportar=$true
    $maxima=[ordered]@{};foreach($name in $entities.Keys){$row=Read-Rows ('SELECT COALESCE(MAX('+$entities[$name]+'),0) AS ultimo FROM ra_integracion_'+$name);$maxima[$name]=[uint64]$row[0].ultimo}
    $before=Protected-Hashes $maxima;$journal.protectedBefore=$before;$journal.existingMaxima=$maxima;Save-Journal
    # Resolve the registered operational connection in memory, following the
    # existing persistence runner. Never print or persist module credentials.
    $journal.phase='REGISTERED_CONNECTION';Save-Journal
    $config=[xml](Get-Content -LiteralPath (Join-Path $RuntimeRoot 'Web.config') -Raw)
    $catalog=New-Object Data.Odbc.OdbcConnection;$catalog.ConnectionString=($config.configuration.connectionStrings.add|Where-Object name -eq 'OdbcServicesGestor').connectionString;$catalog.Open()
    $cmd=$catalog.CreateCommand();$cmd.CommandText="SELECT IP_SERVER_MODULO,DB_NAME_MODULO,USER_DBMS_MODULO,PASW_DBMS_MODULO FROM gestor_modulos WHERE ESTADO_MODULO=1 AND TIPO_MODULO='GESTOR DOCUMENTAL'";$cmd.CommandTimeout=15;$reader=$cmd.ExecuteReader()
    try{if(-not $reader.Read()){throw 'DOC115_MODULE_REQUIRED'};$writerBuilder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder;$writerBuilder.Server=[string]$reader[0];$writerBuilder.Database=[string]$reader[1];$writerBuilder.UserID=[string]$reader[2];$writerBuilder.Password=[string]$reader[3];if($reader.Read()){throw 'DOC115_MODULE_AMBIGUOUS'}}finally{$reader.Dispose();$cmd.Dispose();$catalog.Dispose();$catalog=$null}
    if((Hash-Text ($writerBuilder.Server+'|'+$writerBuilder.Database)) -ne $scope.environmentHash){throw 'DOC115_AUTHORIZED_DATABASE_CHANGED'}
    $controlDatabase=Read-Rows 'SELECT DATABASE() AS database_ref'
    if($controlDatabase.Count -ne 1 -or $controlDatabase[0].database_ref -cne $writerBuilder.Database){throw 'DOC115_CONTROL_DATABASE_MISMATCH'}
    $profileRepo=New-Object MySqlPerfilImportacionRepository($writerBuilder.ConnectionString);$snapshot=$profileRepo.ObtenerPublicacion($context,$scope.versionRef)
    if($snapshot.PerfilId -ne $scope.profileRef -or $snapshot.Configuracion.ModoIntegridad -ne 'COMPLETO_OBLIGATORIO' -or $snapshot.Configuracion.AmbitoIntegridad -ne 'LOTE'){throw 'DOC115_PUBLICATION_CHANGED'}
    $fixture=Join-Path $root ('tools/e2e/fixtures/'+$scope.pdfFixture)
    if((Get-FileHash -LiteralPath $fixture -Algorithm SHA256).Hash.ToLowerInvariant() -ne $scope.fixtureSha256){throw 'DOC115_FIXTURE_CHANGED'}
    $journal.phase='REPOSITORY_INTEGRATION';$journal.status='RUNNING';Save-Journal
    $journal.codeFiles=@();foreach($relative in @('tests/ImportacionCarpetasBehaviorTests.cs','tests/ImportacionCarpetasRepositoryIntegrationTests.cs','tools/e2e/scripts/run-doc111-persistence.ps1','tools/e2e/scripts/support/doc115-repository-integration.ps1','tools/e2e/scripts/compile-doc111-persistence.cjs','tests/doc110-perfil-importacion-behavior.test.cjs','Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb','Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlPerfilImportacionRepository.vb','Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb')){$journal.codeFiles+=@(@{path=$relative;sha256=(Get-FileHash -LiteralPath (Join-Path $root $relative) -Algorithm SHA256).Hash.ToLowerInvariant()})};Save-Journal
    $reference=[Action[string,string]]{param($kind,$id)if($kind -notin @('plan','lote','archivo','intento') -or $id -notmatch '\A[1-9][0-9]{0,9}\z'){throw 'DOC115_REFERENCE_INVALID'};$journal.references+=@([ordered]@{kind=$kind;id=$id});Save-Journal;[Console]::Out.WriteLine('DOC115_07A_REFERENCE='+(@{kind=$kind;id=$id}|ConvertTo-Json -Compress))}
    $result=[ImportacionCarpetasBehaviorTests]::RunDoc115RepositoryIntegration($true,$writerBuilder.ConnectionString,$context,$snapshot,[string[]]$scope.rootNames,$RuntimeRoot,[IO.File]::ReadAllBytes($fixture),$reference)
    $journal.phase='READONLY_FINAL';Save-Journal
    $after=Protected-Hashes $maxima
    foreach($key in $before.Keys){if($before[$key] -cne $after[$key]){throw 'DOC115_PREEXISTING_DATA_CHANGED'}}
    $journal.protectedAfter=$after
    foreach($name in $entities.Keys){$row=Read-Rows ('SELECT COUNT(*) AS total FROM ra_integracion_'+$name+' WHERE '+$entities[$name]+'>?') @([string]$maxima[$name]);$expected=@{plan=3;lote=2;raiz=4;archivo=4;intento=1;evento=4};if([uint64]$row[0].total -ne $expected[$name]){throw 'DOC115_NEW_RECORD_COUNTS_UNEXPECTED'}}
    if((Get-FileHash -LiteralPath (Join-Path $RuntimeRoot 'bin/GestionDocumental-Docuarchi.net.dll') -Algorithm SHA256).Hash.ToLowerInvariant() -ne $scope.backendSha256){throw 'DOC115_BACKEND_CHANGED'}
    $result['preexistingDataUnchanged']=$true;$result['readOnlyControls']=$true;$journal.result=$result;$journal.status='PASS';$journal.finishedUtc=[DateTime]::UtcNow.ToString('o');Save-Journal
    [Console]::Out.WriteLine('DOC115_07A_RESULT='+($result|ConvertTo-Json -Depth 8 -Compress))
}catch{
    $odbc=$null;$candidate=$_.Exception;while($candidate){if($candidate -is [Data.Odbc.OdbcException]){$odbc=$candidate;break};$candidate=$candidate.InnerException}
    $e=$_.Exception;while($e.InnerException){$e=$e.InnerException};$code=if($e.Message -match '\ADOC115_[A-Z0-9_]{3,80}\z'){$e.Message}else{'DOC115_REPOSITORY_INTEGRATION_FAILED'}
    if($code -eq 'DOC115_REPOSITORY_INTEGRATION_FAILED' -and $journal.phase -in @('READONLY_PREFLIGHT','READONLY_FINAL')){$states=@();$native=@();if($odbc){$states=@($odbc.Errors|ForEach-Object {$_.SQLState});$native=@($odbc.Errors|ForEach-Object {$_.NativeError})};$code=Get-Doc115ReadOnlyFailure -Step $journal.readonlyStep -States $states -Native $native}
    $journal.status='STOPPED_REVIEW_REQUIRED';$journal.failure=@{phase=$journal.phase;code=$code};if($journal.phase -in @('READONLY_PREFLIGHT','READONLY_FINAL')){$journal.failure.step=$journal.readonlyStep};$journal.finishedUtc=[DateTime]::UtcNow.ToString('o');Save-Journal
    [Console]::Out.WriteLine('DOC115_07A_FAILURE='+($journal.failure|ConvertTo-Json -Compress));exit 1
}finally{if($control){$control.Dispose()};if($catalog){$catalog.Dispose()};if($runLock){$runLock.Dispose()};$writerBuilder=$null;$config=$null;$snapshot=$null;$ro=$null}
