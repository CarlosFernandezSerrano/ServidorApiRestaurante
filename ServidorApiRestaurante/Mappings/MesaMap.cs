using FluentNHibernate.Mapping;
using ServidorApiRestaurante.Models;

namespace ServidorApiRestaurante.Mappings
{
    public class MesaMap : ClassMap<Mesa>
    {
        public MesaMap()
        {
            // Definir la tabla
            Table("mesas");  // Nombre de la tabla en la base de datos

            // Mapeo de la propiedad Id
            Id(x => x.Id).Column("ID")  // La columna en la base de datos es "ID"
                .GeneratedBy.Identity();  // Se genera automáticamente

            // Mapeo de las propiedades PosX, PosY, ScaleX, ScaleY
            Map(x => x.PosX).Column("posX")  // La columna en la base de datos es "posX"
                .Not.Nullable();  // No puede ser nula

            Map(x => x.PosY).Column("posY")  // La columna en la base de datos es "posY"
                .Not.Nullable();  // No puede ser nula

            Map(x => x.ScaleX).Column("scaleX")  // La columna en la base de datos es "scaleX"
                .Not.Nullable();  // No puede ser nula

            Map(x => x.ScaleY).Column("scaleY")  // La columna en la base de datos es "scaleY"
                .Not.Nullable();  // No puede ser nula

            // Mapeo de la propiedad Disponible
            Map(x => x.Disponible).Column("disponible")  // La columna en la base de datos es "disponible"
                .Not.Nullable().Default("TRUE");  // No puede ser nula, valor por defecto es TRUE

            // Relación muchos a uno con Restaurante
            References(x => x.Restaurante)  // Relación con la clase Restaurante
                .Column("restaurante_ID")  // La columna de la relación en la base de datos es "restaurante_ID"
                .Not.Nullable();  // No puede ser nula
                //.Cascade.All();  // Para que al eliminar un restaurante, se eliminen las mesas asociadas en la aplicación.

            // Es importante configurar la restricción ON DELETE CASCADE a nivel de base de datos
        }
    }
}
