export function validarRespuestaAutenticacion(data: any) {
  const errores: string[] = [];

  if (!data) errores.push("La respuesta está vacía");
  if (!data?.Token) errores.push("No se recibió el token de autenticación");
  if (!data?.Expiracion) errores.push("No se recibió la fecha de expiración");

  if (!data?.Usuario) {
    errores.push("No se recibió el objeto Usuario");
  } else {
    if (!data.Usuario.UsuarioId)
      errores.push("UsuarioId no fue enviado por la API");

    if (!data.Usuario.Login)
      errores.push("Login no fue enviado por la API");

    if (!data.Usuario.Nombre)
      errores.push("Nombre no fue enviado por la API");

    if (data.Usuario.Activo === undefined)
      errores.push("Activo no fue enviado por la API");

    if (!Array.isArray(data.Usuario.Permisos))
      errores.push("Permisos no fue enviado o no es una lista");
  }

  return errores;
}
