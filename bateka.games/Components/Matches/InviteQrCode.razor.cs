using Microsoft.AspNetCore.Components;
using QRCoder;

namespace bateka.games.Components.Matches;

/// <summary>Draws <see cref="Url"/> as a QR code (a PNG made in C#, no JS). Redrawn only when the URL changes.</summary>
public partial class InviteQrCode
{
    private const int PixelsPerModule = 8;
    private const string PngDataUrlPrefix = "data:image/png;base64,";

    [Parameter, EditorRequired] public string Url { get; set; } = string.Empty;

    /// <summary>Shown size in CSS pixels (the PNG is larger, so it stays sharp).</summary>
    [Parameter] public int Size { get; set; } = 200;

    private string? _drawnFor;
    private string? _dataUrl;

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(Url) || Url == _drawnFor) return;

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(Url, QRCodeGenerator.ECCLevel.M);
        using var png = new PngByteQRCode(data);
        _dataUrl = PngDataUrlPrefix + Convert.ToBase64String(png.GetGraphic(PixelsPerModule));
        _drawnFor = Url;
    }
}
