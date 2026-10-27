param([switch]$Inspect,[switch]$Recover)
$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../../..'))
$targetLogin='LUZ.AGUILERA'
$raConnection=$null
$catalogConnection=$null
$restoreRequired=$false
$recoveryPath=Join-Path $repoRoot 'tools/e2e/artifacts/doc110-permission-recovery.json'

function Open-Doc110Database {
    Add-Type -Path (Join-Path $repoRoot 'bin/MySql.Data.dll')
    $configuration=[xml](Get-Content -LiteralPath (Join-Path $repoRoot 'Web.config') -Raw)
    $script:catalogConnection=New-Object System.Data.Odbc.OdbcConnection
    $script:catalogConnection.ConnectionString=($configuration.configuration.connectionStrings.add | Where-Object name -eq 'OdbcServicesGestor').connectionString
    $script:catalogConnection.Open()
    $command=$script:catalogConnection.CreateCommand()
    $command.CommandText="SELECT IP_SERVER_MODULO,DB_NAME_MODULO,USER_DBMS_MODULO,PASW_DBMS_MODULO FROM gestor_modulos WHERE ESTADO_MODULO=1 AND TIPO_MODULO='GESTOR DOCUMENTAL'"
    $reader=$command.ExecuteReader()
    try {
        if(-not $reader.Read()){throw 'DOC110_RA_MODULE_REQUIRED'}
        $builder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder
        $builder.Server=[string]$reader[0];$builder.Database=[string]$reader[1];$builder.UserID=[string]$reader[2];$builder.Password=[string]$reader[3]
        if($reader.Read()){throw 'DOC110_RA_MODULE_AMBIGUOUS'}
        $script:databaseBuilder=$builder
        $script:raConnection=New-Object MySql.Data.MySqlClient.MySqlConnection($builder.ConnectionString)
        $script:raConnection.Open()
    } finally {$reader.Dispose();$command.Dispose();$script:catalogConnection.Dispose();$script:catalogConnection=$null}
}
function Read-Doc110Rows([string]$Sql,[hashtable]$Parameters=@{}) {
    $command=$script:raConnection.CreateCommand();$command.CommandTimeout=5;$command.CommandText=$Sql
    foreach($key in $Parameters.Keys){[void]$command.Parameters.AddWithValue($key,$Parameters[$key])}
    $reader=$command.ExecuteReader();$rows=New-Object 'System.Collections.Generic.List[object]'
    try {
        while($reader.Read()){
            $row=[ordered]@{}
            for($i=0;$i -lt $reader.FieldCount;$i++){$row[$reader.GetName($i)]=if($reader.IsDBNull($i)){$null}else{$reader.GetValue($i)}}
            $rows.Add([pscustomobject]$row)
        }
    } finally {$reader.Dispose();$command.Dispose()}
    return ,$rows.ToArray()
}
function Read-Doc110Target {
    $users=Read-Doc110Rows 'SELECT Id_Remit_Dest_Int AS usuario,Empresa_Gestion_Documental_id_empresa AS empresa,Estado_Usuario AS activo FROM remit_dest_interno WHERE Login_Usuario=@login LIMIT 2' @{'@login'=$targetLogin}
    if($users.Count -ne 1 -or [int]$users[0].activo -ne 1){throw 'DOC110_PERMISSION_USER_NOT_UNIQUE_ACTIVE'}
    if($script:expectedIdentity -and ([string]$users[0].usuario -cne $script:expectedIdentity.user -or [string]$users[0].empresa -cne $script:expectedIdentity.company)){throw 'DOC110_PERMISSION_IDENTITY_CHANGED'}
    $script:targetUser=[string]$users[0].usuario
    $script:targetCompany=[string]$users[0].empresa
    if($script:targetUser -notmatch '^[1-9][0-9]*$' -or $script:targetCompany -notmatch '^[1-9][0-9]*$'){throw 'DOC110_PERMISSION_CONTEXT_INVALID'}
    $keys=Read-Doc110Rows "SELECT COLUMN_NAME AS nombre FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='remit_dest_interno_perfil_produccion' AND CONSTRAINT_NAME='PRIMARY' ORDER BY ORDINAL_POSITION"
    if($keys.Count -ne 1 -or $keys[0].nombre -notmatch '^[A-Za-z_][A-Za-z0-9_]*$'){throw 'DOC110_PERMISSION_PRIMARY_KEY_REQUIRED'}
    $script:primaryColumn=[string]$keys[0].nombre
    $rows=Read-Doc110Rows 'SELECT * FROM remit_dest_interno_perfil_produccion WHERE remit_dest_interno_idremit_dest_interno=@usuario LIMIT 2' @{'@usuario'=$script:targetUser}
    if($rows.Count -ne 1){throw 'DOC110_PERMISSION_ROW_NOT_UNIQUE'}
    $script:primaryValue=$rows[0].PSObject.Properties[$script:primaryColumn].Value
    if($script:expectedIdentity -and ($script:primaryColumn -cne $script:expectedIdentity.column -or [string]$script:primaryValue -cne $script:expectedIdentity.primary)){throw 'DOC110_PERMISSION_ROW_CHANGED'}
    return $rows[0]
}

