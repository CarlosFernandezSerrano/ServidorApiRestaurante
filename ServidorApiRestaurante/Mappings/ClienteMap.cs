using FluentNHibernate.Mapping;
using ServidorApiRestaurante.Models;

namespace ServidorApiRestaurante.Mappings
{
    public class ClienteMap : ClassMap<Cliente>
    {
        public ClienteMap()
        {
            Table("clientes"); // Nombre de la tabla en la base de datos

            // Mapeo de las columnas
            Id(x => x.Id).Column("ID").GeneratedBy.Identity(); // Define la columna ID como PK y autoincrementable
            Map(x => x.Nombre).Column("nombre").Not.Nullable(); // Columna nombre, no puede ser nula
            Map(x => x.Dni).Column("dni").Not.Nullable().Unique(); // Columna dni, no puede ser nula y debe ser único
            Map(x => x.NumTelefono).Column("num_teléfono").Nullable(); // Columna para el número de teléfono, puede ser nula

            // Opcional: si necesitas configuraciones adicionales, como cascadas, las puedes agregar aquí.
        }
    }
}
