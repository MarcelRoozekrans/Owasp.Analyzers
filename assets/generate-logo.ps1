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

# Build shield path matching the SVG: M64 8 L112 28 L112 68 C112 96 88 116 64 124 C40 116 16 96 16 68 L16 28 Z
$path = New-Object System.Drawing.Drawing2D.GraphicsPath
$path.StartFigure()
$path.AddLine([System.Drawing.PointF]::new(64, 8),   [System.Drawing.PointF]::new(112, 28))
$path.AddLine([System.Drawing.PointF]::new(112, 28), [System.Drawing.PointF]::new(112, 68))
# Cubic Bezier: from (112,68) cp1=(112,96) cp2=(88,116) end=(64,124)
$path.AddBezier(
    [System.Drawing.PointF]::new(112, 68),
    [System.Drawing.PointF]::new(112, 96),
    [System.Drawing.PointF]::new(88, 116),
    [System.Drawing.PointF]::new(64, 124)
)
# Cubic Bezier: from (64,124) cp1=(40,116) cp2=(16,96) end=(16,68)
$path.AddBezier(
    [System.Drawing.PointF]::new(64, 124),
    [System.Drawing.PointF]::new(40, 116),
    [System.Drawing.PointF]::new(16, 96),
    [System.Drawing.PointF]::new(16, 68)
)
$path.AddLine([System.Drawing.PointF]::new(16, 68), [System.Drawing.PointF]::new(16, 28))
$path.CloseFigure()

# Shield fill (OWASP red #E8162B = RGB 232, 22, 43)
$brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 232, 22, 43))
$g.FillPath($brush, $path)

# Checkmark (white, thick, round caps)
$pen = New-Object System.Drawing.Pen([System.Drawing.Color]::White, 10)
$pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
$pen.EndCap   = [System.Drawing.Drawing2D.LineCap]::Round
$pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
$g.DrawLines($pen, @(
    [System.Drawing.PointF]::new(36, 66),
    [System.Drawing.PointF]::new(56, 86),
    [System.Drawing.PointF]::new(92, 46)
))

$g.Dispose()
$bmp.Save((Join-Path $PSScriptRoot "logo.png"), [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
Write-Host "Done: logo.png"
