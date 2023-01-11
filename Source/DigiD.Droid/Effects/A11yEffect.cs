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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using DigiD.Common.Constants;
using DigiD.Common.Controls;
using DigiD.Common.Effects;
using DigiD.Common.Helpers;
using DigiD.Controls;
using DigiD.Droid.CustomRenderers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using A11YEffect = DigiD.Droid.Effects.A11YEffect;

[assembly: ResolutionGroupName(AppConfigConstants.A11YEffectGroupName)]
[assembly: ExportEffect(typeof(A11YEffect), nameof(A11YEffect))]
namespace DigiD.Droid.Effects
{
    public class A11YEffect : PlatformEffect
    {
        private bool _clickAttached;
        private const string HYPERLINK_EXPRESSION = @"(?<anchor>a +href *= *)[\\]*[""|'](?<url>https://\S+)[\\]*[""|']";
        private static readonly Regex regex = new Regex(HYPERLINK_EXPRESSION, RegexOptions.Compiled | RegexOptions.Multiline);

        protected override void OnAttached()
        {
            try
            {
                _clickAttached = false;

                if (!(Element?.Effects?.FirstOrDefault(e => e is Common.Effects.A11YEffect) is Common.Effects.A11YEffect effect))
                    return;

                if (Control != null)
                {
                    SetEffectOnControl(Control);
                }
                else if (Container != null && Container is Android.Views.View container)
                {
                    SetEffectOnControl(container);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"====> Fout in A11Effect.OnAttached: {typeof(Element)} - {e.Message}");
            }
        }

        private void SetEffectOnControl(Android.Views.View control)
        {
            var text = AutomationProperties.GetName(Element);
            if (!string.IsNullOrEmpty(text))
                control.ContentDescription = text;

            var controlType = Common.Effects.A11YEffect.GetControlType(Element);

            if (controlType == A11YControlTypes.None)
            {
                control.Clickable = false;
                return;
            }
            if (controlType == A11YControlTypes.Button || controlType == A11YControlTypes.Toggle || controlType == A11YControlTypes.Link || controlType == A11YControlTypes.LinkLabel)
            {
                if (Element is CustomHtmlLabel htmlLabel && (string.IsNullOrEmpty(htmlLabel.Text) || (htmlLabel.Text != null && !(htmlLabel.Text.Contains("<a href") || htmlLabel.Text.Contains("&lt;a href")))))
                {
                    control.Clickable = false;
                    control.LongClickable = false;
                    if (string.IsNullOrEmpty(htmlLabel.Text))
                        AutomationProperties.SetIsInAccessibleTree(htmlLabel, false);

                    return;
                }
                if (Element is Switch @switch)
                {
                    var labelText = control.ContentDescription ?? "";
                    if (AutomationProperties.GetLabeledBy(@switch) is Label label)
                        labelText += label.Text;
                    control.ContentDescription = labelText;
                }

                if (Element is Button && controlType == A11YControlTypes.Link)
                {
                    if (control is Android.Widget.Button btn)
                    {
                        btn.PaintFlags |= Android.Graphics.PaintFlags.UnderlineText;
                        btn.TextAlignment = Android.Views.TextAlignment.ViewStart;
                    }
                }
                else if (!(Element.Parent is ViewCell viewCell))
                {
                    control.Clickable = true;
                    control.ImportantForAccessibility = ImportantForAccessibility.Yes;
                    control.Focusable = true;
                    CheckForGestureRecognizers(control, controlType);
                }
            }
            else if (controlType == A11YControlTypes.Header)
            {
                ViewCompat.SetAccessibilityHeading(control, true);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                    control.AccessibilityHeading = true;

            }
            else if (controlType == A11YControlTypes.LiveUpdate)
            {
                if (Element is Label)
                {
                    control.AccessibilityLiveRegion = AccessibilityLiveRegion.Polite;
                }
                else if (Element is CustomErrorLabel && control is Android.Views.ViewGroup vg)
                {
                    var found = false;
                    for (int x = 0; x < vg.ChildCount && !found; x++)
                    {
                        if (vg.GetChildAt(x) is CustomLabelRenderer child && child.GetChildAt(0) is Android.Widget.TextView tv)
                        {
                            found = true;
                            tv.AccessibilityLiveRegion = AccessibilityLiveRegion.Polite;
                        }
                    }
                }
            }
        }

        protected override void OnDetached()
        {
            if (_clickAttached && Control != null)
                Control.Click -= Control_Click;
            else if (_clickAttached && Container != null)
                Container.Click -= Control_Click;

            _clickAttached = false;
        }

        private void CheckForGestureRecognizers(Android.Views.View control, A11YControlTypes type)
        {
            var controlType = Common.Effects.A11YEffect.GetControlType(Element);
            if (Element is Label label && label.Text != null && type == A11YControlTypes.Link) 
            {
                var match = regex.Match(label.Text);
                if (match.Success)
                    AddGestureForHyperlink(label); // Tekst bevat een link
                else
                    return;
            }

            control.Click += Control_Click;
            _clickAttached = true;
        }

        private void AddGestureForHyperlink(Label label)
        {
            label.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new AsyncCommand(async () =>
                {
                    var match = regex.Match(label.Text);
                    if (match.Success)
                    {
                        var url = new Uri(match.Groups["url"].Value);
                        await Xamarin.Essentials.Launcher.OpenAsync(url);
                    }
                }),
                NumberOfTapsRequired = 1

            });
            var txt = AutomationProperties.GetName(label) ?? label.Text;
            AutomationProperties.SetName(label, txt.StripHtml());
        }

#pragma warning disable S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 17 to the 15 allowed.
        private IList<IGestureRecognizer> CheckViewOrChildsForGestureRecognizers(Android.Views.View control)
#pragma warning restore S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 17 to the 15 allowed.
        {
            IList<IGestureRecognizer> recognizers = new List<IGestureRecognizer>();
            switch (Element)
            {
                case Label label:
                {
                    var children = label.GetChildElements(new Point(control.GetX(), control.GetY()));
                    if (children != null && children.Any())
                    {
                        foreach (var gr in children[0].GestureRecognizers)
                            label.GestureRecognizers.Add(gr);
                    }

                    break;
                }
                case CustomRadioButton radioButton:
                {
                    foreach (var gr in radioButton.GestureRecognizers)
                    {
                        if (gr is TapGestureRecognizer tap && radioButton.Command != null)
                        {
                            tap.Command = radioButton.Command;
                            tap.CommandParameter = radioButton.CommandParameter;
                            recognizers.Add(tap);
                        }
                    }

                    break;
                }
            }

            if (Element is Xamarin.Forms.View view)
            {
                foreach(var gr in view.GestureRecognizers)
                    recognizers.Add(gr);
            }
            return recognizers;
        }

        private void Control_Click(object sender, EventArgs e)
        {
            ExecuteCommandOnGestures();
        }

        private void ExecuteCommandOnGestures()
        {
            var recognizers = CheckViewOrChildsForGestureRecognizers(Control);

            foreach (var recognizer in recognizers)
            {
                if (recognizer is TapGestureRecognizer tapGesture)
                {
                    if (tapGesture.Command?.CanExecute(tapGesture.CommandParameter) == true)
                        tapGesture.Command.Execute(tapGesture.CommandParameter);

                    break;
                }
            }
        }
    }
}
