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
using System.Collections.Generic;
using System.Threading.Tasks;
using DigiD.Common.EID.Exceptions;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.NFC.Exceptions;
using DigiD.Common.NFC.Helpers;

namespace DigiD.Common.EID.BaseClasses
{
    public class BaseCardOperation : ICardOperation
    {
        private bool _isInitialized;

        internal virtual void Init()
        {
            _isInitialized = true;
        }

        public List<IStep> Steps { get; } = new List<IStep>();

        public virtual async Task<bool> Execute()
        {
            if (!_isInitialized)
                Init();

            foreach (var step in Steps)
            {
                Debugger.WriteLine($"{step.GetType()}: Started");

                try
                {
                    var stepResult = await step.Execute();

                    if (!stepResult)
                    {
                        Debugger.WriteLine($"{step.GetType()}: Failed");
                        return false;
                    }
                }
                catch (CardLostException)
                {
                    throw;
                }
                catch (TimeoutException)
                {
                    throw;
                }
                catch (CardNotSupportedException)
                {
                    throw;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
#if DEBUG
                    Debugger.WriteLine(ex.ToString());
#endif
                    return false;
                }

                StepCompleted(step);
                Debugger.WriteLine($"{step.GetType()}: Completed");
            }

            return true;
        }

        public virtual void StepCompleted(IStep stepNumber){}
    }
}
