using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IndexingTask
{
    class db
    {
        public int db_id;
        public string db_url;
        public string db_content;
        public static SqlConnection conn = new SqlConnection("Data Source=DESKTOP-P84ETB9\\SQLEXPRESS;" + "Initial Catalog=IrProject;" + "Integrated Security=true");

        public db(int db_id, string db_url, string db_content)
        {
            this.db_id = db_id;
            this.db_url = db_url;
            this.db_content = db_content;
        }

        public static List<db> SqlConn()
        {
            SqlCommand command;
            List<db> db_content = new List<db>();
            SqlConnection conn = new SqlConnection("Data Source=DESKTOP-P84ETB9\\SQLEXPRESS;" + "Initial Catalog=IrProject;" + "Integrated Security=true");
            conn.Open();
            command = new SqlCommand("select id, url, content from document ", conn);
            command.CommandType = System.Data.CommandType.Text;
            SqlDataReader rd = command.ExecuteReader();
            while (rd.Read())
            {
                db_content.Add(new db(Convert.ToInt32(rd[0]), rd[1].ToString(), rd[2].ToString()));
            }    
            
            rd.Close();
            conn.Close();
            return db_content;
        }

        public static void store(string terms_before, string terms_after, int doc_id,int freq, string pos)
        {
            conn.Open();
            SqlCommand command = new SqlCommand("insert into Terms (terms_before,docid) VALUES(@terms_before,@docid)", db.conn);
            command.Parameters.AddWithValue("@terms_before", terms_before);
            command.Parameters.AddWithValue("@docid", doc_id);
            command.ExecuteNonQuery();
            command = new SqlCommand("INSERT INTO InvertedIndex (Term_after,docid,Frequency,position) VALUES (@Term_after,@docid,@Frequency,@position)", db.conn);
            command.Parameters.AddWithValue("@Term_after", terms_after);
            command.Parameters.AddWithValue("@docid", doc_id);
            command.Parameters.AddWithValue("@Frequency", freq);
            command.Parameters.AddWithValue("@position", pos);
            command.ExecuteNonQuery();
            conn.Close();
        }

    }
}
