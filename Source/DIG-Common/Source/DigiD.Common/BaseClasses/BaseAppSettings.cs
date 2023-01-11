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
﻿using System.Reflection;
using System.Runtime.CompilerServices;
using DigiD.Common.Services;
using Xamarin.Forms;

namespace DigiD.Common.BaseClasses
{
    public class BaseAppSettings
    {
        protected IKeyStore Store { get; }

        public BaseAppSettings()
        {
            Store = DependencyService.Get<IKeyStore>();
        }

        public T GetValue<T>(T defaultValue, [CallerMemberName]string key = "")
        {
            if (Store.TryGetValue(key, out T result))
                return result;

            return defaultValue;
        }

        public void SetValue<T>(T value, [CallerMemberName]string key = "")
        {
            if (value == null)
                Store.Delete(key);
            else
                Store.SetValue(key, value);
        }

        public void Save()
        {
            Store.Save();
        }

        public virtual void Reset()
        {
            foreach (var p in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                Store.Delete(p.Name);
            }

            Save();
        }
    }
}
