using FluentNHibernate.Mapping;
using ServidorApiRestaurante.Models;

namespace ServidorApiRestaurante.Mappings
{
    public class ReservaMap : ClassMap<Reserva>
    {
        public ReservaMap()
        {
            // Definir la tabla en la base de datos
            Table("reservas");

            // Mapeo de la propiedad ID a la columna en la tabla
            Id(x => x.Id).Column("ID").GeneratedBy.Identity(); // ID autoincrementable

            // Mapeo de las demás propiedades
            Map(x => x.Fecha).Column("fecha").Not.Nullable(); // Columna "fecha"
            Map(x => x.Hora).Column("hora").Not.Nullable();   // Columna "hora"
            Map(x => x.Estado).Column("estado").Not.Nullable(); // Columna "estado"

            // Relación muchos a uno con Cliente
            References(x => x.Cliente_Id)
                .Column("cliente_ID")
                .Not.Nullable(); // Relación no nula con Cliente

            // Relación muchos a uno con Mesa
            References(x => x.Mesa_Id)
                .Column("mesa_ID")
                .Not.Nullable(); // Relación no nula con Mesa

            // Configuración de la restricción única en mesa_ID, fecha y hora
        }
    }
}
