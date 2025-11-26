using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace formQuanLy
{
    public class Database
    {
        private static string connectionString =
            @"Server=localhost\SQLEXPRESS01;Database=QuanLyTuyenDuLich;Trusted_Connection=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
 