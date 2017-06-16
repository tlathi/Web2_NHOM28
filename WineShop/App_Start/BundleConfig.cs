using System.Web;
using System.Web.Optimization;

namespace WineShop
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/jquery.actual.min.js",
                        "~/Scripts/theme-script.js",
                        "~/Scripts/option8.js",
                        "~/Scripts/prefix-free.js",
                        "~/assets/lib/jquery/jquery-1.11.2.min.js",
                        "~/assets/lib/bootstrap/js/bootstrap.min.js",
                        "~/assets/lib/select2/js/select2.min.js",
                        "~/assets/lib/jquery.bxslider/jquery.bxslider.min.js",
                        "~/assets/lib/owl.carousel/owl.carousel.min.js",
                        "~/assets/lib/countdown/jquery.plugin.js",
                        "~/assets/lib/countdown/jquery.countdown.js",
                        "~/assets/lib/parallax/jquery.parallax-1.1.3.js",
                        "~/assets/lib/fancyBox/jquery.fancybox.js",
                        "~/assets/lib/jquery.elevatezoom.js",
                        "~/assets/lib/jquery-ui/jquery-ui.min.js",
                        "~/assets/lib/fancyBox/jquery.fancybox.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/animate.css",
                        "~/Content/reset.css",
                        "~/Content/style.css",
                        "~/Content/responsive.css",
                        "~/Content/option8.css",
                        "~/Content/menu.css",
                        "~/Content/search.css",
                        "~/Content/menu3.css",
                        "~/assets/lib/bootstrap/css/bootstrap.min.css",
                        "~/assets/lib/font-awesome/css/font-awesome.min.css",
                        "~/assets/lib/select2/css/select2.min.css",
                        "~/assets/lib/owl.carousel/owl.carousel.css",
                        "~/assets/lib/jquery-ui/jquery-ui.css",
                        "~/assets/lib/fancyBox/jquery.fancybox.css"
                        //"~/assets/lib/jquery.bxslider/jquery.bxslider.css"
                        ));

            bundles.Add(new ScriptBundle("~/Administrator/Content/Scripts").Include(
                    "~/Areas/Administrator/Content/assets/plugins/pace/pace.min.js",
                    "~/Areas/Administrator/Content/assets/plugins/jquery/jquery-1.11.3.min.js",
                    "~/Areas/Administrator/Content/assets/plugins/bootstrapv3/js/bootstrap.min.js",
                    "~/Areas/Administrator/Content/assets/plugins/jquery-block-ui/jqueryblockui.min.js",
                    "~/Areas/Administrator/Content/assets/plugins/jquery-unveil/jquery.unveil.min.js",
                    "~/Areas/Administrator/Content/assets/plugins/jquery-scrollbar/jquery.scrollbar.min.js",
                    "~/Areas/Administrator/Content/assets/plugins/jquery-numberAnimate/jquery.animateNumbers.js",
                    "~/Areas/Administrator/Content/assets/plugins/jquery-validation/js/jquery.validate.min.js",
                    "~/Areas/Administrator/Content/assets/plugins/bootstrap-select2/select2.min.js",
                    "~/Areas/Administrator/Content/webarch/js/webarch.js",
                    "~/Areas/Administrator/Content/assets/js/chat.js"
                ));

            bundles.Add(new StyleBundle("~/Administrator/Content/css").Include(
                    "~/Areas/Administrator/Content/assets/plugins/pace/pace-theme-flash.css",
                    "~/Areas/Administrator/Content/assets/plugins/bootstrapv3/css/bootstrap.min.css",
                    "~/Areas/Administrator/Content/assets/plugins/bootstrapv3/css/bootstrap-theme.min.css",
                    "~/Areas/Administrator/Content/assets/plugins/font-awesome/css/font-awesome.css",
                    "~/Areas/Administrator/Content/assets/plugins/animate.min.css",
                    "~/Areas/Administrator/Content/assets/plugins/jquery-scrollbar/jquery.scrollbar.css",
                    "~/Areas/Administrator/Content/webarch/css/webarch.css"
                ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                      "~/Scripts/jquery.validate*"));
        }
    }
}