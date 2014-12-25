using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;

namespace Database_fireworks_.Web
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DemoService
    {
        // Return the list of valid data for table authentication
        [OperationContract]
        public List<table_signup1> GetRows()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            var selected_rows = from rows in db.table_signup1s select rows;
            return selected_rows.ToList();
        }

        [OperationContract]
        public List<serverdb> GetRows1()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            var selected_rows = from rows in db.serverdbs select rows;
            return selected_rows.ToList();
        }

        //insert a new data into the database for table authentication
        [OperationContract]

        public void InsertData(string sup_tbox_fname, string sup_tbox_lname, string sup_tbox_duname, string sup_tbox_add, int malefemale, string dob, string sup_pbox_pswd, string sup_tblox_pemail, string sup_tbox_aemail)
        {
            DataClasses1DataContext db = new DataClasses1DataContext();

            //create a new row object
            table_signup1 row = new table_signup1
            {

                user_id = Guid.NewGuid(),
                first_name = sup_tbox_fname,
                last_name = sup_tbox_lname,
                user_name = sup_tbox_duname,
                address = sup_tbox_add,
                date_of_birth = dob,
                gender = malefemale,
                password = sup_pbox_pswd,
                primary_email = sup_tblox_pemail,
                alternate_email = sup_tbox_aemail

            };

            //add the new item to the collection
            db.table_signup1s.InsertOnSubmit(row);

            //submit this change to the database
            db.SubmitChanges();

        }

        
        // Add more operations here and mark them with [OperationContract]
        [OperationContract]

        public void InsertData1(string username, string filename, string richtext)
        {
            DataClasses1DataContext db = new DataClasses1DataContext();

            //create a new row object
            serverdb row = new serverdb
            {

                uid = Guid.NewGuid(),
                username = username,
                filename = filename,
                richtext = richtext

            };

            //add the new item to the collection
            db.serverdbs.InsertOnSubmit(row);

            //submit this change to the database
            db.SubmitChanges();

        }

        // Delete the item specified by the passed key
        [OperationContract]
        public void DeleteRow(Guid uid)
        {

            DataClasses1DataContext db = new DataClasses1DataContext();

            var selected_row = from rows in db.serverdbs where rows.uid==uid select rows;

            // Delete the selected "rows".  There will actual be only one
            // item in this collection because the Guid is unique and is the
            // primary key for the table.
            db.serverdbs.DeleteAllOnSubmit(selected_row);

            // Submit the change to the database.
            db.SubmitChanges();

        }

    }
}
