﻿#pragma checksum "C:\Users\arman\Desktop\VUProject\SilverlightSignalRTest\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "67233BAF33D921C408EA842DAB4615C8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SilverlightSignalRTest {
    
    
    public partial class MainPage : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBox txtName;
        
        internal System.Windows.Controls.TextBox txtUserID;
        
        internal System.Windows.Controls.TextBox txtMessage;
        
        internal System.Windows.Controls.Button btnSend;
        
        internal System.Windows.Controls.ListBox lstMessages;
        
        internal System.Windows.Controls.ListBox lstUsers;
        
        internal System.Windows.Controls.Image imgCenter;
        
        internal System.Windows.Controls.Image imgMouse;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/SilverlightSignalRTest;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.txtName = ((System.Windows.Controls.TextBox)(this.FindName("txtName")));
            this.txtUserID = ((System.Windows.Controls.TextBox)(this.FindName("txtUserID")));
            this.txtMessage = ((System.Windows.Controls.TextBox)(this.FindName("txtMessage")));
            this.btnSend = ((System.Windows.Controls.Button)(this.FindName("btnSend")));
            this.lstMessages = ((System.Windows.Controls.ListBox)(this.FindName("lstMessages")));
            this.lstUsers = ((System.Windows.Controls.ListBox)(this.FindName("lstUsers")));
            this.imgCenter = ((System.Windows.Controls.Image)(this.FindName("imgCenter")));
            this.imgMouse = ((System.Windows.Controls.Image)(this.FindName("imgMouse")));
        }
    }
}

