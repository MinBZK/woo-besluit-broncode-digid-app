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
