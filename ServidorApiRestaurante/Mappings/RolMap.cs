using FluentNHibernate.Mapping;
using ServidorApiRestaurante.Models;

namespace ServidorApiRestaurante.Mappings
{
    public class RolMap : ClassMap<Rol>
    {
        public RolMap()
        {
            // Definir la tabla
            Table("rols");  // Asegúrate de que el nombre de la tabla sea correcto en tu base de datos

            // Mapear la propiedad Id con la clave primaria y la columna correspondiente
            Id(x => x.Id).Column("ID")  // La columna se llama "ID" en la base de datos
                .GeneratedBy.Identity();  // El valor se genera automáticamente

            // Mapear la propiedad Nombre a la columna correspondiente en la base de datos
            Map(x => x.Nombre).Column("nombre")  // La columna se llama "nombre"
                .Not.Nullable();  // No puede ser nula en la base de datos
        }
    }
}
