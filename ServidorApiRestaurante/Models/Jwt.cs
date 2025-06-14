﻿using ServidorApiRestaurante.Controllers;
using System.Security.Claims;
using System.Security.Principal;

namespace ServidorApiRestaurante.Models
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }

        public static dynamic ValidarToken(ClaimsIdentity? identity)
        {
            try
            {
                if (identity.Claims.Count() == 0)
                {
                    return new
                    {
                        success = false,
                        message = "Error, al verificar si estas enviando un token válido",
                        result = ""
                    };
                }

                // Obtengo el id del trabajador del token
                var claim = identity.Claims.FirstOrDefault(x => x.Type == "id");

                if (claim != null)
                {
                    var id = claim.Value;
                    bool existe = TrabajadorController.ExisteTrabajadorConID(int.Parse(id));

                    if (existe)
                    {
                        return new { success = true };
                    }
                    else
                    {
                        return new { success = false };
                    }
                }

                // El id es nulo
                return new { 
                    success = false, 
                    message = "No se encontró el claim 'id' en el token." 
                };


            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = "Catch: "+ex.Message,
                    result = ""
                };
            }
        }

        public static dynamic ValidarRol(ClaimsIdentity? identity)
        {
            try
            {
                // Obtengo el id del trabajador del token
                var claim = identity.Claims.FirstOrDefault(x => x.Type == "id");

                if (claim != null)
                {
                    var id = claim.Value;
                    Trabajador trabajador = TrabajadorController.ObtenerTrabajadorPorId(int.Parse(id));

                    if (trabajador != null)
                    {
                        Rol rol = RolController.ObtenerRolPorId(trabajador.Rol_Id);

                        try
                        {
                            if (rol != null && rol.Nombre.CompareTo("Gerente") == 0)
                            {
                                return new { success = true };
                            }
                        }
                        catch (Exception ex)
                        {
                            return new
                            {
                                success = false,
                                message = "Catch: " + ex.Message,
                                result = ""
                            };
                        }
                    }
                    else
                    {
                        return new { success = false };
                    }
                }

                // El id es nulo
                return new
                {
                    success = false,
                    message = "No se encontró el claim 'id' en el token."
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = "Catch: " + ex.Message,
                    result = ""
                };
            }

        }
    }
}
