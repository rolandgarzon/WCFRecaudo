using System;
using System.Runtime.Serialization;

namespace WCFServiceRecaudo
{
    public class DatosCuponPago: BaseRespuesta
    {

        [DataMember]
        public double nuIdCuponPago { get; set; }

        [DataMember]
        public int nuValorCuponPago { get; set; }

        [DataMember]
        public DateTime daFechaGeneracion { get; set; }

        [DataMember]
        public int nuIdTipoCuponPago { get; set; }

        [DataMember]
        public int nuIdEstadoCuponPago { get; set; }

        [DataMember]
        public double nuIdCuentaCobro { get; set; }

        [DataMember]
        public int nuIdPais { get; set; }

        [DataMember]
        public int nuIdDepartamento { get; set; }

        [DataMember]
        public int nuIdMunicipio { get; set; }

        [DataMember]
        public string nuIdUsuario { get; set; }

        [DataMember]
        public string nuIdMaquina { get; set; }

    }
}