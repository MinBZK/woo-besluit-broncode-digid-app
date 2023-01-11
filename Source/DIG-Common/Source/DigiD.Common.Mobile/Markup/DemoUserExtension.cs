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
using DigiD.Common.Mobile.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.Common.Mobile.Markup
{
    public abstract class DemoUserExtension<T> : IMarkupExtension
    {
        public T On { get; set; }
        public T Off { get; set; }

        public virtual object ProvideValue(IServiceProvider serviceProvider)
        {
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

        protected T GetValue() => DemoHelper.CurrentUser != null ? On : Off;
    }

    public class ThicknessDemoUserExtension : DemoUserExtension<Thickness> { }
    public class BooleanDemoUserExtension : DemoUserExtension<bool> { }
}
