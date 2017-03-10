using System.Runtime.Serialization;

namespace WCFServiceRecaudo
{
    [DataContract]
    public class BaseRespuesta
    {
        [DataMember]
        public string MensajeRespuestaOk { get; set; }
        [DataMember]
        public string MensajeRespuestaError { get; set; }
    }
}