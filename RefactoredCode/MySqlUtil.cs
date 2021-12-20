using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace backend
{
    public class MySqlUtil
    {
        public static dynamic messages;
        public static dynamic setting;
        public static dynamic allowedExtension;

        public static IConfiguration appSetting;
        public static string GetConnectionString()
        {
            return appSetting["connectionString"];
        }

        public static List<dynamic> ExecuteSql(string sqlcmd)
        {
            var results = new List<dynamic>();
            MySqlTransaction transaction = null;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    MySqlCommand cmd = new MySqlCommand(sqlcmd, conn, transaction);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return results;
        }

        public static int SPInsertWattageCriteria(int upperLimit, int lowerLimit, int percentage, MySqlTransaction transaction, MySqlConnection conn)
        {
            int CriteriaId = 0;

            MySqlCommand cmd = new MySqlCommand("proc_insert_wattage_criteria", conn, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@percentage", percentage);
            cmd.Parameters.AddWithValue("@upper_limit", upperLimit);
            cmd.Parameters.AddWithValue("@lower_limit", lowerLimit);
            cmd.Parameters.Add("@wattage_criteria_id", MySqlDbType.Int32);
            cmd.Parameters["@wattage_criteria_id"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            CriteriaId = Convert.ToInt32(cmd.Parameters["@wattage_criteria_id"].Value);

            return CriteriaId;
        }

        public static int SPInsertLumenCriteria(int upperLimit, int lowerLimit, int percentage, MySqlTransaction transaction, MySqlConnection conn)
        {
            int CriteriaId = 0;

            MySqlCommand cmd = new MySqlCommand("proc_insert_lumen_criteria", conn, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@percentage", percentage);
            cmd.Parameters.AddWithValue("@upper_limit", upperLimit);
            cmd.Parameters.AddWithValue("@lower_limit", lowerLimit);
            cmd.Parameters.Add("@lumen_criteria_id", MySqlDbType.Int32);
            cmd.Parameters["@lumen_criteria_id"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            CriteriaId = Convert.ToInt32(cmd.Parameters["@lumen_criteria_id"].Value);

            return CriteriaId;
        }

        public static List<dynamic> SPDeleteWattageLumenCriteria(MySqlTransaction transaction, MySqlConnection conn)
        {
            var results = new List<dynamic>();
            string spName = "proc_empty_wattage_lumen_criteria";
            MySqlCommand cmd = new MySqlCommand(spName, conn, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
            return results;
        }

        public static int SPInsertReplacementCriteria(int groupId, string lightSource, int wattage, MySqlTransaction transaction, MySqlConnection conn)
        {
            int CriteriaId = 0;
            string spName = "proc_insert_replacement_criteria";

            MySqlCommand cmd = new MySqlCommand(spName, conn, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@group_id", groupId);
            cmd.Parameters.AddWithValue("@light_source", lightSource);
            cmd.Parameters.AddWithValue("@wattage", wattage);
            cmd.Parameters.Add("@replacement_criteria_id", MySqlDbType.Int32);
            cmd.Parameters["@replacement_criteria_id"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            CriteriaId = Convert.ToInt32(cmd.Parameters["@replacement_criteria_id"].Value);

            return CriteriaId;
        }

        public static List<dynamic> SPDeleteReplacementCriteria(MySqlTransaction transaction, MySqlConnection conn)
        {
            var results = new List<dynamic>();
            string spName = "proc_empty_replacement_criteria";
            MySqlCommand cmd = new MySqlCommand(spName, conn, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
            return results;
        }

        public static void SPInsertProduct(string[] ProductValues, MySqlTransaction transaction, MySqlConnection conn, ref string rowColumn)
        {

            string[] Db_ParamValues = new string[] {"@space","@application","@primaryApplication","@family","@name","@description","@watts","@lumens","@efficacy",
                                                    "@light_source","@replacement","@voltage", "@cct","@cri","@operating_temp","@surge_supression","@dimming",
                                                    "@beam","@lens","@finish_options","@standout_features","@additional_options","@mounting_options","@certificate",
                                                    "@warranty","@wp_photo_title","@wp_product_id"};
            string spName = "proc_insert_product";


            MySqlCommand cmd = new MySqlCommand(spName, conn, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            AddParamswithValues(cmd, Db_ParamValues, ProductValues, ref rowColumn);
            cmd.ExecuteNonQuery();
        }

        private static MySqlCommand AddParamswithValues(MySqlCommand cmd, string[] ParamNames, string[] ProductValues, ref string rowColumn)
        {
            for (int i = 0; i < ParamNames.Length; i++)
            {
                rowColumn = (i + 1).ToString();
                if (ParamNames[i].Equals("@watts") || ParamNames[i].Equals("@lumens") || ParamNames[i].Equals("@efficacy"))
                {
                    ProductValues[i] = string.IsNullOrEmpty(ProductValues[i]) ? "0" : Math.Round(float.Parse(ProductValues[i])).ToString();
                    cmd.Parameters.AddWithValue(ParamNames[i], ProductValues[i]);
                }
                else if (ParamNames[i].Equals("@wp_product_id"))
                {
                    ProductValues[i] = string.IsNullOrEmpty(ProductValues[i]) ? "0" : ProductValues[i];
                    cmd.Parameters.AddWithValue(ParamNames[i], int.Parse(ProductValues[i]));
                }
                else if (ParamNames[i].Equals("@replacement"))
                {
                    cmd.Parameters.AddWithValue(ParamNames[i], Regex.Replace(ProductValues[i], @"\D", "").Trim());
                }
                else
                {
                    cmd.Parameters.AddWithValue(ParamNames[i], ProductValues[i]);
                }

            }

            return cmd;
        }

        public static void SPDeleteProduct(MySqlTransaction transaction, MySqlConnection conn)
        {
            string spName = "proc_empty_product";
            MySqlCommand cmd = new MySqlCommand(spName, conn, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }

        public static void SPDeleteValidation(MySqlTransaction transaction, MySqlConnection conn)
        {
            string spName = "proc_empty_validation";
            MySqlCommand cmd = new MySqlCommand(spName, conn, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }

        public static int SPInsertLegend(string name, string description, MySqlTransaction transaction, MySqlConnection conn)
        {
            int LegendId = 0;
            string spName = "proc_insert_legend";

            MySqlCommand cmd = new MySqlCommand(spName, conn, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.Add("@legend_id", MySqlDbType.Int32);
            cmd.Parameters["@legend_id"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            LegendId = Convert.ToInt32(cmd.Parameters["@legend_id"].Value);
            return LegendId;
        }

        public static void SPDeleteLegend(MySqlTransaction transaction, MySqlConnection conn)
        {
            string spName = "proc_empty_legend";
            MySqlCommand cmd = new MySqlCommand(spName, conn, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }

        public static DataTable ExecuteQuery(string sqlCmd)
        {
            DataTable dt = new DataTable();
            var results = new List<dynamic>();
            MySqlTransaction transaction = null;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    MySqlCommand cmd = new MySqlCommand(sqlCmd, conn, transaction);
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }

                return dt;
            }
        }

        public static List<dynamic> ExecuteNewSqlQuery(string sql)
        {
            var results = new List<dynamic>();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        int count = reader.FieldCount;
                        while (reader.Read())
                        {
                            Dictionary<string, string> dict = new Dictionary<string, string>();
                            for (int i = 0; i < count; i++)
                            {
                                if (Convert.ToString(reader[i]) != null)
                                    dict.Add(reader.GetName(i), Convert.ToString(reader[i]));
                                else
                                    dict.Add(reader.GetName(i), (reader[i]).ToString());
                            }
                            results.Add(dict);
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return results;
        }

        public static List<dynamic> ExecuteSqlQuery(string sql)
        {
            var results = new List<dynamic>();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int count = reader.FieldCount;
                        while (reader.Read())
                        {
                            Dictionary<string, string> dict = new Dictionary<string, string>();
                            for (int i = 0; i < count; i++)
                            {
                                dict.Add(reader.GetName(i), reader[i].ToString());
                            }
                            results.Add(dict);
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return results;
        }

        public static List<dynamic> ExecuteTransactionSqlQuery(string sql, MySqlTransaction transaction, MySqlConnection conn)
        {
            var results = new List<dynamic>();

            MySqlCommand cmd = new MySqlCommand(sql, conn, transaction);
            using (var reader = cmd.ExecuteReader())
            {
                int count = reader.FieldCount;
                while (reader.Read())
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    for (int i = 0; i < count; i++)
                    {
                        dict.Add(reader.GetName(i), Convert.ToString(reader[i]));
                    }
                    results.Add(dict);
                }
                reader.Close();
            }
            return results;
        }

        public static List<dynamic> ExecuteSqlQuery1(string sql, MySqlParameter[] param)
        {
            var results = new List<dynamic>();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddRange(param);
                    using (var reader = cmd.ExecuteReader())
                    {
                        int count = reader.FieldCount;
                        while (reader.Read())
                        {
                            Dictionary<string, string> dict = new Dictionary<string, string>();
                            for (int i = 0; i < count; i++)
                            {
                                dict.Add(reader.GetName(i), Convert.ToString(reader[i]));
                            }
                            results.Add(dict);
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return results;
        }


        public static DataTable SPGetApplicationTypes(string spaceId)
        {
            MySqlTransaction transaction = null;
            DataTable dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    MySqlCommand cmd = new MySqlCommand("proc_get_application_types", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@space_id", spaceId);
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    var x = ex.ToString();
                    throw;
                }

            }
            return dt;
        }

        public static DataTable SPGetLightSources(string groupNumber)
        {
            MySqlTransaction transaction = null;
            DataTable dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    MySqlCommand cmd = new MySqlCommand("proc_get_light_sources", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@groupNumber", groupNumber);
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    var x = ex.ToString();
                    throw;
                }

            }
            return dt;
        }

        public static DataTable SPGetLampOptions()
        {
            MySqlTransaction transaction = null;
            DataTable dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    MySqlCommand cmd = new MySqlCommand("proc_get_lamp_options", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    var x = ex.ToString();
                    throw;
                }

            }
            return dt;
        }

        public static Dictionary<string, string> GetMessgaes(string sql)
        {
            Dictionary<string, string> message = new Dictionary<string, string>();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            message.Add(Convert.ToString(reader["parameter"]), Convert.ToString(reader["value"]));
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return message;
        }

        public static Dictionary<string, string> GetExtension(string sql)
        {
            Dictionary<string, string> message = new Dictionary<string, string>();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            message.Add(Convert.ToString(reader["id"]), Convert.ToString(reader["extension"]));
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return message;
        }

        public static List<dynamic> ExecuteSqlCommandQuery(MySqlCommand cmd)
        {
            var results = new List<dynamic>();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                try
                {
                    conn.Open();
                    //MySqlCommand cmd = new MySqlCommand(sql, conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        int count = reader.FieldCount;
                        while (reader.Read())
                        {
                            Dictionary<string, string> dict = new Dictionary<string, string>();
                            for (int i = 0; i < count; i++)
                            {

                                dict.Add(reader.GetName(i), Convert.ToString(reader[i]));
                            }
                            results.Add(dict);
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return results;
        }


    }
}