﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml.Serialization;

namespace DNC.Properties
{


    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.0.0.0")]
    [global::System.Configuration.SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Binary)]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {

        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Binary)]
        public global::System.Collections.ObjectModel.ObservableCollection<DNC.Models.ModelBase> EnumeratedList
        {
            get
            {
                return ((global::System.Collections.ObjectModel.ObservableCollection<DNC.Models.ModelBase>)(this["EnumeratedList"]));
            }
            set
            {
                this["EnumeratedList"] = value;
            }
        }
    }
}
