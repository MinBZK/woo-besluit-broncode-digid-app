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
