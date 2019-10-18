using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DNC.Views
{
    /// <summary>
    /// Interaction logic for PropertiesDialog.xaml
    /// </summary>
    public partial class PropertiesDialog : Window
    {
        /// <summary>
        /// Automatically Generates a Properties Window at Runtime based on the Type
        /// </summary>
        /// <param name="type">Type of Object to be passed</param>
        /// <param name="data">Object to be passed</param>
        public PropertiesDialog()
        {
            InitializeComponent();
        }

        public bool? GenerateDialog<T>(object data)
        {
            Type type = typeof(T);
            foreach (PropertyInfo pInfo in type.GetProperties())
            {
                foreach (CustomAttributeData a in pInfo.GetCustomAttributesData())
                {
                    Debug.WriteLine($"{pInfo.Name}: has attribute {a.AttributeType}");
                }
            }

            return ShowDialog();
        }

    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class PropertyNameAttribute : Attribute
    {
        public PropertyNameAttribute(string name)
        {
            // TODO: Implement code here
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class PropertyTypeAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameworkElement">How do you want this data to be displayed in the properties window</param>
        public PropertyTypeAttribute(Type frameworkElement)
        {
            if (frameworkElement != typeof(FrameworkElement)) throw new InvalidOperationException("Type is not framework element");
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class ChildPropertyAttribute : Attribute
    {
        public ChildPropertyAttribute()
        {
            // TODO: Implement code here
            throw new NotImplementedException();
        }
    }
}
