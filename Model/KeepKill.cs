
namespace SonistoRepackage.Model
{
    public class KeepKill
    {
        private bool _keep = true;
        private bool _kill = false;
        public bool keep
        {
            get
            {
                return this._keep;
            }
            set
            {
                this._keep = value;
                this._kill = !value;
            }
        }
        public bool kill {
            get
            {
                return this._kill;
            }
            set
            {
                this._kill = value;
                this._keep = !value;
            }
        }
    }
}
