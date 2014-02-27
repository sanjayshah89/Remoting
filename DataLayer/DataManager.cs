using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using System.Xml;
using System.Configuration;

namespace DataLayer
{
    public class DataManager : IDisposable
    {

        private SqlConnection dbConn;
        private SqlTransaction sqlTransation;
        private string connectionString = string.Empty;


        private int defaultCommandTimeout = 10000; // This needs to be picked up from the config file
        public bool IsConnected { get; set; }

        #region public DataManager()

        protected DataManager()
        {
            // Obtain Connection string from the web.config file - WCF Service
            try
            {
                if (ConfigurationManager.ConnectionStrings["ConnectionString"] == null)
                {
                    throw new ConfigurationErrorsException("Internal Server Error : Connection string is not properly configured in your configuration file.");
                }
                connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            catch (System.Configuration.ConfigurationErrorsException)
            {
                throw;
            }

            // Using the connection string connect to database
            try
            {
                dbConn = new SqlConnection(connectionString);
                dbConn.Open();
            }
            catch (InvalidOperationException)
            {
                IsConnected = false;
                throw;
            }
            catch (SqlException)
            {
                IsConnected = false;
                throw;
            }
            catch (ArgumentException)
            {
                IsConnected = false;
                throw;
            }


            // Sanity check on the open connection
            if (dbConn == null)
            {
                IsConnected = false;
                throw new ArgumentException("Connection object is not filled, connection can not be opened now, the state is broken or closed");

            }
            if ((dbConn.State == ConnectionState.Broken) || (dbConn.State == ConnectionState.Closed))
            {
                IsConnected = false;
                throw new ArgumentException("Connection object is not filled, connection can not be opened now, the state is broken or closed");
            }

            IsConnected = true;
        }
        #endregion public DataManager()

        #region public void Dispose()
        public void Dispose()
        {
            Dispose(true);
            /// Since Dispose will release all resources that an object holds onto, there’s no reason for 
            /// the garbage collector to finalize the object. To tell the garbage collector that the object 
            /// does not need to be finalized, even though it has a finalizer, call GC.SuppressFinalize after 
            /// the cleanup code has executed in your Dispose method.

            GC.SuppressFinalize(this);

        } // end Dispose

        #endregion public void Dispose()

        #region ~DataManager()
        ~DataManager()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }
        #endregion ~DataManager()

