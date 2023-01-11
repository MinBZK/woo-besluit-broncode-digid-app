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
﻿using Android.Content;
using Android.Graphics;
using DigiD.Droid.CustomRenderers;
using DigiD.Common.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer (typeof(Button), typeof(CustomButtonRenderer))]
namespace DigiD.Droid.CustomRenderers
{
	internal class CustomButtonRenderer : ButtonRenderer
    {
	    public CustomButtonRenderer(Context context) : base(context)
        {
            
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged (e);

            if (e.NewElement == null || Control == null)
                return;

            Control.SetAllCaps(false);
            Control.PaintFlags = Control.PaintFlags | PaintFlags.SubpixelText;
            Control.SoundEffectsEnabled = false;
            Control.BreakStrategy = Android.Text.BreakStrategy.Balanced;

            Control.SetMaxLines(3);


            foreach (var effect in Element.Effects)
            {
                if (effect is A11YEffect)
                {
                    var controlType = A11YEffect.GetControlType(Element);
                    if (controlType == A11YControlTypes.MenuItem || controlType == A11YControlTypes.Link)
                        Control.Gravity = Android.Views.GravityFlags.Left | Android.Views.GravityFlags.CenterVertical;
                }
            }
        }
    }
}
