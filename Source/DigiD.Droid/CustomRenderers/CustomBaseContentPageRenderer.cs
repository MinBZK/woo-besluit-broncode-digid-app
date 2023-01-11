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
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using AndroidX.AppCompat.Widget;
using DigiD.Common.BaseClasses;
using DigiD.Common.Helpers;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Mobile.Helpers;
using DigiD.Droid.CustomRenderers;
using DigiD.UI.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Application = Xamarin.Forms.Application;

[assembly: ExportRenderer(typeof(BaseContentPage), typeof(CustomBaseContentPageRenderer))]
namespace DigiD.Droid.CustomRenderers
{
    public class CustomBaseContentPageRenderer : PageRenderer
    {
        public CustomBaseContentPageRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            Clickable = false;  //zie Pureween/A11yTools

            if (e.OldElement != null || Element == null)
                return;

            if (!(e.NewElement is BaseContentPage page))
                return;

            if (!(page is MainMenuPage || page is LandingPage))
                NavigationPage.SetTitleIconImageSource(page, null);
        }

        protected override void OnAttachedToWindow()  //zie Pureween/A11yTools
        {
            base.OnAttachedToWindow();
            DisableFocusableInTouchMode();
        }

        protected override void AttachViewToParent(Android.Views.View child, int index, LayoutParams @params)  //zie Pureween/A11yTools
        {
            base.AttachViewToParent(child, index, @params);
            DisableFocusableInTouchMode();
        }

        private void DisableFocusableInTouchMode()  //zie Pureween/A11yTools
        {
            var view = Parent;

            string className = $"{view?.GetType().Name}";

            while (!className.Contains("PlatformRenderer") && view != null)
            {
                view = view.Parent;
                className = $"{view?.GetType().Name}";
            }

            if (view is global::Android.Views.View androidView)
            {
                androidView.Focusable = false;
                androidView.FocusableInTouchMode = false;
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            if (!(Element is BaseContentPage))
                return;

            OrientationHelper.ResizeViews(Element, new Size(w, h), Element is LandingPage);
        }

        protected override async void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (changed)
                await SetLayout();
        }

        private async Task SetLayout()
        {
            var toolbar = ((Activity)Context).FindViewById<Toolbar>(Xamarin.Forms.Platform.Android.Resource.Id.toolbar);

            if (toolbar != null)
            {
                try
                {
                    // var dm = ThemeHelper.IsInDarkMode;

                    if (OrientationHelper.IsInLandscapeMode && DemoHelper.CurrentUser != null)
                    {
                        await Task.Delay(10);  //KTR zonder deze delay wordt de titel niet (altijd) getoond.
                        toolbar.Title = $"{DemoHelper.CurrentUser.UserName} - {((CommonBaseViewModel)Element.BindingContext).PageId}";
                    }
                    else
                    {
                        toolbar.Title = null;
                    }
                }
                catch (Exception)
                {
                    //Maybe toolbar is already disposed
                }
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            var toolbar = ((Activity)Context).FindViewById<Toolbar>(Xamarin.Forms.Platform.Android.Resource.Id.toolbar);

            if (toolbar == null)
                return;

            if (Application.Current.MainPage is CustomNavigationPage navPag && navPag.Navigation.NavigationStack.Count >= 2)
            {
                var vm = (CommonBaseViewModel)navPag.CurrentPage.BindingContext;

                if (vm?.HasBackButton == true)
                {
                    var backButtonIcon = ThemeHelper.IsInDarkMode ? Resource.Drawable.icon_terug_dark : Resource.Drawable.icon_terug;
                    toolbar.NavigationIcon = AndroidX.Core.Content.ContextCompat.GetDrawable(Context, backButtonIcon);
                }
            }
        }
    }
}
