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
using System.Threading.Tasks;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models;
using DigiD.Common.Settings;
using Xamarin.Forms;

namespace DigiD.Api.Demo
{
    public class UsageHistoryService : IUsageHistoryService
    {
        private readonly List<UsageHistoryModel> _recordsNL = new List<UsageHistoryModel>();
        private readonly List<UsageHistoryModel> _recordsEN = new List<UsageHistoryModel>();
        private const int PageSize = 10;

        private readonly List<string> _optionsNL = new List<string>
        {
            "Bekijkt gebruiksgeschiedenis",
            "Pincode succesvol ingevoerd in DigiD app met ID-check voor webdienst Mijn DigiD (telefoon)",
            "Inloggen gelukt voor activeren app met een andere app",
            "SSO authenticatie domein MijnOverheid gelukt",
            "Inloggen met DigiD app met ID-check gelukt bij webdienst MijnOverheid",
            "Inloggen met identiteitskaart succesvol voor webdienst Mijn DigiD",
            "DigiD app geactiveerd"
        };

        private readonly List<string> _optionsEN = new List<string>
        {
            "Views usage history",
            "Pin code successfully entered in DigiD app with ID check for web service My DigiD (telephone)",
            "Sign in successful for activating app with another app",
            "SSO authentication domain MijnOverheid succeeded",
            "Log in with DigiD app with ID check successful at web service MijnOverheid",
            "Successful login with identity card for web service My DigiD",
            "DigiD app activated"
        };

        public UsageHistoryService()
        {
            var rnd = new Random();
            
            for (var x = 1; x <= 50; x++)
            {
                _recordsNL.Add(new UsageHistoryModel {DateTime = DateTime.Now.AddDays(-x), Message = _optionsNL[rnd.Next(0, 6)]});
                _recordsEN.Add(new UsageHistoryModel {DateTime = DateTime.Now.AddDays(-x), Message = _optionsEN[rnd.Next(0, 6)]});
            }
        }

        public async Task<List<UsageHistoryModel>> GetUsageHistory(int pageIndex = 0)
        {
            await Task.Delay(100);

            var model = new UsageHistoryRequestModel
            {
                Language = DependencyService.Get<IGeneralPreferences>().Language.ToUpper(),
                PageId = pageIndex
            };

            return DemoHelper.Log("/apps/logs/get_logs", model, (model.Language == "NL" ? _recordsNL : _recordsEN).Skip(model.PageId * PageSize).Take(PageSize).ToList());
        }
    }
}
