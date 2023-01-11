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
#if A11YTEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.BaseClasses;
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using Xamarin.Forms;

namespace DigiD.Helpers
{
    public class A11YTestHelper
    {
        private readonly bool _auto;
        static readonly INavigationService NavigationService = DependencyService.Get<INavigationService>();
        private List<Type> _viewModelsWithParameters;
        private List<Type> _viewModelsNoParameters;
        private int _index;

        public A11YTestHelper(bool auto)
        {
            _auto = auto;
        }
        
        public async Task Start()
        {
            var firstPage = new ContentPage
            {
                Content = new Label
                {
                    TextColor = Color.Orange,
                    Text = "Alle viewmodels zijn nu getest",
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand
                }
            };

            // Eerst het 1 en ander aanmaken om NullReferences te voorkomen.
            WhatsNewHelper.Init();
            Application.Current.MainPage = new CustomNavigationPage(firstPage);
            
            await App.GetActualConfiguration();
            
            DependencyService.Get<IPreferences>().LetterRequestDate = DateTimeOffset.Now.AddDays(-3);

            TestViewModels();

            await CheckPage();
        }

        public async Task Next()
        {
            _index++;

            if (_index <= _viewModelsNoParameters.Count-1)
            {
                var viewModel = (BaseViewModel)Activator.CreateInstance(_viewModelsNoParameters[_index]);
                await NavigationService.PushAsync(viewModel);    
            }
        }

        private async Task CheckPage()
        {
            // nodig voor RdaScanFailedViewModel.ctor
            HttpSession.ActivationSessionData = new ActivationSessionData
            {
                ActivationMethod = ActivationMethod.SMS
            };

            var viewModel = (BaseViewModel)Activator.CreateInstance(_viewModelsNoParameters[_index]);
            await NavigationService.PushAsync(viewModel);

            if (_auto)
            {
                await Task.Delay(5000);

                while (_index != _viewModelsNoParameters.Count)
                {
                    await Next();
                    await Task.Delay(5000);
                }
            }
        }

        private void TestViewModels()
        {
            _viewModelsNoParameters = new List<Type>();
            _viewModelsWithParameters = new List<Type>();

            foreach (var vm in NavigationService.RegisteredPages.Keys)
            {
                if (ViewModelHasParameterLessCtor(vm))
                    _viewModelsNoParameters.Add(vm);
                else
                    _viewModelsWithParameters.Add(vm);
            }
            var teller = 1;
            System.Diagnostics.Debug.WriteLine($"" +
                $"\n***************************************************" +
                $"\n* ViewModels with parameterless constructors    *" +
                $"\n***************************************************");
            foreach (var vm in _viewModelsNoParameters)
                System.Diagnostics.Debug.WriteLine($"{teller++:D2} - {vm.FullName}");

            teller = 1;
            System.Diagnostics.Debug.WriteLine($"" +
                $"\n***************************************************" +
                $"\n* ViewModels with parameter constructors    *" +
                $"\n***************************************************");
            foreach (var vm in _viewModelsWithParameters)
                System.Diagnostics.Debug.WriteLine($"{teller++:D2} - {vm.FullName}");

            _viewModelsNoParameters = _viewModelsNoParameters.OrderBy(x => x.FullName).ToList();
        }

        private static bool ViewModelHasParameterLessCtor(Type vm)
        {
            var constructors = vm.GetConstructors();
            foreach (var ctr in constructors)
            {
                if (ctr.GetParameters().Length == 0)
                    return true;
            }

            return false;
        }
    }
}
#endif