        #region protected virtual void Dispose(bool disposing)
        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (dbConn != null)
                {
                    dbConn.Close();
                    dbConn.Dispose();
                    dbConn = null;
                }
                sqlTransation = null;
            }
        }
        #endregion protected virtual void Dispose(bool disposing)

        #region protected SqlCommand GetNewCommand()
        /// <summary>
        /// Return a sql command for use with this class
        /// </summary>
        /// <returns>SqlCommand</returns>
        protected SqlCommand NewCommand()
        {
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Connection = dbConn;
            sqlCommand.Transaction = sqlTransation;
            sqlCommand.CommandTimeout = this.defaultCommandTimeout;

            return sqlCommand;
        }
        #endregion protected SqlCommand GetNewCommand()

        #region protected System.Data.DataSet ExecuteQuery(SqlCommand sqlCommand)
        /// <summary>
        /// Execute a query that does expect a result
        /// </summary>
        /// <param name="sqlCommand">an SqlCommand object</param>
        /// <returns>a dataset containing results</returns>
        protected DataSet ExecuteQuery(SqlCommand sqlCommand)
        {
            DataSet dataSet = null;
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand))
            {
                dataSet = new DataSet();
                dataSet.Locale = CultureInfo.InvariantCulture;
                // The Return Value
                // The number of rows successfully added to or refreshed in the DataSet. 
                // This does not include rows affected by statements that do not return rows
                dataAdapter.Fill(dataSet);
            }
            return dataSet;
        }
        #endregion protected System.Data.DataSet ExecuteQuery(SqlCommand sqlCommand)

        #region protected object ExecuteScalar(SqlCommand command)
        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. 
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected object ExecuteScalar(DbCommand command)
        {
            object ret = command.ExecuteScalar();

            return ret != DBNull.Value ? ret : null;
        }

        #endregion protected object ExecuteScalar(SqlCommand command)

        protected void AddStringParameter(SqlCommand command, string parameterName, string parameterValue)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = DbType.String;
            if (string.IsNullOrEmpty(parameterValue))
            {
                param.Value = DBNull.Value;
            }
            else
            {
                param.Value = parameterValue;
            } // end if
            command.Parameters.Add(param);
        }

        protected void AddStringOutputParameter(SqlCommand command, string parameterName, int size)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.String;
            param.Value = string.Empty;
            param.Size = size;
            command.Parameters.Add(param);
        }



        protected void AddIntParameter(SqlCommand command, string parameterName, int parameterValue)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = DbType.Int32;
            param.Value = parameterValue;
            command.Parameters.Add(param);

        }


        protected void AddIntOutputParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.Int32;
            command.Parameters.Add(param);

        }

        protected void AddIntReturnParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.ReturnValue;
            param.DbType = DbType.Int32;
            command.Parameters.Add(param);

        }

        protected void AddUIntParameter(SqlCommand command, string parameterName, uint parameterValue)
        {

            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = DbType.Int64;
            param.Value = parameterValue;
            command.Parameters.Add(param);

        }

        protected void AddUIntOutputParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.Int64;
            command.Parameters.Add(param);
        }

        protected void AddLongParameter(SqlCommand command, string parameterName, long parameterValue)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = DbType.Int64;
            param.Value = parameterValue;
            command.Parameters.Add(param);

        }

        protected void AddLongOutputParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.Int64;
            command.Parameters.Add(param);
        }

        protected void AddBoolParameter(SqlCommand command, string parameterName, bool parameterValue)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = DbType.Boolean;
            param.Value = parameterValue;
            command.Parameters.Add(param);

        }

        protected void AddBoolOuputParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.Boolean;
            param.Value = true;
            command.Parameters.Add(param);
        }

        protected void AddDecimalParameter(SqlCommand command, string parameterName, decimal parameterValue)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = DbType.Decimal;
            param.Value = parameterValue;
            command.Parameters.Add(param);
        }

        protected void AddFloatParameter(SqlCommand command, string parameterName, float parameterValue)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = DbType.Decimal;
            param.Value = parameterValue;
            command.Parameters.Add(param);
        }

        protected void AddDecimalOutputParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.Decimal;
            command.Parameters.Add(param);
        }
        protected void AddDoubleOutputParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.Double;
            command.Parameters.Add(param);
        }

        protected void AddDecimalReturnParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.ReturnValue;
            param.DbType = DbType.Decimal;
            command.Parameters.Add(param);
        }

        protected void AddDateTimeParameter(SqlCommand command, string parameterName, DateTime parameterValue)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = DbType.DateTime;
            param.Value = parameterValue;
            command.Parameters.Add(param);
        }
        protected void AddDateTimeOutputParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.DateTime;
            command.Parameters.Add(param);
        }

        protected void AddGuidParameter(SqlCommand command, string parameterName, System.Guid parameterValue)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = DbType.Guid;
            if (parameterValue.Equals(Guid.Empty) == true)
            {
                param.Value = DBNull.Value;
            }
            else
            {
                param.Value = parameterValue;
            } // end if
            command.Parameters.Add(param);
        }

        protected void AddGuidOutputParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.Guid;
            command.Parameters.Add(param);
        }

        protected void AddXmlParameter(SqlCommand command, string parameterName, string parameterValue)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = DbType.Xml;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
        }

        protected void AddXmlParameter(SqlCommand command, string parameterName, XmlReader parameterValue)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = DbType.Xml;
            parameter.Value = new SqlXml(parameterValue);
            command.Parameters.Add(parameter);

        }


        protected void AddXmlOutputParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.Xml;
            command.Parameters.Add(param);
        }



        protected void AddBinaryParameter(SqlCommand command, string parameterName, byte[] parameterValue)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.DbType = DbType.Binary;
            param.Value = new SqlBinary(parameterValue);
            command.Parameters.Add(param);

        }

        protected void AddBinaryOutputParameter(SqlCommand command, string parameterName)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Direction = ParameterDirection.Output;
            param.DbType = DbType.Binary;
            command.Parameters.Add(param);

        }

    }
}
