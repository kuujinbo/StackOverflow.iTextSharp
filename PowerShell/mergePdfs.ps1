# http://stackoverflow.com/questions/33123337
$workingDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path;

$IODirectory = (Get-Item $workingDirectory).parent.fullname;
$inputDirectory = Join-Path $IODirectory '__INPUT';
$outputDirectory = Join-Path $IODirectory '__OUTPUT';

$baseDirectory = (Get-Item $workingDirectory).parent.parent.fullname;
$iTextDllDirectory = Join-Path $baseDirectory '\packages';
$dll = ls $iTextDllDirectory -recurse | where {-not $_.PSIsContainer -and $_ -imatch "^itextsharp.dll$"};

[void] [System.Reflection.Assembly]::LoadFrom($dll.FullName);
$output = [System.IO.Path]::Combine($outputDirectory, 'PowerShell.mergePdfs.pdf');
$fileStream = New-Object System.IO.FileStream($output, [System.IO.FileMode]::OpenOrCreate);
$document = New-Object iTextSharp.text.Document;
$pdfCopy = New-Object iTextSharp.text.pdf.PdfCopy($document, $fileStream);
$document.Open();

foreach ($pdf in $pdfs) {
    $reader = New-Object iTextSharp.text.pdf.PdfReader($pdf.FullName);
    $pdfCopy.AddDocument($reader);
    $reader.Dispose();  
}

$pdfCopy.Dispose();
$document.Dispose();
$fileStream.Dispose();