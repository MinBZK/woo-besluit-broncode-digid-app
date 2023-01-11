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
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using DigiD.Common;
using DigiD.Common.Controls;
using DigiD.Common.Helpers;
using DigiD.Common.Services;
using DigiD.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace DigiD.Droid.CustomRenderers
{
    public class CustomEntryRenderer : EntryRenderer, Android.Views.View.IOnTouchListener
    {
        private Drawable _showPasswordImage;
        private Drawable _showPasswordImageDarkmode;
        private Drawable _hidePasswordImage;
        private Typeface _typeface;
        private bool _eyeAdded;
        private bool _tellingPassword;
        private bool _isShowingPassword;

        ImageButton _imageButton;

        public CustomEntryRenderer(Context context) : base(context)
        {
        }
        
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (!(Element is Entry customEntry))
                return;

            if (Element.Parent is Frame frame)
            {
                AutomationProperties.SetIsInAccessibleTree(frame, false);
            }

            if (Element.IsPassword)
            {
                Control.SetRawInputType(InputTypes.ClassText | InputTypes.TextVariationPassword);
                AddEyeImage();
            }
            else
            {
                Control.InputType = InputTypes.ClassText | InputTypes.TextFlagNoSuggestions | InputTypes.TextVariationNormal;
            }

            if (customEntry.Keyboard == Keyboard.Numeric)
            {
                Control.SetRawInputType(InputTypes.ClassNumber);
            }

            Control.Background = null;

            // geen fullscreen edit in landscape-mode, en alle overige poespas vh keyboard uitzetten
            Control.ImeOptions = (Android.Views.InputMethods.ImeAction)(Android.Views.InputMethods.ImeFlags.NoFullscreen
                | Android.Views.InputMethods.ImeFlags.NoAccessoryAction
                | Android.Views.InputMethods.ImeFlags.NoPersonalizedLearning);

            Control.PaintFlags = Control.PaintFlags | PaintFlags.SubpixelText;
            Control.Gravity = GravityFlags.CenterVertical;
            Control.TextSize = (float)Element.FontSize;
            SetHintFont();

            if (EditText == null || !(EditText.Parent is ViewGroup vg))
                return;

            if (!(Element.Parent?.Parent is CustomEntryField))
                EditText.FocusChange += EditText_FocusChange; // KTR zie commentaar in functie
            vg.ImportantForAccessibility = ImportantForAccessibility.No;
            vg.Focusable = false;
            vg.ContentDescription = null;
        }

        private async void EditText_FocusChange(object sender, FocusChangeEventArgs e)
        {
            // KTR LET OP door toevoeging van deze code krijgt de Entry focus, op Android
            // gaat dat anders niet altijd goed, bijvoorbeeld bij de VerificationCodeEntryView
            await Task.Delay(100);
        }

        private void SetHintFont()
        {
            if (!string.IsNullOrEmpty(Element.Placeholder))
            {
                var namedSize = (int)Device.GetNamedSize(NamedSize.Large, typeof(Entry));
                var fontSize = (int)Math.Round(namedSize - (namedSize * .1));
                var newPlaceholder = new SpannableString(Element.Placeholder);
                newPlaceholder.SetSpan(new AbsoluteSizeSpan(fontSize, true), 0, newPlaceholder.Length(), SpanTypes.InclusiveExclusive);
                if (Element.FontFamily != null)
                {
                    var hintTypeFace = Typeface.Create(Element.FontFamily, TypefaceStyle.Normal);
                    if (hintTypeFace != null)
                        newPlaceholder.SetSpan(hintTypeFace, 0, newPlaceholder.Length(), SpanTypes.InclusiveExclusive);
                }
                Control.HintFormatted = newPlaceholder;
            }
        }

        private void AddEyeImage()
        {
            if (_eyeAdded)
                return;

            _eyeAdded = true;

            if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
            {
                _imageButton = new ImageButton();
                AutomationProperties.SetName(_imageButton, AppResources.AccessibilityShowPassword);
                _imageButton.BackgroundColor = Xamarin.Forms.Color.Transparent;
                _imageButton.IsVisible = true;
                _imageButton.HeightRequest = 48;
                _imageButton.WidthRequest = 48;
                _imageButton.Padding = 12;
                _imageButton.Aspect = Aspect.AspectFit;
                _imageButton.HorizontalOptions = LayoutOptions.EndAndExpand;
                _imageButton.Source = new FileImageSource { File = "icon_wachtwoord_default" + (ThemeHelper.IsInDarkMode ? "_dark.png" : ".png") };
                _imageButton.Command = new Command(() => ToggleShowPassword());
                
                if (Element.Parent is BorderedFrame bf && bf.Parent is CustomEntryField cef)
                {
                    var row = Grid.GetRow(cef.Frame);
                    Grid.SetRow(_imageButton, row);
                    Grid.SetColumnSpan(_imageButton, 2);
                    Grid.SetColumn(_imageButton, 0);
                    cef.Children.Add(_imageButton);


                    cef.LowerChild(cef.Entry);
                    cef.RaiseChild(_imageButton);
                }
            }
            else
            {
                _showPasswordImage = Context?.GetDrawable(DigiD.Droid.Resource.Drawable.icon_wachtwoord_default);
                _showPasswordImageDarkmode = Context?.GetDrawable(DigiD.Droid.Resource.Drawable.icon_wachtwoord_default_dark);
                _hidePasswordImage = Context?.GetDrawable(DigiD.Droid.Resource.Drawable.icon_wachtwoord_on);
                Control.SetOnTouchListener(this);
            }

            if (!DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                SetFontAndEye(ThemeHelper.IsInDarkMode ? _showPasswordImageDarkmode : _showPasswordImage);
        }

        private void SetFontAndEye(Drawable drawable)
        {
            drawable.SetBounds(0, 0, Control.LineHeight, Control.LineHeight);
            Control.SetCompoundDrawables(null, null, drawable, null);
            try
            {
                if (_typeface == null)
                    _typeface = Typeface.Create(Element.FontFamily, TypefaceStyle.Normal);

                Control.SetTypeface(_typeface, TypefaceStyle.Normal);
            } 
            catch (Exception)
            {
                //Do nothing
            }

            Control.Invalidate();
        }

        private bool HandleVoiceOver(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down when _tellingPassword:
                case MotionEventActions.Move when _tellingPassword:
                    return true;
                case MotionEventActions.Move when Element.IsPassword && e.DownTime > 3000:
                    ToggleShowPassword();
                    _tellingPassword = true;
                    return true;
                case MotionEventActions.Up when _tellingPassword:
                    ToggleShowPassword();
                    _tellingPassword = false;
                    return true;
            }

            return false;
        }

        private bool HandleDownEvent(Drawable drawable, Android.Views.View v, MotionEvent e)
        {
            var eventX = (int)e.GetX();
            var bounds = drawable.Bounds;

            if (eventX >= (v.Width - bounds.Width()) && eventX <= v.Width)
            {
                e.Action = MotionEventActions.Cancel;   // Prevent the keyboard from coming up

                _isShowingPassword = true;
                ToggleShowPassword();
                return true;
            }

            return false;
        }

        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                return HandleVoiceOver(e);

            var dmImage = ThemeHelper.IsInDarkMode ? _showPasswordImageDarkmode : _showPasswordImage;
            var drawable = Element.IsPassword ? dmImage : _hidePasswordImage;
            if (!_isShowingPassword && e.Action == MotionEventActions.Down && drawable != null)
                return HandleDownEvent(drawable, v, e);

            if (!_isShowingPassword || e.Action != MotionEventActions.Up) 
                return false;

            _isShowingPassword = false;
            ToggleShowPassword();
            
            return true;
        }

