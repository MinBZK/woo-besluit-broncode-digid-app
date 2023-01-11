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
﻿using System.Threading.Tasks;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.Cards;
using DigiD.Common.EID.CardSteps.FileOperations;
using DigiD.Common.EID.Interfaces;

namespace DigiD.Common.EID.CardSteps.CAv2
{
    internal class ChipAuthenticationRdw : BaseSecureCardOperation, IStep
    {
        public byte[] Ricc { get; set; }
        public byte[] Ticc { get; set; }
        
        internal ChipAuthenticationRdw(ISecureCardOperation operation) : base((Gap)operation)
        {

        }

        internal override void Init()
        {
            base.Init();
            Steps.Add(new StepReadEFCardSecurity(this));
            Steps.Add(new StepSelectMF(this));
            Steps.Add(new StepMseSetAT(this));
            Steps.Add(new StepGeneralAuthenticate(this, ((DrivingLicense)GAP.Card).TA.PublicKey));
        }

        public override async Task<bool> Execute()
        {
            var result = await base.Execute();
            if (result)
                ((DrivingLicense)GAP.Card).CA = this;

            return result;
        }

        public override void StepCompleted(IStep stepNumber)
        {
            GAP.StepCompleted(stepNumber);
        }
    }
}
