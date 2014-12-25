/****************************************************************************

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

-- Copyright 2009 Terence Tsang
-- admin@shinedraw.com
-- http://www.shinedraw.com
-- Your Flash vs Silverlight Repositry

****************************************************************************/


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
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Browser;

/*
*	A Image Advance Carousel Demonstratoin in C#
*   from shinedraw.com
*/

namespace ImageAdvanceCarousel
{
    public partial class ImageCarousel : UserControl
    {

        private String[] IMAGES = { "images/shiny1.jpg", "images/shiny2.jpg", "images/shiny3.jpg", "images/shiny4.jpg", "images/shiny5.jpg", "images/shiny6.jpg", "images/shiny7.jpg", "images/shiny8.jpg", "images/shiny9.jpg", "images/shiny10.jpg", "images/shiny11.jpg", "images/shiny12.jpg"};    // images
        private static double IMAGE_WIDTH = 128;        // Image Width
        private static double IMAGE_HEIGHT = 128;       // Image Height        
        private static double SPRINESS = 0.4;		    // Control the Spring Speed
        private static double DECAY = 0.5;			    // Control the bounce Speed
        private static double SCALE_DOWN_FACTOR = 0.5;  // Scale between images
        private static double OFFSET_FACTOR = 100;      // Distance between images
        private static double OPACITY_DOWN_FACTOR = 0.4;    // Alpha between images
        private static double MAX_SCALE = 2;            // Maximum Scale
        private static double CRITICAL_POINT = 0.001;


        private double _xCenter;
        private double _yCenter;

        // Display something at the cetner first
		private double _target = 2;		// Target moving position
		private double _current  = 2;	// Current position
		private double _spring = 0;		// Temp used to store last moving 
		private List<Image> _images  = new List<Image>();	// Store the added images

        private static int FPS = 24;                // fps of the on enter frame event
        private DispatcherTimer _timer = new DispatcherTimer(); // on enter frame simulator

        public ImageCarousel()
        {
            InitializeComponent();

            // Save the center position
            _xCenter = Width / 2;
            _yCenter = Height / 2;

            addImages();
        }



        /////////////////////////////////////////////////////        
		// Handlers 
		/////////////////////////////////////////////////////	

        // reposition the images
        void _timer_Tick(object sender, EventArgs e)
        {

            if (Math.Abs(_target - _current) < CRITICAL_POINT) return;

            for(int i = 0; i < _images.Count; i++){
                Image image = _images[i];
                posImage(image, i);
            }

			// compute the current position
			// added spring effect
			_spring = (_target - _current) * SPRINESS + _spring * DECAY;
			_current += _spring;
            
        }


		
		/////////////////////////////////////////////////////        
		// Private Methods 
		/////////////////////////////////////////////////////	
	
		
		// add images to the stage
		private void addImages(){
			for(int i = 0; i < IMAGES.Length; i++){
				// get the image resources from the xap
				string url = IMAGES[i];
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(url, UriKind.Relative));
                image.Width = 128;
                image.Height = 128;
                // add and reposition the image
				LayoutRoot.Children.Add(image);
				posImage(image, i);
				_images.Add(image);
			}
		}

        // move the index
        private void moveIndex(int value)
        {
            _target += value;
            _target = Math.Max(0, _target);
            _target = Math.Min(_images.Count - 1, _target);
        }
		
		// reposition the image
		private void posImage(Image image , int index){
            double diffFactor = index - _current;
			
			
			// scale and position the image according to their index and current position
            // the one who closer to the _current has the larger scale
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = MAX_SCALE - Math.Abs(diffFactor) * SCALE_DOWN_FACTOR;
            scaleTransform.ScaleY = MAX_SCALE - Math.Abs(diffFactor) * SCALE_DOWN_FACTOR;
            image.RenderTransform = scaleTransform;

            // reposition the image
            double left = _xCenter - (IMAGE_WIDTH * scaleTransform.ScaleX) / 2 + diffFactor * OFFSET_FACTOR;
            double top = _yCenter - (IMAGE_HEIGHT * scaleTransform.ScaleY) / 2;
            image.Opacity = 1 - Math.Abs(diffFactor) * OPACITY_DOWN_FACTOR;

            image.SetValue(Canvas.LeftProperty, left);
            image.SetValue(Canvas.TopProperty, top);

            // order the element by the scaleX
            image.SetValue(Canvas.ZIndexProperty, (int)Math.Abs(scaleTransform.ScaleX * 100));
		}
		
        /////////////////////////////////////////////////////        
        // Public Methods
        /////////////////////////////////////////////////////	

        // start the timer
        public void Start()
        {
            // start the enter frame event
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / FPS);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }

        // Move the Images to the left
        public void MoveLeft()
        {
            moveIndex(-1);
        }

        // Move the Images to the right
        public void MoveRight()
        {
            moveIndex(1);
        }

        // Drill Down Effect
        public void DrillDown()
        {
            DrillDownStory.Begin();
        }

        // Drill Up Effect
        public void DrillUp()
        {
            DrillUpStory.Begin();
        }

        // Appear from condension
        public void DrillAppear()
        {
            DrillAppearStory.Begin();
        }

        // Expand and Disappear
        public void DrillDisappear()
        {
            DrillDisappearStory.Begin();
        }
    }
}
