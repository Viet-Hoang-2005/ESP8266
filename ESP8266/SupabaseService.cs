using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ESP8266
{
    public class SupabaseService
    {
        private static readonly string connString = "Host=aws-0-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bzjfiynoyelxlpowlhty;Password=laptrinhmang;SSL Mode=Require;Trust Server Certificate=true";
        private static NpgsqlConnection _connection;
        private static readonly object _lock = new object();

        // Singleton pattern để đảm bảo chỉ có một kết nối
        public static NpgsqlConnection GetConnection()
        {
            lock (_lock)
            {
                if (_connection == null || _connection.State == System.Data.ConnectionState.Closed)
                {
                    _connection = new NpgsqlConnection(connString);
                    _connection.Open();
                }
                return _connection;
            }
        }

        // Đóng kết nối khi không cần thiết (gọi khi ứng dụng kết thúc hoặc cần reset)
        public static void CloseConnection()
        {
            lock (_lock)
            {
                if (_connection != null && _connection.State != System.Data.ConnectionState.Closed)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }

        // Phương thức Insert dữ liệu
        public static int Insert(string tableName, Dictionary<string, object> data)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    var columns = string.Join(", ", data.Keys);
                    var parameters = string.Join(", ", data.Keys.Select(k => "@" + k));
                    var query = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters}) RETURNING id_user";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        foreach (var kvp in data)
                        {
                            cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Lỗi Insert vào {tableName}: {ex.Message}");
            }
        }

        // Phương thức Query (thực thi truy vấn tùy chỉnh)
        public static void Query(string sql, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var kvp in parameters)
                            {
                                cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                            }
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Lỗi Query: {ex.Message}");
            }
        }

        // Phương thức Select (trả về DataTable)
        public static DataTable Select(string sql, Dictionary<string, object> parameters = null)
        {
            var dataTable = new DataTable();
            try
            {
                using (var conn = GetConnection())
                {
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var kvp in parameters)
                            {
                                cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                            }
                        }
                        using (var adapter = new NpgsqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
                return dataTable;
            }
            catch (NpgsqlException ex)
            {
                throw new Exception($"Lỗi Select: {ex.Message}");
            }
        }
    }
}