using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Browser;
using System.Windows.Threading;
using Database_fireworks_.ServiceReference1;


using Liquid;
using FallingSnow;
using System.Xml;
using System.Text;
using System.IO;

using Liquid.Components;
using Liquid.Components.Internal;
using FullScreenDemo;
using ImageAdvanceCarousel;

namespace MainMenu
{
    public partial class editor : UserControl
    {
        private Image _dragging = null;

        #region fullscreen variables
        private static double APP_WIDTH = 1280;  // Application Width
        private static double APP_HEIGHT = 650; // Application Height
        private bool _scale = false;           // _scale flag

        #endregion

        #region imagecarousel objects
        List<ImageCarousel> _imageCarousels = new List<ImageCarousel>(); // Store the Added Carousels
        int _currentLayer = 0;                                           // Store the Layer index
        ImageCarousel _currentCarousel;                                  // Current Carousel
        int currentimgindex = 3;

        #endregion

        #region private declarations

        private Point _last = new Point();
        //private Point _lasttool = new Point();
        private bool mouse_down = false;

        #endregion

        #region Private Properties for showing formatting buttons status

        private SolidColorBrush _buttonFillStyleApplied = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        private SolidColorBrush _buttonFillStyleNotApplied = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private SolidColorBrush initialtextcolor = new SolidColorBrush(Color.FromArgb(255,0,0,0));
        private Color initialbackcolor = new Color();
        
        //private SpellChecker _spellChecker;
        private bool _ignoreFormattingChanges = false;

        #endregion


        int codeviewflag;   //Flag used to indicate current view; 1=Code view; 0=Design View
        #region Global Variables for capturing values during code conversion
        string _name;   //For capturing name in code generation
        string _value;  //For capturing value in code generation
        #endregion
        Button button_clicked;  //For capturing the button clicked to be applyed correspondingly in formatting
        private DispatcherTimer _timer;  //Variable to keep track of time ont the menu bar
                
        //constructor for editor class
        public editor()
        {
            InitializeComponent();

            //if (Database_fireworks_.status.global_richtext != " ")
            //{
                filename.Text = Database_fireworks_.status.global_filename;
                rtb.RichText = Database_fireworks_.status.global_richtext;
            //}

            //Event hadlers for drag and drop
            LayoutRoot.MouseMove += new MouseEventHandler(Image_MouseMove);
            rtb.ContentDropped += new RichTextBoxEventHandler(richTextBox_ContentDropped);
            buttonimage.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);

            #region timer control for date and time on menubar
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 24);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
            datetime.Text = DateTime.Now.ToString();
            #endregion

            VisualStateManager.GoToState(this, "start", true);  
            codeviewflag = 0;       //to mark current view as design view
            PopulateSnowFlakes();   //To populate stars in the background

            #region imagecarousel event handlers def and functions calls
            MoveLeftButton.MouseLeftButtonDown += new MouseButtonEventHandler(MoveLeftButton_MouseLeftButtonDown);
            MoveRightButton.MouseLeftButtonDown += new MouseButtonEventHandler(MoveRightButton_MouseLeftButtonDown);
            imgtext.Text = "images/shiny" + currentimgindex.ToString() + ".jpg";
            Startimg();
            #endregion

            #region Color initialisation
            initialbackcolor = Color.FromArgb(255,255,255,255);
            tool_bold.Background = _buttonFillStyleNotApplied;
            tool_italic.Background = _buttonFillStyleNotApplied;
            tool_num.Background = _buttonFillStyleNotApplied;
            tool_bul.Background = _buttonFillStyleNotApplied;
            tool_alignl.Background = _buttonFillStyleApplied;
            tool_alignc.Background = _buttonFillStyleNotApplied;
            tool_alignr.Background = _buttonFillStyleNotApplied;

            #endregion

            #region EventHandlers
            rtb.SelectionChanged += new RichTextBoxEventHandler(rtb_SelectionChanged);
            rtb.ShowContextMenu += new RichTextBoxEventHandler(richTextBox_ShowContextMenu);
            rtb.TextPatternMatch += new RichTextBoxEventHandler(richTextBox_TextPatternMatch);
            rtb.StyleCreated += new RichTextBoxEventHandler(rtb_StyleCreated);
            rtb.StyleDeleted += new RichTextBoxEventHandler(rtb_StyleDeleted);
            rtb.ElementWrite += new RichTextBoxEventHandler(rtb_ElementWrite);

            #endregion

            #region image carousel
            ImageAdvanceCarousel.ImageAdvanceCarousel _imageAdvanceCarousel11 = new ImageAdvanceCarousel.ImageAdvanceCarousel();
            LayoutRoot.Children.Insert(0, _imageAdvanceCarousel11);
            _imageAdvanceCarousel11.Start();
            #endregion

