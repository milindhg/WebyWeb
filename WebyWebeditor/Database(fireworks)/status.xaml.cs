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
using Liquid;
using Database_fireworks_.ServiceReference1;


namespace Database_fireworks_
{
    public partial class status : UserControl
    {
        public status()
        {
            InitializeComponent();
            //Displaying user name in the corner
            status_tblock_uname.Text = "Welcome " + Page.global_uname+"...";
            status_lstbox.SelectionChanged += new SelectionChangedEventHandler(status_lstbox_SelectionChanged);
            //Select all query
            query = "select all";
            global_richtext = " ";
            //Call for loading database
            load_database();
       }

        public static string global_filename;//Global variable for file name
        public static string global_richtext;//Global variable for rich text
        public static Guid global_uid;

        string query;

        void status_lstbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (status_lstbox.SelectedItem != null)
            {
                global_filename = status_lstbox.SelectedItem.ToString();//Assign file name to global varible
                query = "get uid";//Query get user ID
                load_database();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            status_rect_fader.Visibility = Visibility.Visible;
            status_popup_filename.Show();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            App.Navigate(new Page());
        }

        void load_database()
        {
            DemoServiceClient webservice = new DemoServiceClient();
            Database_fireworks_.ServiceReference1.DemoServiceClient webService = new DemoServiceClient();
            webservice.GetRows1Completed += new EventHandler<GetRows1CompletedEventArgs>(webservice_GetRows1Completed);
            webservice.GetRows1Async();
        }

        void webservice_GetRows1Completed(object sender, GetRows1CompletedEventArgs e)
        {
            int i;
            if (query == "select all")
            {
                for (i = 0; i != e.Result.Count; i++)
                {
                    if (Page.global_uname == e.Result[i].username)
                    {
                        status_lstbox.Items.Add(e.Result[i].filename);
                    }
                }
            }

            if (query == "edit file")
            {
                for (i = 0; i != e.Result.Count; i++)
                {
                    if (Page.global_uname == e.Result[i].username&&global_filename==e.Result[i].filename)
                    {
                        global_richtext = e.Result[i].richtext;
                        App.Navigate(new MainMenu.editor());
                    }
                }
            }
            if (query == "get uid")
            {
                for (i = 0; i != e.Result.Count; i++)
                {
                    if (Page.global_uname == e.Result[i].username && global_filename == e.Result[i].filename)
                    {
                        global_uid = (Guid) e.Result[i].uid;
                    }
                }
            }
        }

        private void status_button_edit_Click(object sender, RoutedEventArgs e)
        {
            query = "edit file";
            load_database();           
        }

        private void status_button_remove_Click(object sender, RoutedEventArgs e)
        {
             // Now access the service to delete the item
            DemoServiceClient webService = new DemoServiceClient();
            webService.DeleteRowAsync(global_uid);
            System.Windows.MessageBox.Show("File "+global_filename+" has been deleted.");

            //check for index of list box
            status_lstbox.SelectedIndex=-1;
            status_lstbox.Items.Clear();            
            query = "select all";
            load_database();
        }

        private void status_popup_filename_Closed(object sender, Liquid.DialogEventArgs e)
        {
            if (status_popup_filename.Result == DialogButtons.OK)
            {
                global_filename = status_tbox_filename.Text;
                global_richtext = " ";
                App.Navigate(new MainMenu.editor());
            }
            status_rect_fader.Visibility = Visibility.Collapsed;
        }

        private void SL_wmv_MediaEnded(object sender, RoutedEventArgs e)
        {
            //Loop for playing video
            SL_wmv.Stop();
            SL_wmv.Play();
        }
    }
}
