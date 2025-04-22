using SkiaSharp;

namespace MiniFlexCrmApi.Util;

public class DefaultProfileImageGenerator
{
    public static string Create(string name, int userId)
    {
        // Generate a random color based on user ID
        var random = new Random(userId);
        var hue = random.NextDouble();
        var saturation = 0.7 + (random.NextDouble() * 0.3); // 0.7-1.0
        var value = 0.7 + (random.NextDouble() * 0.3); // 0.7-1.0
        
        // Convert HSV to RGB
        var rgb = HsvToRgb(hue, saturation, value);
        var color = new SKColor((byte)rgb[0], (byte)rgb[1], (byte)rgb[2]);
        
        // Create a 200x200 image
        using var surface = SKSurface.Create(new SKImageInfo(200, 200));
        var canvas = surface.Canvas;
        
        // Fill background with the color
        canvas.Clear(color);
        
        // Get initials
        var initials = string.Empty;
        var splitNames = name.Split(" ");
        if (splitNames.Length > 1)
        {
            var firstInitial = !string.IsNullOrEmpty(splitNames[0]) ? splitNames[0][0].ToString().ToUpper() : "";
            var lastInitial = !string.IsNullOrEmpty(splitNames[splitNames.Length-1]) ? splitNames[splitNames.Length-1][0].ToString().ToUpper() : "";
            initials = $"{firstInitial}{lastInitial}";
        } 
        else if (string.IsNullOrWhiteSpace(name))
            initials = "()";
        else if (splitNames.Length == 1)
            initials = name[0].ToString();
        

        // Draw initials
        using var font = new SKFont(SKTypeface.Default, 80);
        using var paint = new SKPaint();
        paint.Color = SKColors.White;
        paint.IsAntialias = true;

        canvas.DrawText(initials, 100, 110, SKTextAlign.Center, font, paint);
        
        // Convert to base64
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return Convert.ToBase64String(data.ToArray());
    }
    
    private static int[] HsvToRgb(double h, double s, double v)
    {
        var r = 0;
        var g = 0;
        var b = 0;
        
        var i = (int)Math.Floor(h * 6);
        var f = h * 6 - i;
        var p = v * (1 - s);
        var q = v * (1 - f * s);
        var t = v * (1 - (1 - f) * s);
        
        switch (i % 6)
        {
            case 0:
                r = (int)(v * 255);
                g = (int)(t * 255);
                b = (int)(p * 255);
                break;
            case 1:
                r = (int)(q * 255);
                g = (int)(v * 255);
                b = (int)(p * 255);
                break;
            case 2:
                r = (int)(p * 255);
                g = (int)(v * 255);
                b = (int)(t * 255);
                break;
            case 3:
                r = (int)(p * 255);
                g = (int)(q * 255);
                b = (int)(v * 255);
                break;
            case 4:
                r = (int)(t * 255);
                g = (int)(p * 255);
                b = (int)(v * 255);
                break;
            case 5:
                r = (int)(v * 255);
                g = (int)(p * 255);
                b = (int)(q * 255);
                break;
        }
        
        return [r, g, b];
    }
}