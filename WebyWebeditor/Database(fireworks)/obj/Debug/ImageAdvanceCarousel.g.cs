#pragma checksum "E:\silv\our_project\Database(fireworks)\Database(fireworks)\ImageAdvanceCarousel.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E50FD2AE2BAB18EF092DEE7062929CF3"
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


namespace ImageAdvanceCarousel {
    
    
    public partial class ImageAdvanceCarousel : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Canvas Holder;
        
        internal System.Windows.Controls.Image MoveLeftButton;
        
        internal System.Windows.Controls.Image MoveRightButton;
        
        internal System.Windows.Shapes.Rectangle currimg;
        
        internal System.Windows.Controls.TextBlock imgtext;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Database(fireworks);component/ImageAdvanceCarousel.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.Holder = ((System.Windows.Controls.Canvas)(this.FindName("Holder")));
            this.MoveLeftButton = ((System.Windows.Controls.Image)(this.FindName("MoveLeftButton")));
            this.MoveRightButton = ((System.Windows.Controls.Image)(this.FindName("MoveRightButton")));
            this.currimg = ((System.Windows.Shapes.Rectangle)(this.FindName("currimg")));
            this.imgtext = ((System.Windows.Controls.TextBlock)(this.FindName("imgtext")));
        }
    }
}
