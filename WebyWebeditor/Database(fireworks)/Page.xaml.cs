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
using Database_fireworks_.ServiceReference1;


namespace Database_fireworks_
{
    public partial class Page : UserControl
    {
        
        public Page()
        {
            InitializeComponent();
            auth_btn_submit.Click += new RoutedEventHandler(auth_btn_submit_Click);
            auth_btn_create.Click += new RoutedEventHandler(auth_btn_create_Click);
            ColorfulFireworks.ColorfulFireworks cf = new ColorfulFireworks.ColorfulFireworks();
            LayoutRoot.Children.Insert(0, cf);
            LayoutRoot.Children.Remove(Cover);
            cf.Start();
            auth_uname.Focus();
            i = 0;
        }

        public static string global_uname;//variable for assigning global user name

        /*To register new user*/
        void auth_btn_create_Click(object sender, RoutedEventArgs e)
        {
            App.Navigate(new signup());//Go to signup page
        }

        /*Check existing users authentication*/
        void auth_btn_submit_Click(object sender, RoutedEventArgs e)
        {
            DemoServiceClient webservice = new DemoServiceClient();

            mybuttton = sender as Button;

            Database_fireworks_.ServiceReference1.DemoServiceClient webService = new DemoServiceClient();

            webService.GetRowsCompleted += new EventHandler<GetRowsCompletedEventArgs>(webService_GetRowsCompleted);

            webService.GetRowsAsync();
            
        }

        int i;//flag for counter
        int flag;//To check user entry valid or not
        Button mybuttton = new Button();

        void webService_GetRowsCompleted(object sender, GetRowsCompletedEventArgs e)
        {
            /*Check valid entry of user in database */
            if (mybuttton.Name == "auth_btn_submit")
            {
                for (i = 0; i != e.Result.Count; i++)
                {
                    if ((auth_uname.Text == e.Result[i].user_name) && (auth_pbox.Password == e.Result[i].password))
                    {
                        global_uname = e.Result[i].user_name;//assign user name to global user name
                        flag = 1;//Set if user entry matches
                        break;
                    }
                }

                if (flag == 0)//User entry does not match
                {
                    System.Windows.Browser.HtmlPage.Window.Alert("Invalid User Name or Password.");
                }

                if (flag == 1)//User entry matches
                {
                    App.Navigate(new status());
                }

                auth_uname.Text = "";//clear user name text box
                auth_pbox.Password = "";//clear password text box
                i = 0;//assign counter to 0
                flag = 0;//set flag to user entry does not match
            }


        }
  
    }
}
