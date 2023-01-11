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
