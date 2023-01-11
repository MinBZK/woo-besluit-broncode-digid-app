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
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DigiD.Common.Helpers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "<Pending>")]
    public sealed class Timer
    {
        private readonly TimeSpan _timeSpan;
        private readonly Func<Task> _callback;
        private readonly bool _continuous;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public Timer(TimeSpan timeSpan, Func<Task> callback, bool continuous = true)
        {
            _timeSpan = timeSpan;
            _callback = callback;
            _continuous = continuous;
        }

        public void Start()
        {
            var cts = _cancellationTokenSource; // safe copy
            
            Device.StartTimer(_timeSpan, () =>
            {
                if (cts.IsCancellationRequested)
                {
                    return false;
                }

                AsyncUtil.RunSync(_callback);
                return _continuous; //true to continuous, false to single use
            });
        }

        public void Stop()
        {
            Interlocked.Exchange(ref _cancellationTokenSource, new CancellationTokenSource()).Cancel();
        }
    }
}
