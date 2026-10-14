$ErrorActionPreference = "Stop"

$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot "../../.."))
$applicationAssembly = Join-Path $repoRoot "bin/GestionDocumental-Docuarchi.net.dll"
$itextAssembly = Join-Path $repoRoot "bin/itextsharp.dll"
if (-not (Test-Path -LiteralPath $applicationAssembly) -or -not (Test-Path -LiteralPath $itextAssembly)) {
    throw "DOC104_ASSEMBLY_NOT_FOUND"
}

Add-Type -AssemblyName System.Drawing
Add-Type -Path $itextAssembly
$assembly = [Reflection.Assembly]::LoadFrom($applicationAssembly)
$strategyType = $assembly.GetType("GestionDocumental_Docuarchi.net.EstrategiaCargaPdf", $true)
$selectMethod = $strategyType.GetMethod("Seleccionar", [Reflection.BindingFlags]::Public -bor [Reflection.BindingFlags]::Static)

function Get-Hash([string]$path) {
    return (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
}

function Get-PageContentHash([string]$path) {
    $reader = [iTextSharp.text.pdf.PdfReader]::new($path)
    try {
        $sha = [Security.Cryptography.SHA256]::Create()
        try { return ([BitConverter]::ToString($sha.ComputeHash($reader.GetPageContent(1)))).Replace("-", "") }
        finally { $sha.Dispose() }
    }
    finally { $reader.Close() }
}

function Test-LinearizedHeader([string]$path) {
    $stream = [IO.File]::Open($path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::ReadWrite)
    try {
        $length = [Math]::Min(4096, [int]$stream.Length)
        $bytes = [byte[]]::new($length)
        $read = $stream.Read($bytes, 0, $length)
        return [Text.Encoding]::ASCII.GetString($bytes, 0, $read).Contains("/Linearized")
    }
    finally { $stream.Dispose() }
}

function New-SyntheticPdf([string]$target, [int]$rotation, [bool]$linearizedMarker) {
    $stream = [IO.File]::Open($target, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::None)
    try {
        $document = [iTextSharp.text.Document]::new([iTextSharp.text.PageSize]::A4)
        try {
            $writer = [iTextSharp.text.pdf.PdfWriter]::GetInstance($document, $stream)
            $document.Open()
            $writer.AddPageDictEntry([iTextSharp.text.pdf.PdfName]::ROTATE, [iTextSharp.text.pdf.PdfNumber]::new($rotation))
            if ($linearizedMarker) {
                $writer.AddPageDictEntry([iTextSharp.text.pdf.PdfName]::new("Linearized"), [iTextSharp.text.pdf.PdfNumber]::new(1))
            }
            $document.Add([iTextSharp.text.Paragraph]::new("DOC104 ROTATION $rotation")) | Out-Null
        }
        finally { if ($document.IsOpen()) { $document.Close() } }
    }
    finally { $stream.Dispose() }
    if ($linearizedMarker) {
        $append = [IO.File]::Open($target, [IO.FileMode]::Append, [IO.FileAccess]::Write, [IO.FileShare]::None)
        try {
            $padding = [byte[]]::new(1100000)
            $append.Write($padding, 0, $padding.Length)
        }
        finally { $append.Dispose() }
    }
}

function Add-Graph([string]$source, [string]$target, [string]$graph, [int]$rotation) {
    $reader = [iTextSharp.text.pdf.PdfReader]::new($source)
    $stream = [IO.File]::Open($target, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::None)
    $stamper = $null
    try {
        if ($reader.NumberOfPages -ne 1) { throw "DOC104_PAGE_COUNT_INVALID" }
        $pageSize = $reader.GetPageSize(1)
        [double]$pageHeight = $pageSize.Height
        [double]$pageWidth = $pageSize.Width
        if (($rotation % 180) -ne 0) {
            $pageHeight = $pageSize.Width
            $pageWidth = $pageSize.Height
        }
        [double]$canvasWidth = 900
        [double]$canvasHeight = 1200
        [double]$annotationX = 225
        [double]$annotationY = 840
        [double]$annotationHeight = 90
        [double]$widthScale = if ($pageWidth -lt $canvasWidth) { $canvasWidth / $pageWidth } else { $pageWidth / $canvasWidth }
        [double]$heightScale = if ($pageHeight -lt $canvasHeight) { $canvasHeight / $pageHeight } else { $pageHeight / $canvasHeight }
        [single]$pdfX = $annotationX / $widthScale
        [single]$pdfY = ($annotationY / $heightScale) - $annotationHeight

        $stamper = [iTextSharp.text.pdf.PdfStamper]::new($reader, $stream)
        $image = [iTextSharp.text.Image]::GetInstance($graph)
        $image.SetAbsolutePosition($pdfX, $pdfY)
        $image.ScaleAbsolute(90, 90)
        $stamper.GetOverContent(1).AddImage($image)
        $stamper.Close()
        $stamper = $null
        $stream = $null
        $reader.Close()
        $reader = $null
        return [pscustomobject]@{ X = $pdfX; Y = $pdfY }
    }
    finally {
        if ($null -ne $stamper) { $stamper.Close() }
        if ($null -ne $stream) { $stream.Dispose() }
        if ($null -ne $reader) { $reader.Close() }
    }
}

function Assert-StampedMatrix([string]$path, [single]$x, [single]$y) {
    $reader = [iTextSharp.text.pdf.PdfReader]::new($path)
    try {
        $content = [Text.Encoding]::ASCII.GetString($reader.GetPageContent(1))
        $number = "[-+]?[0-9]+(?:\.[0-9]+)?"
        $matches = [regex]::Matches($content, "(?m)($number)\s+0\s+0\s+($number)\s+($number)\s+($number)\s+cm")
        $matrix = $matches | Where-Object {
            [Math]::Abs([double]$_.Groups[1].Value - 90) -lt 0.01 -and
            [Math]::Abs([double]$_.Groups[2].Value - 90) -lt 0.01
        } | Select-Object -Last 1
        if ($null -eq $matrix) { throw "DOC104_GRAPH_MATRIX_NOT_FOUND" }
        if ([Math]::Abs([double]$matrix.Groups[3].Value - $x) -gt 0.02 -or
            [Math]::Abs([double]$matrix.Groups[4].Value - $y) -gt 0.02) {
            throw "DOC104_GRAPH_POSITION_CHANGED"
        }
        return Get-PageContentHash $path
    }
    finally { $reader.Close() }
}

$testRoot = Join-Path ([IO.Path]::GetTempPath()) ("doc104-characterization-" + [Guid]::NewGuid().ToString("N"))
[IO.Directory]::CreateDirectory($testRoot) | Out-Null
try {
    $graph = Join-Path $testRoot "graph.png"
    $bitmap = [Drawing.Bitmap]::new(90, 90, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
    try {
        $graphics = [Drawing.Graphics]::FromImage($bitmap)
        try {
            $graphics.Clear([Drawing.Color]::Transparent)
            $graphics.FillRectangle([Drawing.Brushes]::Red, 0, 0, 89, 89)
            $graphics.FillRectangle([Drawing.Brushes]::Black, 10, 10, 20, 20)
        }
        finally { $graphics.Dispose() }
        $bitmap.Save($graph, [Drawing.Imaging.ImageFormat]::Png)
    }
    finally { $bitmap.Dispose() }

    $fingerprints = @()
    foreach ($rotation in @(0, 90, 180, 270)) {
        $before = Join-Path $testRoot "before-$rotation.pdf"
        $after = Join-Path $testRoot "after-$rotation.pdf"
        New-SyntheticPdf $before $rotation $false
        $beforeHash = Get-Hash $before
        $beforeVisualHash = Get-PageContentHash $before
        $position = Add-Graph $before $after $graph $rotation
        if ((Get-Hash $before) -ne $beforeHash) { throw "DOC104_ORIGINAL_CHANGED" }
        $visualHash = Assert-StampedMatrix $after $position.X $position.Y
        $probe = [IO.File]::Open($after, [IO.FileMode]::Open, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
        $probe.Dispose()
        $moved = "$after.moved"
        [IO.File]::Move($after, $moved)
        [IO.File]::Move($moved, $after)
        $fingerprints += [pscustomobject]@{ Rotation = $rotation; Pages = 1; BeforeVisual = $beforeVisualHash; AfterVisual = $visualHash }
    }

    $linearizedBefore = Join-Path $testRoot "linearized-before.pdf"
    $linearizedAfter = Join-Path $testRoot "linearized-after.pdf"
    New-SyntheticPdf $linearizedBefore 0 $true
    if (-not (Test-LinearizedHeader $linearizedBefore)) { throw "DOC104_LINEARIZED_BASELINE_MISSING" }
    $modeBefore = [string]$selectMethod.Invoke($null, [object[]]@([string]$linearizedBefore))
    Add-Graph $linearizedBefore $linearizedAfter $graph 0 | Out-Null
    $modeAfter = [string]$selectMethod.Invoke($null, [object[]]@([string]$linearizedAfter))
    $linearizedAfterHeader = Test-LinearizedHeader $linearizedAfter
    if ($modeBefore -ne "range" -or $modeAfter -ne "stream" -or -not $linearizedAfterHeader) {
        "DOC104_CHARACTERIZATION_DIAGNOSTIC_BEFORE=$modeBefore"
        "DOC104_CHARACTERIZATION_DIAGNOSTIC_AFTER=$modeAfter"
        "DOC104_CHARACTERIZATION_DIAGNOSTIC_HEADER_AFTER=$linearizedAfterHeader"
        throw "DOC104_LINEARIZATION_FALLBACK_INVALID"
    }

    if ($fingerprints.Count -ne 4 -or ($fingerprints | Select-Object -ExpandProperty AfterVisual -Unique).Count -ne 4) {
        throw "DOC104_VISUAL_FINGERPRINTS_INVALID"
    }
    $artifactDirectory = Join-Path $repoRoot "tools/e2e/artifacts"
    [IO.Directory]::CreateDirectory($artifactDirectory) | Out-Null
    $evidence = [ordered]@{
        result = "PASSED"
        rotations = $fingerprints
        graphSize = "90x90"
        linearizedBefore = $true
        linearizedAfter = $linearizedAfterHeader
        loadingModeAfter = $modeAfter
    }
    [IO.File]::WriteAllText((Join-Path $artifactDirectory "doc104-characterization.json"),
        ($evidence | ConvertTo-Json -Depth 5), [Text.Encoding]::UTF8)
    "DOC104_CHARACTERIZATION_ROTATIONS=4"
    "DOC104_CHARACTERIZATION_GRAPH_SIZE=90x90"
    "DOC104_CHARACTERIZATION_LINEARIZED_BEFORE=true"
    "DOC104_CHARACTERIZATION_LINEARIZED_AFTER=$($linearizedAfterHeader.ToString().ToLowerInvariant())"
    "DOC104_CHARACTERIZATION_LOADING_MODE_AFTER=stream"
    "DOC104_CHARACTERIZATION_RESULT=PASSED"
}
finally {
    if ([IO.Directory]::Exists($testRoot)) { [IO.Directory]::Delete($testRoot, $true) }
}
