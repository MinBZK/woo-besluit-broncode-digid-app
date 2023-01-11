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
﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace DigiD.Common.Converters
{
    public class BooleanInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Het kan op een andere manier, maar op deze manier blijft de bestaande code goed werken
            var isNullable = Nullable.GetUnderlyingType(targetType) != null;

            if (!isNullable)
            {
                if (parameter is bool boolParam)
                    return !(bool)value && boolParam;
                return !(bool)value;
            }
            else
            {
                if (value == null)
                    return default(bool?);

                var boolValue = ((bool?)value).Value;
                if (parameter is bool boolParam)
                    return  (bool?)(!boolValue && boolParam);
                return (bool?)(!boolValue);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
