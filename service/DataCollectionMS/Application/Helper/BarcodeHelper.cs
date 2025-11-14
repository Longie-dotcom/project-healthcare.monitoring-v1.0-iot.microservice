namespace Application.Helper
{
    public static class BarcodeType
    {
        public static readonly string Standard = "Standard";
        public static readonly string Emergency = "Emergency";
        public static readonly string Offline = "Offline";
    }

    public class BarcodeRaw
    {
        public string BarcodeType { get; set; } = string.Empty;
        public string TestOrderCode { get; set; } = string.Empty;
        public string UserIdentityNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class BarcodeHelper
    {
        public string GenerateBarcode(BarcodeRaw barcodeRaw)
        {
            throw new NotImplementedException();
        }

        public bool IsValidatedBarcode(string barcode)
        {
            throw new NotImplementedException();
        }

        public void GenerateBarcodeImage(string barcode)
        {
            throw new NotImplementedException();
        }
    }
}
