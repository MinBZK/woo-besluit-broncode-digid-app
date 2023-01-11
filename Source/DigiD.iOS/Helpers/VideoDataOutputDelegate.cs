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
using System.Diagnostics;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;

namespace DigiD.iOS.Helpers
{
    public class VideoDataOutputDelegate : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        private volatile bool _disposed;

        public Action<CGImage> ImageCaptured { get; set; }

        public override void DidDropSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
#if DEBUG
            var attachment = sampleBuffer.GetAttachments(CMAttachmentMode.ShouldPropagate);
            if (attachment.ContainsKey(new NSString("DroppedFrameReason")))
            {
                var reason = attachment["DroppedFrameReason"].ToString();
                Debug.WriteLine($"Frame dropped: {reason}");
            }
            else
            {
                Debug.WriteLine($"Frame dropped no reason found");
            }
#endif
            sampleBuffer?.Dispose();
        }

        public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
            if (!_disposed)
            {
                using (sampleBuffer)
                {
                    if (connection.VideoOrientation == AVCaptureVideoOrientation.Portrait)
                    {
                        try
                        {
                            CGImage cgImage = null;

                            // Get a pixel buffer from the sample buffer
                            using (var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer)
                            {
                                // Lock the base address
                                if (pixelBuffer != null)
                                {
                                    // Prepare to decode buffer
                                    var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;

                                    // Decode buffer - Create a new colorspace
                                    using (var cs = CGColorSpace.CreateDeviceRGB())
                                    {
                                        pixelBuffer.Lock(CVPixelBufferLock.None);
                                        // Create new context from buffer
                                        using (var context = new CGBitmapContext(pixelBuffer.BaseAddress,
                                            pixelBuffer.Width,
                                            pixelBuffer.Height,
                                            8,
                                            pixelBuffer.BytesPerRow,
                                            cs,
                                            (CGImageAlphaInfo)flags))
                                        {
                                            // Get the image from the context
                                            cgImage = context.ToImage();
                                            // Unlock and return image
                                            pixelBuffer.Unlock(CVPixelBufferLock.None);
                                        }
                                    }
                                }
                            }

                            if (cgImage == null) return;

                            CGImage viewportImage = null;
                            using (cgImage)
                            {
                                var newHeight = cgImage.Width * 0.75f;
                                var y = (cgImage.Height - newHeight) / 2f;
                                viewportImage = cgImage.WithImageInRect(new CGRect(0f, y, cgImage.Width, newHeight));
                            }
                            ImageCaptured?.Invoke(viewportImage);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            Debug.WriteLine(ex.StackTrace);
                        }
                    }
                    else
                    {
                        connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
                    }
                }
            }
            else
            {
                connection.Enabled = false;
                sampleBuffer?.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing)
            {
                ImageCaptured = null;
            }

            base.Dispose(disposing);
        }
    }
}
