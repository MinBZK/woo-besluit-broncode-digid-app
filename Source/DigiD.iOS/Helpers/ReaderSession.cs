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
using System.Threading.Tasks;
using CoreNFC;
using DigiD.Common.NFC.Enums;
using Foundation;

namespace DigiD.iOS.Helpers
{
    public class ReaderSession : NFCTagReaderSessionDelegate
    {
        private readonly Func<Task> _tagFoundAction;
        private readonly Func<NfcError, Task> _error;
        private readonly Action _resetAction;
        public INFCIso7816Tag Tag { get; private set; }


        public ReaderSession(Func<Task> tagFoundAction, Func<NfcError, Task> errorAction, Action resetAction)
        {
            _tagFoundAction = tagFoundAction;
            _error = errorAction;
            _resetAction = resetAction;
        }

        public void Reset()
        {
            Tag = null;
        }

        public override void DidDetectTags(CoreNFC.NFCTagReaderSession session, INFCTag[] tags)
        {
            var tag = tags[0].GetNFCIso7816Tag();
            Tag = tag;

            if (tag == null)
                return;

            session.ConnectTo(tag, async (e) =>
            {
                if (e == null)
                {
                    await _tagFoundAction.Invoke();
                }
            });
        }

        public override async void DidInvalidate(CoreNFC.NFCTagReaderSession session, NSError error)
        {
            if (Tag == null && error.Code == 200)
                await _error.Invoke(NfcError.Cancelled);

            if (Tag == null && error.Code == 201)
                await _error.Invoke(NfcError.Timeout);

            _resetAction.Invoke();
        }
    }
}
