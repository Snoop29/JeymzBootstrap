namespace BOOTSTRAP.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string[] Fields { get; set; }
        public object[] Values { get; set; }

        public User(string fullName, string email, string password)
        {
            FullName = fullName;
            Email = email;
            Password = password;
            Fields = new string[] { "FullName", "Email", "Password" };
            Values = new object[] { fullName, email, password };
        }

        // INSERT INTO 
        public string GenerateInsertQuery(string tableName)
        {
            string query = "INSERT INTO " + tableName + " (";
            for (int i = 0; i < Fields.Length; i++)
            {
                query += Fields[i];
                if (i < Fields.Length - 1) query += ", ";
            }
            query += ") VALUES (";
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i] is string)
                {
                    query += "'" + Values[i] + "'";
                }
                else
                {
                    query += Values[i];
                }
                if (i < Values.Length - 1) query += ", ";
            }
            query += ")";
            return query;
        }

        // SELECT 
        public string GenerateSelectQuery(string tableName, string field)
        {
            string query = "SELECT ";
            for (int i = 0; i < Fields.Length; i++)
            {
                query += Fields[i];
                if (i < Fields.Length - 1) query += ", ";
            }
            query += " FROM " + tableName;
            query += " WHERE "+field+" = " +Id;
            return query;
        }

        // UPDATE
        public string GenerateUpdateQuery(string tableName, string field)
        {
            string query = "UPDATE " + tableName + " SET ";
            for (int i = 0; i < Fields.Length; i++)
            {
                query += Fields[i] + " = ";
                if (Values[i] is string)
                {
                    query += "'" + Values[i] + "'";
                }
                else
                {
                    query += Values[i];
                }
                if (i < Fields.Length - 1) query += ", ";
            }
            query += " WHERE " + field + " = " +Id;
            return query;
        }

        // DELETE 
        public string GenerateDeleteQuery(string tableName, string field)
        {
            string query = "DELETE FROM " + tableName + " WHERE " + field + " = " +Id;
            return query;
        }
        // SELECT ALL (for the dashboard table)
        public static string GenerateSelectAllQuery(string tableName)
        {
            return "SELECT Id, FullName, Email, Password FROM " + tableName;
        }

        // Used only for checking login credentials
        public string GenerateLoginQuery(string tableName)
        {
            return "SELECT Id, FullName, Email FROM " + tableName +
                   " WHERE Email = '" + Email + "' AND Password = '" + Password + "'";
        }
    }
}