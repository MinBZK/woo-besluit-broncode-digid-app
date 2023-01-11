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
﻿using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;

namespace DigiD.Common.EID.CardSteps.PACE
{
    /// <inheritdoc />
    /// <summary>
    /// This Step will reset the pace session with new credentials
    /// </summary>
    public class PaceReset : PaceOperation
    {
        private readonly PasswordType _resetType;

        internal PaceReset(ISecureCardOperation operation, CardCredentials credentials, PasswordType passwordType, PasswordType resetType) 
            : base(operation, credentials, passwordType)
        {
            _resetType = resetType;
        }

        /// <inheritdoc />
        /// <summary>
        /// Set all steps
        /// </summary>
        internal override void Init()
        {
            base.Init();

            Steps.Add(new StepResetPace(_resetType, this));
            Steps.Add(new StepSetAT(Operation));
            Steps.Add(new StepGetEncryptedNonce(Operation));
            Steps.Add(new StepGenerateKeyPair(this, Operation));
            Steps.Add(new StepMapNonce(Operation));
            Steps.Add(new StepGenerateAgreementKeyPair(Operation));
            Steps.Add(new StepGetCardAgreementPublicKey(Operation));
            Steps.Add(new StepCalculateMacKeys(Operation));
            Steps.Add(new StepMutualAuthentication(Operation));
            Steps.Add(new StepValidateOperation(Operation));
        }
    }
}
