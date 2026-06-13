using BarcodeStandard;
using SkiaSharp;
using QRCoder;

public class BarcodeService : IBarcodeService
{
    
    public string GenerateBarcodeValue()
    {
        Random random = new Random();

        return random.NextInt64(
            100000000000,
            999999999999).ToString();
    }

   
    public byte[] GenerateBarcode(string code)
    {
        var barcode = new Barcode();

        using var image = barcode.Encode(
            BarcodeStandard.Type.Code128,
            code,
            SKColors.Black,
            SKColors.White,
            400,
            150
        );

        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    public byte[] GenerateQrCode(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(
            data,
            QRCodeGenerator.ECCLevel.Q
        );

        var qrCode = new PngByteQRCode(qrCodeData);

        return qrCode.GetGraphic(20);
    }
}