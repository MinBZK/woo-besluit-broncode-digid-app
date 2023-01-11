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
