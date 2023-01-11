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
﻿using DigiD.Common.Constants;
using DigiD.Common.Controls;
using DigiD.Common.Mobile.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Application = Xamarin.Forms.Application;

namespace DigiD.Common.Mobile.ControlTemplates
{
    public static class TemplateSelector
    {
        public static ControlTemplate CurrentTemplate => new ControlTemplate(typeof(ActiveTemplate));
    }

    public class ActiveTemplate : Grid
    {
        private static View CreateDemoBar()
        {
            var panel = new StackLayout
            {
                Style = (Style)Application.Current.Resources["DemoBar"],
            };

            for (var x = 1; x <= 2; x++)
            {
                var demoLabel = new CustomFontLabel
                {
                    Style = (Style)Application.Current.Resources["DemoBarLabel"],
                    Text = $"{DemoHelper.CurrentUser.UserName}"
                };

                AutomationProperties.SetIsInAccessibleTree(demoLabel, x == 1);
                panel.Children.Add(demoLabel);
            }

            panel.SetBinding(IsVisibleProperty, new Binding("Parent.BindingContext.IsDemoMode", source: RelativeBindingSource.TemplatedParent));
            var gesture = new TapGestureRecognizer();
            gesture.SetBinding(TapGestureRecognizer.CommandProperty, new Binding("Parent.BindingContext.DemoBarPressed", source: RelativeBindingSource.TemplatedParent));
            panel.GestureRecognizers.Add(gesture);
            return panel;
        }

        private readonly View _demoBar;
        private readonly CustomFontLabel _label;

        public ActiveTemplate()
        {
            DeviceDisplay.MainDisplayInfoChanged -= DisplayInfoChanged;
            DeviceDisplay.MainDisplayInfoChanged += DisplayInfoChanged;
            SetDynamicResource(BackgroundColorProperty, "PageBackgroundColor");

            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            RowSpacing = 0;

            var contentPresenter = new ContentPresenter();
            AutomationProperties.SetIsInAccessibleTree(contentPresenter, false);
            Children.Add(contentPresenter, 0, 1);
            AutomationProperties.SetIsInAccessibleTree(this, false);

            if (DebugConstants.IsPresentingMode)
                return;

#if DEBUG||TEST||ACC||DEV
            _demoBar = null;
            _label = null;

            var hasDemoBar = false;

            if (Device.Idiom != TargetIdiom.Desktop && DemoHelper.CurrentUser != null)
            {
                hasDemoBar = true;
                _demoBar = CreateDemoBar();
                if (OrientationHelper.IsInLandscapeMode)
                    RowDefinitions[0].Height = new GridLength(0, GridUnitType.Absolute);
                Children.Add(_demoBar, 0, 0);
            }

            _label = new CustomFontLabel
            {
                Margin = new Thickness(0, Device.RuntimePlatform == Device.Android ? 0 : hasDemoBar ? 0 : 20, 10, 0),
                Style = (Style)Application.Current.Resources["DemoBarPageLabel"]
            };
            AutomationProperties.SetIsInAccessibleTree(_label, false);
            _label.SetBinding(Label.TextProperty, new Binding("Parent.BindingContext.PageId", source: RelativeBindingSource.TemplatedParent));

            Children.Add(_label, 0, hasDemoBar ? 0 : 1);
#endif
            
#if A11YTEST
            var btn = new BorderedButton
            {
                Text = "Next",
                Margin = 20
            };
            btn.Clicked += (sender, args) => MessagingCenter.Send(sender, "A11YTEST_NEXT");
            Children.Add(btn, 0, hasDemoBar ? 0 : 1);
#endif
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (_demoBar != null)
            {
                RaiseChild(_demoBar);
                RaiseChild(_label);
            }
        }
        private void DisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            if (OrientationHelper.IsInLandscapeMode && !RowDefinitions[0].Height.IsAbsolute)
            {
                if (_demoBar != null)
                    _demoBar.IsVisible = false;
                if (_label != null)
                    _label.IsVisible = false;

                RowDefinitions[0].Height = new GridLength(0, GridUnitType.Absolute);
                ForceLayout();
            }
            else if (!OrientationHelper.IsInLandscapeMode && RowDefinitions[0].Height.IsAbsolute) // komt terug  van landscape
            {
                RowDefinitions[0].Height = new GridLength(0, GridUnitType.Auto);
                if (_demoBar != null)
                    _demoBar.IsVisible = true;
                if (_label != null)
                    _label.IsVisible = true;
            }
        }
    }
}
