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
using System.Linq;
using Belastingdienst.MCC.TestAAP.Commons;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace DigiD.UITests.Pageobjects
{
    public class GegevensInvullenPagina_AP109 : Pageobject<GegevensInvullenPagina_AP109>
    {
        private const string _title = "Gegevens invullen";
        private const string _waitText = "Vul de gegevens in van het identiteitsbewijs dat u wilt scannen.";
        private Query _radioButtonPaspoort = x => x.Marked("Paspoort");
        private Query _radioButtonIdentiteitskaart = x => x.Marked("Identiteitskaart");
        private const string _volgendeButton = "Volgende";
        private Query _invulveldDocumentnummer = x => x.Marked("DocumentNumber");
        private Query _invulveldDocumentGeldigTot = x => x.Marked("ExpiryDate");
        private Query _invulveldGeboortedatum = x => x.Marked("DateOfBirth");
        
        
        private GegevensInvullenPagina_AP109(string title) : base(title, _waitText) { }

        public static GegevensInvullenPagina_AP109 Load(string title = _title)
            => new GegevensInvullenPagina_AP109(title);

        public GegevensInvullenPagina_AP109 ControleerOfJuisteTekstWordtGetoond()
           => WaitForElementToAppear(_title);

        public GegevensInvullenPagina_AP109 ActiveerKnopVolgende()
           => TapOn(_volgendeButton);

        public GegevensInvullenPagina_AP109 GegevensInvullenPaspoort()
             => TapOn(_radioButtonPaspoort);

        public GegevensInvullenPagina_AP109 GegevensInvullenIdentiteitskaart()
             => TapOn(_radioButtonIdentiteitskaart);

        public GegevensInvullenPagina_AP109 InvullenDocumentnummer(string documentnummer)
            => WaitForElementToAppear(_invulveldDocumentnummer)
               .Tap(_invulveldDocumentnummer)
                .EnterText(documentnummer);

        public GegevensInvullenPagina_AP109 InvullenDocumentGeldigTot(string geldigtotdatum)
            => WaitForElementToAppear(_invulveldDocumentGeldigTot)
               .Tap(_invulveldDocumentGeldigTot)
                .EnterText(geldigtotdatum);

        public GegevensInvullenPagina_AP109 InvullenGebDatum(string geboortedatum)
            => WaitForElementToAppear(_invulveldGeboortedatum)
               .Tap(_invulveldGeboortedatum)
                .EnterText(geboortedatum);

        

                
    }
}


