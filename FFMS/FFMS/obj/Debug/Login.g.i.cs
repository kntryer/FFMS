﻿#pragma checksum "..\..\Login.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "FC1427868C1326412C06936399DD0A24"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34014
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace FFMS {
    
    
    /// <summary>
    /// Login
    /// </summary>
    public partial class Login : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 33 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox txtMemberName;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox txtPassWord;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label register;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label findpwd;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\Login.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button login;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/FFMS;component/login.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Login.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.txtMemberName = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.txtPassWord = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 3:
            this.register = ((System.Windows.Controls.Label)(target));
            
            #line 35 "..\..\Login.xaml"
            this.register.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Label_Click);
            
            #line default
            #line hidden
            
            #line 35 "..\..\Login.xaml"
            this.register.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Mouse_Enter);
            
            #line default
            #line hidden
            
            #line 35 "..\..\Login.xaml"
            this.register.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Mouse_Leave);
            
            #line default
            #line hidden
            return;
            case 4:
            this.findpwd = ((System.Windows.Controls.Label)(target));
            
            #line 36 "..\..\Login.xaml"
            this.findpwd.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Label_Click);
            
            #line default
            #line hidden
            
            #line 36 "..\..\Login.xaml"
            this.findpwd.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Mouse_Enter);
            
            #line default
            #line hidden
            
            #line 36 "..\..\Login.xaml"
            this.findpwd.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Mouse_Leave);
            
            #line default
            #line hidden
            return;
            case 5:
            this.login = ((System.Windows.Controls.Button)(target));
            
            #line 39 "..\..\Login.xaml"
            this.login.Click += new System.Windows.RoutedEventHandler(this.login_Click);
            
            #line default
            #line hidden
            
            #line 39 "..\..\Login.xaml"
            this.login.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Mouse_Enter);
            
            #line default
            #line hidden
            
            #line 39 "..\..\Login.xaml"
            this.login.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Mouse_Leave);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

