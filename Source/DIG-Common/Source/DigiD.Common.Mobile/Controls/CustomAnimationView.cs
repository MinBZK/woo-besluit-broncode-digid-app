// De openbaarmaking van dit bestand is in het kader van de WOO geschied en 
// dus gericht op transparantie en niet op hergebruik. In het geval dat dit 
// bestand hergebruikt wordt, is de EUPL licentie van toepassing, met 
// uitzondering van broncode waarvoor een andere licentie is aangegeven.
//
// Het archief waar dit bestand deel van uitmaakt is te vinden op:
//   https://github.com/MinBZK/woo-verzoek-broncode-digid-app
//
// Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
//
// Voor overige vragen over dit WOO-verzoek kunt u mailen met:
//   mailto://open@logius.nl
//
﻿using System;
using System.Runtime.CompilerServices;
using DigiD.Common.Services;
using Lottie.Forms;
using Xamarin.Forms;

namespace DigiD.Common.Mobile.Controls
{
    public class CustomAnimationView : AnimationView
    {
        public static readonly BindableProperty AlternateTextProperty = BindableProperty.Create(nameof(AlternateText), typeof(string)
            , typeof(CustomAnimationView), string.Empty, BindingMode.TwoWay, propertyChanged: AlternateTextPropertyChanged);

        public static readonly BindableProperty AnimationResumeTextProperty = BindableProperty.Create(nameof(AnimationResumeText), typeof(string)
            , typeof(CustomAnimationView), AppResources.AnimationResumeText, BindingMode.TwoWay);

        private static void AlternateTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var newString = newValue as string;

            if (bindable is CustomAnimationView view && newValue != null)
            {
                AutomationProperties.SetIsInAccessibleTree(view, !string.IsNullOrEmpty(newString));
                if (newString != null)
                {
                    if (newString.EndsWith(AppResources.AnimationPauseText))
                        AutomationProperties.SetName(view, newString);
                    else
                    {
                        if (view.IsPauseEnabled)
                            AutomationProperties.SetName(view, string.Concat(newString, ", ", AppResources.AnimationPauseText));
                        else
                            AutomationProperties.SetName(view, newString);
                    }
                }
            }
        }

        public string AlternateText 
        {
            get {
                var text = (string)GetValue(AlternateTextProperty);
                if (!string.IsNullOrEmpty(text) && IsPauseEnabled)
                    return $"{text}, {AppResources.AnimationPauseText}";
                else
                    return text;
            }
            set => SetValue(AlternateTextProperty, value); 
        }

        public string AnimationResumeText
        {
            get => (string)GetValue(AnimationResumeTextProperty);
            set => SetValue(AnimationResumeTextProperty, value);
        }

        public bool IsPauseEnabled { get; set; } = true;

        bool _isPaused = false;
        public CustomAnimationView()
        {
            
            AutomationProperties.SetName(this, AlternateText);
            Clicked += TogglePlay;
        }

        private void TogglePlay(object sender, EventArgs e)
        {
            if (_isPaused)
                Play();
            else
                Pause();
        }

        public void Play()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (_isPaused)
                        ResumeAnimation();
                    else
                        PlayAnimation();

                    _isPaused = false;
                    AutomationProperties.SetName(this, AlternateText);
                    SpeakIfNecessary();
                }
                catch (Exception)
                {
                    // niets mee doen, object is waarschijnlijk disposed.
                }
            });
        }
        public void Pause()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    PauseAnimation();
                    _isPaused = true;
                    AutomationProperties.SetName(this, AnimationResumeText);
                    SpeakIfNecessary();
                }
                catch (Exception)
                {
                    // niets mee doen, object is waarschijnlijk disposed.
                }
            });
        }


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName != null && propertyName == nameof(Animation) && Animation != null)
                return;

            switch (propertyName)
            {
                case nameof(IsFocused):
                {
                    if (IsFocused)
                        SpeakIfNecessary();
                    break;
                }
                case nameof(IsAnimating):
                {
                    if (IsAnimating)
                        SpeakIfNecessary();
                    break;
                }
                case nameof(IsVisible):
                {
                    if (IsVisible)
                    {
                        if (!_isPaused)
                            Play();
                        else
                            SpeakIfNecessary();
                    }

                    break;
                }
            }
        }

        private void SpeakIfNecessary()
        {
            if (!DependencyService.Get<IA11YService>().IsInVoiceOverMode() || !IsVisible)
                return;

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!_isPaused)
                    await DependencyService.Get<IA11YService>().Speak(AlternateText);
                else 
                    await DependencyService.Get<IA11YService>().Speak(AnimationResumeText);
            });
        }
    }
}
