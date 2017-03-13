using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceRecaudo
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IWSRecaudo
    {

        [OperationContract]
        List<DatosSuscriptor> obtenerDatosSuscriptor(int nuIdsuscriptor);

        [OperationContract]
        List<DatosCuponPago> obtenerDatosCuponPago(int nuIdcuponpago);

        [OperationContract]
        Boolean obtenerEstadoFacturacion(int nuIdsuscriptor);

        [OperationContract]
        List<DatosCuponPago> generarDatosCuponPago(int nuIdSuscriptor, int nuValorCuponPago, long nuIdCuentaCobro);

    }


}
