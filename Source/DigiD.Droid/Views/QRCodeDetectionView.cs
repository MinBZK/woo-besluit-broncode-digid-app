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
using System.Threading.Tasks;
using System.Windows.Input;
using Android.Content;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Android.Gms.Vision.Detector;
using ScrollView = Android.Widget.ScrollView;
using View = Android.Views.View;

namespace DigiD.Droid.Views
{
    internal class QRCodeDetectionView : FrameLayout, ISurfaceHolderCallback, IProcessor
    {
        public QRCodeDetectionView(Context context) : base(context)
        {
            var inflater = LayoutInflater.FromContext(context);

            if (inflater == null) return;
            _view = inflater.Inflate(DigiD.Droid.Resource.Layout.VisionLayout, this);

            if (_view != null)
            {
                _horizontalScrollView = _view.FindViewById<HorizontalScrollView>(DigiD.Droid.Resource.Id.horizontalScrollView);
                _scrollView = _view.FindViewById<ScrollView>(DigiD.Droid.Resource.Id.scrollView);
                _cameraView = _view.FindViewById<SurfaceView>(DigiD.Droid.Resource.Id.cameraSurface);
            }

            CreateCameraSource();
        }

        private readonly View _view;
        private readonly HorizontalScrollView _horizontalScrollView;
        private readonly ScrollView _scrollView;
        private readonly SurfaceView _cameraView;
        private CameraSource _cameraSource;
        private bool _ready;
        internal Func<string, Task> ObjectCaptured { get;set; }
        private bool _started;

        public void ReceiveDetections(Detections detections)
        {
            if (_ready) return;

            if (detections == null) return;

            var detectedItems = detections.DetectedItems;
            if (detectedItems.Size() > 0)
            {
                HandleDetectedItems(detectedItems);
            }
        }

        public void Release()
        {
            //No implementation needed
        }

        public ICommand StopCommand => new Command(StopCamera);

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            //No implementation needed
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (_cameraSource == null)
                CreateCameraSource();

            StartCamera();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            if (_cameraSource != null)
            {
                _cameraSource.Release();
                _cameraSource = null;
            }
        }

        protected void StopCamera()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (_cameraSource == null)
                    return;

                _cameraSource.Stop();
                _cameraSource.Release();
                _cameraSource = null;
            });
        }

        private void CreateCameraSource()
        {
            var detector = CreateDetector();

            _cameraSource = new CameraSource.Builder(Context, detector)
                    .SetFacing(CameraFacing.Back)
                    .SetRequestedFps(30)
                    .SetRequestedPreviewSize(1920, 1080)
                    .SetAutoFocusEnabled(true)
                    .Build();

            _cameraView?.Holder?.AddCallback(this);

            _horizontalScrollView.LayoutChange += (sender, e) =>
            {
                if (!_started) return;
                if (_ready) return;

                _cameraSource?.Stop();
                try
                {
                    _cameraSource?.Start(_cameraView.Holder);
                }
                catch (Exception ex)
                {
                    Log.Error("StartCamera", "Unable to start camera source.", ex);
                    _cameraSource?.Release();
                    _cameraSource = null;
                }

                UpdateCameraViewHolder();
            };
        }

        private void StartCamera()
        {
            if (_cameraSource == null) return;

            try
            {
                _cameraSource.Start(_cameraView.Holder);

                UpdateCameraViewHolder();

                _started = true;
            }
            catch (Exception e)
            {
                Log.Error("StartCamera", "Unable to start camera source.", e);
                _cameraSource.Release();
                _cameraSource = null;
            }
        }

        private void UpdateCameraViewHolder()
        {
            var longSide = Math.Max(_view.Width, _view.Height);
            var smallSide = Math.Min(_view.Width, _view.Height);

            var smallSideFromLongSide = longSide / 16 * 9;
            var longSideFromSmallSide = smallSide / 9 * 16;

            if (longSide < longSideFromSmallSide)
                longSide = longSideFromSmallSide;
            else
                smallSide = smallSideFromLongSide;

            var portrait = _view.Width < _view.Height;
            var cameraViewWidth = portrait ? smallSide : longSide;
            var cameraViewHeight = portrait ? longSide : smallSide;

            var scrollX = (_scrollView.Width - _horizontalScrollView.Width) / 2;
            var scrollY = (_cameraView.Height - _scrollView.Height) / 2;

            // Scroll de _scrollView horizontaal in de _horizontalScrollView nadat die de
            // berekende breedte heeft aangenomen die we met Holder.SetFixedSize hebben geset.
            if (cameraViewWidth == _scrollView.Width && _horizontalScrollView.ScrollX != scrollX)
                _horizontalScrollView.ScrollX = scrollX;

            // Scroll de _cameraView verticaal in de _scrollView nadat die de berekende
            // hoogte heeft aangenomen die we met Holder.SetFixedSize hebben geset.
            if (cameraViewHeight == _cameraView.Height && _scrollView.ScrollY != scrollY)
                _scrollView.ScrollY = scrollY;

            if (_cameraView.Width != cameraViewWidth || _cameraView.Height != cameraViewHeight)
                _cameraView?.Holder?.SetFixedSize(cameraViewWidth, cameraViewHeight);
        }

        protected async void HandleDetectedItems(SparseArray detectedItems)
        {
            var qrCode = ((Barcode)detectedItems.ValueAt(0)).RawValue;

            _ready = true;
            StopCamera();

            await ObjectCaptured.Invoke(qrCode);
        }

        protected Detector CreateDetector()
        {
            var barcodeDetector = new BarcodeDetector.Builder(Context)
                .SetBarcodeFormats(BarcodeFormat.QrCode)
                .Build();

            barcodeDetector.SetProcessor(this);

            if (!barcodeDetector.IsOperational)
            {
                // Note: The first time that an app using a Vision API is installed on a
                // device, GMS will download a native libraries to the device in order to do detection.
                // Usually this completes before the app is run for the first time.  But if that
                // download has not yet completed, then the above call will not detect any text,
                // barcodes, or faces.
                //
                // isOperational() can be used to check if the required native libraries are currently
                // available.  The detectors will automatically become operational once the library
                // downloads complete on device.
                Log.Error("Main Activity", "Detector dependancies are not yet available");
            }

            return barcodeDetector;
        }
    }
}
