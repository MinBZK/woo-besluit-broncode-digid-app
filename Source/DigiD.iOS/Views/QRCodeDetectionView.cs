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
using AVFoundation;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace DigiD.iOS.Views
{
    [Register("QRCodeDetectionView")]
    public class QRCodeDetectionView : UIView
    {
        private AVCaptureVideoPreviewLayer _previewLayer;
        private AVCaptureSession _session;
        private MetadataObjectsDelegate _metadataObjectsDelegate;

        public Action<string> ObjectCaptured { get; set; }
        
        public QRCodeDetectionView()
        {
            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _metadataObjectsDelegate?.Dispose();
            _previewLayer?.Dispose();
            _session?.Dispose();
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            if (_previewLayer != null)
            {
                _previewLayer.Frame = new CGRect(0, 0, rect.Width, rect.Height);
            }
        }

        public override void LayoutSublayersOfLayer(CALayer layer)
        {
            base.LayoutSublayersOfLayer(layer);
            if (_previewLayer != null)
            {
                _previewLayer.Frame = Bounds;
            }
        }


        private void Initialize()
        {
            this.BackgroundColor = UIColor.Black;
            GetCameraPreview();
        }

        private void GetCameraPreview()
        {
            // Configure the AV input device
            var camera = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);

            if (camera == null)
                return;

            if (camera.LockForConfiguration(out _))
            {
                var ranges = camera.ActiveFormat.VideoSupportedFrameRateRanges;
                if (ranges.Length > 0)
                    camera.ActiveVideoMinFrameDuration = ranges[0].MinFrameDuration;
                camera.UnlockForConfiguration();
            }

            var cameraInput = AVCaptureDeviceInput.FromDevice(camera);

            // Create a capture session
            _session = new AVCaptureSession
            {
                SessionPreset = AVCaptureSession.PresetPhoto
            };

            // Set the camera as input device
            _session.AddInput(cameraInput);

            var metadataOutput = new AVCaptureMetadataOutput();
            if (_session.CanAddOutput(metadataOutput))
            {
                var metadataQueue = new DispatchQueue("com.AVCam.metadata");

                _metadataObjectsDelegate = new MetadataObjectsDelegate
                {
                    DidOutputMetadataObjectsAction = DidOutputMetadataObjects
                };

                metadataOutput.SetDelegate(_metadataObjectsDelegate, metadataQueue);

                _session.AddOutput(metadataOutput);
            }

            _previewLayer = new AVCaptureVideoPreviewLayer(_session)
            {
                Frame = Layer.Bounds, 
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill,
            };

            if (_previewLayer.Connection != null)
                _previewLayer.Connection.VideoOrientation = GetOrientation();
            
            Layer.AddSublayer(_previewLayer);

            metadataOutput.MetadataObjectTypes = AVMetadataObjectType.QRCode;
            _session.StartRunning();
        }

        private void DidOutputMetadataObjects(AVMetadataObject[] arg2)
        {
            if (arg2.Any())
            {
                _session.StopRunning();

                if (arg2.First() is AVMetadataMachineReadableCodeObject aVMetadataMachineReadableCodeObject)
                {
                    ObjectCaptured?.Invoke(aVMetadataMachineReadableCodeObject.StringValue);
                }
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var videoPreviewLayerConnection = _previewLayer?.Connection;

            if (videoPreviewLayerConnection != null)
            {
                videoPreviewLayerConnection.VideoOrientation = GetOrientation();
            }
        }

        private static AVCaptureVideoOrientation GetOrientation()
        {
            switch (UIApplication.SharedApplication.StatusBarOrientation)
            {
                case UIInterfaceOrientation.LandscapeLeft:
                    return AVCaptureVideoOrientation.LandscapeLeft;
                case UIInterfaceOrientation.LandscapeRight:
                    return AVCaptureVideoOrientation.LandscapeRight;
            }

            return AVCaptureVideoOrientation.Portrait;
        }
    }

    internal class MetadataObjectsDelegate : AVCaptureMetadataOutputObjectsDelegate
    {
        public Action<AVMetadataObject[]> DidOutputMetadataObjectsAction;

        public override void DidOutputMetadataObjects(AVCaptureMetadataOutput captureOutput, AVMetadataObject[] metadataObjects, AVCaptureConnection connection)
        {
            DidOutputMetadataObjectsAction?.Invoke(metadataObjects);
        }
    }
}
