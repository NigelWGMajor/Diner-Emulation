using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Nix.Library.SqlStore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using WorkflowManager.Models;

namespace WorkflowManager.Services
{
    public interface IDataService
    {

    }
    public class DataStore : IDataStore
    {
        // Simplified db access: shold be translatable for any db.
        // You can insert one record into a table and get the id
        // you can update a record if you know the id
        // you can retrieve one record by id
        // you can retrieve records where a condition is true

        private string _connectionString;
        // using Microsoft.Data.SqlClient;
        public DataStore(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// Insert or update a type into a table.
        /// If the identity field is > 0 this will try to update
        /// The type can be partial, but cannot contain properties that do not
        /// exist in the data table. The identity field is returned.
        /// </summary>
        /// <typeparam name="T">A type that has properties all of which are present in the table</typeparam>
        /// <param name="tableName">A table that is able to hold the type</param>
        /// <param name="identityName">The name of the identity field</param>
        /// <param name="data">The type with data for a single row</param>
        /// <returns></returns>
        public long Store<T>(string tableName, string identityName, T data)
        {
            long identityValue = 0;
            StringBuilder sb = new StringBuilder();
            StringBuilder suffix = new StringBuilder(); ;
            Type t = typeof(T);
            bool isUpdate = false;
            var properties = t.GetProperties();
            foreach (var property in properties)
            {
                if (property.Name.ToLower().CompareTo(identityName.ToLower()) == 0)
                {   // id

                    object x = property.GetValue(data);
                    identityValue = Convert.ToInt64(x);
                    if (identityValue > 0)
                    {   // this is an update.
                        isUpdate = true;
                        suffix.Append($" where {identityName} = {identityValue}");
                    }
                    else
                    {   // this is an insert.
                        isUpdate = false;
                    }
                }
            }
            if (isUpdate)
            {
                bool initial = true;
                sb.Append($"update {tableName} set ");
                foreach (var property in properties)
                {   // update t set x=a, y=b ... | where i= z
                    if (property.Name.ToLower().CompareTo(identityName.ToLower()) == 0)
                    {   // don't set the identity property!
                        continue;
                    }
                    if (property.PropertyType == typeof(string))
                    {
                        if (initial)
                            sb.Append($"{property.Name}='{property.GetValue(data)}'");
                        else
                            sb.Append($", {property.Name}='{property.GetValue(data)}'");
                        initial = false;
                    }
                    else
                    {
                        if (initial)
                            sb.Append($"{property.Name}={property.GetValue(data).ToString()}");
                        else
                            sb.Append($", {property.Name}={property.GetValue(data).ToString()}");
                        initial = false;
                    }
                }
                sb.Append(suffix);
            }
            else
            {   // insert into t (x, y, z) values | (a, b, c)
                bool initial = true;
                sb.Append($"insert into {tableName} (");
                foreach (var property in properties)
                {
                    if (property.Name.ToLower().CompareTo(identityName.ToLower()) == 0)
                    {   // don't set the identity property!
                        continue;
                    }
                    if (property.PropertyType == typeof(string))
                    {
                        if (initial)
                        {
                            sb.Append($"{property.Name}");
                            suffix.Append($"'{property.GetValue(data)}'");
                        }
                        else
                        {
                            sb.Append($", {property.Name}");
                            suffix.Append($", '{property.GetValue(data)}'");
                        }
                        initial = false;
                    }
                    else
                    {
                        if (initial)
                        {
                            sb.Append($"{property.Name}");
                            suffix.Append($"{property.GetValue(data).ToString()}");
                        }
                        else
                        {
                            sb.Append($", {property.Name}");
                            suffix.Append($", {property.GetValue(data).ToString()}");
                        }
                        initial = false;
                    }
                }
                sb.Append($") output inserted.{identityName} values (");
                sb.Append(suffix);
                sb.Append(")");
            }
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(sb.ToString(), connection);
                command.Connection.Open();
                if (isUpdate)
                {
                    command.ExecuteNonQuery();
                    return identityValue;
                }
                else // is insert
                {
                    object x = command.ExecuteScalar();
                    return Convert.ToInt64(x);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="identityName"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public T[] Recall<T>(string tableName, string identityName, long identity)
        {
            T[] x = new T[0];
            string sql = $"select * from {tableName} where {identityName}={identity} for json path";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Connection.Open();
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    var j0 = result.GetString(0);
                    x = JsonConvert.DeserializeObject<T[]>(j0);
                }
            }
            return x;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public T[] Recall<T>(string tableName, string whereClause)
        {
            T[] x = new T[0];
            string sql = $"select * from {tableName} where {whereClause} for json path";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Connection.Open();
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    var j0 = result.GetString(0);
                    x = JsonConvert.DeserializeObject<T[]>(j0);
                }
            }
            return x;
        }
    }
}
