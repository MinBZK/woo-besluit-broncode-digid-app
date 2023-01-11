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
using System.ComponentModel;
using CoreGraphics;
using DigiD.Common;
using DigiD.Common.Helpers;
using DigiD.Common.Services;
using DigiD.Common.Settings;
using DigiD.iOS.CustomRenderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace DigiD.iOS.CustomRenderers
{
    public class CustomEntryRenderer : EntryRenderer, IUITextFieldDelegate
    {
        private UITextField _textField;
        private int _cursorPosition;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                _textField?.Dispose();
                return;
            }

            if (Control != null)
            {
                Control.AdjustsFontForContentSizeCategory = true;
                Control.AccessibilityLanguage = DependencyService.Get<IGeneralPreferences>().Language;

                if (e.NewElement.Keyboard == Keyboard.Numeric)
                {
                    if (UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                        Control.TextContentType = UITextContentType.OneTimeCode;
                }
                else if (Control.AutocapitalizationType != UITextAutocapitalizationType.AllCharacters)
                {
                    Control.AutocapitalizationType = UITextAutocapitalizationType.None;
                    Control.AutocorrectionType = UITextAutocorrectionType.No;
                }
            }
            
            SetPlaceholderText();

            if (e.NewElement.IsPassword)
            {
                AddEyeImage();
            }

            SetNoBorder();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control != null)
                SetNoBorder();

            if (sender is UITextField && e.PropertyName.Equals(Entry.IsPasswordProperty.PropertyName))
                AutomationProperties.SetIsInAccessibleTree(Element, Element.IsPassword);

            if (e.PropertyName == Entry.PlaceholderProperty.PropertyName ||
                e.PropertyName == AutomationProperties.NameProperty.PropertyName)
                SetPlaceholderText();
        }

        private void SetPlaceholderText(string newPlaceholder = null)
        {
            var automationName = AutomationProperties.GetName(Element);

            if (newPlaceholder != null)
                Control.AccessibilityLabel = newPlaceholder;
            else if (!string.IsNullOrEmpty(automationName))
            {
                Control.AccessibilityLabel = automationName;
                Control.Placeholder = "";
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_showPasswordButton != null)
            {
                _showPasswordButton.TouchDown -= ShowPasswordText;
                _showPasswordButton.TouchDownRepeat -= ShowPasswordText;
                _showPasswordButton.TouchCancel -= HidePasswordText;
                _showPasswordButton.TouchUpInside -= HidePasswordText;
                _showPasswordButton.TouchUpOutside -= HidePasswordText;
                _showPasswordButton.Dispose();
            }

            _textField?.Dispose();
            
            base.Dispose(disposing);
        }

        private UIButton _showPasswordButton;

        private void AddEyeImage()
        {
            if (_textField != null || Control == null)
                return;

            _textField = Control;

            _textField.ShouldChangeCharacters = ChangeCharacters;

            _showPasswordButton = new UIButton(UIButtonType.Custom);
            _showPasswordButton.SetImage(
                ThemeHelper.IsInDarkMode
                    ? new UIImage("icon_wachtwoord_default_dark.png")
                    : new UIImage("icon_wachtwoord_default.png"), UIControlState.Normal);

            _showPasswordButton.Frame = new CGRect(new CGPoint(0, 0), _showPasswordButton.CurrentImage.Size);
            _showPasswordButton.TouchDown -= ShowPasswordText;
            _showPasswordButton.TouchDown += ShowPasswordText;
            _showPasswordButton.TouchDownRepeat -= ShowPasswordText;
            _showPasswordButton.TouchDownRepeat += ShowPasswordText;
            _showPasswordButton.TouchCancel -= HidePasswordText;
            _showPasswordButton.TouchCancel += HidePasswordText;
            _showPasswordButton.TouchUpInside -= HidePasswordText;
            _showPasswordButton.TouchUpInside += HidePasswordText;
            _showPasswordButton.TouchUpOutside -= HidePasswordText;
            _showPasswordButton.TouchUpOutside += HidePasswordText;
            _showPasswordButton.IsAccessibilityElement = true;
            _showPasswordButton.AccessibilityTraits = UIAccessibilityTrait.Button;
            _showPasswordButton.AccessibilityLabel = $"{AppResources.AccessibilityShowPassword}, {AppResources.AccessibilityTapAndHold}";

            _textField.AdjustsFontForContentSizeCategory = true;
            _textField.RightView = _showPasswordButton;
            _textField.RightViewMode = UITextFieldViewMode.Always;
            _textField.RightView.ExclusiveTouch = true;
        }

        private void HidePasswordText(object sender, EventArgs e)
        {
            if (!Element.IsPassword)
            {
                Element.IsPassword = true;
                Element.CursorPosition = _cursorPosition;
            }
        }

        private async void ShowPasswordText(object sender, EventArgs e)
        {
            if (Element.IsPassword)
            {
                _cursorPosition = Element.CursorPosition;
                Element.IsPassword = false;
                Control.ShouldBeginEditing = Editable;
            }
            var text = string.IsNullOrEmpty(Element.Text) ? AppResources.AccessibilityNotEnteredYet : Element.Text;
            await DependencyService.Get<IA11YService>().Speak(text);

        }

        private void SetNoBorder()
        {
            // Omdat BorderStyle op None zetten niet voldoende was (aan de rechterkant bleeft een lijn zichtbaar)
            // de border maar dezelfde kleur gegeven als de achtergrond zodat ie niet zichtbaar is.
            Control.Layer.MasksToBounds = true;
            Control.Layer.BorderColor = Element.BackgroundColor.ToUIColor().CGColor;
            Control.Layer.BorderWidth = 1;
            Control.BorderStyle = UITextBorderStyle.None;
        }

        private bool Editable(UITextField textField)
        {
            return Element.IsPassword;
        }

        private bool ChangeCharacters(UITextField textField, NSRange range, string replacementString)
        {
            // Moet op deze manier, ShouldClear werkt namelijk niet op Password fields.
            // zie https://stackoverflow.com/questions/7305538/uitextfield-with-secure-entry-always-getting-cleared-before-editing
            var text = new NSString(textField.Text);
            var updatedString = text.Replace(range, new NSString(replacementString));
            textField.Text = updatedString.ToString();
            _cursorPosition = textField.Text.Length;

            textField.SendActionForControlEvents(UIControlEvent.EditingChanged);
            return false;
        }

    }
}
