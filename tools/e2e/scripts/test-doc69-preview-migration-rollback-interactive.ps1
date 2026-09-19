param([string]$Dsn = 'workflowdocument')

$ErrorActionPreference = 'Stop'
$connection = $null
$passwordPointer = [IntPtr]::Zero
$temporaryTable = 'workflow_import_preview_descriptor_doc69_test'

function Scalar([System.Data.Odbc.OdbcConnection]$Connection,[string]$Sql,[object[]]$Values) {
    $command=$Connection.CreateCommand()
    try {
        $command.CommandText=$Sql
        foreach($value in $Values){$parameter=$command.CreateParameter();$parameter.Value=$value;[void]$command.Parameters.Add($parameter)}
        return $command.ExecuteScalar()
    } finally {$command.Dispose()}
}

function Execute([System.Data.Odbc.OdbcConnection]$Connection,[string]$Sql) {
    $command=$Connection.CreateCommand()
    try {$command.CommandText=$Sql;[void]$command.ExecuteNonQuery()} finally {$command.Dispose()}
}

try {
    $confirmation=Read-Host 'Prueba DOC-69 de migracion adelante/rollback sobre tabla temporal en CERTIFICACION. Escriba SI'
    if($confirmation -cne 'SI'){throw 'DOC69_MIGRATION_TEST_NOT_AUTHORIZED'}
    $user=Read-Host 'Usuario MySQL autorizado para migraciones'
    if([string]::IsNullOrWhiteSpace($user)){throw 'DOC69_MIGRATION_TEST_USER_REQUIRED'}
    $securePassword=Read-Host 'Contraseña MySQL' -AsSecureString
    $passwordPointer=[Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password=[Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $builder=[System.Data.Odbc.OdbcConnectionStringBuilder]::new();$builder['DSN']=$Dsn;$builder['UID']=$user.Trim();$builder['PWD']=$password
    $connection=[System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString);$connection.Open()
    $existing=[int](Scalar $connection 'SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=?' @($temporaryTable))
    if($existing -ne 0){throw 'DOC69_MIGRATION_TEST_TEMP_TABLE_EXISTS'}

    $create=@"
CREATE TABLE $temporaryTable (
  id BIGINT NOT NULL AUTO_INCREMENT, descriptor_hash BINARY(32) NOT NULL, resource_hash BINARY(32) NOT NULL,
  user_id INT NOT NULL, task_id BIGINT NOT NULL, provider_id VARCHAR(40) NOT NULL, content_type VARCHAR(100) NOT NULL,
  content_length BIGINT NOT NULL, content_disposition VARCHAR(20) NOT NULL, safe_file_name VARCHAR(255) NOT NULL,
  content MEDIUMBLOB NOT NULL, status VARCHAR(20) NOT NULL, expires_utc DATETIME NOT NULL, claimed_utc DATETIME NULL,
  consumed_utc DATETIME NULL, created_utc DATETIME NOT NULL, PRIMARY KEY (id),
  UNIQUE KEY ux_doc69_test_hash (descriptor_hash), KEY ix_doc69_test_authority (user_id,task_id,provider_id,status,expires_utc),
  KEY ix_doc69_test_expiry (status,expires_utc)
) ENGINE=InnoDB DEFAULT CHARSET=utf8
"@
    Execute $connection $create
    $columns=[int](Scalar $connection 'SELECT COUNT(*) FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=?' @($temporaryTable))
    $indexes=[int](Scalar $connection 'SELECT COUNT(DISTINCT INDEX_NAME) FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=?' @($temporaryTable))
    $foreignKeys=[int](Scalar $connection 'SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=? AND CONSTRAINT_TYPE=''FOREIGN KEY''' @($temporaryTable))
    if($columns -ne 16 -or $indexes -ne 4 -or $foreignKeys -ne 0){throw 'DOC69_MIGRATION_TEST_FORWARD_INVALID'}
    Execute $connection "DROP TABLE $temporaryTable"
    $remaining=[int](Scalar $connection 'SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=?' @($temporaryTable))
    if($remaining -ne 0){throw 'DOC69_MIGRATION_TEST_ROLLBACK_INVALID'}
    Write-Host 'DOC69_MIGRATION_TEST_COLUMNS=16'
    Write-Host 'DOC69_MIGRATION_TEST_INDEXES=4'
    Write-Host 'DOC69_MIGRATION_TEST_FOREIGN_KEYS=0'
    Write-Host 'DOC69_MIGRATION_TEST_ROLLBACK_REMAINING=0'
    Write-Host 'DOC69_MIGRATION_TEST_VERDICT=PASS'
} catch {
    $safeCode=if($_.Exception.Message -match '^DOC69_MIGRATION_TEST_[A-Z_]+$'){$_.Exception.Message}else{'DOC69_MIGRATION_TEST_FAILED'}
    Write-Error "$safeCode. No se mostraron credenciales ni detalles internos."
    exit 2
} finally {
    if($connection){
        try {
            if([int](Scalar $connection 'SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=?' @($temporaryTable)) -eq 1){Execute $connection "DROP TABLE $temporaryTable"}
        } catch {}
        $connection.Dispose()
    }
    if($passwordPointer -ne [IntPtr]::Zero){[Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer)}
    $password=$null;$securePassword=$null
}
