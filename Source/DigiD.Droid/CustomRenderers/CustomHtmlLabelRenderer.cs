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
﻿using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using DigiD.Controls;
using DigiD.Droid.CustomRenderers;
using LabelHtml.Forms.Plugin.Droid;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(CustomHtmlLabel), typeof(CustomHtmlLabelRenderer))]
namespace DigiD.Droid.CustomRenderers
{
    public class CustomHtmlLabelRenderer : HtmlLabelRenderer
    {
        public CustomHtmlLabelRenderer(Context context) : base(context)
        { }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            Control.PaintFlags = PaintFlags.SubpixelText | PaintFlags.AntiAlias;
            Control.SetAllCaps(false);

            //Als de HtmlLabel tekst geen hyperlink(s) bevat, dan de tekst ook niet klikbaar maken
            if (e.PropertyName == Label.TextProperty.PropertyName && (string.IsNullOrEmpty(Element.Text) || (Element.Text != null && !(Element.Text.Contains("<a href") || Element.Text.Contains("&lt;a href")))))
            {
                Control.Clickable = false;
                Control.LongClickable = false;
                if (string.IsNullOrEmpty(Element.Text))
                    AutomationProperties.SetIsInAccessibleTree(Element, false);
            }
        }
    }
}
