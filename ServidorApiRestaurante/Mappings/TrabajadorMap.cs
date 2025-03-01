using FluentNHibernate.Mapping;
using ServidorApiRestaurante.Models;

namespace ServidorApiRestaurante.Mappings
{
    public class TrabajadorMap : ClassMap<Trabajador>
    {
        public TrabajadorMap()
        {
            Table("trabajadores"); // Nombre de la tabla en MySQL

            // Mapeo de la clave primaria
            Id(x => x.Id)
                .Column("ID")
                .GeneratedBy.Identity();

            // Mapeo del campo "nombre" con restricción UNIQUE y NOT NULL
            Map(x => x.Nombre)
                .Column("nombre")
                .Not.Nullable()
                .Unique();

            // Mapeo de la contraseña (almacena el hash) en la columna "contraseña"
            Map(x => x.HashContraseña)
                .Column("contraseña")
                .Not.Nullable();

            // Relación muchos a uno con Rol
            References(x => x.Rol)
                .Column("rol_ID")
                .Not.Nullable();

            // Relación muchos a uno con Restaurante.
            // Nota: Para que al eliminar un restaurante se eliminen los trabajadores asignados,
            // se debe configurar la restricción ON DELETE CASCADE a nivel de base de datos.
            References(x => x.Restaurante)
                .Column("restaurante_ID")
                .Not.Nullable();
        }
    }
}
