/*
 * Author:      Curtis Herbert (me@forgottenexpanse.com)
 * License:     BSD License 
 * Version: 	1.0 (2011-03-31)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Newtonsoft.Json;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

namespace ConsumedByCode.SignatureToImage
{
    /// <summary>
    /// A server-side supplement to Signature Pad (http://thomasjbradley.ca/lab/signature-to-image) used to create an image 
    /// of the user's signature based on either the name they entered or the hand-drawn signature they provided.
    /// </summary>
    public class SignatureToImage
    {
        public Color PenColor { get; set; }
        public Color Background { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int PenWidth { get; set; }
        public int FontSize { get; set; }

        private const string FONT_FAMILY = "Journal";

        /// <summary>
        /// Gets a new signature gernator with the default options.
        /// </summary>
        public SignatureToImage()
        {
            PenColor = Color.Black;
            Background = Color.White;
            Height = 55;
            Width = 198;
            PenWidth = 2;
            FontSize = 24;
        }

        /// <summary>
        /// Draws a signature based on the JSON provided by Signature Pad.
        /// </summary>
        /// <param name="json">JSON string of line drawing commands.</param>
        /// <returns>Bitmap image containing the user's signature.</returns>
        public Bitmap SigJsonToImage(string json)
        {
            Bitmap signatureImage = new Bitmap(Width, Height);
            signatureImage.MakeTransparent();
            using (Graphics signatureGraphic = Graphics.FromImage(signatureImage))
            {
                signatureGraphic.Clear(Background);
                signatureGraphic.SmoothingMode = SmoothingMode.AntiAlias;
                Pen pen = new Pen(PenColor, PenWidth);
                List<SignatureLine> lines = (List<SignatureLine>)JsonConvert.DeserializeObject(json ?? string.Empty, typeof(List<SignatureLine>));
                foreach (SignatureLine line in lines)
                {
                    signatureGraphic.DrawLine(pen, line.lx, line.ly, line.mx, line.my);
                }
            }
            return signatureImage;
        }

        /// <summary>
        /// Draws a signature using the journal font.
        /// </summary>
        /// <param name="name">User's name to create a signature for.</param>
        /// <param name="fontPath">Full path of journal.ttf. Should be passed if system doesn't have the font installed.</param>
        /// <returns>Bitmap image containing the user's signature.</returns>
        public Bitmap SigNameToImage(string name, string fontPath = null)
        {
            //we need a reference to the font, be it the .tff in the site project or the version installed on the host
            if (string.IsNullOrEmpty(fontPath) && !FontFamily.Families.Any(f => f.Name.Equals(FONT_FAMILY)))
            {
                throw new ArgumentException("FontPath must point to the copy of journal.ttf when the system does not have the font installed", "fontPath");                
            }

            Bitmap signatureImage = new Bitmap(Width, Height);
            signatureImage.MakeTransparent();
            using (Graphics signatureGraphic = Graphics.FromImage(signatureImage))
            {
                signatureGraphic.Clear(Background);

                Font font;
                if (!string.IsNullOrEmpty(fontPath))
                {
                    //to make sure the host doesn't need the font installed, use a private font collection
                    PrivateFontCollection collection = new PrivateFontCollection();
                    collection.AddFontFile(fontPath);
                    font = new Font(collection.Families.First(), FontSize);
                }
                else
                {
                    //fall back to the version installed on the host
                    font = new Font(FONT_FAMILY, FontSize);
                }
                
                signatureGraphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                signatureGraphic.DrawString(name ?? string.Empty, font, new SolidBrush(PenColor), new PointF(0, 0));
            }
            return signatureImage;
        }

        /// <summary>
        /// Line drawing commands as generated by the Signature Pad JSON export option.
        /// </summary>
        private class SignatureLine
        {
            public int lx { get; set; }
            public int ly { get; set; }
            public int mx { get; set; }
            public int my { get; set; }
        }
    }
}