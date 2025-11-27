using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace Logic.Services
{
    public class QrCodeGeneratorService
    {
        public byte[] GenerateQrCode(string userId, int eventId)
        {
            // Step 1: Create the payload string
            string payload = $"user:{userId};event:{eventId}";

            // Step 2: Initialize QR generator
            using (var qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);

                // Step 3: Render QR code as bitmap
                using (var qrCode = new QRCode(qrCodeData))
                using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                {
                    // Step 4: Convert to byte array (PNG)
                    using (var ms = new MemoryStream())
                    {
                        qrBitmap.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
        }

        public (string userId, int eventId) ExtractIds(string qrPayload)
        {
            var parts = qrPayload.Split(';');

            string userId = null;
            int eventId = 0;

            foreach (var part in parts)
            {
                var kv = part.Split(':', 2); // only split into 2 parts
                if (kv.Length == 2)
                {
                    var key = kv[0].Trim().ToLower();
                    var value = kv[1].Trim();

                    if (key == "user")
                        userId = value;
                    else if (key == "event")
                        eventId = int.Parse(value);
                }
            }

            return (userId, eventId);
        }
    }
}
