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
﻿using System.Security;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.CardSteps.FileOperations;
using DigiD.Common.EID.Interfaces;

namespace DigiD.Common.EID.CardSteps.BAC
{
    public class BacOperation : BaseCardOperation, IStep
    {
        internal const string ENC_C = "00-00-00-01";
        internal const string MAC_C = "00-00-00-02";

        private readonly Gap _operation;
        
        public SecureString Mrz { get; protected set; }
        public byte[] KSeed { get; set; }
        public byte[] KEnc { get; set; }
        public byte[] KMac { get; set; }
        public byte[] RND_IC { get; set; }

        internal BacOperation(Gap operation, SecureString mrz)
        {
            _operation = operation;
            Mrz = mrz;
        }

        internal override void Init()
        {
            base.Init();

            Steps.Add(new StepSelecteDLApplication(_operation));
            Steps.Add(new StepCalculateKSeed(this));
            Steps.Add(new StepCalculateBasicAccessKeys(this));
            Steps.Add(new StepRequestRandomNumber(this));
            Steps.Add(new StepGeneralAuthenticate(_operation, this));
            Steps.Add(new StepReadEFCOM(_operation));
        }
    }
}
