// Deze broncode is openbaar gemaakt vanwege een Woo-verzoek zodat deze 
// gericht is op transparantie en niet op hergebruik. Hergebruik van 
// de broncode is toegestaan onder de EUPL licentie, met uitzondering 
// van broncode waarvoor een andere licentie is aangegeven.
//
// Het archief waar dit bestand deel van uitmaakt is te vinden op:
//   https://github.com/MinBZK/woo-besluit-broncode-digid-app
//
// Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
//
// Voor overige vragen over dit Woo-besluit kunt u mailen met open@logius.nl
//
// This code has been disclosed in response to a request under the Dutch
// Open Government Act ("Wet open Overheid"). This implies that publication 
// is primarily driven by the need for transparence, not re-use.
// Re-use is permitted under the EUPL-license, with the exception 
// of source files that contain a different license.
//
// The archive that this file originates from can be found at:
//   https://github.com/MinBZK/woo-besluit-broncode-digid-app
//
// Security vulnerabilities may be responsibly disclosed via the Dutch NCSC:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// using the reference "Logius, publicly disclosed source code DigiD-App" 
//
// Other questions regarding this Open Goverment Act decision may be
// directed via email to open@logius.nl
//
﻿using System;
using System.IO;
using Android.Graphics;
using Android.Widget;
using CSJ2K;
using CSJ2K.j2k.util;
using DigiD.Controls;
using DigiD.Droid.CustomRenderers;
using FFImageLoading;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(WidPhoto), typeof(WidPhotoRenderer))]
namespace DigiD.Droid.CustomRenderers
{
    public class WidPhotoRenderer : ImageRenderer
    {
        public WidPhotoRenderer(Android.Content.Context context) : base(context)
        {
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null && e.NewElement is WidPhoto widPhoto)
            {
                var androidImage = new ImageView(Context);
                try
                {
                    var source = (StreamImageSource)widPhoto.Source;
                    var cancellationToken = System.Threading.CancellationToken.None;
                    var stream = await source.Stream(cancellationToken);
                    stream.Seek(0, SeekOrigin.Begin);
                    var bytes = stream.ToByteArray();

                    CSJ2K.Util.AndroidBitmapImageCreator.Register();
                    var paramList = new ParameterList(J2kImage.GetDefaultDecoderParameterList(null));
                    paramList["rate"] = "1"; // zolang de rate maar niet -1 (default) of 0 is.
                    var bmp = J2kImage.FromBytes(bytes, paramList).As<Bitmap>();

                    if (bmp != null)
                    {
                        androidImage.SetImageBitmap(bmp);
                        SetNativeControl(androidImage);
                    }
                }
                catch (InvalidOperationException ioe)
                {
                    System.Diagnostics.Debug.WriteLine($"Fout bij lezen jp2 data: [{ioe.Message}]");
                }
                catch (Exception exc)
                {
                    System.Diagnostics.Debug.WriteLine($"Fout bij lezen jp2 data: [{exc.Message}]");
                }
            }
        }
    }
}
