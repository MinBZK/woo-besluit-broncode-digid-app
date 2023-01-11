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
using System.Linq;
using System.Threading.Tasks;
using DigiD.Common.Constants;
using DigiD.Common.Controls;
using DigiD.Common.Effects;
using DigiD.Controls;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using A11YEffect = DigiD.iOS.Effects.A11YEffect;
using CoreEffect = DigiD.Common.Effects.A11YEffect;

[assembly: ResolutionGroupName(AppConfigConstants.A11YEffectGroupName)]
[assembly: ExportEffect(typeof(A11YEffect), nameof(A11YEffect))]
namespace DigiD.iOS.Effects
{
    public class A11YEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (!(Element.Effects?.FirstOrDefault(e => e is Common.Effects.A11YEffect) is Common.Effects.A11YEffect))
                    return;

                if (Control != null)
                {
                    SetEffectOnControl(Control);
                }
                else if (Container != null && Container is UIView container)
                {
                    SetEffectOnControl(container);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"====> Fout in A11Effect.OnAttached: {typeof(Element)} - {e.Message}");
            }
        }

        private void SetEffectOnControl(UIView control)
        {
            var text = AutomationProperties.GetName(Element);
            var hint = AutomationProperties.GetHelpText(Element);
            var inA11YTree = AutomationProperties.GetIsInAccessibleTree(Element).GetValueOrDefault(true);

            if (inA11YTree)
            {
                control.IsAccessibilityElement = inA11YTree;
                control.AccessibilityLabel = text;
                control.AccessibilityHint = hint;
                var newControlType = CoreEffect.GetControlType(Element);

                if (Container != null)
                {
                    // het Element is een Container
                    control.ShouldGroupAccessibilityChildren = true;
                }
                if (newControlType == A11YControlTypes.None)
                {
                    control.AccessibilityTraits = UIAccessibilityTrait.None;
                    return;
                }
                if (newControlType == A11YControlTypes.Button || newControlType == A11YControlTypes.Toggle)
                {
                    if (Element is Switch @switch)
                    {
                        var labelText = "";
                        if (AutomationProperties.GetLabeledBy(@switch) is Label label)
                            labelText = label.Text;
                        control.AccessibilityLabel = labelText;
                    }
                    else
                    {
                        control.AccessibilityTraits = UIAccessibilityTrait.None;
                        control.AccessibilityTraits = UIAccessibilityTrait.Button;
                    }
                }
                else if (newControlType is A11YControlTypes.Link or A11YControlTypes.LinkLabel)
                {
                    control.AccessibilityTraits = UIAccessibilityTrait.Link;
                    if (Element is Button)
                    {
                        if (control is UIButton btn)
                        {
                            var txt = new NSMutableAttributedString(btn.CurrentTitle);
                            txt.AddAttribute(UIStringAttributeKey.UnderlineStyle, NSObject.FromObject(NSUnderlineStyle.Single), new Foundation.NSRange(0, btn.CurrentTitle.Length));
                            btn.SetAttributedTitle(txt, UIControlState.Normal);
                            btn.LineBreakMode = UILineBreakMode.WordWrap;
                            btn.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                        }
                    }
                    else if (Element is Label labelWithHyperlinkSpan && !(Element is CustomHtmlLabel))
                    {
                        if (labelWithHyperlinkSpan.FormattedText != null)
                        {
                            CheckForGestureRecognizers(control, labelWithHyperlinkSpan);
                        }
                        else
                        {
                            // Als de tekst geen hyperlinks bevatten dan dat ook niet zo laten uitspreken
                            if (!string.IsNullOrEmpty(text) && (text.Contains("<a href") || text.Contains("&lt;a href")))
                                control.AccessibilityTraits ^= UIAccessibilityTrait.Link;
                        }
                    }
                    else
                    {
                        // Overig Element met link-effect, controleer tekst nu op leeg-zijn of op link:
                        if (Element is CustomHtmlLabel htmlLabel)
                        {
                            if (string.IsNullOrEmpty(htmlLabel.Text))
                            {
                                control.AccessibilityTraits ^= UIAccessibilityTrait.Link;
                                AutomationProperties.SetIsInAccessibleTree(Element, false);
                            }
                            if (htmlLabel.Text != null && !(htmlLabel.Text.Contains("<a href") || htmlLabel.Text.Contains("&lt;a href")))
                                control.AccessibilityTraits ^= UIAccessibilityTrait.Link;
                        }
                    }
                }

                if (newControlType == A11YControlTypes.Header)
                    control.AccessibilityTraits = UIAccessibilityTrait.Header;

                if (newControlType == A11YControlTypes.LiveUpdate && (Element is Label || Element is CustomErrorLabel))
                {
                    Element.PropertyChanged += Element_PropertyChanged;
                }
            }
        }

        private void CheckForGestureRecognizers(UIView control, Label labelWithHyperlinkSpan)
        {
            var spans = labelWithHyperlinkSpan.FormattedText?.Spans;
            if (spans == null)
                return;
            foreach (var span in spans)
                foreach (var gr in span.GestureRecognizers)
                    labelWithHyperlinkSpan.GestureRecognizers.Add(gr);
        }

        private void Element_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var text = "";

            if (sender is Label label)
            {
                if (e.PropertyName.Equals(Label.TextProperty.PropertyName) ||
                    e.PropertyName.Equals(AutomationProperties.NameProperty.PropertyName) ||
                    (e.PropertyName.Equals(Label.IsVisibleProperty.PropertyName) && label.IsVisible))
                {
                    var a11yTxt = AutomationProperties.GetName(label);
                    text = string.IsNullOrEmpty(a11yTxt) ? label.Text : a11yTxt;
                }
            }
            else if (sender is CustomErrorLabel errorLabel)
            {
                if (e.PropertyName.Equals(nameof(errorLabel.ErrorText)) || e.PropertyName.Equals(nameof(errorLabel.IsVisible)))
                {
                    text = errorLabel.ErrorText;
                }
            }
            if (!string.IsNullOrEmpty(text))
            {
                Task.Run(async () =>
                {
                    System.Diagnostics.Debug.WriteLine($"\nA11YEffect.Element_PropertyChanged: '{text}'");
                    await Task.Delay(2000);
                    UIAccessibility.PostNotification(UIAccessibilityPostNotification.Announcement, NSObject.FromObject(text));
                });
            }
        }

        protected override void OnDetached()
        {
            if (Element is Label label && CoreEffect.GetControlType(Element) == A11YControlTypes.LiveUpdate)
            {
                Element.PropertyChanged -= Element_PropertyChanged;
            }
        }
    }
}
