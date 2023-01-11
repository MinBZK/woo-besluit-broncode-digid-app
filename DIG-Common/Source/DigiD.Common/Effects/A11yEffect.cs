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
﻿using DigiD.Common.Constants;
using System.Linq;
using Xamarin.Forms;

namespace DigiD.Common.Effects
{
    public class A11YEffect : RoutingEffect
    {
        public static readonly BindableProperty ControlTypeProperty = BindableProperty.CreateAttached("ControlType", typeof(A11YControlTypes), typeof(A11YEffect), A11YControlTypes.None, propertyChanged: ControlTypePropertyChanged);

        private static void ControlTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is View view))
                return;

            var oldControlType = (A11YControlTypes)oldValue;
            var newControlType = (A11YControlTypes)newValue;

            if (oldControlType != newControlType)
            {
                if (newControlType != A11YControlTypes.None)
                {
                    view.Effects.Add(new A11YEffect());
                }
                else
                {
                    var toRemove = view.Effects.FirstOrDefault(e => e is A11YEffect && GetControlType(view) == oldControlType);
                    if (toRemove != null)
                        view.Effects.Remove(toRemove);
                }
            }
        }

        public static A11YControlTypes GetControlType(BindableObject view) => (A11YControlTypes)view?.GetValue(ControlTypeProperty);
        public static void SetControlType(BindableObject view, A11YControlTypes value) => view?.SetValue(ControlTypeProperty, value);

        public A11YEffect() : base($"{AppConfigConstants.A11YEffectGroupName}.{nameof(A11YEffect)}")
        {
        }
    }
}
