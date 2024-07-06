using System.Drawing;
using System.Web.Http;
using MASTERPM.Web.Api.Base;
using QRCoder;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Collections.Generic;

namespace MASTERPM.Web.Api.QRCodeHelper
{
    public class QRCodeHelperController
    {
        
        public static Image GetQRCodeImage(string qrText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(1);
            var byteArrayIn = BitmapToBytes(qrCodeImage);
            byteArrayIn = AddBit(byteArrayIn);
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        private static Byte[] AddBit(Byte[] byteArrayIn)
        {
            var length = byteArrayIn.Length;
            List<Byte> termsList = new List<Byte>();
            for (int i = 0; i < length; i++)
            {
                termsList.Add(byteArrayIn[i]);
            }
            termsList.Add(120);
            termsList.Add(120);
            termsList.Add(120);
            termsList.Add(120);
            termsList.Add(120);

            // You can convert it back to an array if you would like to
            Byte[] terms = termsList.ToArray();

            return terms;
        }



        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }

    
}