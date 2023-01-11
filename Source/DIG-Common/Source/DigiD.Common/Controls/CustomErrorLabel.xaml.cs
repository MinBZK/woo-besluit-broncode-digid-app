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
﻿using DigiD.Common.Helpers;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.Common.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomErrorLabel : Grid
    {
        public string ErrorText {
            get => __errorLabel__.Text;
            set => __errorLabel__.Text = value;
        }

        public LineBreakMode LineBreakMode {
            get => __errorLabel__.LineBreakMode;
            set => __errorLabel__.LineBreakMode = value;
        }

        public CustomErrorLabel()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (nameof(IsVisible).Equals(propertyName))
            {
                __errorLabel__.IsVisible = IsVisible;
                if (IsVisible)
                {
                    // In het geval van static texten, deze tekst "verversen" zodat het "LiveUpdate" effect ook zijn werk kan doen.
                    __errorLabel__.Text = __errorLabel__.Text.ChangeForA11y();
                }
            }
        }
    }
}
