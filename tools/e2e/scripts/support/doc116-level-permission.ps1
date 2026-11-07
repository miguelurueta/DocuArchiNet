# Dot-sourced only by the existing DOC110 administrative bridge in explicit DOC116 mode.
function Assert-Doc116LevelResources($Resource) {
    $keys=@('userRef','companyRef','levelRef','lotId','permissionId','environmentHash')
    if($null -eq $Resource -or @($Resource.PSObject.Properties.Name).Count -ne $keys.Count){throw 'DOC116_LEVEL_RESOURCE_INVALID'}
    foreach($name in $Resource.PSObject.Properties.Name){if($keys -cnotcontains $name){throw 'DOC116_LEVEL_RESOURCE_INVALID'}}
    foreach($key in @('userRef','companyRef','levelRef','lotId','permissionId')){
        $number=0L
        if($Resource.$key -isnot [string] -or $Resource.$key -cnotmatch '^[1-9][0-9]{0,9}$' -or -not [long]::TryParse($Resource.$key,[ref]$number) -or $number -gt 4294967295L){throw 'DOC116_LEVEL_RESOURCE_INVALID'}
    }
    if($Resource.environmentHash -isnot [string] -or $Resource.environmentHash -cnotmatch '^[a-f0-9]{64}$'){throw 'DOC116_LEVEL_RESOURCE_INVALID'}
}
function Get-Doc116LevelHash($Row) {
    $fields=[ordered]@{}
    foreach($p in ($Row.PSObject.Properties | Sort-Object Name)){if($p.Name -cne 'agregar_expediente'){$fields[$p.Name]=$p.Value}}
    $sha=[Security.Cryptography.SHA256]::Create()
    try{return ([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes(($fields|ConvertTo-Json -Compress -Depth 8))))).Replace('-','').ToLowerInvariant()}finally{$sha.Dispose()}
}
function Assert-Doc116LevelRow($Resource,$Row) {
    if($null -eq $Row -or [string]$Row.id_permisos_niveles -cne $Resource.permissionId -or [string]$Row.ra_pro_niveles_id_nivel -cne $Resource.levelRef -or [string]$Row.remit_dest_interno_id_Remit_Dest_Int -cne $Resource.userRef -or [string]$Row.doc116_company -cne $Resource.companyRef -or [string]$Row.doc116_actor_company -cne $Resource.companyRef -or [string]$Row.doc116_lot -cne $Resource.lotId){throw 'DOC116_LEVEL_SCOPE_CHANGED'}
    if([string]$Row.doc116_owner -ceq $Resource.userRef){throw 'DOC116_LEVEL_OWNER_BYPASS'}
    if([string]$Row.doc116_owner -cnotmatch '^[1-9][0-9]*$' -or [string]$Row.doc116_level_active -cne '1' -or [string]$Row.doc116_actor_active -cne '1' -or [string]$Row.estado_permiso -cne '1' -or @('0','1') -cnotcontains [string]$Row.agregar_expediente){throw 'DOC116_LEVEL_NOT_EFFECTIVE'}
}
function Invoke-Doc116LevelProtocol($Resource,$Original,$Previous,[scriptblock]$Read,[scriptblock]$Set,[scriptblock]$Save,[scriptblock]$Next,[scriptblock]$Emit,[bool]$Recovery=$false) {
    Assert-Doc116LevelResources $Resource;Assert-Doc116LevelRow $Resource $Original
    $otherHash=Get-Doc116LevelHash $Original
    $record=[pscustomobject][ordered]@{version=1;userRef=$Resource.userRef;companyRef=$Resource.companyRef;levelRef=$Resource.levelRef;lotId=$Resource.lotId;permissionId=$Resource.permissionId;environmentHash=$Resource.environmentHash;originalPermission=1;otherFieldsHash=$otherHash;stage='READY';restored=$true}
    if($Recovery){
        if($null -eq $Previous -or $Previous.version -ne 1 -or $Previous.originalPermission -ne 1 -or $Previous.otherFieldsHash -cne $otherHash){throw 'DOC116_LEVEL_RECOVERY_MISMATCH'}
        foreach($k in @('userRef','companyRef','levelRef','lotId','permissionId','environmentHash')){if($Previous.$k -cne $Resource.$k){throw 'DOC116_LEVEL_RECOVERY_MISMATCH'}}
    }else{
        if($null -ne $Previous -and $Previous.restored -ne $true){throw 'DOC116_LEVEL_PREVIOUS_RECOVERY_REQUIRED'}
        if([string]$Original.agregar_expediente -cne '1'){throw 'DOC116_LEVEL_ORIGINAL_NOT_GRANTED'}
    }
    $restoreNeeded=$Recovery
    try{
        if(-not $Recovery){& $Emit ([pscustomobject]@{ok=$true;stage='READY';userRef=$Resource.userRef;companyRef=$Resource.companyRef;levelRef=$Resource.levelRef;permissionId=$Resource.permissionId;otherFieldsUnchanged=$true})}
        while(-not $Recovery){
            $operation=& $Next
            if($null -eq $operation -or $operation -ceq 'CLOSE'){break}
            if($operation -ceq 'REVOKE'){
                if($restoreNeeded){throw 'DOC116_LEVEL_ALREADY_REVOKED'}
                $current=& $Read;Assert-Doc116LevelRow $Resource $current
                if([string]$current.agregar_expediente -cne '1' -or (Get-Doc116LevelHash $current) -cne $otherHash){throw 'DOC116_LEVEL_PRECONDITION_CHANGED'}
                $record.stage='REVOKE_ATTEMPT';$record.restored=$false;& $Save $record
                $restoreNeeded=$true
                & $Set 1 0 $otherHash
                $current=& $Read;Assert-Doc116LevelRow $Resource $current
                if([string]$current.agregar_expediente -cne '0' -or (Get-Doc116LevelHash $current) -cne $otherHash){throw 'DOC116_LEVEL_REVOKE_UNCONFIRMED'}
                $record.stage='REVOKED';& $Save $record
                & $Emit ([pscustomobject]@{ok=$true;stage='REVOKED';otherFieldsUnchanged=$true})
            }elseif($operation -ceq 'RESTORE'){
                if(-not $restoreNeeded){throw 'DOC116_LEVEL_NOT_REVOKED'}
                $current=& $Read;Assert-Doc116LevelRow $Resource $current
                if((Get-Doc116LevelHash $current) -cne $otherHash){throw 'DOC116_LEVEL_OTHER_FIELDS_CHANGED'}
                if([string]$current.agregar_expediente -ceq '0'){& $Set 0 1 $otherHash}
                $current=& $Read;Assert-Doc116LevelRow $Resource $current
                if([string]$current.agregar_expediente -cne '1' -or (Get-Doc116LevelHash $current) -cne $otherHash){throw 'DOC116_LEVEL_RESTORE_UNCONFIRMED'}
                $record.stage='RESTORED';$record.restored=$true;& $Save $record;$restoreNeeded=$false
                & $Emit ([pscustomobject]@{ok=$true;stage='RESTORED';otherFieldsUnchanged=$true})
            }else{throw 'DOC116_LEVEL_OPERATION_INVALID'}
        }
    }finally{
        if($restoreNeeded){
            $current=& $Read;Assert-Doc116LevelRow $Resource $current
            if((Get-Doc116LevelHash $current) -cne $otherHash){throw 'DOC116_LEVEL_OTHER_FIELDS_CHANGED'}
            if([string]$current.agregar_expediente -ceq '0'){& $Set 0 1 $otherHash}
            $current=& $Read;Assert-Doc116LevelRow $Resource $current
            if([string]$current.agregar_expediente -cne '1' -or (Get-Doc116LevelHash $current) -cne $otherHash){throw 'DOC116_LEVEL_RESTORE_UNCONFIRMED'}
            $record.stage='RESTORED';$record.restored=$true;& $Save $record;$restoreNeeded=$false
            & $Emit ([pscustomobject]@{ok=$true;stage='RESTORED_AFTER_EXIT';otherFieldsUnchanged=$true})
        }
    }
}
function Read-Doc116LevelRow($Resource,$Transaction=$null) {
    $sql='SELECT p.*,n.remit_dest_interno_id_Remit_Dest_Int AS doc116_owner,n.estado_nivel AS doc116_level_active,u.Empresa_Gestion_Documental_id_empresa AS doc116_company,a.Empresa_Gestion_Documental_id_empresa AS doc116_actor_company,a.Estado_Usuario AS doc116_actor_active,l.lote_id AS doc116_lot FROM ra_pro_permisos_niveles p INNER JOIN ra_pro_niveles n ON n.id_nivel=p.ra_pro_niveles_id_nivel INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int INNER JOIN remit_dest_interno a ON a.id_Remit_Dest_Int=p.remit_dest_interno_id_Remit_Dest_Int INNER JOIN ra_integracion_lote l ON l.nivel_ref=n.id_nivel AND l.solicitante_ref=a.id_Remit_Dest_Int AND l.ambito_ref=a.Empresa_Gestion_Documental_id_empresa WHERE p.id_permisos_niveles=@permission AND p.ra_pro_niveles_id_nivel=@level AND p.remit_dest_interno_id_Remit_Dest_Int=@actor AND l.lote_id=@lot LIMIT 2'
    if($null -ne $Transaction){$sql+=' FOR UPDATE'}
    $rows=Read-Doc110Rows $sql @{'@permission'=$Resource.permissionId;'@level'=$Resource.levelRef;'@actor'=$Resource.userRef;'@lot'=$Resource.lotId} $Transaction
    if($rows.Count -ne 1){throw 'DOC116_LEVEL_ROW_NOT_UNIQUE'}
    Assert-Doc116LevelRow $Resource $rows[0];return $rows[0]
}
function Set-Doc116LevelPermission($Resource,[int]$Expected,[int]$Value,[string]$OtherHash) {
    if(-not (($Expected -eq 1 -and $Value -eq 0) -or ($Expected -eq 0 -and $Value -eq 1))){throw 'DOC116_LEVEL_TRANSITION_INVALID'}
    $tx=$script:raConnection.BeginTransaction()
    try{
        $row=Read-Doc116LevelRow $Resource $tx
        if([string]$row.agregar_expediente -cne [string]$Expected -or (Get-Doc116LevelHash $row) -cne $OtherHash){throw 'DOC116_LEVEL_CAS_PRECONDITION_FAILED'}
        $command=$script:raConnection.CreateCommand();$command.CommandTimeout=5;$command.Transaction=$tx
        $command.CommandText='UPDATE ra_pro_permisos_niveles SET agregar_expediente=@value WHERE id_permisos_niveles=@permission AND ra_pro_niveles_id_nivel=@level AND remit_dest_interno_id_Remit_Dest_Int=@actor AND estado_permiso=1 AND agregar_expediente=@expected'
        foreach($pair in @(@('@value',$Value),@('@permission',$Resource.permissionId),@('@level',$Resource.levelRef),@('@actor',$Resource.userRef),@('@expected',$Expected))){[void]$command.Parameters.AddWithValue($pair[0],$pair[1])}
        try{if($command.ExecuteNonQuery() -ne 1){throw 'DOC116_LEVEL_CAS_FAILED'}}finally{$command.Dispose()}
        $tx.Commit()
    }catch{try{$tx.Rollback()}catch{};throw}finally{$tx.Dispose()}
}
function Read-Doc116LevelIndependently($Resource) {
    $prior=$script:raConnection
    $builder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder($script:databaseBuilder.ConnectionString);$builder.Pooling=$false
    $connection=New-Object MySql.Data.MySqlClient.MySqlConnection($builder.ConnectionString)
    try{$connection.Open();$script:raConnection=$connection;return Read-Doc116LevelRow $Resource}finally{$script:raConnection=$prior;$connection.Dispose()}
}
function Invoke-Doc116LevelBridge([string]$ResourcesFile,[bool]$Recovery,[bool]$Inspection) {
    $lock=$null;$script:raConnection=$null
    try{
        if($env:DOC116_AUTH_LEVEL_PERMISSION -cne 'true' -or $env:DOC116_LEVEL_RESOURCES_SHA256 -cnotmatch '^[a-f0-9]{64}$'){throw 'DOC116_LEVEL_AUTHORIZATION_REQUIRED'}
        if(-not [IO.Path]::IsPathRooted($ResourcesFile) -or (Get-Item -LiteralPath $ResourcesFile).Length -gt 1048576){throw 'DOC116_LEVEL_RESOURCE_INVALID'}
        if((Get-FileHash -LiteralPath $ResourcesFile -Algorithm SHA256).Hash.ToLowerInvariant() -cne $env:DOC116_LEVEL_RESOURCES_SHA256){throw 'DOC116_LEVEL_RESOURCE_CHANGED'}
        $resource=Get-Content -LiteralPath $ResourcesFile -Raw | ConvertFrom-Json;Assert-Doc116LevelResources $resource
        $directory=Join-Path $repoRoot 'tools/e2e/artifacts';[void][IO.Directory]::CreateDirectory($directory)
        $current=Get-Item -LiteralPath $directory
        while($null -ne $current){if(($current.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0){throw 'DOC116_LEVEL_JOURNAL_PATH_INVALID'};if($current.FullName -ceq $repoRoot){break};$current=$current.Parent}
        $journal=Join-Path $directory 'doc116-level-permission-recovery.json'
        foreach($target in @($journal,$journal+'.lock')){if((Test-Path -LiteralPath $target) -and ((Get-Item -LiteralPath $target).Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0){throw 'DOC116_LEVEL_JOURNAL_PATH_INVALID'}}
        $lock=[IO.File]::Open($journal+'.lock',[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
        $previous=if(Test-Path -LiteralPath $journal){Get-Content -LiteralPath $journal -Raw | ConvertFrom-Json}else{$null}
        Open-Doc110Database
        $sha=[Security.Cryptography.SHA256]::Create();try{$environmentHash=([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($script:databaseBuilder.Server+'|'+$script:databaseBuilder.Database)))).Replace('-','').ToLowerInvariant()}finally{$sha.Dispose()}
        if($environmentHash -cne $resource.environmentHash){throw 'DOC116_LEVEL_ENVIRONMENT_CHANGED'}
        $tables=Read-Doc110Rows "SELECT TABLE_NAME,ENGINE FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN ('ra_pro_permisos_niveles','ra_pro_niveles','remit_dest_interno','ra_integracion_lote')"
        if($tables.Count -ne 4 -or @($tables|Where-Object {[string]$_.ENGINE -cne 'InnoDB'}).Count -gt 0){throw 'DOC116_LEVEL_TRANSACTIONAL_TABLES_REQUIRED'}
        $original=Read-Doc116LevelIndependently $resource
        if($Inspection){if($null -ne $previous -and $previous.restored -ne $true){throw 'DOC116_LEVEL_PREVIOUS_RECOVERY_REQUIRED'};if([string]$original.agregar_expediente -cne '1'){throw 'DOC116_LEVEL_ORIGINAL_NOT_GRANTED'};[pscustomobject]@{ok=$true;stage='INSPECT';permission='agregar_expediente';nonOwner=$true;originalGranted=$true}|ConvertTo-Json -Compress;return}
        $read={Read-Doc116LevelIndependently $resource}
        $set={param($expected,$value,$hash) Set-Doc116LevelPermission $resource $expected $value $hash}
        $save={param($record)
            if((Test-Path -LiteralPath $journal) -and ((Get-Item -LiteralPath $journal).Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0){throw 'DOC116_LEVEL_JOURNAL_PATH_INVALID'}
            $temp=$journal+'.'+$PID+'.tmp';$stream=[IO.File]::Open($temp,[IO.FileMode]::CreateNew,[IO.FileAccess]::Write,[IO.FileShare]::None)
            try{$bytes=[Text.Encoding]::UTF8.GetBytes(($record|ConvertTo-Json -Compress -Depth 8));$stream.Write($bytes,0,$bytes.Length);$stream.Flush($true)}finally{$stream.Dispose()}
            if(Test-Path -LiteralPath $journal){[IO.File]::Replace($temp,$journal,[NullString]::Value)}else{[IO.File]::Move($temp,$journal)}
        }
        $next={
            $task=[Console]::In.ReadLineAsync();if(-not $task.Wait(300000)){return $null};$line=$task.Result;if($null -eq $line){return $null}
            $value=$line|ConvertFrom-Json
            if(@($value.PSObject.Properties.Name).Count -ne 1 -or $value.PSObject.Properties.Name -cnotcontains 'operation' -or @('REVOKE','RESTORE','CLOSE') -cnotcontains $value.operation){throw 'DOC116_LEVEL_OPERATION_INVALID'}
            return $value.operation
        }
        $emit={param($result) $result|ConvertTo-Json -Compress}
        Invoke-Doc116LevelProtocol $resource $original $previous $read $set $save $next $emit $Recovery
    }catch{[pscustomobject]@{ok=$false;code='DOC116_LEVEL_OPERATION_OR_RECOVERY_FAILED'}|ConvertTo-Json -Compress;[Environment]::ExitCode=1}
    finally{if($script:raConnection){$script:raConnection.Dispose()};if($script:catalogConnection){$script:catalogConnection.Dispose()};if($lock){$lock.Dispose()}}
}
