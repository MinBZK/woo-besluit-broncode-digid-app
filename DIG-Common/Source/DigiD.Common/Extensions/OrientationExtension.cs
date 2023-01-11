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
using DigiD.Common.Enums;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.Common.Markup
{
    public abstract class OrientationExtension<T> : IMarkupExtension
    {
        public T Landscape { get; set; }
        public T Portrait { get; set; }

        public virtual object ProvideValue(IServiceProvider serviceProvider)
        {
            // Vraag het object op die referenties bevat naar de property en het object
            // waaraan deze markupextension is gekoppeld
            var provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();

            // Het TargetObject is de View en de TargetProperty is de property waaraan deze markupextension is gekoppeld
            if (provideValueTarget.TargetObject is BindableObject targetObject &&
                provideValueTarget.TargetProperty is BindableProperty targetProperty)
            {
               
                // Als de orientatie van het beeldscherm wijzigt
                DeviceDisplay.MainDisplayInfoChanged += (s, e) => {
                    var value = GetValue();

                    // schrijven we de waarde behorende bij de nieuwe orientatie naar de eigenschap
                    // waaraan deze markupextension is gekoppeld 
                    targetObject.SetValue(targetProperty, value);
                };
            }
            var value = GetValue();
            return value;
        }

        protected T GetValue() => DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Landscape ?
            Landscape :
            Portrait;
    }

    public class StackOrientationExtension : OrientationExtension<StackOrientation> { }
    public class ThicknessOrientationExtension : OrientationExtension<Thickness> { }
    // Als de property's optioneel zijn, dan moeten alle variabelen nullable zijn.
    public class ChunkSizeOrientationExtension : OrientationExtension<int> { }
    public class ChunkOrientationOrientationExtension : OrientationExtension<ChunkOrientationEnum> { }
    public class DoubleOrientationExtension : OrientationExtension<double?> { }
    public class IntOrientationExtension : OrientationExtension<int> { }
    public class GridColumnOrientationExtension : OrientationExtension<int?> { }
    public class GridRowOrientationExtension : OrientationExtension<int?> { }
    public class ColorOrientationExtension : OrientationExtension<Color> { }
    public class BooleanOrientationExtension : OrientationExtension<bool> { }
    public class LayoutOptionsOrientationExtension : OrientationExtension<LayoutOptions> { }
}
