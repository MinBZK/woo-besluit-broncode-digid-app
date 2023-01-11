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
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Renderscripts;
using Android.Runtime;
using Android.Util;
using Android.Views;
using R = DigiD.Droid.Resource;

namespace DigiD.Droid.Views
{
    /// <summary>
    /// Converted from https://github.com/mmin18/RealtimeBlurView by Benjamin Mayrargue 2019/02/25
    /// A realtime blurring overlay (like iOS UIVisualEffectView).
    /// Just put it above  the view you want to Blur and it doesn't have to be in the same ViewGroup.
    /// app:realtimeBlurRadius="10dp"
    /// app:realtimeDownsampleFactor="4"
    /// app:realtimeOverlayColor="#aaffffff"
    /// </summary>
    [Register("DigiD.RealtimeBlurView")]
    public class RealtimeBlurView : View
    {
        private float mDownsampleFactor; // default 4
        private int mOverlayColor; // default #aaffffff
        private float mBlurRadius; // default 10dp (0 < r <= 25)

        private bool mDirty;
        private Bitmap mBitmapToBlur, mBlurredBitmap;
        private Canvas mBlurringCanvas;
        private RenderScript mRenderScript;
        private ScriptIntrinsicBlur mBlurScript;
        private Allocation mBlurInput, mBlurOutput;
        private bool mIsRendering;
        private readonly Paint mPaint;

        private readonly Rect mRectSrc = new Rect();
        private readonly Rect mRectDst = new Rect();

        // mDecorView should be the root view of the activity (even if you are on a different window like a dialog)
        private View mDecorView;

        // If the view is on different root view (usually means we are on a PopupWindow),
        // we need to manually call invalidate() in onPreDraw(), otherwise we will not be able to see the changes
        private bool mDifferentRoot;
        private static int RENDERING_COUNT { get; set; }
        private readonly MyPreDrawListener preDrawListener;

        [Preserve(Conditional = true)]
        public RealtimeBlurView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            TypedArray a = context.ObtainStyledAttributes(attrs, R.Styleable.RealtimeBlurView);
            mBlurRadius = a.GetDimension(R.Styleable.RealtimeBlurView_realtimeBlurRadius,
                TypedValue.ApplyDimension(ComplexUnitType.Dip, 10, context.Resources.DisplayMetrics));
            mDownsampleFactor = a.GetFloat(R.Styleable.RealtimeBlurView_realtimeDownsampleFactor, 4);
            mOverlayColor = a.GetColor(R.Styleable.RealtimeBlurView_realtimeOverlayColor, Color.Argb(0xaa, 255, 255, 255).ToArgb());
            a.Recycle();

            mPaint = new Paint();
            preDrawListener = new MyPreDrawListener(this);
        }

        [Preserve(Conditional = true)]
        protected RealtimeBlurView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public void SetBlurRadius(float radius)
        {
            if (mBlurRadius != radius)
            {
                mBlurRadius = radius;
                mDirty = true;
                Invalidate();
            }
        }

        public void SetDownsampleFactor(float factor)
        {
            if (factor <= 0)
                throw new ArgumentException("Downsample factor must be greater than 0.",nameof(factor));

            if (mDownsampleFactor != factor)
            {
                mDownsampleFactor = factor;
                mDirty = true; // may also change Blur radius
                ReleaseBitmap();
                Invalidate();
            }
        }

        public void SetOverlayColor(int color)
        {
            if (mOverlayColor != color)
            {
                mOverlayColor = color;
                Invalidate();
            }
        }

        private void ReleaseBitmap()
        {
            if (mBlurInput != null)
            {
                mBlurInput.Destroy();
                mBlurInput = null;
            }

            if (mBlurOutput != null)
            {
                mBlurOutput.Destroy();
                mBlurOutput = null;
            }

            if (mBitmapToBlur != null)
            {
                mBitmapToBlur.Recycle();
                mBitmapToBlur = null;
            }

            if (mBlurredBitmap != null)
            {
                mBlurredBitmap.Recycle();
                mBlurredBitmap = null;
            }
        }

        private void ReleaseScript()
        {
            if (mRenderScript != null)
            {
                mRenderScript.Destroy();
                mRenderScript = null;
            }

            if (mBlurScript != null)
            {
                mBlurScript.Destroy();
                mBlurScript = null;
            }
        }

