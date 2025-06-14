﻿
using System.Text.Json.Serialization;

namespace ServidorApiRestaurante.Models
{
    public class Trabajador
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; } = string.Empty;
        public int Rol_Id { get; set; }
        public int Restaurante_Id { get; set; }


        public Trabajador(string nombre, string contraseña, int rol_Id, int restaurante_Id)
        {
            this.Nombre = nombre;
            this.Password = contraseña; 
            this.Rol_Id = rol_Id;
            this.Restaurante_Id = restaurante_Id;
        }

        [JsonConstructor]
        public Trabajador(int id, string nombre, string password, int rol_Id, int restaurante_Id)
        {
            this.Id = id;
            this.Nombre = nombre;
            this.Password = password;
            this.Rol_Id = rol_Id;
            this.Restaurante_Id = restaurante_Id;
        }


    }
}
