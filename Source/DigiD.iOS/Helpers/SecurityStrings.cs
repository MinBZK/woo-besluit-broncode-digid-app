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
using DigiD.Common.Helpers;
using ObjCRuntime;

namespace DigiD.iOS.Helpers
{
	internal static class SecurityStrings
	{
		static SecurityStrings()
		{
			var handle = Dlfcn.dlopen(Constants.SecurityLibrary, 0);
			
            if (handle == IntPtr.Zero)
            {
                return;
            }

			try
			{
				// keys
				SecAttrTokenID = Dlfcn.GetStringConstant(handle, "kSecAttrTokenID");
				SecAttrKeyType = Dlfcn.GetStringConstant(handle, "kSecAttrKeyType");
				SecAttrKeySizeInBits = Dlfcn.GetStringConstant(handle, "kSecAttrKeySizeInBits");
				SecPrivateKeyAttrs = Dlfcn.GetStringConstant(handle, "kSecPrivateKeyAttrs");
				SecPublicKeyAttrs = Dlfcn.GetStringConstant(handle, "kSecPublicKeyAttrs");
				SecAttrAccessControl = Dlfcn.GetStringConstant(handle, "kSecAttrAccessControl");
				SecAttrIsPermanent = Dlfcn.GetStringConstant(handle, "kSecAttrIsPermanent");
				SecAttrApplicationTag = Dlfcn.GetStringConstant(handle, "kSecAttrApplicationTag");
                SecAttrCanSign = Dlfcn.GetStringConstant(handle, "kSecAttrCanSign");
				SecAttrLabel = Dlfcn.GetStringConstant(handle, "kSecAttrLabel");
                SecUseAuthenticationContext = Dlfcn.GetStringConstant(handle, "kSecUseAuthenticationContext");

                // values
                SecAttrTokenIDSecureEnclave = Dlfcn.GetStringConstant(handle, "kSecAttrTokenIDSecureEnclave");
				SecAttrKeyTypeEC = Dlfcn.GetStringConstant(handle, "kSecAttrKeyTypeEC");
			}
			catch (Exception e)
			{
                AppCenterHelper.TrackError(e);
			}
			finally
			{
				Dlfcn.dlclose(handle);
			}
		}

		public static string SecAttrKeyType { get; }
		public static string SecAttrTokenID { get; }
		public static string SecAttrKeySizeInBits { get; }
		public static string SecPrivateKeyAttrs { get; }
		public static string SecPublicKeyAttrs { get; }
		public static string SecAttrAccessControl { get; }
		public static string SecAttrIsPermanent { get; }
		public static string SecAttrApplicationTag { get; }
        public static string SecAttrCanSign { get; }
		public static string SecAttrTokenIDSecureEnclave { get; }
		public static string SecAttrKeyTypeEC { get; }
		public static string SecAttrLabel { get; }
        public static string SecUseAuthenticationContext { get; }
    }
}
