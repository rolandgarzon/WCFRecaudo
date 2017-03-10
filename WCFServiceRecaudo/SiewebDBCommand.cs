
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OracleClient;
using System.Data;
using System.Data.Common;

/// <summary>
/// Summary description for SiewebDBCommand
/// </summary>
/// 
namespace WCFServiceRecaudo
{
    public class SiewebDBCommand : IDisposable
    {
        #region Variables privadas

        private DataBaseConnect dbConection = new DataBaseConnect();
        private IntPtr handle;
        private Component component = new Component();
        private bool disposed = false;
        private List<object> paramComposition;
        private List<List<object>> inputParametersList;
        private List<List<object>> outputParametersList;
        private List<DbParameter> returnedParameters;
        private DbDataReader returnQueryData;

        #endregion //Variables privadas

        #region Variable públicas

        /// <summary>
        /// Cadena de comandos que se ejecutaran en la base de datos
        /// </summary>
        public string QueryString;

        #endregion //Variables públicas

        #region Constructor

        public SiewebDBCommand()
        {
            inputParametersList = new List<List<object>>();
            outputParametersList = new List<List<object>>();
            returnedParameters = new List<DbParameter>();
        }

        #endregion //Constructor

        #region Métodos públicos

        /// <summary>
        /// Obtiene una colección de datos retornados por una consulta SQL
        /// </summary>
        /// <returns>coleccion de datos retornada por la consulta</returns>
        public DataTable ExecuteStringCommand()
        {
            return ExecuteStringCommand(0);
        }

        /// <summary>
        /// Obtiene una colección de datos en la posición ingresada que son retornados 
        /// por una consulta SQL
        /// </summary>
        /// <returns>coleccion de datos retornada por la consulta</returns>
        public DataTable ExecuteStringCommand(int tablePosition)
        {
            DataTable dtDataBase = new DataTable();

            if (!string.IsNullOrEmpty(QueryString))
            {
                OracleCommand dataBaseCommand = new OracleCommand();
                dataBaseCommand.Connection = dbConection.CreateConection();
                dataBaseCommand.CommandText = QueryString;

                OracleDataAdapter dataBaseAdapter = new OracleDataAdapter(dataBaseCommand);
                DataSet dbDataSet = new DataSet();

                dataBaseAdapter.Fill(dbDataSet);
                dbConection.CloseConnection();
                dataBaseCommand.Dispose();

                dtDataBase = dbDataSet.Tables[tablePosition];
                dbDataSet.Dispose();
            }
            return dtDataBase;
        }

        /// <summary>
        /// Ejecuta una sentencia Non query en la base de datos
        /// </summary>
        public void ExecuteStringNonQueryCommand()
        {
            ExecuteStringNonQueryCommand(0);
        }

