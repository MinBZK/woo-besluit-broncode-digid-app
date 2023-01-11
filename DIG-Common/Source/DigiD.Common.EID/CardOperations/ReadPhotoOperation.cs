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
using System.Linq;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.CardSteps;
using DigiD.Common.EID.CardSteps.FileOperations;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.CardFiles;

namespace DigiD.Common.EID.CardOperations
{
    public class ReadPhotoOperation : BaseSecureCardOperation
    {
        private readonly CardCredentials _credentials;
        private readonly GAPType _gapType;
        private readonly PasswordType _passwordType;
        public byte[] Photo { get; set; }

        /// <summary>
        /// Get Photo from document (ePassport of eDriving License)
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="progressChangedAction"></param>
        /// <param name="gapType"></param>
        public ReadPhotoOperation(CardCredentials credentials, GAPType gapType, PasswordType passwordType, Action<float> progressChangedAction)
            : base(progressChangedAction)
        {
            _credentials = credentials;
            _gapType = gapType;
            _passwordType = passwordType;
        }

        internal override void Init()
        {
            base.Init();

            Steps.Add(new StepGap(_credentials, _passwordType, _gapType, this, ChangeProgress));
            
            if (_gapType != GAPType.Bac)
                Steps.Add(new StepSelecteDLApplication(this));

            Steps.Add(new StepReadFiles(this, true));
        }

        public override void StepCompleted(IStep stepNumber)
        {
            base.StepCompleted(stepNumber);
            var photo = (PhotoFile)GAP.Card.DataGroups.Values.FirstOrDefault(x => x is PhotoFile);
            Photo = photo?.Photo;
        }
    }
}
