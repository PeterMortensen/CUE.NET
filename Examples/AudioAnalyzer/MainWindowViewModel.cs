using System.ComponentModel;
using System.Runtime.CompilerServices;
using AudioAnalyzer.Annotations;

namespace AudioAnalyzer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Properties & Fields

        #endregion

        #region Constructors

        #endregion

        #region Methods

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
