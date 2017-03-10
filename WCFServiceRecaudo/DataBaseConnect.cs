namespace WCFServiceRecaudo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Data.OracleClient;
    using System.Configuration;

    /// <summary>
    /// Summary description for DataBaseConnect
    /// </summary>
    public class DataBaseConnect
    {
        #region Variables
        private OracleConnection con;
        #endregion //Variables

        #region Constantes

        //Cadena de coneccion
        string oradb = ConfigurationManager.ConnectionStrings["ConexionSieWeb"].ConnectionString;

        #endregion //Constantes

        #region Métodos públicos

        /// <summary>
        /// Crea y abre una conexión a la base de datos
        /// </summary>
        /// <returns>Una conexión abierta a la base de datos</returns>
        public OracleConnection CreateConection()
        {
            con = new OracleConnection(oradb);
            con.Open();

            return con;
        }

        /// <summary>
        /// Cierra la conexión a la base de datos
        /// </summary>
        public void CloseConnection()
        {
            if (con != null)
            {
                if (con.State == System.Data.ConnectionState.Open ||
                    con.State == System.Data.ConnectionState.Connecting ||
                    con.State == System.Data.ConnectionState.Executing ||
                    con.State == System.Data.ConnectionState.Fetching)
                {
                    con.Close();
                }
            }
        }
        #endregion //Métodos públicos
    }

}