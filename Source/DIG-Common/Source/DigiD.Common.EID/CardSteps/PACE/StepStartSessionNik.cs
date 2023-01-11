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
using System.Threading.Tasks;
using DigiD.Common.EID.Constants;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models.Network.Requests;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Helpers;
using DigiD.Common.Http.Models;

namespace DigiD.Common.EID.CardSteps.PACE
{
    internal class StepStartSessionNik : IStep
    {
        private readonly Gap _gap;

        internal StepStartSessionNik(Gap gap)
        {
            _gap = gap;
        }

        public async Task<bool> Execute()
        {
            if (_gap.SessionData == null)
                return true;

            var requestData = new
            {
                userConsentType = _gap.UserConsent == UserConsent.PP ? "PP" : "PIP",
                documentType = Convert.ToBase64String(_gap.Card.CardAID)
            };

            var data = new EIDBaseRequest(requestData, _gap.SessionData.SessionId);

            var response = await EIDSession.Client.PostAsync<BaseResponse>(new Uri($"{_gap.SessionData.ServerAddress}{WidConstants.NIK_START_SESSION}"), data);
            _gap.ApiResult = response.ApiResult;
            return response.ApiResult == ApiResult.Ok;
        }
    }
}
