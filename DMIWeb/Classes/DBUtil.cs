using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.SqlClient;

namespace DMIWeb
{
    public class DBUtil
    {
        public static string getSQLForHighestUnitCode()
        {
            return string.Format(@"
                (CASE WHEN Inventory.[ThirdUnitID] is NULL THEN (CASE WHEN Inventory.[SecUnitID] is NULL THEN U1.[Code] ELSE U2.[Code] END) ELSE U3.[Code] END)
            ");
        }


        public static string getSQLToFormatQty(string variableName) 
        {
            return string.Format(@" 
                        Replace(convert(varchar,convert(Money,CAST(ISNULL({0},0) as varchar(15))),1),'.00','') + ISNULL(' ' + U1.[Code] +' ','') 
                ", variableName);
        }


        public static DataTable getData(string sql, string relationName, string relationColumnName)
        {
            DataSet dataset = getData(sql).DataSet;

            for (int i = 1; i < dataset.Tables.Count; i++)
                dataset.Relations.Add(relationName, dataset.Tables[0].Columns[relationColumnName], dataset.Tables[i].Columns[relationColumnName]);

            //dataset.Relations.Add(relationName, dataset.Tables[0].Columns[relationColumnName], dataset.Tables[1].Columns[relationColumnName]);
            return dataset.Tables[0]; //parent is assumed to be the first table
        }

        public static DataTable getData(string sql)
        {
            DataTable datatable = new DataTable();
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                datatable = getData(new SqlCommand(sql, conn));
            }
            return datatable;
        }

        public static DataTable getData(string sql, SqlConnection conn)
        {
            return getData(new SqlCommand(sql, conn));
        }

        //Can be used to retrieve more than 1 tables. sql = SELECT..; SELECT..; 
        public static DataTable getData(SqlCommand cmd)
        {
            DataSet dataset = new DataSet();
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(dataset);
            }
            return dataset.Tables[0]; //parent is assumed to be the first table
        }

        public static DataSet getDataSet(string sql)
        {
            DataSet dataset = new DataSet();
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            using(SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(dataset);
            }
            return dataset;
        }

        public static string sanitize(params System.Web.UI.WebControls.TextBox[] textboxes)
        {
            foreach (System.Web.UI.WebControls.TextBox textbox in textboxes)
                textbox.Text = sanitize(textbox.Text);

            if (textboxes.Length == 1)
                return textboxes[0].Text;
            else
                return null;
        }

        public static string sanitize(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            else
                return str.Replace(";", string.Empty); //sanitize input in case of sql injection
        }

    }
}