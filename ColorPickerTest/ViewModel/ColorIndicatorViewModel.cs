using GalaSoft.MvvmLight;

namespace Gymdata.Accreditation.ViewModel
{
    public class ColorIndicatorViewModel: ViewModelBase
    {
        private string _color;

        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                RaisePropertyChanged(() => Color);
            }
        }
    }
}
