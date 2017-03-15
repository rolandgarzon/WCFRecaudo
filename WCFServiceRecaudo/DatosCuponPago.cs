using System;
using System.Runtime.Serialization;

namespace WCFServiceRecaudo
{
    public class DatosCuponPago: BaseRespuesta
    {

        [DataMember]
        public long nuIdCuponPago { get; set; }

        [DataMember]
        public int nuValorCuponPago { get; set; }

        [DataMember]
        public long nuIdSuscriptor { get; set; }

        [DataMember]
        public string vaNombre { get; set; }

        [DataMember]
        public double nuSaldoPendiente { get; set; }

        [DataMember]
        public double saldoafavor { get; set; }

        [DataMember]
        public double valortotalfacturado { get; set; }

        [DataMember]
        public double valortotalfacturadoglobal { get; set; }

        [DataMember]
        public DateTime daFechaGeneracion { get; set; }

        [DataMember]
        public int nuIdTipoCuponPago { get; set; }

        [DataMember]
        public int nuIdEstadoCuponPago { get; set; }

        [DataMember]
        public long nuIdCuentaCobro { get; set; }

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