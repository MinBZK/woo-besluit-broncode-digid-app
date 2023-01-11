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
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DigiD.Common.EID.Cards;
using DigiD.Common.EID.Models.CardFiles;
using DigiD.Common.NFC.Enums;

namespace DigiD.Common.EID.Models
{
    public class Card
    {
        internal virtual Task<bool> ReadFiles(bool withPhoto, SMContext context)
        {
            return Task.FromResult(true);
        }

        public static readonly List<Card> AvailableCards = new List<Card>
        {
            new DrivingLicense
            {
                ATR = new byte[] {0x80, 0x31, 0x80, 0x65, 0xB0, 0x85, 0x04, 0x00, 0x11, 0x12, 0x0F, 0xFF, 0x82,0x90, 0x00},
                CardAID = new byte[]{0xA0, 0x00, 0x00, 0x04, 0x56, 0x45, 0x44, 0x4C, 0x2D, 0x30, 0x31 },
                PolymorphicAID = new byte[]{ 0xA0, 0x00, 0x00, 0x07, 0x73, 0x50, 0x43, 0x41 },
                DocumentType = DocumentType.DrivingLicense,
                KeyLength = 32,
                MessageDigestAlgorithm = "SHA-256",
                SignatureAlgorithm = "SHA384withECDSA",
                CANLength = 10,
                ChangePINSteps = 17,
                RandomizeSteps = 33,
                ResumeSteps = 23,
                StatusSteps = 7,
            },
            new eNIK
            {
                ATR = new byte[] {0x80, 0x73, 0x84, 0x21, 0x40},
                CardAID = new byte[]{0xA0, 0x00, 0x00, 0x02, 0x47, 0x10, 0x01},
                PolymorphicAID = new byte[]{0xA0, 0x00, 0x00, 0x07, 0x88, 0x50, 0x43, 0x41, 0x2D, 0x65, 0x4D, 0x52, 0x54, 0x44 },
                DocumentType = DocumentType.IDCard,
                KeyLength = 32,
                MessageDigestAlgorithm = "SHA-256",
                SignatureAlgorithm = "SHA384withECDSA",
                CANLength = 6,
                ChangePINSteps = 17,
                RandomizeSteps = 27,
                ResumeSteps = 15,
                StatusSteps = 9,
            }
        };

        public byte[] ATR { get; set; }
        public DocumentType DocumentType { get; set; }

        public string RDADocumentType => DocumentType == DocumentType.DrivingLicense ? "DRIVING_LICENCE" : "TRAVEL_DOCUMENT";
        public byte[] PolymorphicAID { get; set; }
        public byte[] CardAID { get; private set; }
        public int KeyLength { get; set; }
        public string MessageDigestAlgorithm { get; set; }
        public int CANLength { get; set; }
        public string SignatureAlgorithm { get; set; }

        //Card files
        public EFDir EF_Dir { get; set; }
        public EFCardAccess EF_CardAccess { get; set; }
        public EFSOd EF_SOd { get; set; }
        public EFCOM EFCOM { get; set; }
        public Dictionary<int, File> DataGroups { get; } = new Dictionary<int, File>();

        //Operation steps
        internal int ChangePINSteps { get; set; }
        internal int RandomizeSteps { get; set; }
        internal int ResumeSteps { get; set; }
        internal int StatusSteps { get; set; }

        public string CANName
        {
            get
            {
                switch (DocumentType)
                {
                    case DocumentType.DrivingLicense:
                        return "rijbewijsnummer";
                    case DocumentType.IDCard:
                        return "CAN";
                }

                return string.Empty;
            }
        }
    }
}
