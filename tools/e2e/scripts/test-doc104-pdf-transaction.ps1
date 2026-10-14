$ErrorActionPreference = "Stop"

$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot "../../.."))
$applicationAssembly = Join-Path $repoRoot "bin/GestionDocumental-Docuarchi.net.dll"
$itextAssembly = Join-Path $repoRoot "bin/itextsharp.dll"
if (-not (Test-Path -LiteralPath $applicationAssembly) -or -not (Test-Path -LiteralPath $itextAssembly)) {
    throw "DOC104_ASSEMBLY_NOT_FOUND"
}

Add-Type -Path $itextAssembly
$assembly = [Reflection.Assembly]::LoadFrom($applicationAssembly)
$classType = $assembly.GetType("GestionDocumental_Docuarchi.net.Class_ItexShare", $true)
$contextType = $classType.GetNestedType("PdfGraphOperationContext", [Reflection.BindingFlags]::NonPublic)
$instance = [Activator]::CreateInstance($classType)
$validateMethod = $classType.GetMethod("ValidatePdf", [Reflection.BindingFlags]::Instance -bor [Reflection.BindingFlags]::NonPublic)
$replaceMethod = $classType.GetMethod("ReplacePdfSafely", [Reflection.BindingFlags]::Instance -bor [Reflection.BindingFlags]::NonPublic)

function New-SyntheticPdf([string]$target, [int]$pages) {
    $stream = [IO.File]::Open($target, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::None)
    $document = [iTextSharp.text.Document]::new()
    try {
        [iTextSharp.text.pdf.PdfWriter]::GetInstance($document, $stream) | Out-Null
        $document.Open()
        for ($index = 1; $index -le $pages; $index++) {
            $document.Add([iTextSharp.text.Paragraph]::new("DOC104-$index")) | Out-Null
            if ($index -lt $pages) { $document.NewPage() | Out-Null }
        }
    }
    finally {
        if ($document.IsOpen()) { $document.Close() }
        $stream.Dispose()
    }
}

