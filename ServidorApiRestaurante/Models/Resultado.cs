using System.Text.Json.Serialization;

namespace ServidorApiRestaurante.Models
{
    public class Resultado
    {
        public int Result { get; set; }

        [JsonConstructor]
        public Resultado(int result)
        {
            this.Result = result;
        }
    }
}
