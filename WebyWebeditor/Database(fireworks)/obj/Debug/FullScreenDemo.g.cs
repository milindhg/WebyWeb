#pragma checksum "E:\silv\our_project\Database(fireworks)\Database(fireworks)\FullScreenDemo.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B6569A5D2BD7D61636A59D1AA765B81D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
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


namespace FullScreenDemo {
    
    
    public partial class FullScreenDemo : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Canvas LayoutRoot;
        
        internal System.Windows.Media.ScaleTransform Scale;
        
        internal System.Windows.Media.TranslateTransform Translate;
        
        internal System.Windows.Controls.Button FullScreen1;
        
        internal System.Windows.Controls.Button FullScreen2;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Database(fireworks);component/FullScreenDemo.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Canvas)(this.FindName("LayoutRoot")));
            this.Scale = ((System.Windows.Media.ScaleTransform)(this.FindName("Scale")));
            this.Translate = ((System.Windows.Media.TranslateTransform)(this.FindName("Translate")));
            this.FullScreen1 = ((System.Windows.Controls.Button)(this.FindName("FullScreen1")));
            this.FullScreen2 = ((System.Windows.Controls.Button)(this.FindName("FullScreen2")));
        }
    }
}
