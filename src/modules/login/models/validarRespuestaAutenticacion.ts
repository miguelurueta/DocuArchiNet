export function validarRespuestaAutenticacion(data: any) {
  const errores: string[] = [];

  if (!data) errores.push("La respuesta está vacía");
  if (!data?.token) errores.push("No se recibió el token de autenticación");
  if (!data?.expiracion) errores.push("No se recibió la fecha de expiración");

  if (!data?.usuario) {
    errores.push("No se recibió el objeto Usuario");
  } else {
    if (!data.usuario.usuarioId)
      errores.push("UsuarioId no fue enviado por la API");

    if (!data.usuario.login)
      errores.push("Login no fue enviado por la API");

    if (!data.usuario.nombre)
      errores.push("Nombre no fue enviado por la API");

    if (data.usuario.activo === undefined)
      errores.push("Activo no fue enviado por la API");

    if (!Array.isArray(data.usuario.permisos))
      errores.push("Permisos no fue enviado o no es una lista");
  }

  return errores;
}
