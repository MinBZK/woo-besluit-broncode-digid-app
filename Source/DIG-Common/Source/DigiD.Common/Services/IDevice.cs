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
﻿using System.Threading.Tasks;
using DigiD.Common.Enums;

namespace DigiD.Common.Services
{
    public interface IDevice
    {
        int PiwikId { get; }
        bool HasHomeButton { get; }
        string RuntimePlatform { get; }
        string DeviceTypeName { get; }
        string DefaultLanguage { get; }
        bool IsScreenCaptured { get; }
        bool IsInDeveloperMode { get; }
        SystemFontSize GetSystemFontSize();

        void OpenSettings();
        bool OpenBrowser(string name, string uri);
        Task<string> UserAgent();
        void CloseApp();
        Task<bool> AskForLocalNotificationPermission();
    }
}
