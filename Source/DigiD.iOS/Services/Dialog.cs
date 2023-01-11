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
using CoreGraphics;
using DigiD.Common;
using DigiD.Common.Helpers;
using DigiD.Common.Mobile.Controls;
using DigiD.Common.Services;
using DigiD.iOS.Services;
using DigiD.UI;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency (typeof(Dialog))]
namespace DigiD.iOS.Services
{
	internal class Dialog : IDialog
	{
	    private UIView _loadingOverlay;
        
		public void ShowProgressDialog ()
		{
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (_loadingOverlay != null)
                    return;
                try
                {
                    var rootViewController = UIApplication.SharedApplication.Delegate.GetWindow().RootViewController;
                    var childViewController = rootViewController?.ChildViewControllers[0];

                    var bounds = childViewController?.View?.Bounds;

                    if (bounds == null)
                        return;

                    var _overlay = new LoadingOverlay();
                    AutomationProperties.SetIsInAccessibleTree(_overlay, false);

                    if (DependencyService.Get<IA11YService>().IsInVoiceOverMode() && (_overlay.FindByName("_overlayAnimation") is CustomAnimationView animationView) && !string.IsNullOrEmpty(animationView.AlternateText))
                    {
                        await DependencyService.Get<IA11YService>().Speak(animationView.AlternateText);
                    }

                    _loadingOverlay = ConvertFormsToNative(_overlay, bounds.Value);

                    childViewController.View.AccessibilityTraits = UIAccessibilityTrait.None;
                    childViewController.View.Add(_loadingOverlay);
                    await DependencyService.Get<IA11YService>().Speak(AppResources.AccessibilitySpinnerText);
                }
                catch (Exception e)
                {
                    AppCenterHelper.TrackError(e);
                }
            });
		}

        public void HideProgressDialog()
		{
            Device.BeginInvokeOnMainThread(() =>
            {
                if (_loadingOverlay == null || _loadingOverlay.Hidden) return;

                _loadingOverlay.RemoveFromSuperview();
                _loadingOverlay = null;
            });
		}

	    private static UIView ConvertFormsToNative(VisualElement view, CGRect size)
	    {
	        var renderer = Platform.CreateRenderer(view);

	        renderer.NativeView.Frame = size;

	        renderer.NativeView.AutoresizingMask = UIViewAutoresizing.All;
	        renderer.NativeView.ContentMode = UIViewContentMode.ScaleToFill;

	        renderer.Element.Layout(size.ToRectangle());

	        var nativeView = renderer.NativeView;

	        nativeView.SetNeedsLayout();

	        return nativeView;
	    }
	}
}
