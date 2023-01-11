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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DigiD.Common.Helpers;
using DigiD.Common.Models.Piwik;
using Xamarin.Forms;

namespace DigiD.Common.Services.Piwik
{
    /// <summary>
    /// Piwik tracker. Met deze class kun je gegevens aan Piwik aanleveren. Gebruik de Configure methode om
    /// de service te intialiseren. Await de task voordat je gegevens verstuurt. Met de twee <see cref="Track">Track</see>
    /// methoden kun je gegevens klaarzetten voor verzending. Met <see cref="Submit"/> kun je de gegevens direct versturen,
    /// anders wordt dat automatisch om de 30 seconden gedaan.  
    /// </summary>
    public class PiwikTracker
    {
        private static PiwikTracker _tracker;

        private readonly IPiwikDispatcher _dispatcher;
        private const int NumberOfEventsDispatchedAtOnce = 20;
        private const int DefaultAutoSendTimerSeconds = 30;

        // state helpers
        private static bool _isInitialized;
        private bool _isDispatching;
        private bool _nextEventStartsANewSession = true;
        private string SiteId => DependencyService.Get<IDevice>().PiwikId.ToString(CultureInfo.InvariantCulture);

        private readonly Session _session;
        private static readonly Device Device = new Device();

        private readonly Queue<PiwikEvent> _queue = new Queue<PiwikEvent>();


        #region Constructors
        private PiwikTracker(string piwikUrl, TimeSpan autoSendTimer = default) : this(new HttpDispatcher(piwikUrl), autoSendTimer)
        {
        }

        private PiwikTracker(IPiwikDispatcher dispatcher, TimeSpan autoSendTimer = default)
        {
            _dispatcher = dispatcher;
            _session = new Session();

            var delayTime = autoSendTimer == default ? DefaultAutoSendTimerSeconds * 1000 : autoSendTimer.TotalMilliseconds;
#pragma warning disable CA2000 // Dispose objects before losing scope
            var dispatchTimer = new System.Timers.Timer(delayTime) { AutoReset = true, Enabled = true };
#pragma warning restore CA2000 // Dispose objects before losing scope
            dispatchTimer.Elapsed += (sender, e) => Task.Run(StartDispatch);
        }

        /// <summary>
        /// Configure the specified piwikUrl and piwikSiteId.
        /// </summary>
        /// <returns>The configure.</returns>
        /// <param name="piwikUrl">Piwik URL.</param>
        /// <param name="autoSendTimer">Tijd tussen automatisch verzenden. Default is 30 seconden.</param>

        public static async Task Configure(string piwikUrl, TimeSpan autoSendTimer = default(TimeSpan))
        {
            Device.UserAgent = await DependencyService.Get<IDevice>().UserAgent();
            Device.ScreenSize =  DisplayHelper.ScreenSize;

            _tracker = new PiwikTracker(new HttpDispatcher(piwikUrl), autoSendTimer);
            _isInitialized = true;
            _tracker._dispatcher.ScreenSize = Device.ScreenSize;
            _tracker._dispatcher.UserAgent = Device.UserAgent;
        }

        #endregion

        #region User functionality

        /// <summary>
        /// Track a single screen.
        /// </summary>
        /// <returns>The track.</returns>
        /// <param name="screenName">Screen name.</param>
        /// <param name="viewModel">Name of the calling viewmodel</param>
        public static void Track(string screenName, string viewModel)
        {
            Track(new[] { screenName }, viewModel);
        }

        /// <summary>
        /// Track the specified screenName.
        /// </summary>
        /// <returns>The track.</returns>
        /// <param name="screenName">Screen names. This list will be concatenated to a space delimitered string</param>
        /// <param name="viewModel"></param>
        public static void Track(string[] screenName, string viewModel)
        {
            if (!_isInitialized) return;
            _tracker.TrackEvent(screenName, viewModel);
        }

        /// <summary>
        /// Track the specified category, action, name and value.
        /// </summary>
        /// <returns>The track.</returns>
        /// <param name="category">Category.</param>
        /// <param name="action">Action.</param>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public static void Track(string category, string action, string name, float? value = null)
        {
            if (!_isInitialized) return;
            _tracker.TrackEvent(category, action, name, value);

        }

        /// <summary>
        /// Perform a send now.
        /// </summary>
        /// <returns>The submit.</returns>
        public static async Task Submit()
        {
            if (!_isInitialized) return;
            await _tracker.StartDispatch();
        }
        #endregion

        private void TrackEvent(string category, string action, string name = null, float? value = null)
        {
            var eventItem = new PiwikEvent
            {
                SiteID = SiteId,
                Session = _session,
                EventCategory = category,
                EventName = name,
                EventAction = action,
                EventValue = value.ToString(),
                Language = CultureInfo.DefaultThreadCurrentCulture.IetfLanguageTag,
                IsNewSession = _nextEventStartsANewSession
            };

            Enqueue(eventItem);
        }

        private void TrackEvent(string[] screenName, string viewModel)
        {
            var eventItem = new PiwikEvent
            {
                SiteID = SiteId,
                Session = _session,
                ActionName = screenName,
                Language = CultureInfo.DefaultThreadCurrentCulture.IetfLanguageTag,
                IsNewSession = _nextEventStartsANewSession
            };

            eventItem.Url += viewModel;

            Enqueue(eventItem);
        }

        private void Enqueue(PiwikEvent piwikEventItem)
        {
            lock (_queue)
            {
                _queue.Enqueue(piwikEventItem);
                _nextEventStartsANewSession = false;
            }
        }

        /// <summary>
        /// Start dispatching the queued events, if there are any.
        /// </summary>
        /// <returns>The dispatch.</returns>
        private async Task StartDispatch()
        {
            if (!_isInitialized || _isDispatching) return;
            try
            {
                _isDispatching = true;
                IEnumerable<PiwikEvent> toSend;
                lock (_queue)
                {
                    toSend = _queue.Take(NumberOfEventsDispatchedAtOnce).ToList();
                }

                if (toSend.Any() && await _dispatcher.Send(toSend))
                {
                    lock (_queue)
                    {
                        foreach (var unused in toSend)
                            _queue.Dequeue();
                    }
                    // gelukt, dus misschien nog een rondje doen als ie vol is
                    if (_queue.Count >= NumberOfEventsDispatchedAtOnce)
                    {
                        _isDispatching = false;
                        await StartDispatch();
                    }
                }
            }
            finally
            {
                _isDispatching = false;
            }
        }


    }
}
