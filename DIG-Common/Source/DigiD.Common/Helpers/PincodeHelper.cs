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
using System.Globalization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using DigiD.Common.EID.Helpers;
using DigiD.Common.Models;
using Xamarin.Forms;

namespace DigiD.Common.Helpers
{
    public static class PinCodeHelper
    {
        public static PincodeValidationResult IsPinCodeComplexEnough(SecureString pin)
        {
            var result = new PincodeValidationResult
            {
                Success = true,
                Header = Device.Idiom == TargetIdiom.Desktop ? AppResources.DA1xx_Header : AppResources.PincodeComplexHeader
            };

            if (pin.IsIncreasing())
            {  
                result.Success = false;
                result.Message = Device.Idiom == TargetIdiom.Desktop ? AppResources.DA110_Message : AppResources.PincodeIncreasingError;
            } 
            else if (pin.IsDecreasing()) 
            {
                result.Success = false;
                result.Message = Device.Idiom == TargetIdiom.Desktop ? AppResources.DA111_Message :AppResources.PincodeDecreasingError;
            } 
            else if (pin.AreEqual()) 
            {
                result.Success = false;
                result.Message = Device.Idiom == TargetIdiom.Desktop ? AppResources.DA109_Message : AppResources.PincodeDistictError;
            }
				
            return result;
        }

        public static string GenerateMaskCode()
        {
            using (var random = new RNGCryptoServiceProvider())
            {
                var byteArray = new byte[5];
                random.GetBytes(byteArray);

                //convert 5 bytes to an integer
                var randomInteger = BitConverter.ToInt32(byteArray, 0);
                //convert value to positive
                randomInteger = Math.Abs(randomInteger);

                //Take the first 5 chars of the integer
                return randomInteger.ToString(CultureInfo.InvariantCulture).Substring(0, 5);
            }
        }

        public static string MaskPin(SecureString pin, string mask)
        {
            if (pin == null)
                throw new ArgumentNullException(nameof(pin));

            var maskCode = mask.ToCharArray();
            var pinValue = pin.ToPlain().ToCharArray();

            var pinResult = new StringBuilder();

            for (var x = 0; x < 5; x++)
            {
                try
                {
                    var val1 = int.Parse(maskCode[x].ToString(), CultureInfo.InvariantCulture);
                    var val2 = int.Parse(pinValue[x].ToString(), CultureInfo.InvariantCulture);
                    var sum = val1 + val2;
                    pinResult.Append(sum % 10);
                }
                catch (Exception)
                {
                    //Entered pin is invalide
                }
            }

            return pinResult.ToString();
        }
    }
}
