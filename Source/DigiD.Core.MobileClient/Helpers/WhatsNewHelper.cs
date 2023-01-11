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
using System.IO;
using System.Reflection;
using DigiD.Common.Settings;
using DigiD.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using DigiD.ViewModels;
using DigiD.Common.Interfaces;
using System.Threading.Tasks;
using DigiD.Common.SessionModels;

namespace DigiD.Helpers
{
    public static class WhatsNewHelper
    {
        public static WhatsNewDataModel Data { get; private set; }

        public static void Init()
        {
            var assembly = typeof(WhatsNewHelper).GetTypeInfo().Assembly;

            try
            {
                using (var reader = new StreamReader(assembly.GetManifestResourceStream("DigiD.Resources.WhatsNew.json")))
                {
                    Data = JsonConvert.DeserializeObject<WhatsNewDataModel>(reader.ReadToEnd());
                }
            }
            catch
            {
                Data = new WhatsNewDataModel();
            }
        }

        internal static bool MustShow
        {
            get
            {
                if (Data == null)
                    Init();

                var oldVersion = DependencyService.Get<IGeneralPreferences>().WhatsNewVersion ?? string.Empty;
                return Data!.PageData.Count > 0 && !oldVersion.Equals(Data.Version) && !HttpSession.IsApp2AppSession && !HttpSession.IsWeb2AppSession;
            }
        }

        internal static async Task Show(Func<Task> stopAction = null, WhatsNewTourViewModel whatsNewTourViewModel = null)
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    var viewModel = whatsNewTourViewModel ?? new WhatsNewTourViewModel(stopAction);
                    var page = DependencyService.Get<INavigationService>().GetPage(viewModel);
                    await Application.Current.MainPage.Navigation.PushModalAsync(page, false);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Cannot display WhatsNew: {ex.Message}");
                }
            });
        }
    }
}
