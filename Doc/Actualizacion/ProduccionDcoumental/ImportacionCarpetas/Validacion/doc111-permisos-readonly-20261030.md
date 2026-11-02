# DOC-111: permisos actuales de la cuenta histórica

## Estado vigente: perfil habilitado a las 21:07:41 UTC

Por instrucción explícita «habilita el perfil 24», se utilizó el método existente `MySqlPerfilImportacionRepository.CambiarDisponibilidad`, con el contexto administrativo 141/empresa 2 comprobado mediante SELECT. Perfil 24 activo, revisión **11→12**, referencia publicada **38** conservada. Las versiones, reglas y contenido restante de cabecera conservaron su huella SHA-256 `727475f7c0e21749b2f892eacdb5e222381b0823fafaca7dc1b7601a99272f84`. Solo se modificaron disponibilidad, revisión y auditoría de actualización conforme al contrato del repositorio.

Una nueva conexión SELECT a las 21:07:48 UTC confirmó identidad activa, importación y administración concedidas, y perfil 24 activo con referencia publicada válida. Artefacto de activación: `tools/e2e/artifacts/doc111-profile24-activation.json`. La activación queda persistente por solicitud del usuario; no se restaura a inactivo. No se ejecutó login, carga ni aceptación REC.

Los apartados inferiores conservan los estados anteriores.

## Actualización autorizada: 21:05:47 UTC

Tras la instrucción explícita del usuario «cambia permisos», se habilitó únicamente `IMPORTAR_CARPETAS` de 0 a 1 para la cuenta 141/empresa 2 en el destino histórico comprobado. UPDATE condicionado a identidad activa, empresa y valor previo 0; una fila afectada. Tabla InnoDB, transacción y bloqueo de filas; se compararon todos los demás campos de la fila antes de confirmar y permanecieron iguales. No se imprimieron ni guardaron conexiones o credenciales.

Verificación posterior con el inspector SELECT, en una conexión nueva a las 21:05:53 UTC: importación **concedida**, administración concedida, identidad activa, 91 niveles con derechos nativos. El perfil 24 continúa sin cumplir la condición conjunta de disponibilidad/publicación. No se activaron perfiles ni se ejecutó login, carga o REC. El artefacto `doc111-permissions-readonly.json` contiene esta última lectura; la tabla inferior conserva el estado previo.

## Inspección previa

Fecha: 2026-10-30, 21:04:30 UTC. Inspección solicitada por el usuario. Solo SELECT, sin login HTTP, concesiones ni cambios de perfil.

Se reutilizó la resolución del catálogo existente de los verificadores DOC-110. El destino coincide con la huella del ambiente histórico DOC-111; las conexiones se mantuvieron en memoria y no se imprimieron ni persistieron. Cuenta histórica: ID 141, empresa 2. No se afirma que sea la cuenta que se utilizará en la aceptación futura.

| Control | Resultado |
| --- | --- |
| Identidad activa y empresa existente/coincidente | Sí |
| `ADMINISTRAR_PERFILES_IMPORTACION` | Concedido |
| `IMPORTAR_CARPETAS` | **No concedido** |
| Niveles activos de la empresa con propiedad o ambos permisos explícitos (`agregar_expediente`, `carga_archivo`) | 91 |
| Perfil histórico 24 activo y con referencia publicada válida | No cumple la condición conjunta |
| Facultad operativa base DOC-111 | **DENEGADA** |

Administrar perfiles y tener derechos nativos en niveles no sustituyen la facultad de importación. El conteo de niveles solo comprueba el predicado inicial de `ResolverDestino(ABRIR)`; no valida la cadena completa de ancestros, gabinete, ruta, configuración ni autorización de un recurso concreto. La comprobación conjunta del perfil no distingue en este reporte cuál de sus condiciones falla.

Verificador: `tools/e2e/scripts/inspect-doc111-permissions-readonly.ps1`, ejecutado con los IDs históricos predeterminados. Artefacto saneado: `tools/e2e/artifacts/doc111-permissions-readonly.json`. Los errores solo emiten etapa y código fijo; no muestran excepciones de conexión. El resultado `INSPECTED` acredita la consulta, no aceptación ni habilitación.

No se modificaron los permisos ni se activó el perfil 24. No se ejecutó una transición real de revocación 1→0 ni una prueba autenticada; 5.9/6.6 y los REC permanecen pendientes. Antes de cambiar permisos o disponibilidad de perfiles se requiere una instrucción específica del usuario.
