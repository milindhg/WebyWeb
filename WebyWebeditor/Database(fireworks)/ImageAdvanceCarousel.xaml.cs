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
/*
*	A Image Advance Carousel Demonstratoin in C#
*   from shinedraw.com
*/

namespace ImageAdvanceCarousel
{
    public partial class ImageAdvanceCarousel : UserControl
    {
        List<ImageCarousel> _imageCarousels = new List<ImageCarousel>(); // Store the Added Carousels
        int _currentLayer = 0;                                           // Store the Layer index
        ImageCarousel _currentCarousel;                                  // Current Carousel
        int currentimgindex = 3;

        public ImageAdvanceCarousel()
        {
            InitializeComponent();
            // Add the Button Handlers
            MoveLeftButton.MouseLeftButtonDown += new MouseButtonEventHandler(MoveLeftButton_MouseLeftButtonDown);
            MoveRightButton.MouseLeftButtonDown += new MouseButtonEventHandler(MoveRightButton_MouseLeftButtonDown);
            //DrillDownButton.MouseLeftButtonDown += new MouseButtonEventHandler(DrillDownButton_MouseLeftButtonDown);
            //DrillUpButton.MouseLeftButtonDown += new MouseButtonEventHandler(DrillUpButton_MouseLeftButtonDown);

            imgtext.Text = "images/shiny" + currentimgindex.ToString() +".jpg";
        }


        /////////////////////////////////////////////////////        
        // Handlers 
        /////////////////////////////////////////////////////	


        // Drill Up
        void DrillUpButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            drillUp();
        }

        // Drill Down
        void DrillDownButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            drillDown();
        }

        // Move the Current Carousel to Right
        void MoveRightButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentCarousel != null)
            {
                _currentCarousel.MoveRight();
                if (currentimgindex == 12)
                { }
                else
                {
                    currentimgindex++;
                    imgtext.Text = "images/shiny" + currentimgindex.ToString() + ".jpg";
                }
            }
        }

        // Move the Current Carousel to Left
        void MoveLeftButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (_currentCarousel != null)
            {
                _currentCarousel.MoveLeft();
                if (currentimgindex == 1)
                { }
                else
                {
                    currentimgindex--;
                    imgtext.Text = "images/shiny" + currentimgindex.ToString() + ".jpg";
                }
            }
        }

        /////////////////////////////////////////////////////        
        // Private Methods 
        /////////////////////////////////////////////////////	

        private void drillDown(){
            _currentLayer ++;

            // create new layer if not exist
            if (_currentLayer > _imageCarousels.Count)
            {
                _currentCarousel= new ImageCarousel();
                _imageCarousels.Add(_currentCarousel);
                Holder.Children.Add(_currentCarousel);
                _currentCarousel.Start();
                _currentCarousel.DrillAppear();
            }
            else
            {
                // get the current layer if exist
                _currentCarousel = _imageCarousels[_currentLayer - 1];
                _currentCarousel.DrillAppear();
            }

            // Drill down the next layer (need to check if it exist)
            if (_currentLayer - 2 >= 0)
            {
                ImageCarousel imageCarousel = _imageCarousels[_currentLayer - 2];
                imageCarousel.DrillDown();
            }
        }

        // Drill up the current layer
        private void drillUp()
        {
            if (_currentLayer > 1)
            {
                _currentLayer--;
                _currentCarousel = _imageCarousels[_currentLayer - 1];
                _currentCarousel.DrillUp();

                // remove the last layer
                if (_currentLayer + 1 <= _imageCarousels.Count)
                {
                    ImageCarousel imageCarousel = _imageCarousels[_currentLayer ];
                    imageCarousel.DrillDisappear();
                }
            }
        }

        /////////////////////////////////////////////////////        
        // Public Methods 
        /////////////////////////////////////////////////////	

        // Start by drill the first layer
        public void Start()
        {
            drillDown();
            
        }
    }
}