$testRoot = Join-Path ([IO.Path]::GetTempPath()) ("doc104-transaction-" + [Guid]::NewGuid().ToString("N"))
[IO.Directory]::CreateDirectory($testRoot) | Out-Null
try {
    $original = Join-Path $testRoot "original.pdf"
    $staged = Join-Path $testRoot "staged.pdf"
    $backup = Join-Path $testRoot "backup.pdf"
    $failed = Join-Path $testRoot "failed.pdf"
    $lock = Join-Path $testRoot ".original.pdf.graph.lock"
    New-SyntheticPdf $original 2
    New-SyntheticPdf $staged 2
    $originalHash = (Get-FileHash -LiteralPath $original -Algorithm SHA256).Hash

    if (-not [bool]$validateMethod.Invoke($instance, [object[]]@([string]$staged, [int]2, [int]2))) {
        throw "DOC104_STAGED_VALIDATION_FAILED"
    }
    $invalid = Join-Path $testRoot "invalid.pdf"
    [IO.File]::WriteAllBytes($invalid, [byte[]](1, 2, 3))
    if ([bool]$validateMethod.Invoke($instance, [object[]]@([string]$invalid, [int]2, [int]1))) {
        throw "DOC104_INVALID_PDF_ACCEPTED"
    }
    if ((Get-FileHash -LiteralPath $original -Algorithm SHA256).Hash -ne $originalHash) {
        throw "DOC104_ORIGINAL_CHANGED_DURING_VALIDATION"
    }

    $context = [Activator]::CreateInstance($contextType, $true)
    $contextType.GetProperty("StagedPdfPath").SetValue($context, [string]$staged)
    $contextType.GetProperty("BackupPdfPath").SetValue($context, [string]$backup)
    $contextType.GetProperty("FailedPdfPath").SetValue($context, [string]$failed)
    $replaceArguments = [object[]]@($context, [string]$original, [int]2, [int]2, [bool]$false)
    if (-not [bool]$replaceMethod.Invoke($instance, $replaceArguments)) {
        throw "DOC104_SAFE_REPLACE_FAILED"
    }
    if ([bool]$replaceArguments[4] -or (Test-Path -LiteralPath $backup)) {
        throw "DOC104_BACKUP_CLEANUP_FAILED"
    }
    if (-not [bool]$validateMethod.Invoke($instance, [object[]]@([string]$original, [int]2, [int]2))) {
        throw "DOC104_REPLACED_PDF_INVALID"
    }

    $replaceBlockedOriginal = Join-Path $testRoot "replace-blocked-original.pdf"
    $replaceBlockedStaged = Join-Path $testRoot "replace-blocked-staged.pdf"
    $replaceBlockedBackup = Join-Path $testRoot "replace-blocked-backup.pdf"
    $replaceBlockedFailed = Join-Path $testRoot "replace-blocked-failed.pdf"
    New-SyntheticPdf $replaceBlockedOriginal 2
    New-SyntheticPdf $replaceBlockedStaged 2
    $replaceBlockedHash = (Get-FileHash -LiteralPath $replaceBlockedOriginal -Algorithm SHA256).Hash
    $replaceBlockedContext = [Activator]::CreateInstance($contextType, $true)
    $contextType.GetProperty("StagedPdfPath").SetValue($replaceBlockedContext, [string]$replaceBlockedStaged)
    $contextType.GetProperty("BackupPdfPath").SetValue($replaceBlockedContext, [string]$replaceBlockedBackup)
    $contextType.GetProperty("FailedPdfPath").SetValue($replaceBlockedContext, [string]$replaceBlockedFailed)
    $heldOriginal = [IO.File]::Open($replaceBlockedOriginal, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::None)
    try {
        $blockedArguments = [object[]]@($replaceBlockedContext, [string]$replaceBlockedOriginal, [int]2, [int]1, [bool]$false)
        if ([bool]$replaceMethod.Invoke($instance, $blockedArguments)) { throw "DOC104_REPLACE_FAILURE_NOT_DETECTED" }
    }
    finally { $heldOriginal.Dispose() }
    if ((Get-FileHash -LiteralPath $replaceBlockedOriginal -Algorithm SHA256).Hash -ne $replaceBlockedHash) {
        throw "DOC104_REPLACE_FAILURE_CHANGED_ORIGINAL"
    }

    $rollbackOriginal = Join-Path $testRoot "rollback-original.pdf"
    $rollbackStaged = Join-Path $testRoot "rollback-staged.pdf"
    $rollbackBackup = Join-Path $testRoot "rollback-backup.pdf"
    $rollbackFailed = Join-Path $testRoot "rollback-failed.pdf"
    New-SyntheticPdf $rollbackOriginal 2
    New-SyntheticPdf $rollbackStaged 1
    $rollbackHash = (Get-FileHash -LiteralPath $rollbackOriginal -Algorithm SHA256).Hash
    $rollbackContext = [Activator]::CreateInstance($contextType, $true)
    $contextType.GetProperty("StagedPdfPath").SetValue($rollbackContext, [string]$rollbackStaged)
    $contextType.GetProperty("BackupPdfPath").SetValue($rollbackContext, [string]$rollbackBackup)
    $contextType.GetProperty("FailedPdfPath").SetValue($rollbackContext, [string]$rollbackFailed)
    $rollbackArguments = [object[]]@($rollbackContext, [string]$rollbackOriginal, [int]2, [int]1, [bool]$false)
    if ([bool]$replaceMethod.Invoke($instance, $rollbackArguments)) { throw "DOC104_REVALIDATION_FAILURE_NOT_DETECTED" }
    if ((Get-FileHash -LiteralPath $rollbackOriginal -Algorithm SHA256).Hash -ne $rollbackHash -or
        -not [bool]$validateMethod.Invoke($instance, [object[]]@([string]$rollbackOriginal, [int]2, [int]1))) {
        throw "DOC104_ROLLBACK_DID_NOT_RESTORE_ORIGINAL"
    }

    $firstLock = [IO.File]::Open($lock, [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
    try {
        $blocked = $false
        try {
            $secondLock = [IO.File]::Open($lock, [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
            $secondLock.Dispose()
        }
        catch [IO.IOException] {
            $blocked = $true
        }
        if (-not $blocked) { throw "DOC104_DOCUMENT_LOCK_FAILED" }
    }
    finally {
        $firstLock.Dispose()
    }

    $otherLock = Join-Path $testRoot ".other.pdf.graph.lock"
    $firstDistinctLock = [IO.File]::Open($lock, [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
    try {
        $secondDistinctLock = [IO.File]::Open($otherLock, [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
        $secondDistinctLock.Dispose()
    }
    finally { $firstDistinctLock.Dispose() }

    foreach ($released in @($original, $replaceBlockedOriginal, $rollbackOriginal)) {
        $probe = [IO.File]::Open($released, [IO.FileMode]::Open, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
        $probe.Dispose()
    }

    "DOC104_PDF_TRANSACTION_STAGED_VALIDATION=PASSED"
    "DOC104_PDF_TRANSACTION_REPLACE_FAILURE=PASSED"
    "DOC104_PDF_TRANSACTION_REVALIDATION_ROLLBACK=PASSED"
    "DOC104_PDF_TRANSACTION_CONCURRENCY=PASSED"
    "DOC104_PDF_TRANSACTION_RESOURCES_RELEASED=PASSED"
    "DOC104_PDF_TRANSACTION_RESULT=PASSED"
}
finally {
    if ([IO.Directory]::Exists($testRoot)) { [IO.Directory]::Delete($testRoot, $true) }
}
