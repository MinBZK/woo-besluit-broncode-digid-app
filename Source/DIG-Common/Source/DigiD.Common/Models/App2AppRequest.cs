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
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using DigiD.Common.Helpers;

namespace DigiD.Common.Models
{
    public class App2AppRequest
    {
        public string Icon { get; set; }
        public string ReturnUrl { get; set; }
        public string Destination { get; private set; }
        public string Host { get; set; }
        public string SAMLRequest { get; set; }
        public string RelayState { get; set; }
        public string SigAlg { get; set; }
        public string Signature { get; set; }
        public DateTimeOffset IssueInstant { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(SAMLRequest))
                return false;

            try
            {
                var utf8 = Encoding.UTF8;
                var bytes = Convert.FromBase64String(SAMLRequest);

                string xml;

                using (var output = new MemoryStream())
                {
                    using (var input = new MemoryStream(bytes))
                    {
                        try
                        {
                            using (var unzip = new DeflateStream(input, CompressionMode.Decompress))
                            {
                                unzip.CopyTo(output, bytes.Length);
                                unzip.Close();
                            }

                            //Supplied AuthNRequest is inflated, and XML has ben extracted and parsed
                            xml = utf8.GetString(output.ToArray());
                        }
                        catch (Exception ex)
                        {
                            if (ex is InvalidDataException || ex is IOException)
                            {
                                //Supplied AuthNRequest was plain XML and will be parsed
                                xml = utf8.GetString(bytes);
                            }
                            else
                            {
                                //Supplied AuthNRequest was in an unknown format, and can't be parsed
                                AppCenterHelper.TrackError(ex);
                                return false;
                            }
                        }
                    }
                }

                var doc = new XmlDocument();
                doc.LoadXml(xml);
                Destination = doc.DocumentElement?.Attributes["Destination"]?.Value;
                IssueInstant = DateTimeOffset.Parse(doc.DocumentElement?.Attributes["IssueInstant"]?.Value);

                return !string.IsNullOrEmpty(Destination);
            }
            catch (Exception e)
            {
                AppCenterHelper.TrackError(e);
                return false;
            }
        }
    }
}
