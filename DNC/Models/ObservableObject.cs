using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DNC.Models
{
    [Serializable]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public ObservableObject()
        {

        }

        //
        // Summary:
        //     Provides access to the PropertyChanged event handler to derived classes.
        protected PropertyChangedEventHandler PropertyChangedHandler { get; }

        //
        // Summary:
        //     Occurs after a property value changes.
        public event PropertyChangedEventHandler PropertyChanged;

        //
        // Summary:
        //     Extracts the name of a property from an expression.
        //
        // Parameters:
        //   propertyExpression:
        //     An expression returning the property's name.
        //
        // Type parameters:
        //   T:
        //     The type of the property.
        //
        // Returns:
        //     The name of the property returned by the expression.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     If the expression is null.
        //
        //   T:System.ArgumentException:
        //     If the expression does not represent a property.
        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Raises the PropertyChanged event if needed.
        //
        // Parameters:
        //   propertyName:
        //     (optional) The name of the property that changed.
        //
        // Remarks:
        //     If the propertyName parameter does not correspond to an existing property on
        //     the current class, an exception is thrown in DEBUG configuration only.
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //
        // Summary:
        //     Raises the PropertyChanged event if needed.
        //
        // Parameters:
        //   propertyExpression:
        //     An expression identifying the property that changed.
        //
        // Type parameters:
        //   T:
        //     The type of the property that changed.
        public virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Verifies that a property name exists in this ViewModel. This method can be called
        //     before the property is used, for instance before calling RaisePropertyChanged.
        //     It avoids errors when a property name is changed but some places are missed.
        //
        // Parameters:
        //   propertyName:
        //     The name of the property that will be checked.
        //
        // Remarks:
        //     This method is only active in DEBUG mode.
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Assigns a new value to the property. Then, raises the PropertyChanged event if
        //     needed.
        //
        // Parameters:
        //   propertyExpression:
        //     An expression identifying the property that changed.
        //
        //   field:
        //     The field storing the property's value.
        //
        //   newValue:
        //     The property's value after the change occurred.
        //
        // Type parameters:
        //   T:
        //     The type of the property that changed.
        //
        // Returns:
        //     True if the PropertyChanged event has been raised, false otherwise. The event
        //     is not raised if the old value is equal to the new value.
        protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Assigns a new value to the property. Then, raises the PropertyChanged event if
        //     needed.
        //
        // Parameters:
        //   propertyName:
        //     The name of the property that changed.
        //
        //   field:
        //     The field storing the property's value.
        //
        //   newValue:
        //     The property's value after the change occurred.
        //
        // Type parameters:
        //   T:
        //     The type of the property that changed.
        //
        // Returns:
        //     True if the PropertyChanged event has been raised, false otherwise. The event
        //     is not raised if the old value is equal to the new value.
        protected bool Set<T>(string propertyName, ref T field, T newValue)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Assigns a new value to the property. Then, raises the PropertyChanged event if
        //     needed.
        //
        // Parameters:
        //   field:
        //     The field storing the property's value.
        //
        //   newValue:
        //     The property's value after the change occurred.
        //
        //   propertyName:
        //     (optional) The name of the property that changed.
        //
        // Type parameters:
        //   T:
        //     The type of the property that changed.
        //
        // Returns:
        //     True if the PropertyChanged event has been raised, false otherwise. The event
        //     is not raised if the old value is equal to the new value.
        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            throw new NotImplementedException();
        }
    }
}
