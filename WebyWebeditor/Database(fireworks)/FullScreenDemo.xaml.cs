using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

/*
*	A Full Screen Demonstratoin in C#
*   from shinedraw.com
*/

namespace FullScreenDemo
{
    public partial class FullScreenDemo : UserControl
    {
        private static double APP_WIDTH = 550;  // Application Width
        private static double APP_HEIGHT = 400; // Application Height
        private bool _scale = false;           // _scale flag

        public FullScreenDemo()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(FullScreenDemo_Loaded);
        }

        /////////////////////////////////////////////////////        
        // Handlers
        /////////////////////////////////////////////////////	

        // Once it is loaded
        void FullScreenDemo_Loaded(object sender, RoutedEventArgs e)
        {
            // Add Handlers
            Application.Current.Host.Content.Resized += new EventHandler(Content_Resized);
            Application.Current.Host.Content.FullScreenChanged += new EventHandler(Content_Resized);

            FullScreen1.Click += new RoutedEventHandler(FullScreen1_Click);
            FullScreen2.Click += new RoutedEventHandler(FullScreen2_Click);
        }

        // Resize the Applicatoin
        void Content_Resized(object sender, EventArgs e)
        {
            double currentWidth = Application.Current.Host.Content.ActualWidth;
            double currentHeight = Application.Current.Host.Content.ActualHeight;

            if (_scale)
            {
                // Scale up the Canvas
                Translate.X = 0;
                Translate.Y = 0;
                Scale.ScaleX = currentWidth / APP_WIDTH;
                Scale.ScaleY = currentHeight / APP_HEIGHT;
            }
            else
            {
                // position the Canvas to the center
                Translate.X = (currentWidth - APP_WIDTH) /2;
                Translate.Y = (currentHeight - APP_HEIGHT) / 2;
                Scale.ScaleX = 1;
                Scale.ScaleY = 1;
            }
        }

        // enable _scale
        void FullScreen1_Click(object sender, RoutedEventArgs e)
        {
            _scale = !Application.Current.Host.Content.IsFullScreen;
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }

        // disable _scale
        void FullScreen2_Click(object sender, RoutedEventArgs e)
        {
            _scale = false;
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }


    }
}
