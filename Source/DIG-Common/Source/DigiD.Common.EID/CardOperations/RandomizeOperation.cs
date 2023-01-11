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
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.CardSteps;
using DigiD.Common.EID.CardSteps.FileOperations;
using DigiD.Common.EID.CardSteps.Randomize;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Models;
using DigiD.Common.NFC.Enums;

namespace DigiD.Common.EID.CardOperations
{
    public class RandomizeOperation : BaseSecureCardOperation
    {
        private readonly UserConsent _userConsent;
        private readonly string _sessionId;
        private readonly string _serverAddress;
        private readonly CardCredentials _credentials;

        public RandomizeOperation(UserConsent userConsent, string sessionId, string serverAddress, CardCredentials credentials, Action<float> progressChangedAction)  
            : base(progressChangedAction)
        {
            _userConsent = userConsent;
            _credentials = credentials;
            _sessionId = sessionId;
            _serverAddress = serverAddress;
        }

        public override async Task<bool> Execute()
        {
            var sessionData = new EidSessionData
            {
                Consent = _userConsent,
                ServerAddress = _serverAddress,
                SessionId = _sessionId
            };

            if (await new StepGap(_credentials, PasswordType.PIN, GAPType.CAv2, this, ChangeProgress, sessionData).Execute())
                return await base.Execute();

            return false;
        }

        internal override void Init()
        {
            base.Init();

            if (GAP.Card.DocumentType == DocumentType.DrivingLicense)
            {
                Steps.Add(new StepGetSecureCommands(this));
                Steps.Add(new StepSendSecureCommandsToPca(this));
                Steps.Add(new StepSendResponsesToServer(this));
            }
            else
            {
                Steps.Add(new StepRandomizeNik(this));
                Steps.Add(new StepSelectPcaApplicationNik(GAP));
            }
        }
    }
}
