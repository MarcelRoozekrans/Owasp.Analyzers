Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms

# Note: This script uses System.Drawing (GDI+) and requires Windows.
# The generated logo.png is committed to the repository; re-run only when updating the icon design.

$width = 128
$height = 128
$bmp = New-Object System.Drawing.Bitmap($width, $height)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([System.Drawing.Color]::Transparent)

# Shield fill (OWASP red #E8162B)
$brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 232, 22, 43))
$shieldPoints = @(
    [System.Drawing.PointF]::new(64, 8),
    [System.Drawing.PointF]::new(112, 28),
    [System.Drawing.PointF]::new(112, 68),
    [System.Drawing.PointF]::new(64, 124),
    [System.Drawing.PointF]::new(16, 68),
    [System.Drawing.PointF]::new(16, 28)
)
$g.FillPolygon($brush, $shieldPoints)

# Checkmark (white, thick)
$pen = New-Object System.Drawing.Pen([System.Drawing.Color]::White, 10)
$pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
$pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
$pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
$g.DrawLines($pen, @(
    [System.Drawing.PointF]::new(36, 66),
    [System.Drawing.PointF]::new(56, 86),
    [System.Drawing.PointF]::new(92, 46)
))

$g.Dispose()
$bmp.Save((Join-Path $PSScriptRoot "logo.png"), [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-Host "Done: assets\logo.png"
