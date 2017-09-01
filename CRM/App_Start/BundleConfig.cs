using System.Web;
using System.Web.Optimization;

namespace CRM
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.7.2.min.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/jquery.uniform.js",
                        "~/Scripts/colResizable-1.3.js",
                        "~/Scripts/jquery-ui-1.8.21.min.js",
                        "~/Scripts/jquery.elastic.js",
                        "~/Scripts/jquery.cleditor.js",
                        "~/Scripts/jquery.colorbox.js",
                        "~/Scripts/colorpicker.js",
                        "~/Scripts/fullcalendar.js",
                        "~/Scripts/jquery.maskedinput-1.3.js",
                        "~/Scripts/excanvas.js",
                        "~/Scripts/jquery.flot.js",
                        "~/Scripts/jquery.flot.pie.js",
                        "~/Scripts/jquery.flot.resize.js",
                        "~/Scripts/jquery.supertextarea.min.js",
                        "~/Scripts/ui.spinner.js",
                        "~/Scripts/jquery.tipsy.js",
                        "~/Scripts/kanrisha.js",
                        "~/Scripts/bootstrap-datetimepicker.min.js",
                        "~/Scripts/bootstrap-datetimepicker.zh-CN.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/colorbox.css",
                      "~/Content/colorpicker.css",
                      "~/Content/fullcalendar.css",
                      "~/Content/jquery.cleditor.css",
                      "~/Content/normalize.css",
                      "~/Content/tipsy.css",
                      "~/Content/style.css",
                      "~/Content/bootstrap-datetimepicker.min.css"
                      ));
        }
    }
}
