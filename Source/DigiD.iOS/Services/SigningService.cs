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
using DigiD.Common.Interfaces;
using DigiD.iOS.Helpers;
using DigiD.iOS.Services;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(SigningService))]
namespace DigiD.iOS.Services
{
    internal class SigningService : ISigningService
    {
        private const string Key = "SSSSSS";
        private const string TestKey = "SSSSSSS";

        public bool RemoveKeyPair()
        {
            return SecureEnclaveHelper.Remove(Key);
        }

        public bool IsSupported
        {
            get
            {
                if (DeviceInfo.DeviceType != DeviceType.Physical)
                    return false;

                try
                {
                    if (!SecureEnclaveHelper.Generate(TestKey))
                        return false;

                    var result = SecureEnclaveHelper.Export(TestKey) != null;

                    return result;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    SecureEnclaveHelper.Remove(TestKey);
                }
            }
        }

        public bool HardwareSupport => true;

        public bool GenerateKeyPair()
        {
            return SecureEnclaveHelper.Generate(Key);
        }

        public byte[] ExportPublicKey()
        {
            return SecureEnclaveHelper.Export(Key);
        }

        public byte[] Sign(string data)
        {
            return SecureEnclaveHelper.Sign(Key, data);
        }
    }
}