function Get-Doc110OtherHash($Row) {
    $values=[ordered]@{}
    foreach($property in ($Row.PSObject.Properties | Sort-Object Name)){
        if($property.Name -ne 'ADMINISTRAR_PERFILES_IMPORTACION'){$values[$property.Name]=$property.Value}
    }
    $json=ConvertTo-Json -InputObject $values -Compress -Depth 8
    $sha=[Security.Cryptography.SHA256]::Create()
    try {return ([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($json)))).Replace('-','').ToLowerInvariant()}
    finally {$sha.Dispose()}
}
function Save-Doc110Recovery([string]$Stage,[bool]$Restored) {
    $record=[ordered]@{userRef=$script:targetUser;companyRef=$script:targetCompany;primaryColumn=$script:primaryColumn;primaryValue=[string]$script:primaryValue;originalPermission=1;otherFieldsHash=$script:otherHash;environmentHash=$script:environmentHash;stage=$Stage;restored=$Restored;updatedUtc=[DateTime]::UtcNow.ToString('o')}
    [void][IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($recoveryPath))
    [IO.File]::WriteAllText($recoveryPath,($record | ConvertTo-Json -Compress),[Text.Encoding]::UTF8)
}
function Set-Doc110Permission([int]$Expected,[int]$Value) {
    $command=$script:raConnection.CreateCommand();$command.CommandTimeout=5
    $command.CommandText='UPDATE remit_dest_interno_perfil_produccion SET ADMINISTRAR_PERFILES_IMPORTACION=@value WHERE `'+$script:primaryColumn+'`=@primary AND remit_dest_interno_idremit_dest_interno=@usuario AND ADMINISTRAR_PERFILES_IMPORTACION=@expected'
    [void]$command.Parameters.AddWithValue('@value',$Value);[void]$command.Parameters.AddWithValue('@expected',$Expected)
    [void]$command.Parameters.AddWithValue('@primary',$script:primaryValue);[void]$command.Parameters.AddWithValue('@usuario',$script:targetUser)
    try {if($command.ExecuteNonQuery() -ne 1){throw 'DOC110_PERMISSION_COMPARE_AND_SET_FAILED'}}
    finally {$command.Dispose()}
}
function Restore-Doc110Permission {
    if(-not $script:restoreRequired){return}
    try {$current=Read-Doc110Target}
    catch {
        $script:raConnection.Dispose()
        $script:raConnection=New-Object MySql.Data.MySqlClient.MySqlConnection($script:databaseBuilder.ConnectionString)
        $script:raConnection.Open();$current=Read-Doc110Target
    }
    if([string]$current.ADMINISTRAR_PERFILES_IMPORTACION -ceq '0'){Set-Doc110Permission 0 1}
    elseif([string]$current.ADMINISTRAR_PERFILES_IMPORTACION -cne '1'){throw 'DOC110_PERMISSION_RESTORE_CONFLICT'}
    $verified=Read-Doc110Target
    if([string]$verified.ADMINISTRAR_PERFILES_IMPORTACION -cne '1'){throw 'DOC110_PERMISSION_RESTORE_UNCONFIRMED'}
    $script:restoreRequired=$false
    Save-Doc110Recovery 'RESTORED' $true
    if((Get-Doc110OtherHash $verified) -cne $script:otherHash){throw 'DOC110_PERMISSION_OTHER_FIELDS_CHANGED'}
}