            this.Loaded += new RoutedEventHandler(FullScreenDemo_Loaded);   //Event handler for fullscreen

        }

        void _timer_Tick(object sender, EventArgs e)
        {
            datetime.Text = DateTime.Now.ToString();
        }
                       
        #region stars background

        private void PopulateSnowFlakes()
        {
            for (int i = 0; i < 500; i++)
            {
                SnowFlake snowFlake = new SnowFlake();                                
                // 500 and 300 is the width/height of the application
                snowFlake.SetInitialProperties(1200, 800);
                //LayoutRoot.Children.Add(snowFlake);
                starlayoutroot.Children.Add(snowFlake);
            }
        }

        #endregion

        #region Pattern Matching Handling

        private void SetupPatternMatching()
        {
            rtb.TextPatterns.Add(":)");
            rtb.TextPatterns.Add(";)");
            rtb.TextPatterns.Add(":(");
        }

        private void richTextBox_TextPatternMatch(object sender, RichTextBoxEventArgs e)
        {
            switch (e.Parameter.ToString())
            {
                case ":)":
                    e.Parameter = new Image() { Source = new BitmapImage(new Uri("images/happy.png", UriKind.Relative)) };
                    break;
                case ":(":
                    e.Parameter = new Image() { Source = new BitmapImage(new Uri("images/unhappy.png", UriKind.Relative)) };
                    break;
                case ";)":
                    e.Parameter = new Image() { Source = new BitmapImage(new Uri("images/wink.png", UriKind.Relative)) };
                    break;
            }
        }

        #endregion

        #region Menu handlers

        private void testMenu_ItemSelected(object sender, MenuEventArgs e)
        {
            switch (e.Tag.ToString())
            {
                case "open1":
                    OpenFileDialog ofd = new OpenFileDialog()
                    {
                        Filter = "XML Files (*.xml)|*.xml|HTML Files (*.html)|*.html|All Files (*.*)|*.*",
                        Multiselect = false
                    };

                    if (ofd.ShowDialog() == true)
                    {
                        using (StreamReader reader = ofd.File.OpenText())
                        {
                            string data = reader.ReadToEnd();
                            rtb.RichText = data;
                        }
                    }
                    break;
                case "save1":
                    
                    DemoServiceClient webservice = new DemoServiceClient();
                    Database_fireworks_.ServiceReference1.DemoServiceClient webService = new DemoServiceClient();
                    webservice.GetRows1Completed += new EventHandler<GetRows1CompletedEventArgs>(webservice_GetRows1Completed);
                    webservice.GetRows1Async();
                    /////Saving in HTML code------------------------------------------------------
                    //rtb1.Text = ConvertRichTextToHTMLDocument(rtb.RichText);
                    //string myTextFile = rtb1.Text;
                    //HtmlDocument doc = HtmlPage.Document;
                    //HtmlElement downloadData = doc.GetElementById("downloadData");
                    //downloadData.SetAttribute("value", myTextFile);
                    //HtmlElement fileName = doc.GetElementById("fileName");
                    //fileName.SetAttribute("value", "myFile.html");
                    //doc.Submit("generateFileForm");
                    //////////////////////////////-------------------------------------------------
                    break;
                case "save2":
                    ///Saving in HTML code------------------------------------------------------
                    rtb1.Text = ConvertRichTextToHTMLDocument(rtb.RichText);
                    string myTextFile = rtb1.Text;
                    HtmlDocument doc = HtmlPage.Document;
                    HtmlElement downloadData = doc.GetElementById("downloadData");
                    downloadData.SetAttribute("value", myTextFile);
                    HtmlElement fileName = doc.GetElementById("fileName");
                    fileName.SetAttribute("value", "myFile.html");
                    doc.Submit("generateFileForm");
                    ////////////////////////////-------------------------------------------------
                    /////Saving in xml-------------------------------------------------------------
                    //string myrtbFile = rtb.RichText.ToString();
                    //HtmlDocument rtbdoc = HtmlPage.Document;
                    //HtmlElement rtbdownloadData = rtbdoc.GetElementById("downloadData");
                    //rtbdownloadData.SetAttribute("value", myrtbFile);
                    //HtmlElement rtbfileName = rtbdoc.GetElementById("fileName");
                    //rtbfileName.SetAttribute("value", "myFilertb.xml");
                    //rtbdoc.Submit("generateFileForm");
                    /////////////////////////////---------------------------------------------------
                    break;
                case "exit1":
                    textbox1.Text = "first abcd is selected";
                    HtmlPage.Window.Eval("window.close()");
                    break;
                case "undo1":
                    //textbox1.Text = "first abcd is selected";
                    rtb.Undo();
                    break;
                case "redo1":
                    //textbox1.Text = "second pqrs is selected";
                    rtb.Redo();
                    break;
                case "cut1":
                    //textbox1.Text = "second pqrs is selected";
                    rtb.Cut();
                    break;
                case "paste1":
                    //textbox1.Text = "first abcd is selected";
                    rtb.Paste();
                    break;
                case "copy1":
                    //textbox1.Text = "second pqrs is selected";
                    rtb.Copy();
                    break;
                case "delete1":
                    //textbox1.Text = "second pqrs is selected";
                    rtb.Delete(false);
                    break;
                case "selectall1":
                    //textbox1.Text = "second pqrs is selected";
                    rtb.SelectAll();
                    break;
                case "find1":
                    //textbox1.Text = "second pqrs is selected";
                    findPopup.ShowAsModal();
                    break;
                case "Code1":
                    if (codeviewflag == 0)
                    {
                        canvas_code.Visibility = System.Windows.Visibility.Visible;
                        VisualStateManager.GoToState(this, "code_v", true);
                        rtb1.Clear();
                        rtb1.Text = " ";
                        rtb1.Text = ConvertRichTextToHTMLDocument(rtb.RichText);
                        canvas_design.Visibility = System.Windows.Visibility.Collapsed;
                        codeviewflag = 1;
                    }
                    break;
                case "Design1":
                    codeviewflag = 0;
                    canvas_design.Visibility = System.Windows.Visibility.Visible;
                    VisualStateManager.GoToState(this, "design_v", true);
                    canvas_code.Visibility = System.Windows.Visibility.Collapsed;
                    rtb1.RichText = " ";
                    break;
                case "fullscreen":
                    _scale = false;
                    //_scale = !Application.Current.Host.Content.IsFullScreen;
                    Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
                    break;
                case "help1":
                    HtmlPage.Window.Navigate(new Uri("WebHelp.chm", UriKind.Relative));
                    break;                    
            }   

        }

        #endregion

        //code to delete a user's file
        void webservice_GetRows1Completed(object sender, GetRows1CompletedEventArgs e)
        {
            int i;
            DemoServiceClient webservice = new DemoServiceClient();
            for (i = 0; i != e.Result.Count; i++)
                {
                    if (Database_fireworks_.Page.global_uname == e.Result[i].username &&Database_fireworks_.status.global_filename == e.Result[i].filename)
                    {
                        webservice.DeleteRowAsync((Guid)e.Result[i].uid);
                        break;
                    }
                }
            webservice.InsertData1Async(Database_fireworks_.Page.global_uname, Database_fireworks_.status.global_filename, rtb.RichText.ToString());
       }

        #region trial drag and move

        private void myrect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouse_down = true;
            myrect.CaptureMouse();
        }

        private void myrect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouse_down = false;
            myrect.ReleaseMouseCapture();
        }

        private void myrect_MouseMove(object sender, MouseEventArgs e)
        {
            Point newpos;
            Point current = e.GetPosition(LayoutRoot);
            if (mouse_down == true)
            {
                newpos = new Point();
                newpos.X = (double)myrect.GetValue(Canvas.LeftProperty) + (current.X - _last.X);
                newpos.Y = (double)myrect.GetValue(Canvas.TopProperty) + (current.Y - _last.Y);

                myrect.SetValue(Canvas.LeftProperty, newpos.X);
                myrect.SetValue(Canvas.TopProperty, newpos.Y);
            }
            _last = current;
        }

        #endregion

        private void red_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            red.Width += 10;
            red.Height += 10;
        }

        #region Left Side toolbar panes transitions

        private void onclick_toolbar_in(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b.Name == "but_insert")
                VisualStateManager.GoToState(this, "insert", true);
            if (b.Name == "but_format")
                VisualStateManager.GoToState(this, "format", true);
            if (b.Name == "but_table")
                VisualStateManager.GoToState(this, "table", true);
        }

        //Swapping of states of the toolbar on mouseover
        private void onmouseover_toolbar_in(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            if (b.Name == "but_insert")
                VisualStateManager.GoToState(this, "insert", true); //open inser toolbar
            if (b.Name == "but_format")
                VisualStateManager.GoToState(this, "format", true); //open formatting toolbar
            if (b.Name == "but_table")
                VisualStateManager.GoToState(this, "table", true);  //open table formatting toolbar
        }


        #endregion

        #region Code and design tabs
        
        //switch from code view to design view
        private void gotodesignview(object sender, RoutedEventArgs e)
        {
            codeviewflag = 0;
            canvas_design.Visibility = System.Windows.Visibility.Visible;
            VisualStateManager.GoToState(this, "design_v", true);
            canvas_code.Visibility = System.Windows.Visibility.Collapsed;
            rtb1.RichText = " ";

        }

        //swtich from design view to code view
        private void gotocodeview(object sender, RoutedEventArgs e)
        {
            if (codeviewflag == 0)
            {
                canvas_code.Visibility = System.Windows.Visibility.Visible;
                VisualStateManager.GoToState(this, "code_v", true);
                rtb1.Clear();
                rtb1.Text = " ";
                rtb1.Text = ConvertRichTextToHTMLDocument(rtb.RichText);
                canvas_design.Visibility = System.Windows.Visibility.Collapsed;
                codeviewflag = 1;
            }
        }


        #endregion

        #region tooltips (bubble tooltips)

        //code for dynamic changing of tooltip as per the button moved over
        private void show_tooltip(object sender, MouseEventArgs e)
        {

            Point p = e.GetPosition(null);
            Canvas.SetLeft(tooltip_bubble, p.X);
            Canvas.SetTop(tooltip_bubble, p.Y);
            string uripath = "dynamic path set of image";

            Button tool = sender as Button;
            if (tool.Name == "tool_button")
            {
                uripath = "images/button.jpg";
                tooltip_text.Text = "Click to insert button into the page";
                tooltip_bubble.Height = 150;
                tooltip_bubble.Width = 120;
                tooltip_text.Height = 100;
                tooltip_text.Width = 70;
            }
            if (tool.Name == "tool_image")
            {
                uripath = "images/image1.jpg";
                tooltip_text.Text = "Click to insert image into the page";
            }
            if (tool.Name == "tool_label")
            {
                uripath = "images/textblock.jpg";
                tooltip_text.Text = "Click to insert label into the page";
            }
            if (tool.Name == "tool_textbox")
            {
                uripath = "images/textarea.jpg";
                tooltip_text.Text = "Click to insert text field into the page";
            }
            if (tool.Name == "tool_menu")
            {
                uripath = "images/menu1.jpg";
                tooltip_text.Text = "drag and drop to insert a menu into the page";
            }
            if (tool.Name == "tool_dropdown")
            {
                uripath = "images/drop down.jpg";
                tooltip_text.Text = "drag and drop to insert a drop down box into the page";
            }
            if (tool.Name == "tool_radio")
            {
                uripath = "images/radiobutton1.jpg";
                tooltip_text.Text = "Click to insert radio button into the page";
            }
            if (tool.Name == "tool_check")
            {
                uripath = "images/checkbox1.jpg";
                tooltip_text.Text = "Click to insert checkbox into the page";
            }
            if (tool.Name == "tool_hyperlink")
            {
                uripath = "images/hyperlink.jpg";
                tooltip_text.Text = "Click to insert hyperlink into the page";
            }
            if (tool.Name == "tool_shapes")
            {
                uripath = "images/shapes1.jpg";
                tooltip_text.Text = "Click to insert a shape into the page";
            }
            if (tool.Name == "tool_line")
            {
                uripath = "images/line1.jpg";
                tooltip_text.Text = "Click to insert horizontal line into the page";
            }
            if (tool.Name == "tool_date")
            {
                uripath = "images/date2.jpg";
                tooltip_text.Text = "Click to insert a date into the page";
            }
            if (tool.Name == "tool_table")
            {
                uripath = "images/table1.jpg";
                tooltip_text.Text = "Click to insert Table into the page";
            }
            if (tool.Name == "tool_bold")
            {
                uripath = "images/bold.jpg";
                tooltip_text.Text = "Click to make selected text bold";
            }
            if (tool.Name == "tool_italic")
            {
                uripath = "images/italic.jpg";
                tooltip_text.Text = "Click to make the selected text italic";
            }
            if (tool.Name == "tool_underline")
            {
                uripath = "images/underline.jpg";
                tooltip_text.Text = "Click to make selected text underlined";
            }
            if (tool.Name == "tool_alignc")
            {
                uripath = "images/aligncenter.jpg";
                tooltip_text.Text = "Click to align the text to center";
            }
            if (tool.Name == "tool_justify")
            {
                uripath = "images/justify.jpg";
                tooltip_text.Text = "Click to align the text to both left and right";
            }
            if (tool.Name == "tool_alignl")
            {
                uripath = "images/alignleft.jpg";
                tooltip_text.Text = "Click to align the text to left";
            }
            if (tool.Name == "tool_alignr")
            {
                uripath = "images/alignright.jpg";
                tooltip_text.Text = "Click to align the text to right";
            }
            if (tool.Name == "tool_lang_eng")
            {
                uripath = "images/textblock.jpg";
                tooltip_text.Text = "Click to select language English";
            }
            if (tool.Name == "tool_lang_mar")
            {
                uripath = "images/textblock.jpg";
                tooltip_text.Text = "Click to select language Russian";
            }
            if (tool.Name == "tool_addrowup")
            {
                uripath = "images/addrowup.jpg";
                tooltip_text.Text = "Click to insert a row above current row in table";
            }
            if (tool.Name == "tool_addrowdown")
            {
                uripath = "images/addrowdown.jpg";
                tooltip_text.Text = "Click to insert a row below current row in table";
            }
            if (tool.Name == "tool_addcoleft")
            {
                uripath = "images/addcolleft.jpg";
                tooltip_text.Text = "Click to insert a column to the left of current column in table";
            }
            if (tool.Name == "tool_addcolright")
            {
                uripath = "images/addcolright.jpg";
                tooltip_text.Text = "Click to insert a column to the right of current column in table";
            }
            if (tool.Name == "tool_delrow")
            {
                uripath = "images/delrow.jpg";
                tooltip_text.Text = "Click to delete selected row from the table";
            }
            if (tool.Name == "tool_delcol")
            {
                uripath = "images/delcol.jpg";
                tooltip_text.Text = "Click to delete selected column from the table";
            }
            if (tool.Name == "tool_deltable")
            {
                uripath = "images/deltable.jpg";
                tooltip_text.Text = "Click to delete selected column from the table";
            }
            if (tool.Name == "tool_splitcell")
            {
                uripath = "images/splitcell.jpg";
                tooltip_text.Text = "Click to split the current cell into two";
            }
            if (tool.Name == "tool_table_aligntopl")
            {
                uripath = "images/tablealigntopleft.jpg";
                tooltip_text.Text = "Click to align the text to the top left corner of the cell";
            }
            if (tool.Name == "tool_table_aligntopc")
            {
                uripath = "images/tablealigntopcenter.jpg";
                tooltip_text.Text = "Click to center text and align it to the top of the cell";
            }
            if (tool.Name == "tool_table_aligntopr")
            {
                uripath = "images/tablealigntopright.jpg";
                tooltip_text.Text = "Click to align the text to the top right corner of the cell";
            }
            if (tool.Name == "tool_table_alignmidl")
            {
                uripath = "images/tablealignmidleft.jpg";
                tooltip_text.Text = "Click to center text vertically and align it to the left";
            }
            if (tool.Name == "tool_table_alignmidc")
            {
                uripath = "images/tablealignmidcenter.jpg";
                tooltip_text.Text = "Click to center text vertically and horizontally within the cell";
            }
            if (tool.Name == "tool_table_alignmidr")
            {
                uripath = "images/tablealignmidright.jpg";
                tooltip_text.Text = "Click to center text vertically and align it to the right";
            }
            if (tool.Name == "tool_table_alignbotl")
            {
                uripath = "images/tablealignbottomleft.jpg";
                tooltip_text.Text = "Click to center text vertically and align it to the right";
            }
            if (tool.Name == "tool_table_alignbotr")
            {
                uripath = "images/tablealignbottomright.jpg";
                tooltip_text.Text = "Click to center text vertically and align it to the right";
            }
            if (tool.Name == "tool_table_alignbotc")
            {
                uripath = "images/tablealignbottomcenter.jpg";
                tooltip_text.Text = "Click to center text vertically and align it to the right";
            }
            if (tool.Name == "tool_num")
            {
                //uripath = "images/buttelnumberindentation.jpg";
                tooltip_text.Text = "Click to apply numbering";
            }
            if (tool.Name == "tool_bul")
            {
                uripath = "images/buttelnumberindentation.jpg";
                tooltip_text.Text = "Click to apply bullets";
            }
            if (tool.Name == "tool_indent")
            {
                uripath = "images/buttelnumberindentation.jpg";
                tooltip_text.Text = "Increase Indent - Increase the indent level of the paragraph";
            }
            if (tool.Name == "tool_outdent")
            {
                uripath = "images/buttelnumberindentation.jpg";
                tooltip_text.Text = "Decrease Indent - Decrease the indent level of the paragraph";
            }
            if (tool.Name == "tool_sub")
            {
                uripath = "images/buttelnumberindentation.jpg";
                tooltip_text.Text = "Subscript - create small letters below the text baseline";
            }
            if (tool.Name == "tool_super")
            {
                uripath = "images/buttelnumberindentation.jpg";
                tooltip_text.Text = "Superscript - Create small letters above the line of text";
            }
            if (tool.Name == "tool_strike")
            {
                uripath = "images/buttelnumberindentation.jpg";
                tooltip_text.Text = "Strike - Draw line through middle of selected text";
            }
            if (tool.Name == "tool_styles")
            {
                uripath = "images/buttelnumberindentation.jpg";
                tooltip_text.Text = "Click to apply styles";
            }

            Uri uri = new Uri(uripath, UriKind.Relative);
            ImageSource img = new System.Windows.Media.Imaging.BitmapImage(uri);
            tooltip_image.SetValue(Image.SourceProperty, new System.Windows.Media.Imaging.BitmapImage(uri));
            tooltip_bubble.Show();
        }

        //hide tooltip on mouse leave
        private void hide_tooltip(object sender, MouseEventArgs e)
        {
            tooltip_bubble.Close();
        }

        #endregion

        #region style handling

        //setup initial styles
        private void SetupStyles()
        {
            rtb.Styles.Add("H1", new RichTextBoxStyle("H1", "Arial", 28, FontWeights.Bold) { Margin = new Thickness(0, 12, 0, 3) });
            rtb.Styles.Add("H2", new RichTextBoxStyle("H2", "Arial", 24, FontWeights.Bold) { Margin = new Thickness(0, 12, 0, 3) });
            rtb.Styles.Add("H3", new RichTextBoxStyle("H3", "Arial", 22, FontWeights.Bold) { Margin = new Thickness(0, 12, 0, 3) });
            rtb.Styles.Add("Normal", new RichTextBoxStyle("Normal", "Arial", 14, FontWeights.Normal));

            foreach (string styleID in rtb.Styles.Keys)
            {
                AddStyle(styleID);
            }
        }

        //adding custom style dynamically
        private void AddStyle(string styleID)
        {
            if (rtb.Styles[styleID].Link.Length == 0)
            {
                TextBlockPlus textblock = new TextBlockPlus()
                {
                    Text = styleID
                };
                textblock.Tag = styleID;
                textblock.ApplyStyle(rtb.Styles[styleID]);
                selectStyle.Items.Add(textblock);
            }
        }

        //add style to the list after ctyle creation
        private void rtb_StyleCreated(object sender, RichTextBoxEventArgs e)
        {
            AddStyle(((RichTextBoxStyle)e.Parameter).ID);
        }

        //remove style from list on style deletion
        private void rtb_StyleDeleted(object sender, RichTextBoxEventArgs e)
        {
            TextBlockPlus delete = GetStyleTextBlock(((RichTextBoxStyle)e.Parameter).ID);

            if (delete != null)
            {
                selectStyle.Items.Remove(delete);
            }
        }

        //Capture the textblock to which the corresponding style is applied
        private TextBlockPlus GetStyleTextBlock(string styleID)
        {
            TextBlockPlus result = null;

            foreach (TextBlockPlus textblock in selectStyle.Items)
            {
                if (textblock.Tag.ToString() == styleID)
                {
                    result = textblock;
                    break;
                }
            }
            return result;
        }

        //Show the list of styles in a popup
        private void ShowStyle_click(object sender, RoutedEventArgs e)
        {
            stylePopup.Show();
        }

        //apply the corresponding style chosen to be applied from the style list popup
        private void stylePopup_closed(object sender, DialogEventArgs e)
        {
            if (e.Tag.ToString() == "ok")
            {
                string styleID = ((TextBlockPlus)selectStyle.SelectedItem).Tag.ToString();

                if (rtb.SelectionStyle.ID != styleID)
                {
                    ExecuteFormatting(Formatting.Style, styleID);
                }
            }
        }

        #endregion

        #region formatting functions

        //continuously update formatting with the cursor to indicate the formatting applied to the text under cursor
        private void UpdateFormattingControls()
        {
            //initialise as formatting not applied
            tool_bold.Background = _buttonFillStyleNotApplied;
            tool_italic.Background = _buttonFillStyleNotApplied;
            tool_underline.Background = _buttonFillStyleNotApplied;
            tool_bul.Background = _buttonFillStyleNotApplied;
            tool_num.Background = _buttonFillStyleNotApplied;
            tool_alignl.Background = _buttonFillStyleNotApplied;
            tool_alignc.Background = _buttonFillStyleNotApplied;
            tool_alignr.Background = _buttonFillStyleNotApplied;
            tool_hyperlink.Background = _buttonFillStyleNotApplied;
            tool_super.Background = _buttonFillStyleNotApplied;
            tool_sub.Background = _buttonFillStyleNotApplied;
            tool_strike.Background = _buttonFillStyleNotApplied;

            //Mark the bold button as applied if bold applied
            if (rtb.SelectionStyle.Weight == FontWeights.Bold)
            {
                tool_bold.Background = _buttonFillStyleApplied;
            }

            //Mark the italic button as applied if italic applied
            if (rtb.SelectionStyle.Style == FontStyles.Italic)
            {
                tool_italic.Background = _buttonFillStyleApplied;
            }

            //Mark the underline button as applied if underline applied
            if (rtb.SelectionStyle.Decorations == TextDecorations.Underline)
            {
                tool_underline.Background = _buttonFillStyleApplied;
            }

            //Mark the corresponding alignment button as applied if left,center or right alignment applied
            if (rtb.SelectionAlignment == HorizontalAlignment.Left)
            {
                tool_alignl.Background = _buttonFillStyleApplied;
            }
            else if (rtb.SelectionAlignment == HorizontalAlignment.Center)
            {
                tool_alignc.Background = _buttonFillStyleApplied;
            }
            else if (rtb.SelectionAlignment == HorizontalAlignment.Right)
            {
                tool_alignr.Background = _buttonFillStyleApplied;
            }

            //show the list type applied
            if (rtb.SelectionListType != null)
            {
                if (rtb.SelectionListType.Type == BulletType.Bullet)
                {
                    tool_bul.Background = _buttonFillStyleApplied;
                }
                else if (rtb.SelectionListType.Type == BulletType.Number)
                {
                    tool_num.Background = _buttonFillStyleApplied;
                }
            }


            //Mark the hyperlink button as applied if hyperlink applied
            if (rtb.SelectionStyle.Link.Length > 0)
            {
                tool_hyperlink.Background = _buttonFillStyleApplied;
            }

            //Mark the strike button as applied if strike applied
            if (rtb.SelectionStyle.Effect == TextBlockPlusEffect.Strike)
            {
                tool_strike.Background = _buttonFillStyleApplied;
            }

            //Mark the superscript button as applied if superscript applied
            if (rtb.SelectionStyle.Special == RichTextSpecialFormatting.Superscript)
            {
                tool_super.Background = _buttonFillStyleApplied;
            }

            //Mark the subscript button as applied if suubscript applied
            if (rtb.SelectionStyle.Special == RichTextSpecialFormatting.Subscript)
            {
                tool_sub.Background = _buttonFillStyleApplied;
            }

            //set the font family,size,sadow and style applied
            SetSelected(selectFontFamily, rtb.SelectionStyle.Family);
            SetSelected(selectFontSize, rtb.SelectionStyle.Size.ToString());
            SetSelectedByTag(selectShadow, rtb.SelectionStyle.Shadow.ToString());
            selectStyle.SelectedItem = GetStyleTextBlock(rtb.SelectionStyle.ID);

            //Mark the table button as applied if table inserted applied
            bool inTable = (rtb.ActiveTable != null);

            tool_table.IsEnabled = !inTable;        //this is to enable or disable menu items in table toolbar
            tool_edit_table.IsEnabled = inTable;
            tool_addcol.IsEnabled = inTable;
            tool_addrow.IsEnabled = inTable;
            tool_delcol.IsEnabled = inTable;
            tool_delrow.IsEnabled = inTable;          
        }

        private void SetSelected(ComboBox combo, string value)
        {
            if (value != null)
            {
                _ignoreFormattingChanges = true;

                foreach (ComboBoxItem item in combo.Items)
                {
                    if (item.Content.ToString().ToLower() == value.ToLower())
                    {
                        combo.SelectedItem = item;
                        break;
                    }
                }

                _ignoreFormattingChanges = false;
            }
        }

        private void SetSelectedByTag(ComboBox combo, string value)
        {
            if (value != null)
            {
                _ignoreFormattingChanges = true;

                foreach (ComboBoxItem item in combo.Items)
                {
                    if (item.Tag.ToString().ToLower() == value.ToLower())
                    {
                        combo.SelectedItem = item;
                        break;
                    }
                }

                _ignoreFormattingChanges = false;
            }
        }

        #endregion

        #region event handling

        //execute the corresponding formatting applied by clicking the corresponding button on formatting toolbar
        private void ExecuteFormatting(Formatting format, object param)
        {
            if (!_ignoreFormattingChanges)
            {
                rtb.ApplyFormatting(format, param);
                rtb.ReturnFocus();
            }
        }

        //private void richTextBox_CursorMoved(object sender, RichTextBoxEventArgs e)
        //{
        //    lineStatus.Text = "Line: " + richTextBox.LineNumber.ToString() + " of " + richTextBox.NumberOfLines.ToString();
        //}

        //to show the context menu
        private void richTextBox_ShowContextMenu(object sender, RichTextBoxEventArgs e)
        {
            Table table;
            if (e.Source is Table && rtb.ActiveTable != null)
            {
                table = (Table)e.Source;
            }
            e.Cancel = false;
        }

        //to enable and disable alignment button and update formatting applied to the text and display it
        private void rtb_SelectionChanged(object sender, RichTextBoxEventArgs e)
        {
            if (rtb.SelectedText != "")
            {
                //System.Windows.Browser.HtmlPage.Window.Alert("selected");
                tool_alignc.IsEnabled = false;
                tool_alignl.IsEnabled = false;
                tool_alignr.IsEnabled = false;

            }
            else
            {
                tool_alignc.IsEnabled = true;
                tool_alignl.IsEnabled = true;
                tool_alignr.IsEnabled = true;
            }

            UpdateFormattingControls();
        }

        private string BuildPositionInfo(RichTextBoxPosition pos)
        {
            string result = string.Empty;

            if (pos != null)
            {
                result = pos.GlobalIndex.ToString() + " - ";
                if (pos.Element is TextBlockPlus)
                {
                    result += ((TextBlockPlus)pos.Element).Text + " (" + pos.Index.ToString() + ")";
                }
                else
                {
                    result += "[" + pos.Element.ToString() + "]";
                }
            }
            else
            {
                result += "[NULL]";
            }

            return result;
        }

        //to set the font family to selected text
        private void SelectFontFamily_ItemSelected(object sender, EventArgs e)
        {
            if (selectFontFamily != null)
            {
                ExecuteFormatting(Formatting.FontFamily, ((ComboBoxItem)selectFontFamily.SelectedItem).Content.ToString());
            }
        }

        //to set the font size to selected text
        private void SelectFontSize_ItemSelected(object sender, EventArgs e)
        {
            if (selectFontSize != null)
            {
                ExecuteFormatting(Formatting.FontSize, double.Parse(((ComboBoxItem)selectFontSize.SelectedItem).Content.ToString()));
            }
        }

        //to set the font shadow to selected text
        private void SelectShadow_ItemSelected(object sender, EventArgs e)
        {
            if (selectShadow != null)
            {
                switch (((ComboBoxItem)selectShadow.SelectedItem).Tag.ToString())
                {
                    case "Slight":
                        ExecuteFormatting(Formatting.ShadowSlight, "#44888888");
                        break;
                    case "Normal":
                        ExecuteFormatting(Formatting.ShadowNormal, "#44888888");
                        break;
                    default:
                        ExecuteFormatting(Formatting.RemoveShadow, null);
                        break;
                }
            }
        }

        //to set the font color to selected text
        private void SelectColor_ItemSelected(object sender, EventArgs e)
        {
            if (selectColor != null)
            {
                ExecuteFormatting(Formatting.Foreground, selectColor.Selected.ToString());
            }
        }

        //to set the text highlighting color to selected text
        private void SelectBackground_ItemSelected(object sender, EventArgs e)
        {
            if (selectBackground != null)
            {
                ExecuteFormatting(Formatting.Background, selectBackground.Selected.ToString());
            }
        }

        //to apply the zooming applied/selected
        private void Zoom_ItemSelected(object sender, EventArgs e)
        {
            if (zoom != null)
            {
                ComboBoxItem item = (ComboBoxItem)zoom.SelectedItem;

                rtb.Zoom = double.Parse(item.Content.ToString().Trim('%')) * 0.01;
                rtb.ReturnFocus();
            }
        }

        private void Painter_Click(object sender, RoutedEventArgs e)        //to be seen whats this
        {
            rtb.Painter();
            rtb.ReturnFocus();
        }

        //to undo the last done change to the document
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            rtb.Undo();
            rtb.ReturnFocus();
        }

        //to redo the last done change to the document
        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            rtb.Redo();
            rtb.ReturnFocus();
        }

        //to search text in the document
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            rtb.Find(searchTerms.Text);
            rtb.ReturnFocus();
        }

        //to show the hyperlink
        private void Link_Click(object sender, RoutedEventArgs e)
        {
            enterURL.Show();
        }

        //to apply bold to the selected text
        private void implbold(object sender, RoutedEventArgs e)
        {
            Formatting format = (tool_bold.Background == _buttonFillStyleNotApplied ? Formatting.Bold : Formatting.RemoveBold);
            ExecuteFormatting(format, null);
            if (rtb.SelectionStyle.Weight == FontWeights.Bold)
            {
                tool_bold.Background = _buttonFillStyleApplied;
            }
            else
                tool_bold.Background = _buttonFillStyleNotApplied;
        }

        //to apply italic to the selected text
        private void implitalic(object sender, RoutedEventArgs e)
        {
            Formatting format = (tool_italic.Background == _buttonFillStyleNotApplied ? Formatting.Italic : Formatting.RemoveItalic);
            ExecuteFormatting(format, null);

            if (rtb.SelectionStyle.Style == FontStyles.Italic)
            {
                tool_italic.Background = _buttonFillStyleApplied;
            }
            else
                tool_italic.Background = _buttonFillStyleNotApplied;

        }

        //to apply underline to the selected text
        private void implunder(object sender, RoutedEventArgs e)
        {
            if (rtb.SelectionStyle.Decorations == TextDecorations.Underline)
            {
                tool_underline.Background = _buttonFillStyleApplied;
            }
            else
                tool_underline.Background = _buttonFillStyleNotApplied;

            rtb.ApplyFormatting(tool_underline.Background == _buttonFillStyleNotApplied ? Formatting.Underline : Formatting.RemoveUnderline, null);
            rtb.ReturnFocus();
        }

        //to apply center alignment
        private void implalignc(object sender, RoutedEventArgs e)
        {
            Formatting format = (tool_alignc.Background == _buttonFillStyleNotApplied ? Formatting.AlignCenter : Formatting.AlignLeft);
            rtb.ApplyFormatting(tool_alignc.Background == _buttonFillStyleNotApplied ? Formatting.AlignCenter : Formatting.AlignLeft, null);
            rtb.ReturnFocus();
            if (rtb.SelectionAlignment == HorizontalAlignment.Center)
            {
                tool_alignc.Background = _buttonFillStyleApplied;
                tool_alignr.Background = _buttonFillStyleNotApplied;
                tool_alignl.Background = _buttonFillStyleNotApplied;
            }
            else
            {
                //tool_alignr.Background = _buttonFillStyleNotApplied;
            }

        }

        //to apply left alignment
        private void implalignl(object sender, RoutedEventArgs e)
        {
            Formatting format = (tool_alignl.Background == _buttonFillStyleNotApplied ? Formatting.AlignLeft : Formatting.AlignLeft);
            rtb.ApplyFormatting(Formatting.AlignLeft, null);
            rtb.ReturnFocus();
            if (rtb.SelectionAlignment == HorizontalAlignment.Left)
            {
                tool_alignl.Background = _buttonFillStyleApplied;
                tool_alignr.Background = _buttonFillStyleNotApplied;
                tool_alignc.Background = _buttonFillStyleNotApplied;
            }
            else
            {
                //tool_alignl.Background = _buttonFillStyleNotApplied;
            }

        }

        //to apply right alignment
        private void implalignr(object sender, RoutedEventArgs e)
        {
            Formatting format = (tool_alignr.Background == _buttonFillStyleNotApplied ? Formatting.AlignRight : Formatting.AlignLeft);
            rtb.ApplyFormatting(tool_alignr.Background == _buttonFillStyleNotApplied ? Formatting.AlignRight : Formatting.AlignLeft, null);
            rtb.ReturnFocus();
            if (rtb.SelectionAlignment == HorizontalAlignment.Right)
            {
                tool_alignr.Background = _buttonFillStyleApplied;
                tool_alignc.Background = _buttonFillStyleNotApplied;
                tool_alignl.Background = _buttonFillStyleNotApplied;
            }
            else
            {
                tool_alignr.Background = _buttonFillStyleNotApplied;
            }

        }

        //to apply superscript to the selected text
        private void implsuperscr(object sender, RoutedEventArgs e)
        {
            if (rtb.SelectionStyle.Special == RichTextSpecialFormatting.Superscript)
            {
                tool_super.Background = _buttonFillStyleApplied;
            }
            else
                tool_super.Background = _buttonFillStyleNotApplied;

            Formatting format = (tool_super.Background == _buttonFillStyleNotApplied ? Formatting.SuperScript : Formatting.RemoveSpecial);

            if (!_ignoreFormattingChanges)
            {
                rtb.ApplyFormatting(format, null);
                rtb.ReturnFocus();
            }

        }

        //to apply subscript to the selected text
        private void implsubscr(object sender, RoutedEventArgs e)
        {
            if (rtb.SelectionStyle.Special == RichTextSpecialFormatting.Subscript)
            {
                tool_sub.Background = _buttonFillStyleApplied;
            }
            else
                tool_sub.Background = _buttonFillStyleNotApplied;

            //rtb.SelectionStyle.Weight = FontWeights.Black;
            Formatting format = (tool_sub.Background == _buttonFillStyleNotApplied ? Formatting.SubScript : Formatting.RemoveSpecial);

            if (!_ignoreFormattingChanges)
            {
                rtb.ApplyFormatting(format, null);
                rtb.ReturnFocus();
            }

        }

        //to apply strikethrough to the selected text
        private void implstrike(object sender, RoutedEventArgs e)
        {
            if (rtb.SelectionStyle.Effect == TextBlockPlusEffect.Strike)
            {
                tool_strike.Background = _buttonFillStyleApplied;
            }
            else
                tool_strike.Background = _buttonFillStyleNotApplied;

            //rtb.SelectionStyle.Weight = FontWeights.Black;
            Formatting format = (tool_strike.Background == _buttonFillStyleNotApplied ? Formatting.Strike : Formatting.RemoveStrike);

            if (!_ignoreFormattingChanges)
            {
                rtb.ApplyFormatting(format, null);
                rtb.ReturnFocus();
            }

        }

        //to apply numbered list to the text
        private void implnumberlist(object sender, RoutedEventArgs e)
        {
            Formatting format = (tool_num.Background == _buttonFillStyleNotApplied ? Formatting.NumberList : Formatting.RemoveNumber);

            if (!_ignoreFormattingChanges)
            {
                rtb.ApplyFormatting(format, null);
                rtb.ReturnFocus();
            }
            if (rtb.SelectionListType != null)
            {
                if (rtb.SelectionListType.Type == BulletType.Number)
                {
                    tool_num.Background = _buttonFillStyleApplied;
                    tool_bul.Background = _buttonFillStyleNotApplied;
                }
            }
            else
                tool_num.Background = _buttonFillStyleNotApplied;

        }

        //to apply bulleted to the text
        private void implbulletlist(object sender, RoutedEventArgs e)
        {
            Formatting format = (tool_bul.Background == _buttonFillStyleNotApplied ? Formatting.BulletList : Formatting.RemoveNumber);

            if (!_ignoreFormattingChanges)
            {
                rtb.ApplyFormatting(format, null);
                rtb.ReturnFocus();
            }
            if (rtb.SelectionListType != null)
            {
                if (rtb.SelectionListType.Type == BulletType.Bullet)
                {
                    tool_bul.Background = _buttonFillStyleApplied;
                    tool_num.Background = _buttonFillStyleNotApplied;
                }
            }
            else
                tool_bul.Background = _buttonFillStyleNotApplied;

        }

        //to apply indentation to the text
        private void implindent(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.Indent, null);
        }

        //to decrease indentation to the selected text
        private void imploutdent(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.Outdent, null);
        }

        //to paste the text on clipboard
        private void implpaste(object sender, RoutedEventArgs e)
        {
            rtb.Paste();
            rtb.ReturnFocus();
        }

        //to copy the selected text
        private void implcopy(object sender, RoutedEventArgs e)
        {
            rtb.Copy();
            rtb.ReturnFocus();

        }

        //to cut the selected text
        private void implcut(object sender, RoutedEventArgs e)
        {
            rtb.Cut();
            rtb.ReturnFocus();

        }

        //to delete the selected text
        private void impldelete(object sender, RoutedEventArgs e)
        {
            rtb.Delete(false);
            rtb.ReturnFocus();

        }

        //to undo the last done changes to the document
        private void implundo(object sender, RoutedEventArgs e)
        {
            rtb.Undo();
            rtb.ReturnFocus();

        }

        //to redo the last done changes
        private void implredo(object sender, RoutedEventArgs e)
        {
            rtb.Redo();
            rtb.ReturnFocus();

        }

        //to search the text in the document
        private void implfind(object sender, RoutedEventArgs e)
        {
            findPopup.ShowAsModal();            
        }

        #region table related

        private void InsertTable_Click(object sender, RoutedEventArgs e)
        {
            tableRows.IsEnabled = true;
            tableColumns.IsEnabled = true;

            insertEditTableDialog.Title = "Insert Table";
            insertEditTableDialog.Buttons = DialogButtons.Close;
            insertEditTableDialog.CreateButton(DialogButtons.Custom, "Insert", "insert");
            insertEditTableDialog.ShowAsModal();
        }

        private void EditTable_Click(object sender, RoutedEventArgs e)
        {
            foreach (ComboBoxItem item in tableStyle.Items)
            {
                if (item.Tag.ToString() == rtb.ActiveTable.StyleID)
                {
                    tableStyle.SelectedItem = item;
                    break;
                }
            }

            tableRows.Text = rtb.ActiveTable.RowDefinitions.Count.ToString();
            tableColumns.Text = rtb.ActiveTable.ColumnDefinitions.Count.ToString();
            tableRows.IsEnabled = false;
            tableColumns.IsEnabled = false;

            insertEditTableDialog.Title = "Edit Table";
            insertEditTableDialog.Buttons = DialogButtons.Apply | DialogButtons.Close;
            insertEditTableDialog.ShowAsModal();
        }

        private void insertEditTable_Closed(object sender, DialogEventArgs e)
        {
            Table selectedTable = rtb.ActiveTable;
            int rows = 1;
            int columns = 1;
            string styleID;

            if (insertEditTableDialog.Result != DialogButtons.Close && insertEditTableDialog.Result != DialogButtons.None)
            {
                styleID = tablePreview.Tag.ToString();
                int.TryParse(tableRows.Text, out rows);
                int.TryParse(tableColumns.Text, out columns);

                if (!rtb.TableStyles.ContainsKey(styleID))
                {   // Create a new table style if not present
                    rtb.TableStyles.Add(styleID, new RichTextBoxTableStyle(styleID, tablePreview.Background,
                        tablePreview.CellFill, tablePreview.HeaderFill, tablePreview.BorderBrush,
                        tablePreview.BorderThickness, tablePreview.CellBorderBrush, tablePreview.CellBorderThickness, 1));
                }

                switch (e.Tag.ToString())
                {
                    case "insert":
                        rtb.InsertTable(rows, columns, tablePreview.HeaderRows, tablePreview.HeaderColumns, styleID);
                        break;
                    case "apply":
                        selectedTable.HeaderColumns = tablePreview.HeaderColumns;
                        selectedTable.HeaderRows = tablePreview.HeaderRows;
                        rtb.TableStyles[styleID].ApplyToTable(selectedTable);
                        break;
                }
            }
        }

        private void tableStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tableStyle != null)
            {
                if (tableStyle.SelectedItem != null)
                {
                    Table borderItem = (Table)((ComboBoxItem)tableStyle.SelectedItem).Content;

                    if (borderItem != null)
                    {
                        tablePreview.Tag = ((ComboBoxItem)tableStyle.SelectedItem).Tag;
                        tablePreview.Background = borderItem.Background;
                        tablePreview.HeaderColumns = borderItem.HeaderColumns;
                        tablePreview.HeaderRows = borderItem.HeaderRows;
                        tablePreview.BorderBrush = borderItem.BorderBrush;
                        tablePreview.BorderThickness = borderItem.BorderThickness;
                        tablePreview.CellBorderBrush = borderItem.CellBorderBrush;
                        tablePreview.CellBorderThickness = borderItem.CellBorderThickness;
                        tablePreview.HeaderFill = borderItem.HeaderFill;
                        tablePreview.CellFill = borderItem.CellFill;
                    }
                }
            }
        }

        private void table_formatting(object sender, RoutedEventArgs e)
        {
            button_clicked = sender as Button;
            if (button_clicked.Name == "tool_addrow")
            {
                rtb.InsertTableRow(true);
            }
            if (button_clicked.Name == "tool_addcol")
            {
                rtb.InsertTableColumn(true);
            }
            if (button_clicked.Name == "tool_delcol")
            {
                rtb.DeleteTableColumn();
            }
            if (button_clicked.Name == "tool_delrow")
            {
                rtb.DeleteTableRow();
            }
            if (button_clicked.Name == "tool_deltable")
            {
                rtb.Delete(false);
            }
        }

        #endregion

        private void RichTextBox_LinkClicked(object sender, RichTextBoxEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(e.Parameter.ToString()), "_blank");
        }

        private void EnterURL_Closed(object sender, EventArgs e)
        {
            Formatting format = (tool_hyperlink.Background == _buttonFillStyleNotApplied ? Formatting.Link : Formatting.RemoveLink);
            string link = (format == Formatting.Link ? url.Text : "");

            if (!link.StartsWith("http://"))
            {
                link = "http://" + link;
            }

            if (enterURL.Result == DialogButtons.OK)
            {
                ExecuteFormatting(format, link);
            }
        }


        private void preview_Click(object sender, RoutedEventArgs e)
        {
            previewRichText.RichText = rtb.RichText;
            previewPopup.ShowAsModal();
        }


        #endregion

        #region Public Methods for code conversion

        //method for code conversion from XML to HTML
        public static string[] ConvertRichTextToHTML(string richTextXML)
        {
            Dictionary<string, RichTextBoxStyle> styleList = new Dictionary<string, RichTextBoxStyle>();
            RichTextBoxStyle style;

            string m1 = "mmm";              //temporary variable for code conversion
            string m2 = "<LiquidRichText5"; //temporary variable for code conversion
            bool retval;                    //to store the returnvalue
            int indexstart=0;               //variable to keep a track of indexstart while parsing the code
            int indexend;                   //variable to keep a track of indexend while parsing the code
            int listflag = 0;               //to indicate listing applied to the text if any
            string type="garbage";
            int nlcnt = 0;                  //flag to indicate newline

            XmlReader reader;               //XML reader which contains the complete document XML code
            string styleID;                 //style id to be used in styles
            string temp;                    //temporary variable for code conversion
            string tag;                     //temporary variable for code conversion
            string optional;
            StringBuilder html = new StringBuilder();   //html string to contain the HTML body part code
            StringBuilder styles = new StringBuilder(); //html string to contain the HTML style part code
            bool nlonoff = new bool();                  //newline flag
            nlonoff = false;                            //newline flag initialisation

            //reader initialisation to contain the XML format code of the document
            reader = XmlReader.Create(new StringReader(richTextXML));   
            //reading the reader(xml document)
            reader.Read();
            

            //parsing the xml document till the end of the document
            while (reader.Read())
            {
                //check wheter current parsing token is an element
                if (reader.NodeType == XmlNodeType.Element)
                {
                    temp = reader.Name.ToLower();
                    
                    //check whether current element is a style element
                    //and then to append the corresponding style tag to the html document
                    if (temp == RichTextBox.StyleElement)
                    {
                        style = new RichTextBoxStyle(reader);
                        styleList.Add(style.ID, style);

                        if (IsStyleAHeading(style.ID.ToLower()))
                        {
                            tag = "";
                        }
                        else
                        {
                            tag = ".";
                        }

                        styles.Append(tag + style.ID + " {");

                        styles.Append("font-family:" + style.Family + "; ");
                        styles.Append("font-size:" + style.Size.ToString() + "px; ");
                        styles.Append("text-align:" + style.Alignment.ToString() + "; ");
                        styles.Append("color:#" + ((SolidColorBrush)style.Foreground).Color.ToString().Substring(3) + "; ");

                        if (style.Weight == FontWeights.Bold)
                        {
                            styles.Append("font-weight:bold; ");
                        }
                        if (style.Decorations == TextDecorations.Underline)
                        {
                            styles.Append("text-decoration:underline; ");
                        }
                        if (style.Style == FontStyles.Italic)
                        {
                            styles.Append("font-style:italic; ");
                        }
                        if (style.Special == RichTextSpecialFormatting.Subscript)
                        {
                            styles.Append("vertical-align: sub; ");
                        }
                        else if (style.Special ==  RichTextSpecialFormatting.Superscript)
                        {
                            styles.Append("vertical-align: super; ");                        
                        }
                        else if (style.Effect == TextBlockPlusEffect.Strike)
                        {
                            styles.Append("text-decoration: line-through; ");                        
                        }

                        styles.Append(" }\r\n");
                    }
                        //to check whether current element is a text element (normal text in the document)
                    else if (temp == RichTextBox.TextElement)
                    {
                        styleID = reader.GetAttribute("Style");
                        reader.Read();

                        optional = "";
                        if (IsStyleAHeading(styleID.ToLower()))
                        {
                            tag = styleID.ToLower();
                        }
                        else if (styleList[styleID].Link.Length > 0)
                        {
                            tag = "a";
                            optional = " href='" + styleList[styleID].Link + "' class='" + styleID + "'";
                        }
                        else
                        {
                            if (nlonoff)
                            {
                                tag = "p";
                                nlonoff = false;
                            }
                            else
                            {
                                tag = "span";
                            }
                            optional = " class='" + styleID + "'";
                        }

                        if (reader.NodeType == XmlNodeType.CDATA)
                        {
                             
                            string tempreaderval,temp1;
                            tempreaderval = reader.Value;
                            temp1=tempreaderval.Replace(" ", "&nbsp;");

                            if(tag=="p")
                                html.Append("<" + tag + optional + ">" + temp1);
                            else
                                html.Append("<" + tag + optional + ">" + temp1 + "</" + tag + ">");
                        }
                    }
                        //to check whether current element is a newline
                    else if (temp == RichTextBox.NewlineElement)
                    {
                        if (listflag == 1)
                        {
                            nlcnt++;
                        }
                        if (listflag == 0)
                        {
                            reader.Read();
                            html.Append("</p>");

                            html.Append("<br />");
                            nlonoff = true;
                        }
                        if (listflag == 1 && nlcnt > 2)
                        {
                            if(type=="Bullet")
                                html.Append("</ul>");
                            if (type == "Number")
                                html.Append("</ol>");
                            if (type == "Indent")
                                html.Append("</blockquote>");
                            listflag = 0;
                        }
                    }
                        //to check whether current element is a XAML element
                    else if (temp == RichTextBox.XamlElement)
                    {

                        //XAML element parsing
                        reader.Read();
                        temp=reader.Name;
                        if (listflag == 1 && temp != "liquid:Bullet")
                        {
                            if (type == "Bullet")
                                html.Append("</ul>");
                            if (type == "Number")
                                html.Append("</ol>");
                            if (type == "Indent")
                                html.Append("</blockquote>");
                            listflag = 0;
                        }
                            //to insert image code if an image is inserted
                        if (temp == "Image")
                        {
                            string source;
                            string width;
                            string height;

                            m1 = richTextXML;
                            indexstart = m1.IndexOf("Width=\"", indexstart);
                            indexend = m1.IndexOf("Height", indexstart);
                            m2 = m1.Substring(indexstart + 7, (indexend - (indexstart + 7) - 2));
                            width = m2;     //to get image width
                            indexstart = m1.IndexOf("Height=\"", indexstart);
                            indexend = m1.IndexOf("Source", indexstart);
                            m2 = m1.Substring(indexstart + 8, (indexend - (indexstart + 8) - 2));
                            height = m2;    //to get image height
                            retval = m1.Contains("Source=\"");

                            indexstart = m1.IndexOf("Source=\"",indexstart);
                            indexend = m1.IndexOf(".jpg\"",indexstart);
                            m2 = m1.Substring(indexstart + 8, (indexend - indexstart - 4));
                            source = m2;    //to get image source

                            if (source == "images/HR1.jpg")
                            {
                                html.Append("<HR />");
                            }
                            else
                            {
                                html.Append("<img src=\"" + source + "\" Width=\"" + width + "\" Height=\"" + height + "\" />");
                            }
                            indexstart = indexend + 2;

                        }
                        //to insert button code if an button is inserted
                        if (temp == "Button")
                        {
                            string name;
                            string value;

                            m1 = richTextXML;
                            retval = m1.Contains("Name=\"");

                            indexstart = m1.IndexOf("Name=\"", indexstart);
                            indexend = m1.IndexOf("Content=\"", indexstart);
                            m2 = m1.Substring(indexstart + 6, (indexend - (indexstart + 6) - 2));
                            name = m2;  //to get button name

                            indexstart = m1.IndexOf("Content=\"", indexstart);
                            indexend = m1.IndexOf("\"", indexstart + 9);
                            m2 = m1.Substring(indexstart + 9, (indexend - (indexstart + 9)));
                            value = m2; //to get the button value (caption)
                            html.Append("<Input type=\"Button\" name=\"" + name + "\" value=\"" + value + "\" />");
                            indexstart = indexend + 8;
                        }
                        if (temp == "TextBox")
                        {
                            string name;
                            string value;

                            m1 = richTextXML;
                            retval = m1.Contains("Name=\"");

                            indexstart = m1.IndexOf("Name=\"", indexstart);
                            indexend = m1.IndexOf("Text=\"", indexstart);
                            m2 = m1.Substring(indexstart + 6, (indexend - (indexstart + 6) - 2));
                            name = m2;  //to get the texbox name

                            indexstart = m1.IndexOf("Text=\"", indexstart);
                            indexend = m1.IndexOf("\"", indexstart + 6);
                            m2 = m1.Substring(indexstart + 6, (indexend - (indexstart + 6)));
                            value = m2; //to get the textbox text
                            html.Append("<Input type=\"text\" name=\"" + name + "\" value=\"" + value + "\" />");
                            indexstart = indexend + 8;
                        }
                        if (temp == "TextBlock")
                        {
                            string name;
                            string value;

                            m1 = richTextXML;
                            retval = m1.Contains("Name=\"");

                            indexstart = m1.IndexOf("Name=\"", indexstart);
                            indexend = m1.IndexOf("Text=\"", indexstart);
                            m2 = m1.Substring(indexstart + 6, (indexend - (indexstart + 6) - 2));
                            name = m2; //to get the label name

                            indexstart = m1.IndexOf("Text=\"", indexstart);
                            indexend = m1.IndexOf("\"", indexstart + 6);
                            m2 = m1.Substring(indexstart + 6, (indexend - (indexstart + 6)));
                            value = m2; //to get the label value
                            html.Append("<label> " + value + " </label>");
                            indexstart = indexend + 8;
                        }
                        if (temp == "RadioButton")
                        {
                            string name;
                            string value;
                            string _checked;

                            m1 = richTextXML;
                            retval = m1.Contains("Name=\"");

                            indexstart = m1.IndexOf("Name=\"", indexstart);
                            indexend = m1.IndexOf("Content=\"", indexstart);
                            m2 = m1.Substring(indexstart + 6, (indexend - (indexstart + 6) - 2));
                            name = m2;  //to get the radiobutton name

                            indexstart = m1.IndexOf("Content=\"", indexstart);
                            indexend = m1.IndexOf("IsChecked=\"", indexstart + 9);
                            m2 = m1.Substring(indexstart + 9, (indexend - (indexstart + 9) - 2));
                            value = m2; //to get the radiobutton caption
                            indexstart = m1.IndexOf("IsChecked=\"", indexstart);
                            indexend = m1.IndexOf("\"", indexstart + 11);
                            m2 = m1.Substring(indexstart + 11, (indexend - (indexstart + 11)));
                            if (m2 == "True")
                            {
                                _checked = "checked";
                                html.Append("<Input type=\"Radio\" name=\"" + name + "\" value=\"" + value + "\" checked=\"" + _checked + "\" >" + value + "</input>");
                            }
                            else
                                html.Append("<Input type=\"Radio\" name=\"" + name + "\" value=\"" + value + "\" >" + value + "</input>");

                            indexstart = indexend + 8;
                        }
                        if (temp == "CheckBox")
                        {
                            string name;
                            string value;
                            string _checked;

                            m1 = richTextXML;
                            retval = m1.Contains("Name=\"");

                            indexstart = m1.IndexOf("Name=\"", indexstart);
                            indexend = m1.IndexOf("Content=\"", indexstart);
                            m2 = m1.Substring(indexstart + 6, (indexend - (indexstart + 6) - 2));
                            name = m2; //to get the checkbox name

                            indexstart = m1.IndexOf("Content=\"", indexstart);
                            indexend = m1.IndexOf("IsChecked=\"", indexstart + 9);
                            m2 = m1.Substring(indexstart + 9, (indexend - (indexstart + 9) - 2));
                            value = m2; //to get the checkbox caption
                            indexstart = m1.IndexOf("IsChecked=\"", indexstart);
                            indexend = m1.IndexOf("\"", indexstart + 11);
                            m2 = m1.Substring(indexstart + 11, (indexend - (indexstart + 11)));
                            if (m2 == "True")
                            {
                                _checked = "checked";
                                html.Append("<Input type=\"checkbox\" name=\"" + name + "\" value=\"" + value + "\" checked=\"" + _checked + "\" >" + value + "</input>");
                            }
                            else
                                html.Append("<Input type=\"checkbox\" name=\"" + name + "\" value=\"" + value + "\" >" + value + "</input>");

                            indexstart = indexend + 8;
                        }
                        if (temp == "HyperlinkButton")
                        {
                            string name;
                            string value;

                            m1 = richTextXML;
                            retval = m1.Contains("Name=\"");

                            indexstart = m1.IndexOf("Name=\"", indexstart);
                            indexend = m1.IndexOf("NavigateUri=\"", indexstart);
                            m2 = m1.Substring(indexstart + 6, (indexend - (indexstart + 6) - 2));
                            name = m2; //to get the hyperlink text

                            indexstart = m1.IndexOf("NavigateUri=\"", indexstart);
                            indexend = m1.IndexOf("\"", indexstart + 13);
                            m2 = m1.Substring(indexstart + 13, (indexend - (indexstart + 13)));
                            value = m2; //to get the hyperlink URL
                            html.Append("<a href=\"" + value + "\" >" + name + " </a>");
                            indexstart = indexend + 8;
                        }

                        if (temp == "liquid:Bullet")
                        {
                            string number;

                            m1 = richTextXML;
                            retval = m1.Contains("Type=\"");

                            indexstart = m1.IndexOf("Type=\"", indexstart);
                            indexend = m1.IndexOf("Number=\"", indexstart);
                            m2 = m1.Substring(indexstart + 6, (indexend - (indexstart + 6) - 2));
                            type = m2;  //to get the list type

                            indexstart = m1.IndexOf("Number=\"", indexstart);
                            indexend = m1.IndexOf("\"", indexstart + 8);
                            m2 = m1.Substring(indexstart + 8, (indexend - (indexstart + 8)));
                            number = m2;
                            reader.Read();
                            temp = reader.Name;
                            reader.Read();
                            temp = reader.Name;
                            reader.Read();
                            temp = reader.Name;

                            if (number == "1")
                            {
                                if (type == "Bullet")
                                {
                                    html.Append(" <ul>");
                                }
                                if (type == "Number")
                                {
                                    html.Append(" <ol>");
                                }
                                if (type == "Indent")
                                {
                                    html.Append(" <blockquote>");
                                }
                            }
                            listflag = 1;
                            html.Append(" <li> ");

                            reader.Read();
                            temp = reader.Name;

                            //to get the text in list
                            if (reader.NodeType == XmlNodeType.CDATA)
                            {
                                string tempreaderval, temp1;
                                tempreaderval = reader.Value;
                                temp1 = tempreaderval.Replace(" ", "&nbsp;");
                                html.Append(temp1);

                            }
                            html.Append(" </li> ");
                            reader.Read();
                            nlcnt = 1;

                        }

                        reader.Read();
                        temp = reader.Name;

                        //to check table if inserted
                        if (temp == "liquid:Table")
                        {

                            int rowcnt=0;
                            int colcnt=0;

                            reader.Read();
                            temp = reader.Name;

                            reader.Read();
                            temp = reader.Name;
                            if (temp == "Grid.ColumnDefinitions")
                            {
                                temp = "gar";
                                //to count the no. of columns in the table
                                while (temp != "Grid.ColumnDefinitions")
                                {
                                    reader.Read();
                                    temp = reader.Name;
                                    reader.Read();
                                    temp = reader.Name;
                                    if (temp == "ColumnDefinition")
                                    {
                                        colcnt++;
                                    }
                                }

                                    reader.Read();
                                    temp = reader.Name;

                                    reader.Read();
                                    temp = reader.Name;

                                    temp = "gar";
                                    //to count the no. of columns in the table
                                    while (temp != "Grid.RowDefinitions")
                                    {
                                        reader.Read();
                                        temp = reader.Name;
                                        reader.Read();
                                        temp = reader.Name;
                                        if (temp == "RowDefinition")
                                        {
                                            rowcnt++;
                                        }
                                    }
                                
                            }

                            string udata;
                            
                            html.Append("<table height=\"300\" width=\"500\" border=\"2\"> ");
                            for(int i=1;i<=rowcnt;i++)
                            {
                                html.Append(" <tr> ");
                                for(int j=1;j<=colcnt;j++)
                                {
                                    html.Append(" <td> ");

                                    m1 = richTextXML;
                                    indexstart = m1.IndexOf("liquid:RichTextBlock", indexstart);
                                    indexend = m1.IndexOf("/liquid:RichTextBlock", indexstart);

                                    //to get the text inside th cell of table
                                    int tempindex;
                                    tempindex = indexend;
                                    indexstart = m1.IndexOf("![CDATA[", indexstart,(indexend - (indexstart + 8)));
                                    if (indexstart < 0)
                                    {
                                        html.Append(" &nbsp; ");
                                        indexstart = tempindex + 2;
                                    }
                                    else
                                    {
                                        indexend = m1.IndexOf("]]", indexstart);
                                        m2 = m1.Substring(indexstart + 8, (indexend - (indexstart + 8)));
                                        udata = m2;
                                        indexstart = tempindex + 2;
                                        html.Append(udata);
                                    }
                                    html.Append(" </td> ");                                    
                                }
                                html.Append("</tr> ");

                            }
                                html.Append("</table> ");
                        }

                    }
                }
            }

            reader.Close();

            return new string[] { styles.ToString(), html.ToString() };
        }

        public static string ConvertRichTextToHTMLDocument(string richTextXML)
        {
            string[] temp = ConvertRichTextToHTML(richTextXML);

            return "<html><head><style>" + temp[0] + "</style></head><body>" + temp[1] + "</body></html>";
        }

        #endregion

        #region Private Methods for code conversion

        private static bool IsStyleAHeading(string styleID)
        {
            return (styleID == "h1" || styleID == "h2" || styleID == "h3" || styleID == "h4");
        }

        #endregion

        #region Insert functionality

        //to show insert image popup
        private void insertimage(object sender, EventArgs e)
        {
            fader.Visibility = System.Windows.Visibility.Visible;
            VisualStateManager.GoToState(this, "modalfade", true);
            image_popup.ShowAsModal();                       
        }

        //to recognise and insert tag attributes
        void rtb_ElementWrite(object sender, RichTextBoxEventArgs e)
        {
            switch (e.Format)
            {
                case Format.XML:
                    if (e.Source is Button)
                    {
                        e.Parameter = "Name=\"" + e.Source.GetValue(Button.NameProperty).ToString() + "\"" + " Content=\"" + e.Source.GetValue(Button.ContentProperty).ToString() + "\"";
                    }
                    if (e.Source is RadioButton)
                    {
                        e.Parameter = "Name=\"" + e.Source.GetValue(RadioButton.NameProperty).ToString() + "\"" + " Content=\"" + e.Source.GetValue(RadioButton.ContentProperty).ToString() + "\" IsChecked=\"" + e.Source.GetValue(RadioButton.IsCheckedProperty).ToString() + "\" ";
                    }
                    if (e.Source is CheckBox)
                    {
                        e.Parameter = "Name=\"" + e.Source.GetValue(CheckBox.NameProperty).ToString() + "\"" + " Content=\"" + e.Source.GetValue(CheckBox.ContentProperty).ToString() + "\" IsChecked=\"" + e.Source.GetValue(CheckBox.IsCheckedProperty).ToString() + "\" ";
                    }
                    if (e.Source is TextBox)
                    {
                        e.Parameter = "Name=\"" + e.Source.GetValue(TextBox.NameProperty).ToString() + "\"" + " Text=\"" + e.Source.GetValue(TextBox.TextProperty).ToString() + "\"";
                    }
                    if (e.Source is TextBlock)
                    {
                        e.Parameter = "Name=\"" + e.Source.GetValue(TextBlock.NameProperty).ToString() + "\"" + " Text=\"" + e.Source.GetValue(TextBlock.TextProperty).ToString() + "\"";
                    }
                    if (e.Source is HyperlinkButton)
                    {
                        e.Parameter = "Name=\"" + e.Source.GetValue(HyperlinkButton.ContentProperty).ToString() + "\"" + " NavigateUri=\"" + e.Source.GetValue(HyperlinkButton.NavigateUriProperty).ToString() + "\"";
                    }
                    break;
            }
        }

        //to display insert html element popups
        private void insertfunction(object sender, RoutedEventArgs e)
        {
            fader.Visibility = System.Windows.Visibility.Visible;
            VisualStateManager.GoToState(this, "modalfade", true);

            button_clicked = sender as Button;
            if (button_clicked.Name == "tool_button")
            {
                areYouSure.Title = "Insert Button";
            }
            if (button_clicked.Name == "tool_label")
            {
                areYouSure.Title = "Insert Label";
            }
            if (button_clicked.Name == "tool_textbox")
            {
                areYouSure.Title = "Insert TextBox";
            }
            if (button_clicked.Name == "tool_radio")
            {
                areYouSure.Title = "Insert Radiobox";
                stack2.Visibility = System.Windows.Visibility.Visible;
            }
            if (button_clicked.Name == "tool_check")
            {
                areYouSure.Title = "Insert Checkbox";
                stack2.Visibility = System.Windows.Visibility.Visible;
            }
            if (button_clicked.Name == "tool_hyperlink")
            {
                areYouSure.Title = "Insert Hyperlink";
            }
            areYouSure.ShowAsModal();
        }

        //to insert horizontal line
        private void tool_line_Click(object sender, RoutedEventArgs e)
        {
            rtb.Insert("<Xaml><Image Source=\"" + "images/HR1.jpg" + "\" /></Xaml>");
        }
        
        //to insert radiobutton;
        private void insertradio(object sender, RoutedEventArgs e)
        {
            fader.Visibility = System.Windows.Visibility.Visible;
            VisualStateManager.GoToState(this, "modalfade", true);

            rtb.Insert("<Xaml><RadioButton Content=\"New Radio Button\" /></Xaml>");
            rtb.ReturnFocus();
            //rtb.InsertTable(3, 3, 1, 1, "TableDefault");

        }

        //To insert checkbox
        private void insertcheckbox(object sender, RoutedEventArgs e)
        {
            fader.Visibility = System.Windows.Visibility.Visible;
            VisualStateManager.GoToState(this, "modalfade", true);

            rtb.Insert("<Xaml><CheckBox Content=\"New CheckBox\" /></Xaml>");
            rtb.ReturnFocus();
        }

        //Closing event of the insert popups. The actual insertion commands in this
        private void AreYouSure_Closed(object sender, EventArgs e)
        {
            if (areYouSure.Result == DialogButtons.OK)
            {
                _name = name1.Text;
                _value = value1.Text;
                    //insert button
                if (button_clicked.Name == "tool_button")
                {
                    areYouSure.Title = "Insert Button";
                    rtb.Insert("<Xaml><Button Name=\"" + _name + "\" Content=\"" + _value + "\" /></Xaml>");
                }
                    //insert textbox
                if (button_clicked.Name == "tool_textbox")
                {
                    areYouSure.Title = "Insert TextBox";
                    rtb.Insert("<Xaml><TextBox Name=\"" + _name + "\" Text=\"" + _value + "\" /></Xaml>");
                }
                    //insert label
                if (button_clicked.Name == "tool_label")
                {
                    areYouSure.Title = "Insert Label";
                    rtb.Insert("<Xaml><TextBlock Name=\"" + _name + "\" Text=\"" + _value + "\" /></Xaml>");
                }
                    //insert radiobutton
                if (button_clicked.Name == "tool_radio")
                {
                    areYouSure.Title = "Insert Radiobox";
                    if(checked_box.IsChecked==true)
                        rtb.Insert("<Xaml><RadioButton Name=\"" + _name + "\" Content=\"" + _value + "\" IsChecked=\"" + "true" + "\"/></Xaml>");
                    else
                        rtb.Insert("<Xaml><RadioButton Name=\"" + _name + "\" Content=\"" + _value + "\" IsChecked=\"" + "false" + "\"/></Xaml>");
                }
                    //insert checkbox
                if (button_clicked.Name == "tool_check")
                {
                    areYouSure.Title = "Insert Checkbox";
                    if (checked_box.IsChecked == true)
                        rtb.Insert("<Xaml><CheckBox Name=\"" + _name + "\" Content=\"" + _value + "\" IsChecked=\"" + "true" + "\"/></Xaml>");
                    else
                        rtb.Insert("<Xaml><CheckBox Name=\"" + _name + "\" Content=\"" + _value + "\" IsChecked=\"" + "false" + "\"/></Xaml>");
                }
                    //insert hyperlink
                if (button_clicked.Name == "tool_hyperlink")
                {
                    areYouSure.Title = "Insert HyperLink";
                    rtb.Insert("<xaml><HyperlinkButton Content=\"" + _name + "\" NavigateUri=\"" + _value + "\" /></xaml>");
                }

                    //reinitialisation of textboxes in popups
                name1.Text = "";
                value1.Text = "";
                rtb.ReturnFocus();
                checked_box.IsChecked = false;
                _name = "";
                _value = "";
            }
                //reinitialisation of textboxes in popups
            name1.Text = "";
            value1.Text = "";
            rtb.ReturnFocus();
            checked_box.IsChecked = false;
            _name = "";
            _value = "";
            stack2.Visibility = System.Windows.Visibility.Collapsed;
            fader.Visibility = System.Windows.Visibility.Collapsed;
        }

        //to insert drop down box
        private void insertdropdown(object sender, RoutedEventArgs e)
        {
            fader.Visibility = System.Windows.Visibility.Visible;
            VisualStateManager.GoToState(this, "modalfade", true);

            dropdown_popup.ShowAsModal();
        }

        private void dropdownpopup_Closed(object sender, DialogEventArgs e)
        {
            if (dropdown_popup.Result == DialogButtons.OK)
            {
                string drop_name = name3.Text;
                string drop_value = value3.Text;
                string drop_item1 = comboitem1.Text;
                string drop_item2 = comboitem2.Text;
                string drop_item3 = comboitem3.Text;
                rtb.Insert("<Xaml><ComboBox Name=\"" + drop_name + "\" >" +
                            "<ComboBoxItem Content=\"" + drop_item1 + "\" />" +
                            "<ComboBoxItem Content=\"" + drop_item2 + "\" />" +
                            "<ComboBoxItem Content=\"" + drop_item3 + "\" />" +
                            "</ComboBox>" +
                            "</Xaml>");
                rtb.ReturnFocus();
                drop_item1 = "";
                drop_item2 = "";
                drop_item3 = "";
                drop_name = "";
                drop_value = "";
            }
            fader.Visibility = System.Windows.Visibility.Collapsed;

        }

        //to insert image on the closure of image carousel popup
        private void imagepopup_Closed(object sender, DialogEventArgs e)
        {

            string selectedimage, width, height;
            selectedimage = imgtext.Text;
            width = image_width.Text;
            height = image_height.Text;

            if (image_popup.Result == DialogButtons.OK)
            {
                rtb.Insert("<Xaml><Image Width=\"" + width + "\" Height=\"" + height + "\" Source=\"" + selectedimage + "\" /></Xaml>");
                rtb.ReturnFocus();
            }
            fader.Visibility = System.Windows.Visibility.Collapsed;
        }


        #endregion

        #region from imageadvancecarousel code for image carousel effect in the insert image popup
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

        private void drillDown()
        {
            _currentLayer++;

            // create new layer if not exist
            if (_currentLayer > _imageCarousels.Count)
            {
                _currentCarousel = new ImageCarousel();
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
                    ImageCarousel imageCarousel = _imageCarousels[_currentLayer];
                    imageCarousel.DrillDisappear();
                }
            }
        }

        /////////////////////////////////////////////////////        
        // Public Methods 
        /////////////////////////////////////////////////////	

        // Start by drill the first layer
        public void Startimg()
        {
            drillDown();

        }

        #endregion

        #region fullscreen code to display the editor in fullscreen

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
                Translate.X = (currentWidth - APP_WIDTH) / 2;
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

        #endregion


        #region dragdrop
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rtb.ExternalDragStart = true;
            _dragging = (Image)sender;
            dragging.Visibility = Visibility.Visible;
            dragging.Source = _dragging.Source;
            UpdateDragging(e.GetPosition(LayoutRoot));
        }

        private void richTextBox_ContentDropped(object sender, RichTextBoxEventArgs e)
        {
            if (_dragging != null)
            {
                if (((_dragging.Source as BitmapImage).UriSource.OriginalString) == "images/button.jpg")
                {
                    fader.Visibility = System.Windows.Visibility.Visible;
                    VisualStateManager.GoToState(this, "modalfade", true);

                    areYouSure.Title = "Insert Button";
                    areYouSure.ShowAsModal();

                    //e.Parameter = new Button() { Content = "mybutton" };
                    //rtb.Insert("<Xaml><Button Content=\"Newline button\" /></Xaml>");
                }
                if (((_dragging.Source as BitmapImage).UriSource.OriginalString) == "images/checkbox1.jpg")
                {
                    rtb.Insert("<Xaml><CheckBox Content=\"Newline checkbox\" /></Xaml>");
                    //e.Parameter = new CheckBox() { Content = "mycheckbox" };

                }
                if (((_dragging.Source as BitmapImage).UriSource.OriginalString) == "images/radiobutton1.jpg")
                {
                    //e.Parameter = new RadioButton() { Content = "myradio" };
                    rtb.Insert("<Xaml><RadioButton Content=\"Newline radio\" /></Xaml>");

                }
                //e.Parameter = new Image() { Source = _dragging.Source };

                CancelDrag();
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging != null)
            {
                UpdateDragging(e.GetPosition(LayoutRoot));
            }
        }

        private void UpdateDragging(Point p)
        {
            dragging.Margin = new Thickness(p.X, p.Y, 0, 0);
            dragging.Visibility = Visibility.Visible;
        }

        private void CancelDrag()
        {
            dragging.Visibility = Visibility.Collapsed;
            _dragging = null;
            rtb.UpdateLayout();
            //myRichTextBox1.Text = myRichTextBox.RichText;
        }
        #endregion


        #region Goto home and save popup
        //While closing the document there is a popup to ask whether to save the document currently being edited
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            editor_rect_fader.Visibility = Visibility.Visible;
            editor_popup_save.Show();
        }

      
        //on clicking Yes
        private void editor_btn_saveyes_Click(object sender, RoutedEventArgs e)
        {
                DemoServiceClient webservice = new DemoServiceClient();
                Database_fireworks_.ServiceReference1.DemoServiceClient webService = new DemoServiceClient();
                webservice.GetRows1Completed += new EventHandler<GetRows1CompletedEventArgs>(webservice_GetRows1Completed);
                webservice.GetRows1Async();
                Database_fireworks_.App.Navigate(new Database_fireworks_.status());
                
        }

        //on choosing no to save
        private void editor_btn_saveno_Click(object sender, RoutedEventArgs e)
        {
            editor_rect_fader.Visibility = Visibility.Collapsed;
            Database_fireworks_.App.Navigate(new Database_fireworks_.status());
            
        }

        //on choosing cancel (return to editor window)
        private void editor_btn_savecancel_Click(object sender, RoutedEventArgs e)
        {
            editor_popup_save.Close();
            editor_rect_fader.Visibility = Visibility.Collapsed;
            
        }

        //on closing the popup, actually save the document and update in database
        private void savepopup_closed(object sender, DialogEventArgs e)
        {
            editor_rect_fader.Visibility = Visibility.Collapsed;

        }

        #endregion
    }
}
