using SonistoRepackage.Model;

namespace SonistoRepackage.InstallDetection
{
    class ItemForListbox
    {

        public string path { get; set; }
        public string file { get; set; }
        public InstallationPackageChoice choices { get; set; }
        public bool exclude { get; set; }
    }
}