#pragma warning disable S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 24 to the 15 allowed.
        private void ToggleShowPassword()
#pragma warning restore S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 24 to the 15 allowed.
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
            {
                if (Element == null)
                    return;

                var cursorPosition = Element.CursorPosition;

                if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                {
                    Element.IsPassword = !Element.IsPassword;
                    AutomationProperties.SetName(_imageButton, Element.IsPassword ? AppResources.AccessibilityShowPassword : AppResources.AccessibilityHidePassword);
                    var filename = "";

                    if (!Element.IsPassword)
                        filename = "icon_wachtwoord_on.png";
                    else
                    {
                        if (ThemeHelper.IsInDarkMode)
                            filename = "icon_wachtwoord_default_dark.png";
                        else
                            filename = "icon_wachtwoord_default.png";
                    }
                        
                    _imageButton.Source = new FileImageSource { File = filename };

                    if (!Element.IsPassword)
                    {
                        await DependencyService.Get<IA11YService>().Speak($"{Element.Text}, {AutomationProperties.GetName(_imageButton)}", 1000);
                    }

                }
                else
                {
                    Element.IsPassword = !_isShowingPassword;
                    var dmImage = ThemeHelper.IsInDarkMode ? _showPasswordImageDarkmode : _showPasswordImage;
                    SetFontAndEye(Element.IsPassword ? dmImage : _hidePasswordImage);
                }

                Element.CursorPosition = cursorPosition;
            });
        }
    }
}
