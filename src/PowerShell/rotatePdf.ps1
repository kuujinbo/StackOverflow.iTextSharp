#http://stackoverflow.com/questions/35993674
$workingDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path;
$IODirectory = (Get-Item $workingDirectory).parent.fullname;
$outputDirectory = Join-Path $IODirectory '__OUTPUT';
$inputDirectory = Join-Path $IODirectory '__INPUT';
$input = [System.IO.Path]::Combine($inputDirectory, 'samplechapter2.pdf');
$baseDirectory = (Get-Item $workingDirectory).parent.parent.fullname;
$iTextDllDirectory = Join-Path $baseDirectory '\packages';
$dll = ls $iTextDllDirectory -recurse | where {-not $_.PSIsContainer -and $_ -imatch "^itextsharp.dll$"};

[void] [System.Reflection.Assembly]::LoadFrom($dll.FullName);
$output = [System.IO.Path]::Combine($outputDirectory, 'PowerShell.rotatePdf.pdf');
<#
$e = [iTextSharp.text.pdf.PdfName]::ROTATE;

PowerShell 4.0 throws a ExtendedTypeSystemException

The field or property: "ca" for type: "iTextSharp.text.pdf.PdfName" differs 
only in letter casing from the field or property: "CA". The type must be 
Common Language Specification (CLS) compliant.

2.0 works without issue
#>

function Rotate-Pdf {
    [CmdletBinding()]
    param(
        [parameter(Mandatory=$true)] [string]$readerPath
        ,[parameter(Mandatory=$true)] [float]$degrees
    )
    $reader = New-Object iTextSharp.text.pdf.PdfReader($readerPath);
    $rotate = New-Object iTextSharp.text.pdf.PdfName('Rotate');
    $pageCount = $reader.NumberOfPages;
    for ($i = 1; $i -le $pageCount; $i++) {
        # $rotation = $reader.GetPageRotation($i);
        $pageDict = $reader.GetPageN($i);
        $pdfNumber = New-Object iTextSharp.text.pdf.PdfNumber($degrees);
        $pageDict.Put($rotate, $pdfNumber);
    }
    $memoryStream = New-Object System.IO.MemoryStream;
    $stamper = New-Object iTextSharp.text.pdf.PdfStamper($reader, $memoryStream);
    $stamper.Dispose();
    $bytes = $memoryStream.ToArray();
    $memoryStream.Dispose();
    $reader.Dispose();
    return $bytes;
}
$bytes = Rotate-Pdf $input 90;
[System.IO.File]::WriteAllBytes($output, $bytes); 