using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Data;

namespace WCFServiceRecaudo
{
    public class CreaCuponPago
    {
        public List<DatosCuponPago> creaCuponPago(
                                                int nuIdSuscriptor,
                                                int nuValorCuponPago,
                                                int nuIdTipoCuponPago,
                                                int nuIdEstadoCuponPago,
                                                double nuIdCuentaCobro
                                                )
        {
            List<DatosCuponPago> RetornaConsultaCuponPago = new List<DatosCuponPago>();
            string idSecuencia = string.Empty;
            //Obtiene DNS Pc 
            IPHostEntry capturahost = Dns.GetHostEntry(Dns.GetHostName());
            string hostname = capturahost.ToString();
            string vaUser = "RECAUDADOR";
            int nuIdPais = 57; int nuIdDepartamento = 76; int nuIdMunicipio = 834;
            //System.Threading.Thread.Sleep(1000);
            DataTable dtDatosCuponGenerado = new DataTable();
            //obtener la secuencia del cupon
            SiewebDBCommand cmdSecuencia = new SiewebDBCommand();
            cmdSecuencia.QueryString = " SELECT  PROXIMONUMERO FROM cs_secuencia WHERE IDSECUENCIA = 'CMCUPA_IDCUPONPAGO' AND IDPAIS = " + Session["codigopais"].ToString() + "  AND IDDEPARTAMENTO = " + Session["codigodepartamento"].ToString() + " AND IDMUNICIPIO = " + Session["codigomunicipio"].ToString() + " ";
            DataTable dtSecuencia = cmdSecuencia.ExecuteStringCommand();
            if (dtSecuencia.Rows.Count > 0)
            {
                foreach (DataRow record in dtSecuencia.Rows)
                {
                    idSecuencia = record["PROXIMONUMERO"].ToString();
                }
            }
            string vaSecuencia = nuIdPais + nuIdDepartamento + nuIdMunicipio + idSecuencia;
            int numeroSecuencia = Convert.ToInt32(vaSecuencia);
            DateTime daFechaGeneracion = DateTime.Today;
            using (SiewebDBCommand cmdCuponPago = new SiewebDBCommand())
            {
                //cmdCuponPago.QueryString = "INSERT INTO cm_cuponpago VALUES(" + numeroSecuencia + "," + nuValorCuponPago + ",'" + daFechaGeneracion.Day + "/" + daFechaGeneracion.Month + "/" + daFechaGeneracion.Year + "',";
                cmdCuponPago.QueryString = "INSERT INTO cm_cuponpago VALUES(" + numeroSecuencia + "," + nuValorCuponPago + ",'" + daFechaGeneracion + "',";
                cmdCuponPago.QueryString = cmdCuponPago.QueryString + nuIdTipoCuponPago + "," + nuIdEstadoCuponPago + "," + nuIdCuentaCobro + ",";
                cmdCuponPago.QueryString = cmdCuponPago.QueryString + nuIdPais + "," + nuIdDepartamento + "," + nuIdMunicipio + ",";
                cmdCuponPago.QueryString = cmdCuponPago.QueryString + " '" + vaUser + "','" + hostname + "')";
                cmdCuponPago.ExecuteStringNonQueryCommand();
            }

            using (SiewebDBCommand cmdActualizaEstado = new SiewebDBCommand())
            {
                cmdActualizaEstado.QueryString = " UPDATE cm_cuponpago SET idestadocupon = 2 ";
                cmdActualizaEstado.QueryString = cmdActualizaEstado.QueryString + "WHERE  idcuentacobro IN (SELECT idcuentacobro FROM cm_cuentacobro WHERE idsuscriptor = " + nuIdSuscriptor + ") ";
                cmdActualizaEstado.QueryString = cmdActualizaEstado.QueryString + "AND idcuponpago != " + numeroSecuencia + " and   idpais = " + nuIdPais + " and iddepartamento = " + nuIdDepartamento + " and idmunicipio = " + nuIdMunicipio + " and idestadocupon != 3 ";
                cmdActualizaEstado.ExecuteStringNonQueryCommand();
            }


            SiewebDBCommand cmdIncrementaSecuencia = new SiewebDBCommand();
            cmdIncrementaSecuencia.QueryString = " UPDATE cs_secuencia SET PROXIMONUMERO = PROXIMONUMERO + 1 WHERE IDSECUENCIA =  'CMCUPA_IDCUPONPAGO' AND IDPAIS = " + nuIdPais + " AND IDDEPARTAMENTO = " + nuIdDepartamento + " AND IDMUNICIPIO = " + nuIdMunicipio;
            cmdIncrementaSecuencia.ExecuteStringNonQueryCommand();
            //omb.ShowMessage("Se genero con exito el cupón: " + idSecuencia);

            SiewebDBCommand cmdRetornaCuponpago = new SiewebDBCommand();
            cmdRetornaCuponpago.QueryString = "SELECT idcuponpago,valor,fechageneracion,idcuentacobro FROM cm_cuponpago WHERE idcuponpago= " + numeroSecuencia;
            cmdRetornaCuponpago.ExecuteStringNonQueryCommand();

            dtDatosCuponGenerado = cmdRetornaCuponpago.ExecuteStringCommand();
            foreach (DataRow dr in dtDatosCuponGenerado.Rows)
            {
                DatosCuponPago ConsultaCuponPagoNuevo = new DatosCuponPago();
                ConsultaCuponPagoNuevo.nuIdCuponPago = Convert.ToInt64(dr["idcuponpago"]);
                ConsultaCuponPagoNuevo.nuValorCuponPago = Convert.ToInt32(dr["valor"]);
                ConsultaCuponPagoNuevo.daFechaGeneracion = Convert.ToDateTime(dr["fechageneracion"]);
                RetornaConsultaCuponPago.Add(ConsultaCuponPagoNuevo);
            }
            return RetornaConsultaCuponPago;
        } 

    }
}