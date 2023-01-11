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
using DigiD.Common.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.Common.Markup
{
    public abstract class SystemFontSettingExtension<T> : IMarkupExtension
    {
        /// <summary>
        /// Als de property's optioneel zijn, dan moeten de variabelen nullable zijn.
        /// zie ook hieronder de verschillende classes met type int? of double? en de code
        /// int T GetValue().
        ///
        /// De 'Normal' property moet altijd gezet zijn in de xaml, anders krijg je runtime
        /// een exception om je oren.
        ///
        /// Als 'Small' niet gezet is dan wordt de 'Normal' waarde gepakt.
        /// Als 'ExtraLarge' niet is gezet, dan wordt de 'Large' waarde gepakt, is deze niet gezet
        /// dan wordt de 'Normal' waarde gepakt.
        ///
        ///
        /// int's en double' zijn default 0, als deze dus niet nullable zijn, zal bovenstaande niet
        /// werken, daarom wordt er gebruik gemaakt van int? en double?, objecten zijn altijd nullable.
        ///
        /// </summary>
        public virtual T Small { get; set; }
        public virtual T Normal { get; set; }
        public virtual T Large { get; set; }
        public virtual T ExtraLarge { get; set; }

        public virtual object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Normal == null)
                throw new XamlParseException("SystemFontSettingExtension requires a non-null value to be specified for 'Normal' property.");

            // Vraag het object op die referenties bevat naar de property en het object
            // waaraan deze markupextension is gekoppeld
            var provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();

            var value = GetValue();
            // Het TargetObject is de View en de TargetProperty is de property waaraan deze markupextension is gekoppeld
            if (provideValueTarget.TargetObject is BindableObject targetObject &&
                provideValueTarget.TargetProperty is BindableProperty targetProperty)
            {

                // Als de systeem font grootte is aangepast
                // schrijven we de waarde behorende bij de nieuwe system fontsize naar de eigenschap
                // waaraan deze markupextension is gekoppeld 
                targetObject.SetValue(targetProperty, value);
            }

            return value;
        }
        protected T GetValue(
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
            , [System.Runtime.CompilerServices.CallerFilePath] string srcFilePath= ""
            , [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0)
        {
            T result = Normal;
            var currentSystemFontSize = DependencyService.Get<IDevice>().GetSystemFontSize();
            //KTR niet weggooien aub: System.Diagnostics.Debug.WriteLine($"\n{memberName}.{srcFilePath}.{lineNumber} --> currentSystemFontSize: {currentSystemFontSize}");
            switch (currentSystemFontSize)
            {
                case SystemFontSize.XS:
                case SystemFontSize.S:
                    result = Small ?? Normal;
                    break;
                case SystemFontSize.M:
                case SystemFontSize.L:
                    result = Normal;
                    break;
                case SystemFontSize.XL:
                case SystemFontSize.XXL:
                    result = Large ?? Normal;
                    break;
                case SystemFontSize.XXXL:
                case SystemFontSize.ExtraM:
                case SystemFontSize.ExtraL:
                    result = ExtraLarge ?? Large ?? Normal;
                    break;
            }
            //KTR niet weggooien aub: System.Diagnostics.Debug.WriteLine($"\n{memberName}.{srcFilePath}.{lineNumber} --> result: {result}");
            return result;
        }
    }

    public class ChunkOrientationSFSExtension : SystemFontSettingExtension<ChunkOrientationEnum> { }
    public class ChunkSizeSFSExtension : SystemFontSettingExtension<int?> { }
    public class DoubleSFSExtension : SystemFontSettingExtension<double?> { }
    public class IntSFSExtension : SystemFontSettingExtension<int?> { }
    public class StackLayoutOrientationSFSExtension : SystemFontSettingExtension<StackOrientation> { }
    public class ThicknessSFSExtension : SystemFontSettingExtension<Thickness>
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var result = Normal;
            var value = base.ProvideValue(serviceProvider);
            if (value is Thickness thickness)
            {
                if (!thickness.IsEmpty)
                {
                    result = thickness;
                }
            }
            return result;
        }
    }
    public class GridColumnSFSExtension : SystemFontSettingExtension<int?> { }
    public class GridRowSFSExtension : SystemFontSettingExtension<int?> { }
}
