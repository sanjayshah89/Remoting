using Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class UserDataManager:DataManager
    {
        #region public User UserLogOn(string userName, string password)

        /// <summary>
        /// Checks whether the user name and password is correct, and return the corresponding User object
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="password">Password</param>
        /// <returns>User Object</returns>
        public User UserLogOn(string userName, string password)
        {
            DataSet returnDataSet = null;
            User user = null;
           
                using (SqlCommand command = NewCommand())
                {
                    command.CommandText = "[dbo].[UserLogin]";
                    AddStringParameter(command, "@username", userName);
                    AddStringParameter(command, "@password", password);
                    returnDataSet = ExecuteQuery(command);
                    if (returnDataSet != null && returnDataSet.Tables != null && returnDataSet.Tables.Count > 0 && returnDataSet.Tables[0].Rows != null && returnDataSet.Tables[0].Rows.Count > 0)
                    {
                        user = new User();

                        DataRow UserRow = returnDataSet.Tables[0].Rows[0];

                        if (UserRow["Id"] != DBNull.Value)
                            user.Id = Convert.ToInt32(UserRow["Id"].ToString(), CultureInfo.InvariantCulture);
                        if (UserRow["Username"] != DBNull.Value)
                            user.Username = UserRow["Username"].ToString();                      
                        if (UserRow["LoginDate"] != DBNull.Value)
                            user.LoginDate = Convert.ToDateTime(UserRow["LoginDate"].ToString(), CultureInfo.InvariantCulture);                      
                    }
                }           
            return user;
        }
        #endregion public User UserLogOn(string userName, string password)

        #region public Department[] GetDepartments()
        public Department[] GetDepartments()
        {
            DataSet returnDataSet = null;
            Department[] departments = null;
          

            using (SqlCommand command = NewCommand())
            {
                List<Department> DepartmentList = new List<Department>();
                Department department = null;
                
                command.CommandText = "[dbo].[GetDepartments]";                
                returnDataSet = ExecuteQuery(command);
                if (returnDataSet != null && returnDataSet.Tables != null && returnDataSet.Tables.Count > 0 && returnDataSet.Tables[0].Rows != null && returnDataSet.Tables[0].Rows.Count > 0)
                {
                    department = new Department();

                    DataRow UserRow = returnDataSet.Tables[0].Rows[0];

                    if (UserRow["DepartmentId"] != DBNull.Value)
                        department.DepartmentId = Convert.ToInt32(UserRow["DepartmentId"].ToString(), CultureInfo.InvariantCulture);
                    if (UserRow["Name"] != DBNull.Value)
                        department.Name = UserRow["Name"].ToString();
                    if (UserRow["GroupName"] != DBNull.Value)
                        department.GroupName = UserRow["GroupName"].ToString();
                    if (UserRow["ModifiedDate"] != DBNull.Value)
                        department.ModifiedDate = Convert.ToDateTime(UserRow["ModifiedDate"].ToString(), CultureInfo.InvariantCulture);
                }
                if (DepartmentList.Count > 0)
                    departments = DepartmentList.ToArray();
            }
            return departments;
        }
        #endregion public Department[] GetDepartments()

        #region public DataSet GetDepartmentsDataSet()
        public DataSet GetDepartmentsDataSet()
        {
            DataSet returnDataSet = null;         


            using (SqlCommand command = NewCommand())
            {             
                command.CommandText = "[dbo].[GetDepartments]";
                returnDataSet = ExecuteQuery(command);   
            }
            return returnDataSet;
        }
        #endregion public DataSet GetDepartmentsDataSet()

        #region public User[] GetUsers()
        public User[] GetUsers()
        {
            DataSet returnDataSet = null;
            User[] users = null;


            using (SqlCommand command = NewCommand())
            {
                List<User> UserList = new List<User>();
                User user = null;

                command.CommandText = "[dbo].[GetUsers]";
                returnDataSet = ExecuteQuery(command);
                if (returnDataSet != null && returnDataSet.Tables != null && returnDataSet.Tables.Count > 0 && returnDataSet.Tables[0].Rows != null && returnDataSet.Tables[0].Rows.Count > 0)
                {
                    user = new User();

                    DataRow UserRow = returnDataSet.Tables[0].Rows[0];

                    if (UserRow["Id"] != DBNull.Value)
                        user.Id = Convert.ToInt32(UserRow["Id"].ToString(), CultureInfo.InvariantCulture);
                    if (UserRow["Username"] != DBNull.Value)
                        user.Username = UserRow["Username"].ToString();
                    if (UserRow["Password"] != DBNull.Value)
                        user.Password = UserRow["Password"].ToString();
                    if (UserRow["LoginDate"] != DBNull.Value)
                        user.LoginDate = Convert.ToDateTime(UserRow["LoginDate"].ToString(), CultureInfo.InvariantCulture);
                }
                if (UserList.Count > 0)
                    users = UserList.ToArray();
            }
            return users;
        }
        #endregion  public User[] GetUsers()

    }
}
