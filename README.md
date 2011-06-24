#Signature to Image .Net
**License:** BSD License
**Version:** 1.0 (2011-06-24)
**Requirements:** .NET 2.0, [Json.NET](http://json.codeplex.com/)

A server-side supplement to Signature Pad (http://thomasjbradley.ca/lab/signature-to-image) used to create an image of the user's signature based on either the name they entered or the hand-drawn signature they provided.

##Creating image based on Json
If you want to regenerate the signature based on the Json generated by Signature Pad you just need to call the library like so:

    SignatureToImage sigGenerator = new SignatureToImage();
    Bitmap signatureImage = sigGenerator.SigJsonToImage(signatureJson);

##Creating image based on name
Similar to using the Json, if the user typed their name instead of using the pen, just pass in their name to the library and it will use the Journal font to regenerate the signature:

    SignatureToImage sigGenerator = new SignatureToImage();
    Bitmap signatureImage = sigGenerator.SigNameToImage("John Smith");

If you do not have the Journal font installed on your server you can pass in the absolute path to the font file as an optional parameter:

    SignatureToImage sigGenerator = new SignatureToImage();
    Bitmap signatureImage = sigGenerator.SigNameToImage("John Smith", fontPath: HttpContext.Current.Request.PhysicalApplicationPath + @"Resources\journal.ttf");

You probably already have the file in your solution in your site's project as Signature Pad required it so you can reuse that same file.