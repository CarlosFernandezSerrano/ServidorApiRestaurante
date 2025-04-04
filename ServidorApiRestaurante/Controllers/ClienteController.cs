using MySql.Data.MySqlClient;
using ServidorApiRestaurante.Models;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc; // Asegúrate de tener este using

namespace ServidorApiRestaurante.Controllers
{
    public class ClienteController
    {
        

        public static Cliente ObtenerClienteConID(int id)
        {
            using (var connection = new MySqlConnection(BDDController.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Clientes WHERE ID = @id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int Id = reader.GetInt32("ID");
                                string Nombre = reader.GetString("Nombre");
                                string Dni = reader.GetString("Dni");
                                string NumTelefono = reader.GetString("Num_Telefono");

                                Cliente cliente = new Cliente(Id, Nombre, Dni, NumTelefono);

                                return cliente;
                            }
                            else
                            {
                                Trace.WriteLine("1 Error al obtener cliente: ");
                                return new Cliente("", "", "");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error al obtener cliente: " + ex.Message);
                    throw new Exception("Error al obtener cliente: " + ex.Message);
                }
            }
        }

        

        public static void ComprobarQueFuncionaAES()
        {
            Trace.WriteLine("Clave (key): " + Convert.ToBase64String(AESCipher.key));
            Trace.WriteLine("IV (iv): " + Convert.ToBase64String(AESCipher.iv));
            string cadCifrada = AESCipher.Encrypt("Hola Bayan, qué bien que hicimos las paces, la mamá de la mamá");
            Trace.WriteLine("+ + Palabra cifrada: " + cadCifrada);
            //Trace.WriteLine("+ + Prueba: " + AESCipher.AESKeyBase64+"   ;    "+AESCipher.AESIVBase64);
            string cadDescifrada = AESCipher.Decrypt(cadCifrada);

            Trace.WriteLine("+ + Palabra descifrada: " + cadDescifrada);
            Trace.WriteLine(" -- - - -- - - -- - -- - -- - -- - --  -");
            for (int i = 0; i < 5; i++)
            {
                string cadCifrada2 = AESCipher.Encrypt("Hola Bayan, qué bien que hicimos las paces, la mamá de la mamá");
                Trace.WriteLine("+ + Palabra cifrada: " + cadCifrada2);
                //Trace.WriteLine("+ + Prueba: " + AESCipher.AESKeyBase64+"   ;    "+AESCipher.AESIVBase64);
                string cadDescifrada2 = AESCipher.Decrypt(cadCifrada2);

                Trace.WriteLine("+ + Palabra descifrada: " + cadDescifrada2);
            }
            

        }

    }
}
