using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Gymdata.Accreditation.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ColorPickerViewModel _colorPicker;
        public ColorPickerViewModel ColorPicker
        {
            get { return _colorPicker; }
            set
            {
                _colorPicker = value;
                RaisePropertyChanged(() => ColorPicker);
            }
        }

        private string _color = "#00FF00";
        public string Color
        {
            get { return _color; }
            set
            {
                _color = value;
                RaisePropertyChanged(() => Color);
            }
        }

        public MainViewModel()
        {
            ColorPicker = new ColorPickerViewModel();
        }

        private RelayCommand _pickColorCmd;
        public RelayCommand PickColorCmd => _pickColorCmd ?? (_pickColorCmd = new RelayCommand(() =>
        {
            ColorPicker.Color = Color;
            ColorPicker.OnColorPicked = color =>
            {
                this.Color = color;
                ColorPicker.OnColorPicked = null;
                ColorPicker.OnCancelled = null;
            };
            ColorPicker.OnCancelled = () =>
            {
                ColorPicker.OnColorPicked = null;
                ColorPicker.OnCancelled = null;
            };
            ColorPicker.IsOpen = true;
        }));

    }
}