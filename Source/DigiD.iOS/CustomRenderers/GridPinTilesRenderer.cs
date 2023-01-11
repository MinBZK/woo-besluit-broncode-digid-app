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
﻿using CoreAnimation;
using DigiD.Common.Constants;
using DigiD.Common.Controls;
using DigiD.iOS.CustomRenderers;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PinViewTiles), typeof(GridPinTilesRenderer))]
namespace DigiD.iOS.CustomRenderers
{
	internal class GridPinTilesRenderer : VisualElementRenderer<ContentView>
	{
		CAKeyFrameAnimation animation;
		protected override void OnElementChanged (ElementChangedEventArgs<ContentView> e)
		{
			base.OnElementChanged (e);

		    if (e.NewElement == null)
                return;

		    MessagingCenter.Subscribe<PinViewTiles>(this, MessagingConstants.PinError, Shake);
        }

		private void Shake(PinViewTiles tiles)
		{
		    if (tiles != Element)
		        return;

            const float strength = 12.0f;

			animation = CAKeyFrameAnimation.FromKeyPath ("transform.translation.x");
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
			animation.Duration = 0.5;
			animation.Values = new NSObject[]
			{
				NSNumber.FromFloat(-strength),
				NSNumber.FromFloat(strength),
				NSNumber.FromFloat(-strength*0.66f),
				NSNumber.FromFloat(strength*0.66f),
				NSNumber.FromFloat(-strength*0.33f),
				NSNumber.FromFloat(strength*0.33f),
				NSNumber.FromFloat(0)
			};

			Layer.AddAnimation(animation, "shake");
		}

	    protected override void Dispose(bool disposing)
	    {
	        base.Dispose(disposing);

			animation?.Dispose();

			if (disposing)
	            MessagingCenter.Unsubscribe<PinViewTiles>(this, MessagingConstants.PinError);
	    }
    }
}

