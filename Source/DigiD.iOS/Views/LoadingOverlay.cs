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
﻿using System.Drawing;
using Foundation;
using UIKit;

namespace DigiD.iOS.Views
{
    [Register("LoadingOverlay")]
    internal sealed class LoadingOverlay : UIView
    {
        UIActivityIndicatorView activitySpinner;

        public LoadingOverlay(RectangleF frame, string message, string imageSource, string cancelText) : base(frame)
        {
            BackgroundColor = UIColor.Black;
            Alpha = 0.60f;
            AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

            var labelHeight = 22;
            var labelWidth = (float)Frame.Width - 20;

            float centerX = (float)Frame.Width / 2;
            float centerY = (float)Frame.Height / 2;

            if (!string.IsNullOrEmpty(message))
            {
                activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);

                activitySpinner.Frame = new RectangleF(
                    centerX - ((float)activitySpinner.Frame.Width / 2),
                    centerY - (float)activitySpinner.Frame.Height - 20,
                    (float)activitySpinner.Frame.Width,
                    (float)activitySpinner.Frame.Height);

                activitySpinner.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;

                AddSubview(activitySpinner);

                activitySpinner.StartAnimating();

                var loadingLabel = new UILabel(new RectangleF(
                    centerX - (labelWidth / 2),
                    centerY + 20,
                    labelWidth,
                    labelHeight
                    ));

                loadingLabel.BackgroundColor = UIColor.Clear;
                loadingLabel.TextColor = UIColor.White;
                loadingLabel.Text = message;
                loadingLabel.TextAlignment = UITextAlignment.Center;
                loadingLabel.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
                loadingLabel.Font = UIFont.FromName("RO-Regular", loadingLabel.Font.PointSize);
                AddSubview(loadingLabel);
            }
            else
            {
                var imageView = new UIImageView(UIImage.FromBundle(imageSource));

                imageView.Frame = new RectangleF(
                    centerX - ((float)imageView.Frame.Width / 2),
                    centerY - (float)imageView.Frame.Height / 2,
                    (float)imageView.Frame.Width,
                    (float)imageView.Frame.Height);

                imageView.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;

                AddSubview(imageView);
            }
        }

        public void Hide(bool delay)
        {
            Animate(
                delay ? 0.5 : 0, // duration
                () => { Alpha = 0; },
                RemoveFromSuperview
            );
        }
    }
}
