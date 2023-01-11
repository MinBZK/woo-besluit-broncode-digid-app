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
﻿using System.Diagnostics;
using Android.Content;
using Android.Nfc;

namespace DigiD.Droid.Helpers
{
    internal class NfcBroadCastReceiver : BroadcastReceiver
    {
        private readonly System.Action<Tag> _action;
        public NfcBroadCastReceiver(System.Action<Tag> tagDiscoveredAction)
        {
            _action = tagDiscoveredAction;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent != null && NfcConstants.ACTION_NFC.Equals(intent.Action))
            {
                var tag = (Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
                if (IsTagSupported(tag))
                {
                    Debug.WriteLine("Supported tag discovered");
                    _action.Invoke(tag);
                }
                else
                    Debug.WriteLine("Unsupported tag discovered");
            }

            bool IsTagSupported(Tag tag)
            {
                if (tag == null) return false;
                for (var i = 0; i < tag.GetTechList().Length; i++)
                {
                    var sText = tag.GetTechList()[i];
                    if (sText.Equals(NfcConstants.SUPPORTED_NFC))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
