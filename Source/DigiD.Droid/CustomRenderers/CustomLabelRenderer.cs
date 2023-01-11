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
using System.Linq;
using System.Reflection;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views.Accessibility;
using Android.Widget;
using DigiD.Common.Controls;
using DigiD.Common.Effects;
using DigiD.Droid.CustomRenderers;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer (typeof(CustomFontLabel), typeof(CustomLabelRenderer))]
namespace DigiD.Droid.CustomRenderers
{
    internal class CustomLabelRenderer : LabelRenderer
    {
        public CustomLabelRenderer(Context context) : base(context)
        {
        }

        public override void OnInitializeAccessibilityNodeInfo(AccessibilityNodeInfo info)
        {
            base.OnInitializeAccessibilityNodeInfo(info);

            if (Build.VERSION.SdkInt > BuildVersionCodes.M && !info.ImportantForAccessibility && AutomationProperties.GetIsInAccessibleTree(Element).GetValueOrDefault(true))
                info.ImportantForAccessibility = true;

            //https://developer.android.com/reference/android/view/accessibility/AccessibilityNodeInfo#isHeading()
            if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                var effect = Element.Effects.OfType<A11YEffect>().FirstOrDefault();
                if (effect != null)
                {
                    var controlType = Common.Effects.A11YEffect.GetControlType(Element);
                    if (controlType == A11YControlTypes.Header)
                        info.Heading = true;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Label.FormattedTextProperty.PropertyName)
            {
                UpdateFormattedText();
                CheckAutomationName();
            }
            else if (e.PropertyName == Label.TextProperty.PropertyName)
            {
                CheckAutomationName();
            }
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                return;

            CheckForDemoBarLabels();

            Control.PaintFlags = PaintFlags.SubpixelText | PaintFlags.AntiAlias;
            Control.SetAllCaps(false);

            CheckAutomationName();
        }

        private void CheckForDemoBarLabels()
        {
            var demoBarPageLabelStyle = (Style)Application.Current.Resources["DemoBarPageLabel"];
            var demoBarLabelStyle = (Style)Application.Current.Resources["DemoBarLabel"];

            Setter fontSizeSetter = null;

            if (Element.Style == demoBarLabelStyle)
                fontSizeSetter = demoBarLabelStyle.Setters.First(s => s.Property.PropertyName.Equals(nameof(Label.FontSize)));
            else if (Element.Style == demoBarPageLabelStyle)
                fontSizeSetter = demoBarPageLabelStyle.Setters.First(s => s.Property.PropertyName.Equals(nameof(Label.FontSize)));

            if (fontSizeSetter != null)
            {
                Control.SetTextSize(ComplexUnitType.Dip, (float)(double)fontSizeSetter.Value);
            }
        }

        private void CheckAutomationName()
        {
            var name = AutomationProperties.GetName(Element);
            if (!string.IsNullOrEmpty(name))
            {
                AutomationProperties.SetName(Element, "");
                Control.ContentDescription = name;
            }
        }

        private void UpdateFormattedText()
        {
            if (Element.FormattedText != null)
            {
                var extensionType = typeof(FormattedStringExtensions);
                var type = extensionType.GetNestedType("FontSpan", BindingFlags.NonPublic);
                var ss = new SpannableString(Control.TextFormatted);
                var spans = ss.GetSpans(0, ss.ToString().Length, Class.FromType(type));

                foreach (var span in spans)
                {
                    var start = ss.GetSpanStart(span);
                    var end = ss.GetSpanEnd(span);
                    var flags = ss.GetSpanFlags(span);
                    var font = (Font)type.GetProperty("Font").GetValue(span, null);
                    ss.RemoveSpan(span);
                    var newSpan = new CustomTypefaceSpan(Control, font);
                    ss.SetSpan(newSpan, start, end, flags);
                }

                Control.TextFormatted = ss;
            }
        }
    }

    public class CustomTypefaceSpan : MetricAffectingSpan
    {
        private readonly Typeface _typeFace;
        private readonly Typeface _typeFaceBold;
        private readonly Typeface _typeFaceItalic;
        private readonly TextView _textView;
        private readonly Font _font;

        public CustomTypefaceSpan(TextView textView, Font font)
        {
            _textView = textView;
            _font = font;
            // Note: we are ignoring _font.FontFamily (but thats easy to change)
            _typeFace = Typeface.Create("RO-Regular", TypefaceStyle.Normal);
            _typeFaceBold = Typeface.Create("RO-Bold", TypefaceStyle.Bold);
            _typeFaceItalic = Typeface.Create("RO-Italic", TypefaceStyle.Italic);
        }

        public override void UpdateDrawState(TextPaint tp)
        {
            ApplyCustomTypeFace(tp);
        }

        public override void UpdateMeasureState(TextPaint textPaint)
        {
            ApplyCustomTypeFace(textPaint);
        }

        private void ApplyCustomTypeFace(Paint paint)
        {
            var tf = _typeFace;

            if (_font.FontAttributes.HasFlag(FontAttributes.Bold))
            {
                tf = _typeFaceBold;
            }
            else if (_font.FontAttributes.HasFlag(FontAttributes.Italic))
            {
                tf = _typeFaceItalic;
            }

            paint.SetTypeface(tf);
            paint.TextSize = TypedValue.ApplyDimension(ComplexUnitType.Sp, _font.ToScaledPixel(), _textView.Resources.DisplayMetrics);
        }
    }
}
