using System.Runtime.Serialization;

namespace WCFServiceRecaudo
{
    [DataContract]
    public class DatosSuscriptor : BaseRespuesta
    {
        [DataMember]
        public double nuIdSuscriptor { get; set; }
        [DataMember]
        public string vaNombre{ get; set; }
        [DataMember]
        public double nuSaldoPendiente { get; set; }
        [DataMember]
        public int nuFacturasconSaldo { get; set; }
        [DataMember]
        public long nuIdCuentaCobro { get; set; }
        [DataMember]
        public double nuSaldoPendienteUltimaFactura { get; set; }
    }
}
