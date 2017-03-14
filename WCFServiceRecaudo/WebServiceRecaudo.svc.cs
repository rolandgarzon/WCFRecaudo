using System;
using System.Collections.Generic;
using System.Data;



namespace WCFServiceRecaudo
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class wsRecaudo : IWSRecaudo
    {
        /// <summary>
        /// metodo que conulta los datos de un cupon de pago
        /// </summary>
        /// el parametro de ingreso es el numero de cupon
        /// <param name="nuIdcuponpago"></param>
        /// retorna:
        /// numero de cupon
        /// valor
        /// fecha de generacion
        /// estado del cupon
        /// <returns></returns>
        public List<DatosCuponPago> obtenerDatosCuponPago(long nuIdcuponpago)
        {
            List<DatosCuponPago> ConsultaCuponPago = new List<DatosCuponPago>();

            DataTable dtDatosBasicos = new DataTable();
            string vaNumeroCuponPago = "5776834" + nuIdcuponpago;
            double numeroCuponpago = Convert.ToDouble(vaNumeroCuponPago);
            DatosCuponPago DatosCuponPago = new DatosCuponPago();
            try
            {
                using (SiewebDBCommand cmdDatosBasicos = new SiewebDBCommand())
                {
                    cmdDatosBasicos.QueryString = "SELECT idcuponpago,valor,fechageneracion,idestadocupon FROM cm_cuponpago";
                    cmdDatosBasicos.QueryString = cmdDatosBasicos.QueryString + " where idpais =57 and idestadocupon=1";
                    cmdDatosBasicos.QueryString = cmdDatosBasicos.QueryString + " and   iddepartamento =76";
                    cmdDatosBasicos.QueryString = cmdDatosBasicos.QueryString + " and   idmunicipio =834";
                    cmdDatosBasicos.QueryString = cmdDatosBasicos.QueryString + " and   idcuponpago =" + numeroCuponpago;
                    dtDatosBasicos = cmdDatosBasicos.ExecuteStringCommand();
                }

                if (dtDatosBasicos.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtDatosBasicos.Rows)
                    {
                        DatosCuponPago.nuIdCuponPago = Convert.ToInt64(dr["idcuponpago"]);
                        DatosCuponPago.nuValorCuponPago = Convert.ToInt32(dr["valor"]);
                        DatosCuponPago.daFechaGeneracion = Convert.ToDateTime(dr["fechageneracion"]);
                        DatosCuponPago.nuIdEstadoCuponPago = Convert.ToInt32(dr["idestadocupon"]);
                        ConsultaCuponPago.Add(DatosCuponPago);
                    }
                    return ConsultaCuponPago;
                }
                else
                {
                    DatosCuponPago.MensajeRespuestaError = "El numero de cupon no existe o esta inactivo";
                    ConsultaCuponPago.Add(DatosCuponPago);
                    return ConsultaCuponPago;
                }
            }
            catch
            {
                DatosCuponPago.MensajeRespuestaError = "Ocurrio un error no controlado contacte con el administrador";
                ConsultaCuponPago.Add(DatosCuponPago);
                return ConsultaCuponPago;
            }
        }
        /// <summary>
        /// metodo que retorna los datos del suscriptor con el fin de generar un cupon de pago
        /// </summary>
        /// el parametor de entrada es el numero de suscriptor
        /// <param name="nuIdsuscriptor"></param>
        /// <returns></returns>
        public List<DatosSuscriptor> obtenerDatosSuscriptor(long nuIdsuscriptor)
        {
            List<DatosSuscriptor> ConsultaSuscriptor = new List<DatosSuscriptor>();
            DataTable dtDatosBasicos = new DataTable();
            string vaNumeroSuscriptor = "5776834" + nuIdsuscriptor;
            long numeroSuscriptor = Convert.ToInt64(vaNumeroSuscriptor);
            DatosSuscriptor DatosSuscriptor = new DatosSuscriptor();
            try
            {
                using (SiewebDBCommand cmdDatosBasicos = new SiewebDBCommand())
                {
                    //Consultar datos del suscriptor
                    cmdDatosBasicos.QueryString = "select  cm_suscriptor.idsuscriptor suscriptor,trim(cm_suscriptor.nombre)||' '||trim(cm_suscriptor.apellido) nombrecompleto,TRIM(TO_CHAR(cm_suscriptor.saldopendiente,'999G999G990D99')) AS saldopendiente, Max(cm_cuentacobro.idcuentacobro) cuentacobro,facturasconsaldo";
                    cmdDatosBasicos.QueryString = cmdDatosBasicos.QueryString + " from cm_cuentacobro,cm_suscriptor";
                    cmdDatosBasicos.QueryString = cmdDatosBasicos.QueryString + " where cm_cuentacobro.idsuscriptor=cm_suscriptor.idsuscriptor";
                    cmdDatosBasicos.QueryString = cmdDatosBasicos.QueryString + " and cm_suscriptor.idsuscriptor=" + numeroSuscriptor;
                    cmdDatosBasicos.QueryString = cmdDatosBasicos.QueryString + " GROUP BY cm_suscriptor.idsuscriptor,trim(cm_suscriptor.nombre)||' '||trim(cm_suscriptor.apellido),cm_suscriptor.saldopendiente,facturasconsaldo";

                    //Consultar la ultima factura
                    SiewebDBCommand cmdUltimaFactura = new SiewebDBCommand();
                    cmdUltimaFactura.QueryString = "SELECT idcuentacobro,TRIM(TO_CHAR(nvl(sum(decode(idsigno,'DB', (valor + valoriva), ";
                    cmdUltimaFactura.QueryString = cmdUltimaFactura.QueryString + "'CR', -(valor + valoriva), ";
                    cmdUltimaFactura.QueryString = cmdUltimaFactura.QueryString + "'AS', -(valor + valoriva), ";
                    cmdUltimaFactura.QueryString = cmdUltimaFactura.QueryString + "'PA', -(valor + valoriva))), 0 ),'999G999G990D99')) nuSaldoCuentaCobroAcu ";
                    cmdUltimaFactura.QueryString = cmdUltimaFactura.QueryString + "FROM cm_cartera ";
                    cmdUltimaFactura.QueryString = cmdUltimaFactura.QueryString + "WHERE IDSUSCRIPTOR = " + numeroSuscriptor + "";
                    cmdUltimaFactura.QueryString = cmdUltimaFactura.QueryString + "and idcuentacobro = (select max(idcuentacobro) from cm_cartera where IDSUSCRIPTOR = " + numeroSuscriptor + ")";
                    cmdUltimaFactura.QueryString = cmdUltimaFactura.QueryString + "GROUP BY idcuentacobro ";
                    cmdUltimaFactura.QueryString = cmdUltimaFactura.QueryString + "ORDER BY idcuentacobro";

                    //Lleno el DataReader con los datos de la consulta
                    dtDatosBasicos = cmdDatosBasicos.ExecuteStringCommand();

                    //si la consulta encuentra datos es decir que el suscriptor existe

                    if (dtDatosBasicos.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtDatosBasicos.Rows)
                        {
                            //si el suscriptor no tiene saldo pendiente retorna un mensje
                            if (dr["saldopendiente"].ToString() == "0,00")
                            {

                                DatosSuscriptor.MensajeRespuestaError = "El suscriptor no tiene saldo pendiente";
                                ConsultaSuscriptor.Add(DatosSuscriptor);
                                //return ConsultaSuscriptor;
                            }
                            //si el suscriptor tiene saldo pendiente obtiene los datos
                            else
                            {
                                DatosSuscriptor.nuIdSuscriptor = Convert.ToInt64(dr["suscriptor"]);
                                DatosSuscriptor.vaNombre = dr["nombrecompleto"].ToString();
                                DatosSuscriptor.nuSaldoPendiente = Convert.ToDouble(dr["saldopendiente"]);
                                DatosSuscriptor.nuIdCuentaCobro = Convert.ToInt64(dr["cuentacobro"]);
                                DatosSuscriptor.nuFacturasconSaldo = Convert.ToInt32(dr["facturasconsaldo"]);

                                //datatable para el valor de la ultima factura
                                DataTable dtUltimaFactura = cmdUltimaFactura.ExecuteStringCommand();

                                if (dtUltimaFactura.Rows.Count > 0)
                                {
                                    foreach (DataRow ultimaFactRecord in dtUltimaFactura.Rows)
                                    {
                                        DatosSuscriptor.nuSaldoPendienteUltimaFactura = Convert.ToDouble(ultimaFactRecord["nuSaldoCuentaCobroAcu"]);
                                    }
                                }
                                ConsultaSuscriptor.Add(DatosSuscriptor);

                            }
                        }
                        return ConsultaSuscriptor;
                    }
                    //si el numero de suscriptor ingresado no existe retorna un mensaje
                    else
                    {
                        DatosSuscriptor.MensajeRespuestaError = "El suscriptor no existe";
                        ConsultaSuscriptor.Add(DatosSuscriptor);
                        return ConsultaSuscriptor;
                    }
                }
            }
            catch
            {
                DatosSuscriptor.MensajeRespuestaError = "Ocurrio un error no controlado contacte con el administrador";
                ConsultaSuscriptor.Add(DatosSuscriptor);
                return ConsultaSuscriptor;
            }
        }
        /// <summary>
        /// metodo que retorna verdadero si el suscriptor ingresado esta en proceso de facturacion 
        /// y falso en caso contrario es decir que se le puede generar un cupon de abono
        /// 
        /// </summary>
        /// parametro de entrada el numero dle suscriptor
        /// <param name="nuIdsuscriptor"></param>
        /// <returns>
        /// verdadero = esta en proceso de facturacion
        /// falso = se puede generar cupon
        /// </returns>
        public bool obtenerEstadoFacturacion(long nuIdsuscriptor)
        {
            DataTable dtEstadoFacturacion = new DataTable();
            string vaNumeroSuscriptor = "5776834" + nuIdsuscriptor;
            double numeroSuscriptor = Convert.ToDouble(vaNumeroSuscriptor);
            try
            {
                using (SiewebDBCommand cmdDatosBasicos = new SiewebDBCommand())
                {
                    cmdDatosBasicos.QueryString = " SELECT idsuscriptor FROM cm_suscriptor ";
                    cmdDatosBasicos.QueryString = cmdDatosBasicos.QueryString + " WHERE idsuscriptor=" + numeroSuscriptor;
                    cmdDatosBasicos.QueryString = cmdDatosBasicos.QueryString + " and idciclofacturacion in (select idciclofacturacion from cm_vigencia where bloqueomovimiento='S')";

                    dtEstadoFacturacion = cmdDatosBasicos.ExecuteStringCommand();
                    if (dtEstadoFacturacion.Rows.Count > 0)
                    {
                        //si encuentra datos es porque el suscriptor esta bloqueado para moviminetos
                        return true;
                    }
                    else
                    {
                        //si no retorna datos es porque a el suscriptor se le puede generar cupon de abono
                        return false;
                    }
                }
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// metodo que llama la clase crea cuponpago y le envia los parametros para el registro del cupon en la tabla
        /// cm_cuponpago
        /// </summary>
        /// <param name="nuIdSuscriptor"></param>
        /// <param name="nuValorCuponPago"></param>
        /// <param name="nuIdCuentaCobro"></param>
        /// <returns>
        /// numero de cupon
        /// valor del cupon
        /// fecha de generacion
        /// </returns>
        public List<DatosCuponPago> generarDatosCuponPago(long nuIdSuscriptor,int nuValorCuponPago, long nuIdCuentaCobro)
        {
            List<DatosCuponPago> RetornaDatosCuponPago = new List<DatosCuponPago>();
            CreaCuponPago objDatosCuponPago = new CreaCuponPago();
            int nuIdTipoCuponPago = 2;
            int nuIdEstadoCuponPago = 1;
            return RetornaDatosCuponPago = objDatosCuponPago.creaCuponPago(
                                                nuIdSuscriptor,
                                                nuValorCuponPago,
                                                nuIdTipoCuponPago,
                                                nuIdEstadoCuponPago,
                                                nuIdCuentaCobro
                                                );
        }
    }
}
