using FluentNHibernate.Mapping;
using ServidorApiRestaurante.Models;

namespace ServidorApiRestaurante.Mappings
{
    public class RestauranteMap : ClassMap<Restaurante>
    {
        public RestauranteMap()
        {
            // Definir la tabla
            Table("restaurantes");  // El nombre de la tabla en la base de datos

            // Mapear la propiedad Id con la clave primaria y la columna correspondiente
            Id(x => x.Id).Column("ID")  // La columna en la base de datos se llama "ID"
                .GeneratedBy.Identity();  // El valor de "ID" se genera automáticamente

            // Mapear la propiedad Nombre a la columna correspondiente en la base de datos
            Map(x => x.Nombre).Column("nombre")  // La columna se llama "nombre"
                .Not.Nullable();  // El nombre no puede ser nulo

            // Mapear las propiedades de hora_apertura y hora_cierre
            Map(x => x.HoraApertura).Column("hora_apertura")  // La columna se llama "hora_apertura"
                .Not.Nullable();  // No puede ser nula

            Map(x => x.HoraCierre).Column("hora_cierre")  // La columna se llama "hora_cierre"
                .Not.Nullable();  // No puede ser nula
        }
    }
}
