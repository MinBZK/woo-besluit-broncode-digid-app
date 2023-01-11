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
using System.Windows.Input;
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.ViewModels;
using Xamarin.Forms;

namespace DigiD.Common.Controls
{
	public partial class HelpButtonView : Grid
    {
        public static readonly BindableProperty InfoPageTypeProperty = BindableProperty.Create(nameof(InfoPageType), typeof(InfoPageType), typeof(HelpButtonView), InfoPageType.Undefined);

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(HelpButtonView), null, BindingMode.OneTime);

        public InfoPageType InfoPageType
	    {
	        get => (InfoPageType) GetValue(InfoPageTypeProperty);
	        set => SetValue(InfoPageTypeProperty, value);
	    }
        
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public HelpButtonView()
		{
			InitializeComponent ();
        }

        protected override void OnTabIndexPropertyChanged(int oldValue, int newValue)
        {
            base.OnTabIndexPropertyChanged(oldValue, newValue);
            if (__HelpButton__ != null)
                __HelpButton__.TabIndex = newValue;
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            if (InfoPageType != InfoPageType.Undefined)
            {
                var infoViewModel = new InfoViewModel(new Models.HelpModel(InfoPageType));
                Device.BeginInvokeOnMainThread(async () =>  await DependencyService.Get<IPopupService>().OpenPopup(infoViewModel));
            }

            if (Command != null && Command.CanExecute(null))
                Command.Execute(null);
        }
    }
}
