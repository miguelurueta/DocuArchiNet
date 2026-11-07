# DOC-110 — Contrato HTTP y protección del perfil

Entregable documental de 1.5/1.6, para revisión técnica. Nombres nuevos son contratos propuestos, no endpoints implementados. Se preservan WebForms/VB.NET/ASMX y jQuery existentes. No cambia SII, login, CORS global ni el servicio de carga individual.

## Evidencia y elección técnica

`webservice/WebServiceProducion.asmx.vb` usa ScriptService y WebMethod con EnableSession. `js/gestion/WebFormProducionDocumental.js`, alrededor de 608, invoca ASMX con jQuery y JSON. Esto acredita compatibilidad de transporte, no una protección CSRF existente.

La búsqueda en VB/ASPX del repositorio no identificó un validador CSRF reutilizable para el servicio nuevo. Los tokens encontrados en bibliotecas CKEditor no prueban validación del servidor de Producción. Por ello se define una extensión limitada a esta funcionalidad: token aleatorio ligado al contexto autenticado de sesión, entregado por página autorizada y enviado en cabecera del cliente del perfil. Es una elección técnica explícita, no una capacidad que se atribuya al código actual.

El patrón de token sincronizado y su envío en cabecera para AJAX están documentados por [OWASP](https://cheatsheetseries.owasp.org/cheatsheets/Cross-Site_Request_Forgery_Prevention_Cheat_Sheet.html). La sesión o un botón oculto no sustituyen esta comprobación.

## Transporte y contexto

- Fachada propuesta: `webservice/WebServiceImportacionCarpetas.asmx`.
- Operaciones por POST con `Content-Type: application/json; charset=utf-8`, ScriptMethod sin GET y WebMethod con sesión.
- Un argumento DTO `solicitud`: cuerpo JSON `{"solicitud":{...}}`. Nunca enviar una cadena que contenga JSON como sustituto del DTO.
- Identidad autenticada, empresa y conexión RA solo provienen del contexto servidor. No aceptar usuario, empresa autoritativa, conexión o nombres SQL en la solicitud.
- IDs nuevos: cadenas canónicas de 32 caracteres ASCII; referencias legacy: representación canónica del catálogo registrado, no una conversión numérica general en JavaScript.
- Cada respuesta propia usa `ok`, `codigo`, `mensaje`, `operacionId`, `datos`, `incidencias`. ASMX puede envolverla en `d`; el cliente desempaqueta una vez. `operacionId` lo emite el servidor para correlación: no es clave de idempotencia ni concede acceso.
- Revisión esperada se refiere expresamente a cabecera o versión. No usar un único entero ambiguo para ambas.

## Operaciones y DTO mínimos

Todas las operaciones administrativas exigen ADMINISTRAR_PERFILES_IMPORTACION vigente y autorización de ámbito/recurso. Autorización precede lecturas que puedan revelar existencia ajena. Estos nombres se incorporarán al inventario de implementación; no son símbolos legacy.

| Método propuesto | `solicitud` | Resultado / control |
|---|---|---|
| `ListarPerfiles` | `pagina`, `tamanoPagina`, filtros `codigo`, `nombre`, `activo` opcionales | Página de cabeceras autorizadas, total y capacidades. Orden estable código/perfil_id. Rechazar paginación no positiva o superior al límite explícito de la aplicación; nunca consulta sin límite. |
| `ObtenerPerfil` | `perfilId`, `versionId` opcional | Cabecera, versión solicitada autorizada o publicada habitual y referencias de borradores; sin cambiar referencia publicada. |
| `ConsultarHistorial` | `perfilId`, `pagina`, `tamanoPagina` | Resumen de versiones autorizado y paginado; detalle mediante ObtenerPerfil. |
| `ConsultarCatalogo` | `catalogo` de registro cerrado, filtros dependientes de clasificación, paginación | Opciones autorizadas. No nombre de tabla/columna enviado por cliente. |
| `CrearPerfil` | `codigo`, `nombre`, `descripcion` opcional | Crea cabecera y borrador inicial en transacción; retorna IDs/revisiones. Código único por ámbito. |
| `GuardarBorrador` | `perfilId`, `versionId`, `revisionVersionEsperada`, `configuracion`, `reglas` | Guarda solo versión BORRADOR mediante comparación de revisión; no cambia cabecera ni publica. |
| `ActualizarCabecera` | `perfilId`, `revisionPerfilEsperada`, `nombre`, `descripcion` opcional | Actualiza datos visibles de cabecera; código/ámbito no editables por esta operación. No modifica nombre_visible de versiones publicadas. |
| `EnsayarReglas` | `perfilId`, `versionId`, configuración/reglas de borrador a ensayar y `ejemplos` acotados | Valores/incidencias/dependencias tipadas. Autoriza pertenencia antes de procesar; no guarda, lee disco ni crea expediente. |
| `PublicarVersion` | `perfilId`, `versionId`, `revisionPerfilEsperada`, `revisionVersionEsperada` | Valida configuración vigente y confirma publicación/referencia habitual atómicamente. Rechaza revisión obsoleta en cualquiera de los recursos. |
| `CrearVersion` | `perfilId`, `versionOrigenId`, `revisionPerfilEsperada`, `revisionOrigenEsperada` | Copia configuración a borrador nuevo con número asignado bajo bloqueo; no altera origen. |
| `CambiarDisponibilidad` | `perfilId`, `revisionPerfilEsperada`, `activo` booleano | Actualiza disponibilidad con revisión, conserva historial; no ejecuta efectos sobre lotes. |
| `ValidarSeleccionPerfil` | `perfilId`, `versionId`, `nivelRef`, `proposito` | Devuelve compatibilidad y misma referencia de nivel, sin plan ni lote. Solo versión publicada disponible. |

`configuracion` y `reglas` siguen el modelo y contratos 05/06: JSON de esquema cerrado, no diccionario arbitrario aceptado sin validación. Los máximos de tamaño/profundidad/paginación se fijan en configuración técnica de implementación y se prueban en límites; no se atribuye al sistema una capacidad numérica sin medir.

En DOC-110, ValidarSeleccionPerfil admite propósito ADMINISTRACION y permiso administrativo para comprobar configuración. El propósito IMPORTACION queda reservado a la entrega que implemente IMPORTAR_CARPETAS: rechazarlo como capacidad no disponible mientras no exista esa integración. No autorizarlo con OR entre ambos permisos. La selección operativa futura reutiliza el caso de uso y sus verificaciones con autorización propia, sin abrir carga desde DOC-110.

## Token CSRF y ciclo de vida

Componente nuevo propuesto: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionRequestGuard.vb`. Responsabilidad HTTP: validar método, contenido, sesión y token antes de llamar al servicio. Las reglas del perfil permanecen fuera de ese componente.

1. La página de Producción, al preparar controles administrativos para un usuario autorizado, obtiene/genera un token mediante el guard. Fuente aleatoria criptográfica del framework (`RandomNumberGenerator`), 32 bytes; representación Base64. No GUID, fecha, nombre de usuario o Math.random.
2. Mantener token y vinculación de usuario/empresa/contexto autenticado en sesión con claves propias del perfil. No crear cookie nueva ni modificar autenticación. Contexto incoherente, sesión nueva o cambio de identidad/empresa invalida token previo.
3. Renderizar token solo en el control propio autorizado, codificado para HTML; el cliente lo mantiene en memoria de página y lo envía como `X-Importacion-Perfil-CSRF`. No guardarlo en localStorage, URL, reportes o logs.
4. Comprobar cabecera contra token esperado y contexto vigente antes de ejecutar cualquiera de los métodos de esta fachada. Rechazar ausencia, formato inválido o desigualdad con comparación de bytes sin salida temprana por posición. El token no reemplaza permiso ni revisión de concurrencia.
5. Reutilizar token dentro del mismo contexto de sesión para permitir pestañas y UpdatePanel sin invalidación en cada petición. Si se renueva contexto, la UI exige recarga autenticada y no reenvía mutaciones automáticamente.
6. La emisión pertenece a la página autorizada, no a un endpoint anónimo. Respuesta/página que contiene token no debe quedar en caché compartida. No habilitar lectura de tokens ni llamadas del perfil desde otro origen; verificar configuración efectiva del nuevo recorrido sin ampliar CORS global.
7. Método/origen inesperados se rechazan. Si llega Origin, contrastarlo con origen de aplicación autorizado; no derivar una lista de confianza de cabeceras reenviadas arbitrarias. La ausencia de Origin no elimina la validación obligatoria del token. El despliegue detrás de proxy debe verificar su origen efectivo como prueba de integración.

Esta política se limita a la fachada nueva. No convertir la inicialización del guard en migración de seguridad global ni modificar consumidores SII.

## Respuestas y recuperación del cliente

| Situación | HTTP previsto / código funcional | Conducta cliente |
|---|---|---|
| Éxito | 200 / código de operación | Renderizar respuesta real. |
| Solicitud inválida | 400 / SOLICITUD_INVALIDA | Mostrar incidencias; no reintentar automáticamente. |
| Sesión ausente/expirada | 401 / SESION_REQUERIDA | Cerrar capacidad de mutación y solicitar reautenticación/recarga. |
| Permiso o recurso no autorizado | 403 / ACCESO_DENEGADO | No revelar configuración ni existencia ajena. |
| Token inválido | 403 / SOLICITUD_NO_VERIFICADA | Recargar contexto; no reenviar mutación perdida. |
| Revisión o código de perfil en conflicto | 409 / CONFLICTO_REVISION o CODIGO_PERFIL_EXISTENTE | Consultar estado autorizado y presentar conflicto. |
| Configuración inválida | 400 / CONFIGURACION_INVALIDA | Incidencias por campo/regla. |
| Esquema/capacidad no disponible | 503 / CAPACIDAD_NO_DISPONIBLE | Informar indisponibilidad acotada; conservar Producción existente. |
| Error inesperado | 500 / ERROR_INTERNO | Mensaje seguro y operación de correlación; nunca stack/SQL/HTML. |

Los errores producidos antes de ejecutar WebMethod pueden proceder del runtime ASMX/IIS y no tener el sobre. El cliente debe manejar también respuesta vacía/no JSON, timeout y errores de red, sin interpretar HTML como éxito.

Verificar el comportamiento real de 401 y errores bajo IIS. [Microsoft documenta](https://learn.microsoft.com/en-us/dotnet/api/system.web.httpresponse.suppressformsauthenticationredirect) que Forms Authentication puede convertir 401 en redirección; evaluar `SuppressFormsAuthenticationRedirect` y `TrySkipIisCustomErrors` localmente al nuevo recorrido para conservar la respuesta prevista. No afirmar que ya ocurre ni modificar configuración global para resolverlo.

Tras timeout/respuesta perdida: ObtenerPerfil/ConsultarHistorial para conocer revisión y estado antes de repetir guardar/publicar. Tras creación incierta: búsqueda autorizada por código único de perfil, sin crear otro con sufijo. No declarar exactamente-una-vez ni asumir que toda respuesta 409 acredita éxito del intento anterior.

## Matriz de pruebas obligatorias

- Sesión válida con/sin token; token de otra sesión/empresa; cambio de identidad; permiso revocado con token todavía válido.
- Método GET y Content-Type incorrecto rechazados antes de efectos.
- Dos pestañas/UpdatePanel mantienen uso legítimo; sesión expirada exige recuperación sin replay de mutaciones.
- 401/403/409/503, JSON `d`, JSON no doblemente serializado, error HTML del runtime y timeout: ninguna rama deja UI bloqueada o simula éxito.
- Respuesta perdida después de publicar/crear; consulta posterior recupera estado sin duplicar operación.
- Código fuente/fixture/evidencia sin tokens ni credenciales; pruebas de regresión de consumidores compartidos si se requiere extensión.
- Pruebas unitarias del guard y transporte primero; HTTP real solo en ambiente/cuentas autorizados. Tokens válidos y conexión no constituyen autorización de pruebas reales.

1.5/1.6 se completan como contratos documentales. A3 queda definido técnicamente, con implementación y verificación exigidas antes de habilitar mutaciones; no acredita protección desplegada.

Identidades COR-01/COR-02: perfilId, versionId y ReglaId se transportan como String decimal canónico positivo hasta 4294967295; referencias opcionales null. No aceptar GUID, cero, signos, ceros iniciales ni números fuera de rango. La autorización por empresa/usuario permanece obligatoria. Véase 08-migracion-identidad-incremental.md.

## AMP-01 — ConsultarProveedores

Solicitud cerrada: pagina (desde 1), tamanoPagina (1..50), proveedorId/versionId opcionales juntos como IDs decimales canónicos. Contexto de empresa/usuario y guard existentes obligatorios. Respuesta: lista de ProveedorId, VersionId, Nombre, Numero, TipoClave y Propiedades (Nombre/Tipo lógicos). No exponer conexión, tabla, columnas físicas ni secretos. Referencia exacta usa el mismo filtro de ámbito/activo; vacío no concede acceso ni autoriza guardar. El editor conserva la referencia ausente con diagnóstico. Operación de solo lectura, no registra proveedores.
