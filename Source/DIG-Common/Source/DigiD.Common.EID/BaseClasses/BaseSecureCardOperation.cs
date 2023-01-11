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
using System.Diagnostics;
using DigiD.Common.EID.CardSteps;
using DigiD.Common.EID.Interfaces;

namespace DigiD.Common.EID.BaseClasses
{
    public class BaseSecureCardOperation : BaseCardOperation, ISecureCardOperation
    {
        private readonly Action<float> _progressChangedAction;
        internal int TotalSteps;
        private int _currentIndex = 1;

        public BaseSecureCardOperation(Action<float> progressChangedAction = null)
        {
            _progressChangedAction = progressChangedAction;
        }

        public BaseSecureCardOperation(Gap operation)
        {
            GAP = operation;
        }

        public override void StepCompleted(IStep stepNumber)
        {
            ChangeProgress();
        }

        internal void ChangeProgress()
        {
            Debug.WriteLine($"{_currentIndex}/{TotalSteps}");
            _progressChangedAction?.Invoke((float)_currentIndex / TotalSteps);
            _currentIndex++;
        }

        public Gap GAP { get; set; }
    }
}
