using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BuilderScenario.App.Common
{
    public abstract class BaseViewModel :
        INotifyPropertyChanged,
        INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private readonly Dictionary<string, List<string>> _errors
            = new();

        public bool HasErrors => _errors.Any();

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName != null && _errors.ContainsKey(propertyName))
                return _errors[propertyName];

            return Enumerable.Empty<string>();
        }

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
                _errors[propertyName].Add(error);

            ErrorsChanged?.Invoke(this,
                new DataErrorsChangedEventArgs(propertyName));
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this,
                    new DataErrorsChangedEventArgs(propertyName));
            }
        }
    }
}
