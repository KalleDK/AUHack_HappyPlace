using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HarrasBlockerApp
{
    public class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Implementation of INotifyPropertyChanged to avoid duplicate code in each ViewModel
        /// </summary>
        /// <typeparam name="T">Type of calling object</typeparam>
        /// <param name="member">Member property being changed</param>
        /// <param name="val">New value for member property</param>
        /// <param name="propertyName">Name of property. Auto set using <see cref="CallerMemberNameAttribute"/></param>
        protected virtual void SetProperty<T>(ref T member, T val,
            [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(member, val)) return;

            member = val;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Can be used to easily update other properties, such as properties that must compute a new value.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