try {
    Open-Doc110Database
    $original=Read-Doc110Target
    $script:expectedIdentity=@{user=$script:targetUser;company=$script:targetCompany;column=$script:primaryColumn;primary=[string]$script:primaryValue}
    $sha=[Security.Cryptography.SHA256]::Create()
    try {$script:environmentHash=([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($script:databaseBuilder.Server+'|'+$script:databaseBuilder.Database)))).Replace('-','')}
    finally {$sha.Dispose()}
    $script:otherHash=Get-Doc110OtherHash $original
    if($Recover){
        $record=Get-Content -LiteralPath $recoveryPath -Raw | ConvertFrom-Json
        if($record.originalPermission -ne 1 -or $record.userRef -cne $script:targetUser -or $record.companyRef -cne $script:targetCompany -or
          $record.primaryColumn -cne $script:primaryColumn -or $record.primaryValue -cne [string]$script:primaryValue -or $record.environmentHash -cne $script:environmentHash){throw 'DOC110_PERMISSION_RECOVERY_TARGET_MISMATCH'}
        $script:otherHash=[string]$record.otherFieldsHash;$script:restoreRequired=$true
        Restore-Doc110Permission
        [pscustomobject]@{ok=$true;stage='RESTORED';otherFieldsUnchanged=$true} | ConvertTo-Json -Compress
    } else {
        if([string]$original.ADMINISTRAR_PERFILES_IMPORTACION -cne '1'){throw 'DOC110_PERMISSION_ORIGINAL_NOT_GRANTED'}
        if($Inspect){
            [pscustomobject]@{ok=$true;stage='INSPECT';uniqueUser=$true;uniquePermissionRow=$true;singlePrimaryKey=$true;originalGranted=$true;userRef=$script:targetUser;companyRef=$script:targetCompany} | ConvertTo-Json -Compress
        } else {
            if(Test-Path -LiteralPath $recoveryPath){
                $previous=Get-Content -LiteralPath $recoveryPath -Raw | ConvertFrom-Json
                if(-not $previous.restored){throw 'DOC110_PERMISSION_PREVIOUS_RECOVERY_REQUIRED'}
            }
            [pscustomobject]@{ok=$true;stage='READY';userRef=$script:targetUser;companyRef=$script:targetCompany;originalGranted=$true} | ConvertTo-Json -Compress
            while($true){
                $inputTask=[Console]::In.ReadLineAsync()
                if(-not $inputTask.Wait(300000)){break}
                $line=$inputTask.Result
                if($null -eq $line){break}
                $request=$line | ConvertFrom-Json
                switch($request.operation){
                    'REVOKE' {
                        if($script:restoreRequired){throw 'DOC110_PERMISSION_ALREADY_REVOKED'}
                        $current=Read-Doc110Target
                        if([string]$current.ADMINISTRAR_PERFILES_IMPORTACION -cne '1' -or (Get-Doc110OtherHash $current) -cne $script:otherHash){throw 'DOC110_PERMISSION_PRECONDITION_CHANGED'}
                        Save-Doc110Recovery 'REVOKE_ATTEMPT' $false
                        $script:restoreRequired=$true
                        Set-Doc110Permission 1 0
                        $current=Read-Doc110Target
                        if([string]$current.ADMINISTRAR_PERFILES_IMPORTACION -cne '0' -or (Get-Doc110OtherHash $current) -cne $script:otherHash){throw 'DOC110_PERMISSION_REVOKE_UNCONFIRMED'}
                        Save-Doc110Recovery 'REVOKED' $false
                        [pscustomobject]@{ok=$true;stage='REVOKED';otherFieldsUnchanged=$true} | ConvertTo-Json -Compress
                    }
                    'RESTORE' {
                        Restore-Doc110Permission
                        [pscustomobject]@{ok=$true;stage='RESTORED';otherFieldsUnchanged=$true} | ConvertTo-Json -Compress
                    }
                    'CLOSE' {return}
                    default {throw 'DOC110_PERMISSION_OPERATION_INVALID'}
                }
            }
        }
    }
} catch {
    [pscustomobject]@{ok=$false;code='DOC110_PERMISSION_DATABASE_OPERATION_FAILED'} | ConvertTo-Json -Compress
    exit 1
} finally {
    if($restoreRequired){
        try {
            Restore-Doc110Permission
            [pscustomobject]@{ok=$true;stage='RESTORED_AFTER_EXIT';otherFieldsUnchanged=$true} | ConvertTo-Json -Compress
        } catch {
            [pscustomobject]@{ok=$false;code='DOC110_PERMISSION_RESTORE_REQUIRES_RECOVERY'} | ConvertTo-Json -Compress
            [Environment]::ExitCode=1
        }
    }
    if($raConnection){$raConnection.Dispose()}
    if($catalogConnection){$catalogConnection.Dispose()}
}