        protected void Release()
        {
            ReleaseBitmap();
            ReleaseScript();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "<Pending>")]
        protected bool Prepare()
        {
            if (mBlurRadius == 0)
            {
                Release();
                return false;
            }

            float downsampleFactor = mDownsampleFactor;
            float radius = mBlurRadius / downsampleFactor;
            if (radius > 25)
            {
                downsampleFactor = downsampleFactor * radius / 25;
                radius = 25;
            }

            if (mDirty || mRenderScript == null)
            {
                if (mRenderScript == null)
                {
                    try
                    {
                        mRenderScript = RenderScript.Create(Context);
                        mBlurScript = ScriptIntrinsicBlur.Create(mRenderScript, Element.U8_4(mRenderScript));
                    }
#pragma warning disable CS0168 // Variable is declared but never used
                    catch (RSRuntimeException e)
#pragma warning restore CS0168 // Variable is declared but never used
                    {
#if DEBUG
                        if (e.Message != null && e.Message.StartsWith("Error loading RS jni library: java.lang.UnsatisfiedLinkError:"))
                            throw new Exception("Error loading RS jni library, Upgrade buildToolsVersion=\"24.0.2\" or higher may solve this issue");
                        throw;
#else
                        // In release mode, just ignore
                        ReleaseScript();
                        return false;
#endif
                    }
                }

                mBlurScript.SetRadius(radius);
                mDirty = false;
            }

            int width = Width;
            int height = Height;

            int scaledWidth = Math.Max(1, (int) (width / downsampleFactor));
            int scaledHeight = Math.Max(1, (int) (height / downsampleFactor));

            if (mBlurringCanvas == null || mBlurredBitmap == null
                                        || mBlurredBitmap.Width != scaledWidth
                                        || mBlurredBitmap.Height != scaledHeight)
            {
                ReleaseBitmap();

                try
                {
                    mBitmapToBlur = Bitmap.CreateBitmap(scaledWidth, scaledHeight, Bitmap.Config.Argb8888);
                    if (mBitmapToBlur == null)
                    {
                        return false;
                    }

                    mBlurringCanvas = new Canvas(mBitmapToBlur);

                    mBlurInput = Allocation.CreateFromBitmap(mRenderScript, mBitmapToBlur, Allocation.MipmapControl.MipmapNone, AllocationUsage.Script);
                    mBlurOutput = Allocation.CreateTyped(mRenderScript, mBlurInput.Type);

                    mBlurredBitmap = Bitmap.CreateBitmap(scaledWidth, scaledHeight, Bitmap.Config.Argb8888);
                    if (mBlurredBitmap == null)
                    {
                        return false;
                    }

                    return true;
                }
                catch (Java.Lang.OutOfMemoryError)
                {
                    // Bitmap.createBitmap() may cause OOM error
                    // Simply ignore and fallback
                }

                ReleaseBitmap();
                return false;
            }

            return true;
        }

        protected void Blur(Bitmap bitmapToBlur, Bitmap blurredBitmap)
        {
            mBlurInput.CopyFrom(bitmapToBlur);
            mBlurScript.SetInput(mBlurInput);
            mBlurScript.ForEach(mBlurOutput);
            mBlurOutput.CopyTo(blurredBitmap);
        }

        class MyPreDrawListener : Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
        {
            private readonly RealtimeBlurView parent;

            public MyPreDrawListener(RealtimeBlurView parent)
            {
                this.parent = parent;
            }

            public bool OnPreDraw()
            {
                return parent.OnPreDraw();
            }
        }

        private bool OnPreDraw()
        {
            var locations = new int[2];
            var oldBmp = mBlurredBitmap;
            var decor = mDecorView;
            if (decor != null && IsShown && Prepare())
            {
                var redrawBitmap = mBlurredBitmap != oldBmp;
                decor.GetLocationOnScreen(locations);
                int x = -locations[0];
                int y = -locations[1];

                GetLocationOnScreen(locations);
                x += locations[0];
                y += locations[1];

                // just erase transparent
                mBitmapToBlur.EraseColor(mOverlayColor & 0xffffff);

                int rc = mBlurringCanvas.Save();
                mIsRendering = true;
                RENDERING_COUNT++;
                try
                {
                    mBlurringCanvas.Scale(1.0f * mBitmapToBlur.Width / Width, 1.0f * mBitmapToBlur.Height / Height);
                    mBlurringCanvas.Translate(-x, -y);
                    decor.Background?.Draw(mBlurringCanvas);
                    decor.Draw(mBlurringCanvas);
                }
                finally
                {
                    mIsRendering = false;
                    RENDERING_COUNT--;
                    mBlurringCanvas.RestoreToCount(rc);
                }

                Blur(mBitmapToBlur, mBlurredBitmap);

                if (redrawBitmap || mDifferentRoot)
                {
                    Invalidate();
                }
            }

            return true;
        }

        protected View GetActivityDecorView()
        {
            var ctx = Context;
            for (int i = 0; i < 4 && ctx != null && !(ctx is Activity) && ctx is ContextWrapper; i++)
                ctx = ((ContextWrapper) ctx).BaseContext;

            if (ctx is Activity activity)
                return activity.Window.DecorView;

            return null;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            mDecorView = GetActivityDecorView();
            if (mDecorView != null)
            {
                mDecorView.ViewTreeObserver.AddOnPreDrawListener(preDrawListener);
                mDifferentRoot = mDecorView.RootView != RootView;
                if (mDifferentRoot)
                    mDecorView.PostInvalidate();
            }
            else
                mDifferentRoot = false;
        }

        protected override void OnDetachedFromWindow()
        {
            mDecorView?.ViewTreeObserver.RemoveOnPreDrawListener(preDrawListener);
            Release();
            base.OnDetachedFromWindow();
        }

        public override void Draw(Canvas canvas)
        {
            if (mIsRendering)
            {
                // Quit here, don't draw views above me
                //throw new StopException();
                return;
            }

            if (RENDERING_COUNT > 0)
            {
                // Doesn't support blurview overlap on another blurview
            }
            else
                base.Draw(canvas);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            DrawBlurredBitmap(canvas, mBlurredBitmap, mOverlayColor);
        }


        /// <summary>
        /// Custom draw the blurred bitmap and color to define your own shape
        /// </summary>
        protected void DrawBlurredBitmap(Canvas canvas, Bitmap blurredBitmap, int overlayColor)
        {
            if (blurredBitmap != null)
            {
                mRectSrc.Right = blurredBitmap.Width;
                mRectSrc.Bottom = blurredBitmap.Height;
                mRectDst.Right = Width;
                mRectDst.Bottom = Height;
                canvas.DrawBitmap(blurredBitmap, mRectSrc, mRectDst, null);
            }

            mPaint.Color = new Color(overlayColor);
            canvas.DrawRect(mRectDst, mPaint);
        }
    }
}
