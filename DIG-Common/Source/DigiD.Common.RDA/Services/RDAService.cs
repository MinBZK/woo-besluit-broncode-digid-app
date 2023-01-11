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
using System.Threading.Tasks;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.CardFiles;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Exceptions;
using DigiD.Common.NFC.Interfaces;
using DigiD.Common.NFC.Models;
using DigiD.Common.RDA.Enums;
using DigiD.Common.SessionModels;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace DigiD.Common.RDA.Services
{
    internal class RdaService
    {
        private readonly INfcService _service;
        private readonly Func<RdaStatus, Task> _statusChanged;
        private int _count;
        private readonly Action<double> _progressChanged;
        private bool _cardLost;
        public bool IsReading { get; private set; }

        internal RdaService(Func<RdaStatus, Task> statusChanged, Action<double> progressChanged)
        {
            _service = DependencyService.Get<INfcService>();
            _statusChanged = statusChanged;
            _progressChanged = progressChanged;
        }

        internal async Task StartScanning(bool retry)
        {
            var success = false;

            if (await _service.IsTagConnected() && !_cardLost)
            {
                await StartReading();
                return;
            }

            while (!success)
            {
                success = await _service.StartScanningAsync(retry, StartReading, HandleError);
                await Task.Delay(200);
            }
        }

        private async Task HandleError(NfcError e)
        {
            switch (e)
            {
                case NfcError.Cancelled:
                    await _statusChanged.Invoke(RdaStatus.ReadingCancelled);
                    break;
                case NfcError.Timeout:
                    await _statusChanged.Invoke(RdaStatus.ReadingFailed);
                    break;
            }
        }

        private async Task StartReading()
        {
            IsReading = true;
            _cardLost = false;
            await _statusChanged.Invoke(RdaStatus.ReadingStarted);

            try
            {
                var result = await StartSession();
                IsReading = false;

                if (result == RdaStatus.CardLost)
                    _cardLost = true;

                if (_cardLost && Device.RuntimePlatform == Device.iOS)
                    await _statusChanged.Invoke(result);
                else
                {
                    await _service.StopScanningAsync(result != RdaStatus.ReadingCompleted ? AppResources.Failed : null);
                    await _statusChanged.Invoke(result);
                }
            }
            catch (Exception e)
            {
                if (!(e is CardLostException))
                    Crashes.TrackError(e);

                await _service.StopScanningAsync(AppResources.Failed);
                await _statusChanged.Invoke(RdaStatus.ReadingFailed);
            }
        }

        private async Task<RdaStatus> StartSession()
        {
            _progressChanged.Invoke(1d / 8d);

            var atr = await DependencyService.Get<INfcService>().GetATR();
            var card = CardHelper.GetCardByATR(atr);

            if (card == null)
            {
                return RdaStatus.UnknownCard;
            }

            //Select AID
            var cmd = new CommandApdu(CLA.PLAIN, INS.SELECT_FILE, (int)P1.APPLICATION_ID, (int)P2.NONE, card.CardAID);
            await _service.SendApduAsync(cmd.Bytes);
            
            //SELECT MF
            cmd = new CommandApdu(CLA.PLAIN, INS.SELECT_FILE, 0x00, card.DocumentType == DocumentType.IDCard ? 0x0C : 0x00, null, 0);
            await _service.SendApduAsync(cmd.Bytes);

            var efCardAccess = await SelectAndReadFile.Execute<EFCardAccess>(CardFile.EFCardAccess);

            BaseResponse response = await DependencyService.Get<IRdaServices>().Start(card.RDADocumentType, efCardAccess);

            if (response.ApiResult != ApiResult.Ok)
                return RdaStatus.UnknownError;

            if (efCardAccess == null || response.Status == "CHALLENGE")
            {
                //BAC RDA
                return await ExecuteRdaWithBac(response);
            }
            else
            {
                //PACE RDA
                return await ExecuteRdaWithPace(response);
            }
        }

        private async Task<RdaStatus> ExecuteRdaWithBac(BaseResponse response)
        {
            _cardLost = false;
            CommandApdu challengeCommand = null;
            var next = false;
            _count = 0;

            while (!next && response.ApiResult == ApiResult.Ok || response.ApiResult == ApiResult.Failed || response.ApiResult == ApiResult.Verified)
            {
                TransceiveResult data = null;
                CommandApdu commandApdu;
                ResponseApdu responseApdu = null;

                async Task ExecuteCommand(byte[] bytes)
                {
                    commandApdu = new CommandApdu(bytes);
                    data = await _service.SendApduAsync(commandApdu.Bytes);

                    if (data == null || data.IsCardLost)
                        throw new CardLostException();

                    responseApdu = new ResponseApdu(data.Data);
                }

                try
                {
                    switch (response.Status)
                    {
                        case "CHALLENGE":
                            {
                                var startResponse = (RdaStartResponse)response;
                                await ExecuteCommand(startResponse.Select);

                                if (responseApdu?.SW == 0x9000)
                                {
                                    //store the challenge command for possible later use
                                    challengeCommand = new CommandApdu(Convert.FromBase64String(((RdaStartResponse)response).Challenge));
                                    data = await _service.SendApduAsync(challengeCommand.Bytes);
                                    response = await DependencyService.Get<IRdaServices>().Challenge(Convert.ToBase64String(data.Data));
                                    _progressChanged.Invoke(2d / 8d);
                                }
                                else
                                    next = true;
                            }
                            break;
                        case "AUTHENTICATE":
                            {
                                var command = response.GetType() == typeof(SelectResponse)
                                    ? ((SelectResponse)response).Authenticate
                                    : ((CommandResponse)response).Authenticate;

                                await ExecuteCommand(Convert.FromBase64String(command));

                                if (responseApdu?.SW == 0x9000)
                                {
                                    response = await DependencyService.Get<IRdaServices>()
                                        .Authenticate(Convert.ToBase64String(data.Data), null);
                                    _progressChanged.Invoke(3d / 8d);
                                }
                                else
                                {
                                    //re-execute the challenge command in case the previous command has failed
                                    if (challengeCommand == null)
                                        return RdaStatus.UnknownError;

                                    data = await _service.SendApduAsync(challengeCommand.Bytes);
                                    response = await DependencyService.Get<IRdaServices>()
                                        .Authenticate(null, Convert.ToBase64String(data.Data));

                                    if (response.Status == "FAILED")
                                    {
                                        HttpSession.RDASessionData = null;
                                        return RdaStatus.AuthenticationFailed;
                                    }
                                }
                            }
                            break;
                        case "SECURE_MESSAGING":
                            {
                                if (!(response is CommandResponse commands))
                                    return RdaStatus.UnknownError;

                                _count += 1;

                                var sub = _count / 9d;
                                _progressChanged.Invoke(3d / 8d + ((5d / 8d) * sub));

                                var responseCommands = new List<string>();
                                foreach (var command in commands.Commands)
                                {
                                    await ExecuteCommand(Convert.FromBase64String(command));

                                    responseCommands.Add(Convert.ToBase64String(data.Data));

                                    if (responseApdu.SW != 0x9000)
                                        break;
                                }

                                response = await DependencyService.Get<IRdaServices>().SecureMessaging(responseCommands);
                            }
                            break;
                        default:
                            return HandleExceptions(response.Status);
                    }
                }
                catch (CardLostException)
                {
                    return RdaStatus.CardLost;
                }
                catch
                {
                    return RdaStatus.AuthenticationFailed;
                }
            }

            return RdaStatus.AuthenticationFailed;
        }

        private async Task<RdaStatus> ExecuteRdaWithPace(BaseResponse response)
        {
            _cardLost = false;
            _count = 0;
            var startResponse = (RdaStartResponse)response;

            while (response.ApiResult == ApiResult.Ok || response.ApiResult == ApiResult.Failed || response.ApiResult == ApiResult.Verified)
            {
                TransceiveResult data = null;
                CommandApdu commandApdu;
                ResponseApdu responseApdu = null;

                async Task Map(byte[] mappedNonce)
                {
                    await ExecuteCommand(mappedNonce);
                    response = await DependencyService.Get<IRdaServices>().Map(responseApdu.Bytes);
                    _progressChanged.Invoke(2d / 8d);
                }

                async Task ExecuteCommand(byte[] bytes)
                {
                    commandApdu = new CommandApdu(bytes);
                    data = await _service.SendApduAsync(commandApdu.Bytes);

                    if (data == null || data.IsCardLost)
                        throw new CardLostException();

                    responseApdu = new ResponseApdu(data.Data);
                }

                try
                {
                    switch (response.Status)
                    {
                        case "PREPARE":
                            {
                                if (startResponse.Select != null) //Select will be NULL for drivers license
                                    await ExecuteCommand(startResponse.Select);

                                if (startResponse.Select == null || responseApdu?.SW == 0x9000)
                                {
                                    await ExecuteCommand(startResponse.Pace);

                                    if (responseApdu?.SW == 0x9000)
                                    {
                                        await ExecuteCommand(startResponse.Nonce);
                                        response = await DependencyService.Get<IRdaServices>().Prepare(responseApdu.Bytes);
                                    }
                                }

                                _progressChanged.Invoke(1d / 8d);
                                break;
                            }
                        case "MAP" when response is MutualAuthResponseModel model:
                            {
                                await Map(model.MappedNonce);
                                break;
                            }
                        case "MAP" when response is PrepareResponseModel model:
                            {
                                await Map(model.MappedNonce);
                                break;
                            }
                        case "KEY_AGREE" when response is MapResponseModel mapResponse:
                            {
                                await ExecuteCommand(mapResponse.KeyAgree);
                                response = await DependencyService.Get<IRdaServices>().KeyAgreement(responseApdu.Bytes);
                                _progressChanged.Invoke(3d / 8d);
                                break;
                            }
                        case "MUTUAL_AUTH" when response is AgreeKeyResponseModel agreeKeyResponse:
                            {
                                await ExecuteCommand(agreeKeyResponse.Token);

                                if (responseApdu?.SW == 0x9000)
                                    response = await DependencyService.Get<IRdaServices>()
                                        .MutualAuthenticate(responseApdu.Bytes);
                                else
                                {
                                    //if card is not corresponding to the records in Kern, try the next one, start pace, and generate new encrypted nonce, restart the session
                                    await ExecuteCommand(startResponse.Pace);

                                    if (responseApdu?.SW == 0x9000)
                                    {
                                        await ExecuteCommand(startResponse.Nonce);
                                        response = await DependencyService.Get<IRdaServices>().MutualAuthenticate(null, responseApdu.Bytes);

                                        if (response.Status == "FAILED")
                                        {
                                            HttpSession.RDASessionData = null;
                                            return RdaStatus.AuthenticationFailed;
                                        }

                                        _progressChanged.Invoke(1d / 8d);
                                    }
                                }

                                break;
                            }
                        case "SECURE_MESSAGING":
                            {
                                if (!(response is CommandResponse commands))
                                    return RdaStatus.UnknownError;

                                _count += 1;

                                var sub = _count / 9d;
                                _progressChanged.Invoke(3d / 8d + ((5d / 8d) * sub));

                                var responseCommands = new List<string>();
                                foreach (var command in commands.Commands)
                                {
                                    await ExecuteCommand(Convert.FromBase64String(command));

                                    responseCommands.Add(Convert.ToBase64String(data.Data));

                                    if (responseApdu.SW != 0x9000)
                                        break;
                                }

                                response = await DependencyService.Get<IRdaServices>().SecureMessaging(responseCommands);
                            }
                            break;
                        default:
                            return HandleExceptions(response.Status);
                    }
                }
                catch (CardLostException)
                {
                    return RdaStatus.CardLost;
                }
                catch
                {
                    return RdaStatus.AuthenticationFailed;
                }
            }

            return RdaStatus.AuthenticationFailed;
        }

        private RdaStatus HandleExceptions(string status)
        {
            switch (status)
            {
                case "FAILED":
                    return RdaStatus.ReadingFailed;
                case "CANCELLED":
                    return RdaStatus.ReadingCancelled;
                case "ABORTED":
                    return RdaStatus.ReadingAborted;
                case "VERIFIED":
                    _progressChanged.Invoke(1);
                    return RdaStatus.ReadingCompleted;
                default:
                    return RdaStatus.UnknownError;
            }
        }
    }
}