        /// <summary>
        /// Ejecuta una sentencia Non query en la base de datos
        /// </summary>
        public void ExecuteStringNonQueryCommand(int tablePosition)
        {
            OracleTransaction dataBaseTrans;
            OracleConnection DBConnect = new OracleConnection();

            OracleCommand dataBaseCommand = new OracleCommand();
            DBConnect = dbConection.CreateConection();
            dataBaseCommand.Connection = DBConnect;
            dataBaseTrans = DBConnect.BeginTransaction();
            dataBaseCommand.Transaction = dataBaseTrans;

            try
            {
                if (!string.IsNullOrEmpty(QueryString))
                {
                    dataBaseCommand.CommandText = QueryString;
                    dataBaseCommand.Prepare();
                    dataBaseCommand.ExecuteNonQuery();
                    dataBaseTrans.Commit();

                    //OracleDataAdapter dataBaseAdapter = new OracleDataAdapter(dataBaseCommand);
                    dbConection.CloseConnection();
                    dataBaseCommand.Dispose();
                }
            }
            catch (Exception ex)
            {
                dataBaseTrans.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// Adiciona un parametro de entrada a procedimiento almacenado asociado al comando
        /// </summary>
        /// <param name="parameterName">Nombre de identificación del parámetro</param>
        /// <param name="parameterValue">Valor asociado al parámetro</param>
        public void addInParameter(string parameterName, object parameterValue)
        {
            paramComposition = new List<object>();

            paramComposition.Add(parameterName);
            paramComposition.Add(parameterValue);

            inputParametersList.Add(paramComposition);
        }

        /// <summary>
        /// Adiciona un parametro de salida de tipo Int32 al procedimiento almacenado asociado al comando
        /// </summary>
        /// <param name="parameterName">Nombre de identificación del parámetro</param>
        public void addOutNumberParameter(string parameterName)
        {
            paramComposition = new List<object>();

            paramComposition.Add(parameterName);
            paramComposition.Add(OracleType.Number);

            outputParametersList.Add(paramComposition);
        }

        /// <summary>
        /// Adiciona un parametro de salida de tipo string al procedimiento almacenado asociado al comando
        /// </summary>
        /// <param name="parameterName">Nombre de identificación del parámetro</param>
        public void addOutStringParameter(string parameterName)
        {
            paramComposition = new List<object>();

            paramComposition.Add(parameterName);
            paramComposition.Add(OracleType.VarChar);

            outputParametersList.Add(paramComposition);
        }

        /// <summary>
        /// Adiciona un parametro de salida de tipo Cursor al procedimiento almacenado asociado al comando
        /// </summary>
        /// <param name="parameterName">Nombre de identificación del parámetro</param>
        public void addOutCursorParameter(string parameterName)
        {
            paramComposition = new List<object>();

            paramComposition.Add(parameterName);
            paramComposition.Add(OracleType.Cursor);

            outputParametersList.Add(paramComposition);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado en la base de datos
        /// </summary>
        /// <param name="procedureName">Nombre del procedimiento</param>
        public void ExecuteStoredProcedure(string procedureName)
        {
            int outputParametersCount = 0;
            DbConnection conn = dbConection.CreateConection();
            DbCommand cmd = conn.CreateCommand();

            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (List<object> inputParam in inputParametersList)
            {
                DbParameter parameterIn = cmd.CreateParameter();

                parameterIn.ParameterName = inputParam[0].ToString();
                parameterIn.Value = inputParam[1];

                cmd.Parameters.Add(parameterIn);
            }
            inputParametersList.Clear();

            foreach (List<object> outputParam in outputParametersList)
            {
                OracleParameter parameterOut = (OracleParameter)cmd.CreateParameter();

                parameterOut.ParameterName = outputParam[0].ToString();
                parameterOut.OracleType = (OracleType)outputParam[1];
                parameterOut.Size = 32767; //tamaño maximo de un varchar2
                parameterOut.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(parameterOut);
                outputParametersCount++;
            }
            outputParametersList.Clear();

            if (outputParametersCount.Equals(0))
            {
                cmd.ExecuteNonQuery();
            }
            else
            {
                returnQueryData = cmd.ExecuteReader();
            }

            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                DbParameter returnedValue = cmd.Parameters[i];

                if (returnedValue.Direction == ParameterDirection.Output ||
                    returnedValue.Direction == ParameterDirection.InputOutput ||
                    returnedValue.Direction == ParameterDirection.ReturnValue)
                {
                    returnedParameters.Add(returnedValue);
                }
            }

        }

        /// <summary>
        /// Obtiene el objeto de salida del procedimiento almacenado de tipo DbDataReader
        /// </summary>
        /// <returns>Fuente de datos con la información del procedimiento almacenado de tipo DbDataReader</returns>
        public DbDataReader GetStoredProcedureData()
        {
            return returnQueryData;
        }

        /// <summary>
        /// Obtiene el objeto de salida del procedimiento almacenado de tipo DataSet
        /// </summary>
        /// <returns>Fuente de datos con la información del procedimiento almacenado de tipo DataSet</returns>
        public DataSet GetStoredProcedure()
        {
            return convertDataReaderToDataSet(returnQueryData);
        }

        /// <summary>
        /// Obtiene un parametro de salida de tipo string del procedimiento almacenado
        /// </summary>
        /// <param name="parameterName">Nombre del parametro requerido</param>
        /// <returns>Valor del parametro requerido de tipo string</returns>
        public string getOutStringParameter(string parameterName)
        {
            string stringResult = string.Empty;

            if (returnedParameters.Count != 0)
            {
                try
                {
                    foreach (DbParameter param in returnedParameters)
                    {
                        if (param.ParameterName.Equals(parameterName))
                        {
                            stringResult = param.Value.ToString();
                            returnedParameters.Remove(param);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    stringResult = string.Empty;
                    throw ex;
                }
            }

            return stringResult;
        }

        /// <summary>
        /// Obtiene un parametro de salida de tipo Int32 del procedimiento almacenado
        /// </summary>
        /// <param name="parameterName">Nombre del parametro requerido</param>
        /// <returns>Valor del parametro requerido de tipo Int32</returns>
        public Int32 getOutInt32Parameter(string parameterName)
        {
            Int32 intResult = Int32.MinValue;

            if (returnedParameters.Count != 0)
            {
                try
                {
                    foreach (DbParameter param in returnedParameters)
                    {
                        if (param.ParameterName.Equals(parameterName))
                        {
                            intResult = Convert.ToInt32(param.Value);
                            returnedParameters.Remove(param);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    intResult = Int32.MinValue;
                    throw ex;
                }
            }

            return intResult;
        }

        /// <summary>
        /// Obtiene un parametro de salida de tipo Int64 del procedimiento almacenado
        /// </summary>
        /// <param name="parameterName">Nombre del parametro requerido</param>
        /// <returns>Valor del parametro requerido de tipo Int64</returns>
        public Int64 getOutInt64Parameter(string parameterName)
        {
            Int64 doubleResult = Int64.MinValue;

            if (returnedParameters.Count != 0)
            {
                try
                {
                    foreach (DbParameter param in returnedParameters)
                    {
                        if (param.ParameterName.Equals(parameterName))
                        {
                            doubleResult = Convert.ToInt64(param.Value);
                            returnedParameters.Remove(param);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    doubleResult = Int64.MinValue;
                    throw ex;
                }
            }

            return doubleResult;
        }

        /// <summary>
        /// Obtiene un parametro de salida de tipo Decimal del procedimiento almacenado
        /// </summary>
        /// <param name="parameterName">Nombre del parametro requerido</param>
        /// <returns>Valor del parametro requerido de tipo Decimal</returns>
        public Decimal getOutDecimalParameter(string parameterName)
        {
            Decimal decimalResult = Decimal.MinValue;

            if (returnedParameters.Count != 0)
            {
                try
                {
                    foreach (DbParameter param in returnedParameters)
                    {
                        if (param.ParameterName.Equals(parameterName))
                        {
                            decimalResult = Convert.ToDecimal(param.Value);
                            returnedParameters.Remove(param);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    decimalResult = Decimal.MinValue;
                    throw ex;
                }
            }

            return (decimalResult == decimal.MinValue ? 0 : decimalResult);
        }

        /// <summary>
        /// Habilita un control de servidor para que realice la limpieza final antes 
        /// de que se libere de la memoria.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            inputParametersList.Clear();
            outputParametersList.Clear();
            returnedParameters.Clear();
            dbConection.CloseConnection();
        }

        #endregion //Métodos públicos

        #region Métodos privados

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    component.Dispose();
                }

                CloseHandle(handle);
                handle = IntPtr.Zero;

                disposed = true;
            }
        }

        private DataSet convertDataReaderToDataSet(DbDataReader reader)
        {
            DataSet dataSet = new DataSet();
            do
            {
                DataTable schemaTable = reader.GetSchemaTable();
                DataTable dataTable = new DataTable();

                if (schemaTable != null)
                {
                    for (int i = 0; i < schemaTable.Rows.Count; i++)
                    {
                        DataRow dataRow = schemaTable.Rows[i];
                        string columnName = (string)dataRow["ColumnName"];
                        DataColumn column = new DataColumn(columnName, (Type)dataRow["DataType"]);
                        dataTable.Columns.Add(column);
                    }

                    dataSet.Tables.Add(dataTable);

                    while (reader.Read())
                    {
                        DataRow dataRow = dataTable.NewRow();

                        for (int i = 0; i < reader.FieldCount; i++)
                            dataRow[i] = reader.GetValue(i);

                        dataTable.Rows.Add(dataRow);
                    }
                }
                else
                {
                    DataColumn column = new DataColumn("RowsAffected");
                    dataTable.Columns.Add(column);
                    dataSet.Tables.Add(dataTable);
                    DataRow dataRow = dataTable.NewRow();
                    dataRow[0] = reader.RecordsAffected;
                    dataTable.Rows.Add(dataRow);
                }
            }
            while (reader.NextResult());
            return dataSet;
        }

        //Llama el método necesario para limpiar los recursos no manejables
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        #endregion //Métodos privados

        #region Destructor customizado

        ~SiewebDBCommand()
        {
            Dispose(false);
        }

        #endregion //Destructor customizado
    }
}