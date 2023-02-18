namespace Infrastructure.Web.Bundles.Models
{
    public class BundleModel
    {
        public bool IsSuccess { get; set; }
        public string BundleCss { get; set; }
        public string BundleJs { get; set; }
        public string BundleJsCommon { get; set; }
        public string BundleJsChunk { get; set; }
        public string BundleCssCommon { get; set; }
        public string BundleCssChunk { get; set; }
        public string Path { get; set; }
        public string ErrorMessage { get; set; }
    }
}
