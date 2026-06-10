type UnknownRecord = Record<string, unknown>;

const isRecord = (value: unknown): value is UnknownRecord =>
  typeof value === "object" && value !== null;

const hasClaimsLikeArray = (value: unknown): boolean => Array.isArray(value);

export function validarRespuestaAutenticacion(data: unknown) {
  const errores: string[] = [];

  if (!isRecord(data)) {
    errores.push("La respuesta está vacía");
    return errores;
  }

  if (!data.token) errores.push("No se recibió el token de autenticación");
  if (!data.expiracion) errores.push("No se recibió la fecha de expiración");

  const usuario = isRecord(data.usuario) ? data.usuario : null;

  if (!usuario) {
    errores.push("No se recibió el objeto Usuario");
  } else {
    if (!usuario.usuarioId)
      errores.push("UsuarioId no fue enviado por la API");

    if (!usuario.login)
      errores.push("Login no fue enviado por la API");

    if (!usuario.nombre)
      errores.push("Nombre no fue enviado por la API");

    if (usuario.activo === undefined)
      errores.push("Activo no fue enviado por la API");

    if ("permisos" in usuario && usuario.permisos != null && !Array.isArray(usuario.permisos)) {
      errores.push("Permisos no es una lista");
    }

    if ("claims" in usuario && usuario.claims != null && !hasClaimsLikeArray(usuario.claims)) {
      errores.push("Claims de usuario no es una lista");
    }

    const hasLegacyPermisos = Array.isArray(usuario.permisos);
    const hasUserClaims = hasClaimsLikeArray(usuario.claims);
    const hasTopClaims = hasClaimsLikeArray(data.claims);

    if (!hasLegacyPermisos && !hasUserClaims && !hasTopClaims) {
      errores.push("No se recibieron claims ni permisos para autorización");
    }
  }

  return errores;
}
