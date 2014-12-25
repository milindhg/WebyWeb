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
    public partial class signup : UserControl
    {
        public signup()
        {
            InitializeComponent();
            imagevisibility();
            /*Creating event handlers*/
            sup_pbox_pswd.PasswordChanged += new RoutedEventHandler(sup_pbox_pswd_PasswordChanged);
            sup_tbox_fname.GotFocus += new RoutedEventHandler(sup_tbox_fname_GotFocus);
            sup_tbox_lname.GotFocus += new RoutedEventHandler(sup_tbox_lname_GotFocus);
            sup_tbox_add.GotFocus += new RoutedEventHandler(sup_tbox_add_GotFocus);
            sup_pbox_pswd.GotFocus += new RoutedEventHandler(sup_pbox_pswd_GotFocus);
            sup_pbox_vpswd.GotFocus += new RoutedEventHandler(sup_pbox_vpswd_GotFocus);
            sup_tbox_pemail.GotFocus += new RoutedEventHandler(sup_tbox_pemail_GotFocus);
            sup_tbox_aemail.GotFocus += new RoutedEventHandler(sup_tbox_aemail_GotFocus);
            sup_bdate_date.GotFocus += new RoutedEventHandler(sup_bdate_year_GotFocus);
            sup_bdate_month.GotFocus += new RoutedEventHandler(sup_bdate_year_GotFocus);
            sup_bdate_year.GotFocus += new RoutedEventHandler(sup_bdate_year_GotFocus);
            sup_tbox_fname.LostFocus+=new RoutedEventHandler(sup_tbox_fname_LostFocus);
            sup_tbox_lname.LostFocus+=new RoutedEventHandler(sup_tbox_lname_LostFocus);
            sup_tbox_duname.LostFocus += new RoutedEventHandler(sup_tbox_duname_LostFocus);
            sup_tbox_add.LostFocus+=new RoutedEventHandler(sup_tbox_add_LostFocus); 
            sup_pbox_pswd.LostFocus+=new RoutedEventHandler(sup_pbox_pswd_LostFocus);
            sup_pbox_vpswd.LostFocus+=new RoutedEventHandler(sup_pbox_vpswd_LostFocus);
            sup_tbox_pemail.LostFocus+=new RoutedEventHandler(sup_tbox_pemail_LostFocus);
            sup_tbox_aemail.LostFocus += new RoutedEventHandler(sup_tbox_aemail_LostFocus);
            sup_bdate_year.LostFocus += new RoutedEventHandler(sup_bdate_year_LostFocus);
            flag = 1;//all mandatory fields are filled and not having any error
        }

        #region for Lost Focus of all text boxes

        void sup_pbox_vpswd_LostFocus(object sender, RoutedEventArgs e)
        {
            flag = 1;
            sup_tblock_tip.Visibility = Visibility.Collapsed;
            img_pswd.Visibility = Visibility.Collapsed;
        }

        void sup_bdate_year_LostFocus(object sender, RoutedEventArgs e)
        {
            flag = 1;
            sup_tblock_tip.Visibility = Visibility.Collapsed;
            img_dob.Visibility = Visibility.Collapsed;
            char[] sup_chk_year = new char[4];
            if (sup_bdate_year.Text.Length != 4)//check if year is exceeding 4 characters
            {
                img_dob.Visibility = Visibility.Visible;
                flag = 0;
            }
            else if (sup_bdate_year.Text != "")//check if year is blank or not if not then enter
            {
                int temp_flag = 0;
                sup_chk_year = sup_bdate_year.Text.ToCharArray();
                int i;
                for (i = 0; i < 4; i++)//check for characters entered are numbers or not
                {
                    if (!((sup_chk_year[i] >= '0' && sup_chk_year[i] <= '9')))
                    {
                        img_dob.Visibility = Visibility.Visible;
                        flag = 0;
                        temp_flag = 1;
                        break;
                    }
                }
                if (temp_flag == 0)//check for year exceeding today's date or not
                {
                    int temp_year = int.Parse(sup_bdate_year.Text);
                    int temp1 = int.Parse(System.DateTime.Today.Year.ToString());
                    if ((temp_year < 1900) || (temp_year > temp1))
                    {
                        img_dob.Visibility = Visibility.Visible;
                        flag = 0;
                    }
                }
            }
            if (sup_bdate_date.SelectedIndex == -1 || sup_bdate_month.SelectedIndex == -1)//check for selection of Date and month
            {
                img_dob.Visibility = Visibility.Visible;
                flag = 0;
            }
            else//convert date into string and assign to Bdate variable
            {
                global_bdate = sup_bdate_date.SelectionBoxItem.ToString() + "/" + sup_bdate_month.SelectionBoxItem.ToString() + "/" + sup_bdate_year.Text;
            }

        }        

        void sup_tbox_duname_LostFocus(object sender, RoutedEventArgs e)
        {
            flag = 1;
            sup_tblock_tip.Visibility = Visibility.Collapsed;
            img_duname.Visibility = Visibility.Collapsed;
            SolidColorBrush mybrush = new SolidColorBrush();

            if (sup_tbox_duname.Text.Length > 12)//check for desired user name whether exceeding 12 characters
            {
                System.Windows.Browser.HtmlPage.Window.Alert("Maximum limit for desird user name crossed.Maximum limit is 12 characters.");
                img_duname.Visibility = System.Windows.Visibility.Visible;
                //sup_tblock_duname.Foreground = mybrush;
                sup_tblock_duname.Foreground = new SolidColorBrush(Colors.Red);
                flag = 0;
            }

            if (sup_tbox_duname.Text != "")//check for desired user name whether it is blank or not and if not blank then enter
            {
                char[] sup_chk_duname = new char[12];
                int flag1 = 0;//variable for checking error in user name
                
                sup_chk_duname = sup_tbox_duname.Text.ToCharArray();

                /*Check whether first character of user name is not a number or special character*/
                if (!((sup_chk_duname[0] >= 'a' && sup_chk_duname[0] <= 'z') || (sup_chk_duname[0] >= 'A' && sup_chk_duname[0] <= 'Z')))
                {
                    System.Windows.Browser.HtmlPage.Window.Alert("User name should start with character.");
                    img_duname.Visibility = Visibility.Visible;
                    flag1 = 1;//error 
                }

                if (flag1 == 0)//call for checking availability of user name if there exist no error
                {
                    DemoServiceClient webService = new DemoServiceClient();

                    webService.GetRowsCompleted += new EventHandler<GetRowsCompletedEventArgs>(webService_GetRowsCompleted);

                    webService.GetRowsAsync();
                }
            }
        }

        void sup_tbox_aemail_LostFocus(object sender, RoutedEventArgs e)
        {
            flag = 1;
            sup_tblock_tip.Visibility = Visibility.Collapsed;
            img_aemail.Visibility = Visibility.Collapsed;
            SolidColorBrush mybrush = new SolidColorBrush();
            mybrush.Color = Colors.Red;
            char[] sup_chk_duname = new char[30];
            
            if (sup_tbox_aemail.Text != "")//check whether alternet email address is blank or not and if not blank then enter
            {
                /*check whether email id is valid or not by checking availability of '@' character*/
                if (!sup_tbox_aemail.Text.Contains("@"))
                {
                    img_aemail.Visibility = System.Windows.Visibility.Visible;
                    System.Windows.Browser.HtmlPage.Window.Alert("Invalid alternate Email ID.");
                    sup_tblock_aemail.Foreground = mybrush;
                    flag = 0;
                }
                sup_chk_duname = sup_tbox_aemail.Text.ToCharArray();

                /*check whether first character is valid or not by checking occurence of number or special character*/
                if (!((sup_chk_duname[0] >= 'a' && sup_chk_duname[0] <= 'z') || (sup_chk_duname[0] >= 'A' && sup_chk_duname[0] <= 'Z')))
                {
                    img_aemail.Visibility = System.Windows.Visibility.Visible;
                    System.Windows.Browser.HtmlPage.Window.Alert("First character of alternate Email ID can not be this.");
                    sup_tblock_aemail.Foreground = mybrush;
                    flag = 0;
                }
            }
        }

        void sup_tbox_pemail_LostFocus(object sender, RoutedEventArgs e)
        {
            flag = 1;
            sup_tblock_tip.Visibility = Visibility.Collapsed;
            img_pemail.Visibility = Visibility.Collapsed;
            SolidColorBrush mybrush = new SolidColorBrush();
            mybrush.Color = Colors.Red;
            char[] sup_chk_duname = new char[30];


            if (sup_tbox_pemail.Text != "")//check whether Permanent email address is blank or not and if not blank then enter
            {
                /*check whether email id is valid or not by checking availability of '@' character*/
                if (!sup_tbox_pemail.Text.Contains("@"))
                {
                    img_pemail.Visibility = System.Windows.Visibility.Visible;
                    System.Windows.Browser.HtmlPage.Window.Alert("Invalid primary Email ID.");
                    sup_tblock_pemail.Foreground = mybrush;
                    flag = 0;
                }
                sup_chk_duname = sup_tbox_pemail.Text.ToCharArray();

                /*check whether first character is valid or not by checking occurence of number or special character*/
                if (!((sup_chk_duname[0] >= 'a' && sup_chk_duname[0] <= 'z') || (sup_chk_duname[0] >= 'A' && sup_chk_duname[0] <= 'Z')))
                {
                    img_pemail.Visibility = System.Windows.Visibility.Visible;
                    System.Windows.Browser.HtmlPage.Window.Alert("First character of primary Email ID can not be this.");
                    sup_tblock_pemail.Foreground = mybrush;
                    flag = 0;
                }
            }

        }
              
        void sup_pbox_pswd_LostFocus(object sender, RoutedEventArgs e)
        {
            flag = 1;
            sup_tblock_tip.Visibility = Visibility.Collapsed;
            img_pswd.Visibility = Visibility.Collapsed;
            SolidColorBrush mybrush = new SolidColorBrush();
            mybrush.Color = Colors.Red;
            if (sup_pbox_pswd.Password != "")//Check whether entered password is blank or not and if not then enter
            {
                /*Check length of password whether exceeding above 12 characters or not*/
                if (sup_pbox_pswd.Password.Length > 12)
                {
                    System.Windows.Browser.HtmlPage.Window.Alert("Maximum limit for password crossed.Maximum limit is 12 characters.");
                    img_pswd.Visibility = System.Windows.Visibility.Visible;
                    sup_tblock_pswd.Foreground = mybrush;
                    flag = 0;
                }
                /*Check length of password whether exceeding below 4 characters or not*/
                if (sup_pbox_pswd.Password.Length < 4)
                {
                    System.Windows.Browser.HtmlPage.Window.Alert("Minimum limit for password crossed.Minimum limit is 4 characters.");
                    img_pswd.Visibility = System.Windows.Visibility.Visible;
                    sup_tblock_pswd.Foreground = mybrush;
                    flag = 0;
                }
            }

        }

        void sup_tbox_add_LostFocus(object sender, RoutedEventArgs e)
        {
            sup_tblock_tip.Visibility = Visibility.Collapsed;
            
        }

        void sup_tbox_lname_LostFocus(object sender, RoutedEventArgs e)
        {
            flag = 1;
            sup_tblock_tip.Visibility = Visibility.Collapsed;
            img_lname.Visibility = Visibility.Collapsed;
            SolidColorBrush mybrush = new SolidColorBrush();
            mybrush.Color = Colors.Red;
            char[] sup_chk_duname = new char[30];

            if (sup_tbox_lname.Text != "")//Check whether entered last name is blank or not and if not then enter
            {
                /*Check length of last name whether exceeding above 30 characters or not*/
                if (sup_tbox_lname.Text.Length > 30)
                {
                    System.Windows.Browser.HtmlPage.Window.Alert("Maximum limit for last name crossed.Maximum limit is 30 characters.");
                    img_lname.Visibility = System.Windows.Visibility.Visible;
                    sup_tblock_lname.Foreground = mybrush;
                    flag = 0;
                }
                /*Check length of last name whether exceeding below 2 characters or not*/
                else if (sup_tbox_lname.Text.Length < 2)
                {
                    System.Windows.Browser.HtmlPage.Window.Alert("Minimum limit for last name crossed.Minimum limit is 2 characters.");
                    img_lname.Visibility = System.Windows.Visibility.Visible;
                    sup_tblock_lname.Foreground = mybrush;
                    flag = 0;
                }
                else
                {
                    sup_chk_duname = sup_tbox_lname.Text.ToCharArray();
                    int i;
                    /*Check for occurence of numbers or special characters in last name and if it occurs then assign error*/
                    for (i = 0; i < sup_tbox_lname.Text.Length; i++)
                    {
                        if (!((sup_chk_duname[i] >= 'a' && sup_chk_duname[i] <= 'z') || (sup_chk_duname[i] >= 'A' && sup_chk_duname[i] <= 'Z')))
                        {
                            System.Windows.Browser.HtmlPage.Window.Alert("Last name should contain characters only.");
                            img_lname.Visibility = System.Windows.Visibility.Visible;
                            sup_tblock_lname.Foreground = mybrush;
                            flag = 0;
                            break;
                        }
                    }
                }
            }
        }

        void sup_tbox_fname_LostFocus(object sender, RoutedEventArgs e)
        {
            flag = 1;
            sup_tblock_tip.Visibility = Visibility.Collapsed;
            img_fname.Visibility = Visibility.Collapsed;
            SolidColorBrush mybrush = new SolidColorBrush();
            mybrush.Color = Colors.Red;
            char[] sup_chk_duname = new char[30];

            if (sup_tbox_fname.Text != "")//Check whether entered first name is blank or not and if not then enter
            {
                /*Check length of first name whether exceeding above 30 characters or not*/
                if (sup_tbox_fname.Text.Length > 30)
                {
                    System.Windows.Browser.HtmlPage.Window.Alert("Maximum limit for first name crossed.Maximum limit is 30 characters.");
                    img_fname.Visibility = System.Windows.Visibility.Visible;
                    sup_tblock_fname.Foreground = mybrush;
                    flag = 0;
                }
                /*Check length of first name whether exceeding below 2 characters or not*/
                else if (sup_tbox_fname.Text.Length < 2)
                {
                    System.Windows.Browser.HtmlPage.Window.Alert("Minimum limit for first name crossed.Minimum limit is 2 characters.");
                    img_fname.Visibility = System.Windows.Visibility.Visible;
                    sup_tblock_fname.Foreground = mybrush;
                    flag = 0;
                }
                else
                {
                    sup_chk_duname = sup_tbox_fname.Text.ToCharArray();
                    int i;
                    /*Check for occurence of numbers or special characters in first name and if it occurs then assign error*/
                    for (i = 0; i < sup_tbox_fname.Text.Length; i++)
                    {
                        if (!((sup_chk_duname[i] >= 'a' && sup_chk_duname[i] <= 'z') || (sup_chk_duname[i] >= 'A' && sup_chk_duname[i] <= 'Z')))
                        {
                            img_fname.Visibility = System.Windows.Visibility.Visible;
                            System.Windows.Browser.HtmlPage.Window.Alert("First name should contain characters only.");
                            flag = 0;
                            break;
                        }
                    }
                }
            }
        }

        #endregion 

        #region for Got focus of all text boxes

        private void sup_tbox_duname_GotFocus(object sender, RoutedEventArgs e)
        {
            sup_tblock_tip.Visibility = Visibility.Visible;
            img_happy.Visibility = Visibility.Collapsed;
            img_sad.Visibility = Visibility.Collapsed;
            Canvas.SetTop(sup_tblock_tip, 636.550);
            sup_tblock_tip.Text = "Enter your desired user name for accesing your Weby Web account.";
        }

        void sup_bdate_year_GotFocus(object sender, RoutedEventArgs e)
        {
            sup_tblock_tip.Visibility = Visibility.Visible;
            img_dob.Visibility = Visibility.Collapsed;
            Canvas.SetTop(sup_tblock_tip, 520);
            sup_tblock_tip.Text = "Enter your Date Of Birth.(Date/Month/Year)";
        }

        void sup_tbox_aemail_GotFocus(object sender, RoutedEventArgs e)
        {
            sup_tblock_tip.Visibility = Visibility.Visible;
            img_aemail.Visibility = Visibility.Collapsed;
            Canvas.SetTop(sup_tblock_tip,926.7 );
            sup_tblock_tip.Text = "Email should contain '@' character. Email can not contain first character as '@'";
            
        }

        void sup_tbox_pemail_GotFocus(object sender, RoutedEventArgs e)
        {
            sup_tblock_tip.Visibility = Visibility.Visible;
            img_pemail.Visibility = Visibility.Collapsed;
            Canvas.SetTop(sup_tblock_tip,886.7 );
            sup_tblock_tip.Text = "Email should contain '@' character. Email can not contain first character as '@'";
        }

        void sup_pbox_pswd_GotFocus(object sender, RoutedEventArgs e)
        {
            sup_tblock_tip.Visibility = Visibility.Visible;
            img_pswd.Visibility = Visibility.Collapsed;
            Canvas.SetTop(sup_tblock_tip, 680.50);
            sup_tblock_tip.Text = "Maximum number of characters for password are 12 & Minimum number of characters are 4.";
        }

        void sup_pbox_vpswd_GotFocus(object sender, RoutedEventArgs e)
        {
            sup_tblock_tip.Visibility = Visibility.Visible;
            img_vpswd.Visibility = Visibility.Collapsed;
            Canvas.SetTop(sup_tblock_tip, 776.7);
            sup_tblock_tip.Text = "Password and Verified Password must be same.";
        }

        void sup_tbox_add_GotFocus(object sender, RoutedEventArgs e)
        {
            sup_tblock_tip.Visibility = Visibility.Visible;
            Canvas.SetTop(sup_tblock_tip, 420);
            sup_tblock_tip.Text = "Enter your postal address here.";
        }

        void sup_tbox_lname_GotFocus(object sender, RoutedEventArgs e)
        {
            sup_tblock_tip.Visibility = Visibility.Visible;
            img_lname.Visibility = Visibility.Collapsed;
            Canvas.SetTop(sup_tblock_tip, 343);
            sup_tblock_tip.Text = "Maximum number of characters for last name are 30 & Minimum number of characters are 2.";
        }

        void sup_tbox_fname_GotFocus(object sender, RoutedEventArgs e)
        {
            sup_tblock_tip.Visibility = Visibility.Visible;
            img_fname.Visibility = Visibility.Collapsed;
            Canvas.SetTop(sup_tblock_tip,300);
            sup_tblock_tip.Text = "Maximum number of characters for first name are 30 & Minimum number of characters are 2.";
        }

        #endregion

        void sup_pbox_pswd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            checkstrength();
        }

        #region for checking password strength dynamically
        public void checkstrength()
        {
            char[] sup_pbox_char=new char[20];
            SolidColorBrush mybrush = new SolidColorBrush();
            strenghreset();
            if(sup_pbox_pswd.Password!="")//check whether password box is blank or not and if not then enter
            {
            sup_pbox_char=sup_pbox_pswd.Password.ToCharArray();
            int i;
            /*Variables for counting occurences of lower case characters,Upper case characters,Special characters
             and numbers respectively*/
            int lchar_count=0,uchar_count=0,schar_count=0,nchar_count=0;

            for (i = 0; i < sup_pbox_pswd.Password.Length; i++)
            {
                if(sup_pbox_char[i]>='a'&&sup_pbox_char[i]<='z')//lower case character found
                {
                    lchar_count++;
                }
                else if (sup_pbox_char[i] >= 'A' && sup_pbox_char[i] <= 'Z')//upper case character found 
                {
                    uchar_count++;
                }
                else if (sup_pbox_char[i] >='0'  && sup_pbox_char[i] <= '9')//number found 
                {
                    nchar_count++;
                }
                else//special character found
                {
                    schar_count++;
                }
            }

            /*Various combinations for checking the strength of password depending upon count of lower case,upper case
             ,special characters and number.*/
            if (lchar_count == sup_pbox_pswd.Password.Length || uchar_count == sup_pbox_pswd.Password.Length)
            {
                    mybrush.Color = Colors.Red;
                    rct1.Fill = mybrush;
                    sup_tblock_pstrength.Text = "POOR";
            }
            else if (lchar_count != 0 && uchar_count != 0 && nchar_count == 0 && schar_count == 0)
            {
                   mybrush.Color = Colors.Orange;
                   rct1.Fill = mybrush;
                   rct2.Fill = mybrush;
                   sup_tblock_pstrength.Text = "GOOD";
            }
            else if ((lchar_count != 0 && uchar_count != 0 && nchar_count != 0 && schar_count == 0) || (lchar_count != 0 && uchar_count != 0 && nchar_count == 0 && schar_count != 0))
            {
                mybrush.Color = Colors.Blue;
                rct1.Fill = mybrush;
                rct2.Fill = mybrush;
                rct3.Fill = mybrush;
                sup_tblock_pstrength.Text = "BEST";
            }
            else if (lchar_count != 0 && uchar_count != 0 && nchar_count != 0 && schar_count != 0)
            {
                mybrush.Color = Colors.Green;
                rct1.Fill = mybrush;
                rct2.Fill = mybrush;
                rct3.Fill = mybrush;
                rct4.Fill = mybrush;
                sup_tblock_pstrength.Text = "EXCELLENT";
            }
            else if (lchar_count == 0 && uchar_count == 0 && nchar_count != 0 && schar_count != 0)
            {
                mybrush.Color = Colors.Orange;
                rct1.Fill = mybrush;
                rct2.Fill = mybrush;
                sup_tblock_pstrength.Text = "GOOD";
            }
            else if (lchar_count == 0 && uchar_count == 0 && nchar_count != 0 && schar_count == 0)
            {
                mybrush.Color = Colors.Orange;
                rct1.Fill = mybrush;
                rct2.Fill = mybrush;
                sup_tblock_pstrength.Text = "GOOD";
            }
            else
            {
                mybrush.Color = Colors.Orange;
                rct1.Fill = mybrush;
                rct2.Fill = mybrush;
                sup_tblock_pstrength.Text = "GOOD";
            }
        }
        }
        #endregion

        int malefemale;//Variable for assigninggender  male or female from radio button

        /*Flag for checking all mandetory fields are filled or not if flag=0 all fields are not 
        filled or have some error if flag =1 all fields are filled without error*/ 
        int flag=0;

        string global_bdate;//Variable for assigning birthdate of user
                      
        private void sup_rdo_male_Click(object sender, RoutedEventArgs e)
        {
            malefemale = 1;//gender is male
        }

        private void sup_rdo_female_Click(object sender, RoutedEventArgs e)
        {
            malefemale = 0;//gender is female
        }

        /*Function for subscription*/
        private void sup_btn_subscribe_Click(object sender, RoutedEventArgs e)
        {
            char[] sup_chk_duname = new char[30];//variable for converting string to char array
            
            if (sup_tbox_fname.Text == "")//check if First name text box is blank
            {
                img_fname.Visibility = System.Windows.Visibility.Visible;//show error image
                flag = 0;//all mandatory fields are 
            }

            /*check whether date,month or year of birthdate is blank or not*/
            if (sup_bdate_date.SelectedIndex == -1 || sup_bdate_month.SelectedIndex == -1 || sup_bdate_year.Text == "")
            {
                img_dob.Visibility = Visibility.Visible;
                flag = 0;
            }
            /*If birthdate is not blank*/
            else
            {
                int temp_date=int.Parse( sup_bdate_date.SelectionBoxItem.ToString());
                int temp_year = int.Parse(sup_bdate_year.Text);

                /*Check for validation of leap year*/
                if (((sup_bdate_month.SelectionBoxItem.ToString() == "April") || (sup_bdate_month.SelectionBoxItem.ToString() == "June") || (sup_bdate_month.SelectionBoxItem.ToString() == "September") || (sup_bdate_month.SelectionBoxItem.ToString() == "November"))&&(temp_date==31))
                {
                    img_dob.Visibility = Visibility.Visible;
                    flag = 0;
                }
                if ((sup_bdate_month.SelectionBoxItem.ToString() == "February"))
                {
                    if (temp_year % 400 == 0 || (temp_year % 100 != 0 && temp_year % 4 == 0))
                    {
                        if ((temp_date == 30) || (temp_date == 31))
                        {
                            img_dob.Visibility = Visibility.Visible;
                            flag = 0;
                        }
                    }
                    else
                    {
                        if ((temp_date == 30) || (temp_date == 31)||(temp_date==29))
                        {
                            img_dob.Visibility = Visibility.Visible;
                            flag = 0;
                        }
                    }
                }
                
            }

            if (sup_tbox_lname.Text == "")//Check whether last name is blank or not if blank then enter
            {
                //System.Windows.Browser.HtmlPage.Window.Alert("Please enter last name.");
                img_lname.Visibility = System.Windows.Visibility.Visible;
                flag = 0;
            }

            if (sup_tbox_duname.Text == "")//Check whether desired user name is blank or not if blank then enter
            {
                //System.Windows.Browser.HtmlPage.Window.Alert("Please enter Desired user name.");
                img_duname.Visibility = System.Windows.Visibility.Visible;
                flag = 0;
            }

            if (sup_pbox_pswd.Password == "")//Check whether password box is blank or not if blank then enter
            {
                //System.Windows.Browser.HtmlPage.Window.Alert("Please enter password.");
                img_pswd.Visibility = System.Windows.Visibility.Visible;
                flag = 0;
            }

            //Check whether both passwords matches or not
            if (sup_pbox_pswd.Password != sup_pbox_vpswd.Password)
            {
                img_pswd.Visibility = System.Windows.Visibility.Visible;
                img_vpswd.Visibility = System.Windows.Visibility.Visible;
                System.Windows.Browser.HtmlPage.Window.Alert("Passwords does not match.");
                flag = 0;
            }

            //Check whether permanent email is blank or not if blank then enter
            if (sup_tbox_pemail.Text == "")
            {
                //System.Windows.Browser.HtmlPage.Window.Alert("Please enter primary Email ID.");
                img_pemail.Visibility = System.Windows.Visibility.Visible;
                flag = 0;
            }

            //Check whether alternet email is blank or not if blank then enter
            if (sup_tbox_pemail.Text == sup_tbox_aemail.Text && !(sup_tbox_pemail.Text == ""))
            {
                img_aemail.Visibility = System.Windows.Visibility.Visible;
                System.Windows.Browser.HtmlPage.Window.Alert("Primary and Alternate Email ID can not be same.");
                flag = 0;
            }

            /*If there is no error in any mandatory fields then make entry in database*/
            if (flag == 1)
            {
                DemoServiceClient webService = new DemoServiceClient();

                webService.InsertDataAsync(sup_tbox_fname.Text, sup_tbox_lname.Text, sup_tbox_duname.Text, sup_tbox_add.Text, malefemale, global_bdate, sup_pbox_pswd.Password, sup_tbox_pemail.Text, sup_tbox_aemail.Text);

                System.Windows.MessageBox.Show("Congratulation!!! You have subscribed succesfully.");

                App.Navigate(new Page());
            }
                      
           
        }

        public void imagevisibility()
        {
            /*Collapse all images*/
            img_fname.Visibility =Visibility.Collapsed;
            img_lname.Visibility =Visibility.Collapsed;
            img_duname.Visibility = Visibility.Collapsed;
            img_dob.Visibility = Visibility.Collapsed;
            img_pswd.Visibility = Visibility.Collapsed;
            img_vpswd.Visibility = Visibility.Collapsed;
            img_pemail.Visibility =Visibility.Collapsed;
            img_aemail.Visibility =Visibility.Collapsed;
            img_sad.Visibility = Visibility.Collapsed;
            img_happy.Visibility = Visibility.Collapsed;
            flag = 0;
        }

        private void sup_btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            App.Navigate(new Page());
        }

        
        void webService_GetRowsCompleted(object sender, GetRowsCompletedEventArgs e)
        {
            int i,flag=0;
            for (i = 0; i != e.Result.Count;i++ )
            {
                //check whether user name already exist or not
                if (sup_tbox_duname.Text == e.Result[i].user_name)
                {
                    System.Windows.Browser.HtmlPage.Window.Alert("User Name already exist. Please choose another user name.");
                    img_sad.Visibility = Visibility.Visible;
                    sup_tbox_duname.Text = "";
                    flag = 1;
                    break;
                }
            }
            if (flag == 0)
            {
                System.Windows.Browser.HtmlPage.Window.Alert("User Name available.");
                img_happy.Visibility = Visibility.Visible;
            }

        }
                                     
        public void strenghreset()
        {
            //Reset password strength to blank
            SolidColorBrush mybrush = new SolidColorBrush();
            mybrush.Color = Colors.White;
            rct1.Fill = mybrush;
            rct2.Fill = mybrush; 
            rct3.Fill = mybrush; 
            rct4.Fill = mybrush;
        }
                               
    }
}
