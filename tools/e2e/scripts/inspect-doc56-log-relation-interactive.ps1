param([Parameter(Mandatory=$true)][ValidateRange(1,[long]::MaxValue)][long]$DocumentId)
$ErrorActionPreference = 'Stop'
$confirmation = Read-Host 'Inspeccion DOC-56 de relacion exclusivamente SELECT en PRUEBAS. Escriba SI para continuar'
if ($confirmation -cne 'SI') { throw 'DOC56_RELATION_INSPECTION_NOT_AUTHORIZED' }
$user = Read-Host 'Usuario MySQL de solo lectura'
$securePassword = Read-Host 'Contrasena MySQL de solo lectura' -AsSecureString
$pointer = [IntPtr]::Zero
$connection = $null
try {
    $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer)
    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN']='workflowdocument'; $builder['UID']=$user.Trim(); $builder['PWD']=$password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString); $connection.Open()
    $schemasCommand = $connection.CreateCommand()
    $schemasCommand.CommandText = "SELECT table_schema FROM information_schema.tables WHERE table_name='logdocuarchi' ORDER BY table_schema"
    $reader = $schemasCommand.ExecuteReader(); $schemas = @()
    try { while ($reader.Read()) { $schemas += [string]$reader.GetValue(0) } } finally { $reader.Dispose(); $schemasCommand.Dispose() }
    foreach ($schema in $schemas) {
        if ($schema -notmatch '^[A-Za-z0-9_]+$') { continue }
        $command=$connection.CreateCommand()
        $command.CommandText="SELECT id_tran,ID_TAREA_WF,desc_op,MODULO_REGISTRO FROM ``$schema``.logdocuarchi WHERE id_tran=?"
        $parameter=$command.CreateParameter(); $parameter.Value=$DocumentId; [void]$command.Parameters.Add($parameter)
        $rowReader=$command.ExecuteReader(); $count=0
        try {
            while ($rowReader.Read()) {
                $count++
                Write-Host ('DOC56_RELATION_CANDIDATE_{0}_TASK={1}' -f $count, $(if ($rowReader.IsDBNull(1)) {'[NULL]'} else {[string]$rowReader.GetValue(1)}))
                Write-Host ('DOC56_RELATION_CANDIDATE_{0}_OP={1}' -f $count, [string]$rowReader.GetValue(2))
                Write-Host ('DOC56_RELATION_CANDIDATE_{0}_MODULE={1}' -f $count, [string]$rowReader.GetValue(3))
            }
        } finally { $rowReader.Dispose(); $command.Dispose() }
    }
    Write-Host 'DOC56_RELATION_INSPECTION_COMPLETE'
} finally {
    if ($connection) {$connection.Dispose()}
    if ($pointer -ne [IntPtr]::Zero) {[Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer)}
    $password=$null; $securePassword=$null
}
