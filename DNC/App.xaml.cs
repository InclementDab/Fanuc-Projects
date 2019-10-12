using DNC.Models;
using DNC.Properties;
using DNC.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml.Serialization;

namespace DNC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        public static string ToJson<T>(T obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);

        public static T RecursiveGetType<T>(DependencyObject current)
        {

            if (current == null) return default;

            if (current is FrameworkElement f)
                return RecursiveGetType<T>(f);

            if (LogicalTreeHelper.GetParent(current) is T ret)
                return ret;

            return RecursiveGetType<T>(LogicalTreeHelper.GetParent(current));

        }

        public static T RecursiveGetType<T>(FrameworkElement current)
        {
            if (current == null) return default;

            if (current.Parent is T ret)
                return ret;
            if (current.TemplatedParent is T tret)
                return tret;

            return RecursiveGetType<T>(current.TemplatedParent ?? current.Parent);
        }

        public static T RecursiveGetChild<T>(DependencyObject current)
        {
            if (current == null) return default;

            foreach (DependencyObject child in LogicalTreeHelper.GetChildren(current))
            {
                if (child is T r) return r;
                
                var ret = RecursiveGetChild<T>(child);

                if (ret != null)
                    return ret;
            }
            return default;
        }
    }

    /// <summary>
    /// idk about this one
    /// </summary>
    
    [Serializable]
    [SettingsSerializeAs(SettingsSerializeAs.Binary)]
    public class UserSettings : ApplicationSettingsBase
    {
        /// <summary>
        /// Defines Communication Read Timeout
        /// </summary>
        [UserScopedSetting()]
        [DefaultSettingValue("500")]
        public int ReadTimeout { get; set; }

        [DefaultSettingValue("500")]
        public int WriteTimeout { get; set; }
    }

    





    public static class TaskUtilities
    {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.HandleError(ex);
            }
        }
    }

    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public BoolToVisibilityConverter()
        {
            // set defaults
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, TrueValue);
        }
    }

    public sealed class IPAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IPAddress ipAddress) return ipAddress.ToString();

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is string text && IPAddress.TryParse(text, out IPAddress ipAddress)) return ipAddress;

            return DependencyProperty.UnsetValue;
        }
    }

    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        #region SelectedItem Property

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(BindableSelectedItemBehavior), new UIPropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is TreeViewItem item)
            {
                item.SetValue(TreeViewItem.IsSelectedProperty, true);
            }
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItem = e.NewValue;
        }
    }

    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? parameter : Binding.DoNothing;
        }
    }



}
