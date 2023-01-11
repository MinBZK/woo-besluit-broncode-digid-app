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
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using DigiD.Common.EID.Helpers;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Exceptions;
using DigiD.Common.NFC.Interfaces;
using DigiD.Common.NFC.Models;
using DigiD.Droid.Helpers;
using DigiD.Droid.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(NfcService))]
namespace DigiD.Droid.Services
{
    internal class ReaderCallBack : Java.Lang.Object, NfcAdapter.IReaderCallback
    {
        public void OnTagDiscovered(Tag tag)
        {
            var intent = new Intent(NfcConstants.ACTION_NFC);
            intent.PutExtra(NfcAdapter.ExtraTag, tag);
            Xamarin.Essentials.Platform.CurrentActivity.SendBroadcast(intent);
        }
    }

    public class NfcService : INfcService
    {
        private bool _receiverRegistered;
        private IsoDep _isoDepTag;
        private Func<Task> _tagFound;
        private readonly NfcBroadCastReceiver _broadCastReceiver;

        public NfcService()
        {
            _broadCastReceiver = new NfcBroadCastReceiver(async tag =>
            {
                try
                {
                    _isoDepTag = IsoDep.Get(tag);
                    _isoDepTag?.Connect();
                    _isoDepTag?.SetTimeout(20000);
                    await _tagFound.Invoke();
                }
                catch (Exception)
                {
                    //this exception will handled in a later stadium
                }
            });
        }

        public void SetNfCardPresenceCheckTimeout()
        {
            var options = new Bundle();
            options.PutInt(NfcAdapter.ExtraReaderPresenceCheckDelay, 2000);
            NfcHelper.NFCAdapter?.EnableReaderMode(_currentActivity, new ReaderCallBack(), NfcReaderFlags.NfcA | NfcReaderFlags.NfcB | NfcReaderFlags.NfcF | NfcReaderFlags.NfcV | NfcReaderFlags.NoPlatformSounds, options);
        }

        public Task<bool> HasNFCSupport()
        {
#if TESTCLOUD
            return Task.FromResult(false);
#else
            return Task.FromResult(NfcHelper.NFCAdapter != null);
#endif
        }

        public Task<byte[]> GetATR()
        {
            return Task.FromResult(_isoDepTag.GetHistoricalBytes());
        }

        public bool IsCancelled { get; set; }
        public bool IsNFCEnabled => NfcHelper.NFCAdapter?.IsEnabled == true;

        public void Disconnect()
        {
            _isoDepTag?.Close();
        }

        public void UpdateStatus(double percentage)
        {
            //No implementation needed for Android
        }

        public Task<string[]> GetReaders()
        {
            return Task.FromResult(new[] { "Android NFC" });
        }

        public Task<bool> IsTagConnected()
        {
            return Task.FromResult(_isoDepTag is {IsConnected: true});
        }

        public void OpenNFCSettings()
        {
            NfcHelper.OpenSettings();
        }

        private Activity _currentActivity;

        public Task<bool> StartScanningAsync(bool retry, Func<Task> tagFoundAction, Func<NfcError, Task> errorAction)
        {
            if (_receiverRegistered)
                return Task.FromResult(true);


            _tagFound = tagFoundAction;
            _currentActivity = Xamarin.Essentials.Platform.CurrentActivity;
            _currentActivity.RegisterReceiver(_broadCastReceiver, new IntentFilter(NfcConstants.ACTION_NFC));

            EnableForegroundDispatch();
            SetNfCardPresenceCheckTimeout();

            _receiverRegistered = true;

            return Task.FromResult(true);
        }

        public Task StopScanningAsync(string message = null)
        {
            if (_receiverRegistered)
            {
                if (!_currentActivity.IsDestroyed)
                {
                    _currentActivity.UnregisterReceiver(_broadCastReceiver);
                    NfcHelper.NFCAdapter?.DisableReaderMode(_currentActivity);
                    DisableForegroundDispatch();
                }

                _receiverRegistered = false;
            }

            _isoDepTag?.Close();
            _isoDepTag = null;
            return Task.CompletedTask;
        }

        public async Task<TransceiveResult> SendApduAsync(byte[] command)
        {
            var result = new TransceiveResult();

            if (IsCancelled)
            {
                result.Exception = new TaskCanceledException();
                return result;
            }

            try
            {
                var data = await _isoDepTag.TransceiveAsync(command);
                result.Data = CleanData(data);
                return result;
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case TagLostException _:
                        result.Exception = new CardLostException();
                        break;
                    default:
                        result.Exception = ex;
                        break;
                }

                return result;
            }
        }

        public void Reset()
        {
            if (_isoDepTag.IsConnected)
                _isoDepTag.Close();
        }

        /// <summary>
        /// Will strip the first tag and length of the incomming byte[]
        /// </summary>
        /// <param name="bytes">Original bytes</param>
        /// <returns>Clean bytes</returns>
        private static byte[] CleanData(byte[] bytes)
        {
            if (bytes[0] == 0x53)
            {
                var length = ByteHelper.GetLength(bytes);
                var data = bytes.Skip(length.Item2).ToArray();
                return data;
            }

            return bytes;
        }

        //this method disables the NFC foreground dispatch, making the NFC events available to other applications
        private void DisableForegroundDispatch()
        {
            if (MainActivity.IsAppVisible)
                NfcHelper.NFCAdapter?.DisableForegroundDispatch(_currentActivity);
        }

        //this method enables the NFC foreground dispatch - i.e. all the NFC events will go directly to this activity
        private void EnableForegroundDispatch()
        {
            var localIntent = new Intent(NfcConstants.ACTION_NFC);

            localIntent.SetPackage(_currentActivity.PackageName);
            localIntent.SetFlags(ActivityFlags.ReceiverRegisteredOnly);
            try
            {
                NfcHelper.NFCAdapter.EnableForegroundDispatch(
                    _currentActivity,
                    PendingIntent.GetBroadcast(_currentActivity, 0, localIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable),
                    new[] { new IntentFilter(NfcAdapter.ActionTechDiscovered) },
                    new[] { new[]{ NfcConstants.SUPPORTED_NFC }
                    });
            }
            catch (Java.Lang.IllegalStateException)
            {
                //Nothing to do with this exception
            }
        }
    }
}
