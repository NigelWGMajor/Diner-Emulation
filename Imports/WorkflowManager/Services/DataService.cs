using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Nix.Library.SqlStore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using WorkflowManager.Models;

namespace WorkflowManager.Services
{
    public interface IDataStore
    {
        long Store<T>(string tableName, string identityName, T data);
        T[] Recall<T>(string tableName, string identityName, long identity);
        T[] Recall<T>(string tableName, string condition);
        T[] Recall<T>(string sql);
        Task<long> StoreAsync<T>(string tableName, string identityName, T data);
        Task<T[]> RecallAsync<T>(string tableName, string identityName, long identity);
        Task<T[]> RecallAsync<T>(string tableName, string condition);
        Task<T[]> RecallAsync<T>(string sql);
    }
    public class DataStore : IDataStore
    {
        // Simplified db access Store or Recall <T>:
        // 
        // Identifiers shuold be long (bigint in sql)
        //
        // Store: You can insert an object into the store and get an identifier
        //        You can update an object in the store if you know the identifier
        //
        // Recall: You can retrieve an object by identifier
        //         You can retrieve objects matching some condition
        //
        // For store, the type must be a subset of the store columns
        // For recall, only the matched properties are populated 

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
                    if (property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(bool)
                        || property.PropertyType == typeof(DateTime))
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
                    if (property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(bool)
                        || property.PropertyType == typeof(DateTime))
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

        public async Task<long> StoreAsync<T>(string tableName, string identityName, T data)
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
                    if (property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(bool)
                        || property.PropertyType == typeof(DateTime))
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
                    if (property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(bool)
                        || property.PropertyType == typeof(DateTime))
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
                    await command.ExecuteNonQueryAsync();
                    return identityValue;
                }
                else // is insert
                {
                    object x = await command.ExecuteScalarAsync();
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
        public async Task<T[]> RecallAsync<T>(string tableName, string identityName, long identity)
        {
            T[] x = new T[0];
            string sql = $"select * from {tableName} where {identityName}={identity} for json path";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Connection.Open();
                var result = await command.ExecuteReaderAsync();
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
        /// <param name="condition"></param>
        /// <returns></returns>
        public T[] Recall<T>(string tableName, string condition)
        {
            T[] x = new T[0];
            string sql = $"select * from {tableName} where {condition} for json path";
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
        public async Task<T[]> RecallAsync<T>(string tableName, string condition)
        {
            T[] x = new T[0];
            string sql = $"select * from {tableName} where {condition} for json path";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Connection.Open();
                var result = await command.ExecuteReaderAsync();
                while (result.Read())
                {
                    var j0 = result.GetString(0);
                    x = JsonConvert.DeserializeObject<T[]>(j0);
                }
            }
            return x;
        }
        public T[] Recall<T>(string sql)
        {
            T[] x = new T[0];
            if (!sql.ToLower().Contains("json"))
                sql += " for json path";
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

        public async Task<T[]> RecallAsync<T>(string sql)
        {
            T[] x = new T[0];
            if (!sql.ToLower().Contains("json"))
                sql += " for json path";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Connection.Open();
                var result = await command.ExecuteReaderAsync();
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
