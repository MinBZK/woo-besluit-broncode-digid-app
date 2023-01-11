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
﻿using DigiD.Common.Interfaces;
using Xamarin.Forms;

namespace DigiD.Security
{
    internal class SecureElementImplementation : ISigningService
    {
        public bool IsSupported => DependencyService.Get<ISigningService>().IsSupported;
        public bool HardwareSupport => DependencyService.Get<ISigningService>().HardwareSupport;
        public bool RemoveKeyPair()
        {
            return DependencyService.Get<ISigningService>().RemoveKeyPair();
        }

        public bool GenerateKeyPair()
        {
            return DependencyService.Get<ISigningService>().GenerateKeyPair();
        }

        public byte[] ExportPublicKey()
        {
            return DependencyService.Get<ISigningService>().ExportPublicKey();
        }

        public byte[] Sign(string data)
        {
            return DependencyService.Get<ISigningService>().Sign(data);
        }
    }
}
