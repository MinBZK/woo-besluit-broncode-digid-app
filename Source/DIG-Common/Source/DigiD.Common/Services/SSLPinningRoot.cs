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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using DigiD.Common.Constants;
using DigiD.Common.Helpers;
using DigiD.Common.NFC.Helpers;

namespace DigiD.Common.Services
{
    public class SslPinningRoot : ISslPinningService
    {
        public bool ValidateCertificate(string domain, List<X509Certificate> certificates)
        {
            var rootPins = new List<string>
            {
                "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS", //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
                "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS" //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
            };
            
            if (AppConfigConstants.Exclusions.Contains(domain))
            {
                Debugger.WriteLine($"SSL Pinning ignored for {domain}");
                return true;
            }

            if (certificates == null || certificates.Count == 0)
            {
                Debugger.WriteLine("Could not validate SSL for because no certificates was given");
                return false;
            }

            //get root certificate from chain where issuer is the same as subject
            var rootCertificate = certificates.FirstOrDefault(x => x.Issuer == x.Subject);
            var leaf = certificates.FirstOrDefault(x => x.GetSubjectName().Equals(domain, StringComparison.InvariantCultureIgnoreCase));

            if (rootCertificate == null)
            {
#if DEBUG || TST || ACC
                Debugger.WriteLine("No root certificate found");
                return true;
#endif
                return false;
            }

            if (rootCertificate.IsExpired())
            {
                Debugger.WriteLine($"SSL pinning thumbprint is NOT valid for '{domain}' because root certificate is expired");
                return false;
            }

            if (leaf == null)
            {
#if DEBUG || TEST || ACC
                //This can happen with a wildcard certificate
                //Subtract the domain from the alternative names
                leaf = certificates.FirstOrDefault(x => x.GetSubjectAlternativeNames().Any(y => y == domain));
#endif

                if (leaf == null)
                {
                    Debugger.WriteLine("No expected leaf certificate found");
                    return false;
                }
            }
            
            if (leaf.IsExpired())
            {
                Debugger.WriteLine($"SSL pinning thumbprint is NOT valid for '{domain}' because certificate is expired");
                return false;
            }

            using (var sha256 = new SHA256Managed())
            {
                var hash = sha256.ComputeHash(rootCertificate.GetPublicKey());
                var thumbprint = Convert.ToBase64String(hash);

                if (rootPins.All(x => x != thumbprint))
                {
                    Debugger.WriteLine($"SSL pinning thumbprint for root is NOT valid for '{domain}'");
                    return false;
                }

                Debugger.WriteLine($"SSL pinning thumbprint is valid for '{domain}'");
                return true;
            }
        }
    }
}
