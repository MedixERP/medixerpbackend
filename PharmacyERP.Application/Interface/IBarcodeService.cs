public interface IBarcodeService
{
    string GenerateBarcodeValue();

    byte[] GenerateBarcode(string code);

    byte[] GenerateQrCode(string data);
}