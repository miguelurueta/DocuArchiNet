# DOC-111: rollback real y autoridad operativa

Resultado del 2026-10-27: **PASS** de tres fallos controlados en la transacción MySQL y de las verificaciones de identidad/permiso descritas. Progreso: **32/130** tareas cerradas. No se habilita aún la importación ni se acredita la vía HTTP.

## Rollback del repositorio en MySQL 5.1

Se reutilizaron el plan 2 y los demás registros de la prueba autorizada, sin conservar filas adicionales. El perfil E2E 24 se activó temporalmente y se restauró a inactivo en finally. La extensión de la misma prueba reutiliza las autorizaciones recibidas y la instrucción posterior de continuar con esta verificación; no modifica reglas, versiones ni permisos.

Un constructor Friend del repositorio permite instalar un observador interno de la transacción para la prueba. El constructor productivo mantiene el observador ausente. No hay propiedad DTO, parámetro HTTP ni endpoint que lo habilite. El observador consulta con la conexión y transacción reales del repositorio y luego lanza una excepción controlada. La prueba no llama Rollback por su cuenta: verifica el comportamiento del Using de la transacción productiva.

| Punto del fallo | Efectos provisionales comprobados dentro de la transacción | Resultado tras la excepción |
|---|---|---|
| Antes de insertar el evento | Un lote del plan 2, dos raíces y dos archivos; sin evento nuevo | Sin lote/inventario nuevo; plan 2 REVISABLE/revisión 1 |
| Después de insertar el evento | Inventario completo y un evento de confirmación nuevo | También desaparece el evento provisional |
| Después del UPDATE del plan | Inventario/evento y plan 2 CONFIRMADO/revisión 2, aún sin commit | El plan vuelve a REVISABLE/revisión 1 y no persiste el lote |

El reloj inyectado de la fixture se fijó dentro del TTL guardado del plan 2. No se reescribieron sus fechas para reabrirlo ni se afirma una nueva validación de expiración en tiempo real. La prueba anterior cubre por separado las ramas de expiración y repetición.

Antes y después coinciden los hashes de **todas las filas de las seis tablas operativas**: dos planes, un lote, dos raíces, dos archivos, cero intentos y un evento. La repetición del plan 1 sigue devolviendo el lote 1. Las filas e identidades persistidas originales se conservan.

Los contadores AUTO_INCREMENT sí avanzan al reservar identidades dentro de intentos revertidos; no se reiniciaron. Valores observados: lote 2→5, raíz 3→9, archivo 3→9 y evento 2→4; plan e intento permanecen en 3 y 1. Esto no representa filas adicionales ni reutilización de identidades. No se promete rollback de esos contadores.

El perfil 24 pasó por revisión 8 durante la activación y quedó **inactivo en revisión 9**. Las revisiones anteriores 5→6→7 pertenecen a la corrida inicial. El contenido restante de la cabecera, todas sus versiones/reglas, los demás perfiles y los permisos conservan sus hashes. No se creó ni recibió ningún documento.

Ejecución con el arnés existente: `tools/e2e/scripts/run-doc111-persistence.ps1 -AuthorizedPersistence -AuthorizedTemporaryProfile24 -Mode Rollback`. Evidencia saneada: `tools/e2e/artifacts/doc111-rollback-result.json`, con puntos alcanzados, conteos, hashes, asignación de identidades y SHA de fuentes. El modo Rollback exige la fixture y revisiones esperadas; no sirve para repetir ciegamente una corrida ya terminada o sobre otros datos.

## Identidad activa y permiso vigente

`ImportacionCarpetasContextAdapter.vb` resuelve una identidad operativa sin heredar ni exigir administración. Lee las claves de sesión de Gestión ya existentes y consulta el origen RA de esa misma sesión mediante ModuleSessionConnectionStringResolver. En una consulta parametrizada comprueba usuario, empresa existente, usuario activo y IMPORTAR_CARPETAS; una relación ausente/ambigua o un permiso distinto de 1 deniega. Columna/esquema/conexión no disponibles producen CAPACIDAD_NO_DISPONIBLE sin exponer datos de conexión.

La consulta se ejecuta de nuevo en cada Resolver. No se leen flags de permiso de Session, ni se acepta un contexto previo como concesión. En esta etapa devuelve identidad/empresa/facultad; no inventa nivel, ruta o sello. La autorización de recursos y su destino pertenece a 5.3–5.4 y sigue pendiente; el contexto base no basta para escribir mediante el repositorio.

El arnés local prueba permiso 1 y pérdida a 0/null usando el mismo objeto de sesión, usuario inactivo, empresa ausente, relación duplicada, ausencia de fila/columna y sesiones no válidas, con denegación antes de IO cuando corresponde. Esos casos son dobles explícitos, no cuentas ni sesiones reales.

Una prueba adicional usa HttpContext/Session locales con la configuración RA solo en memoria y el adaptador/lector productivos contra la base configurada. El permiso real de la cuenta 141 es 0 y ambas lecturas deniegan incluso con flags locales de importación/administración en 1. Solo hubo SELECT; no se hizo login HTTP, no se alteraron permisos y no se acredita una revocación real 1→0 en una sesión autenticada. Evidencia: `tools/e2e/artifacts/doc111-context-read-result.json`.

## Verificación y tareas

`node --test tests/doc110-perfil-importacion-behavior.test.cjs`: PASS con DOC-110 y familias DOC-111, incluyendo autoridad operativa. Compilación Debug del proyecto con MSBuild, DeployOnBuild=false y salidas `.tmp/doc111-build`: PASS, sin desplegar.

Se cierran **4.5, 4.6, 5.1 y 5.2**. Siguen pendientes autorización del nivel/gabinete por acción, sello/ruta, guard JSON/multipart, consumo operativo del perfil, recepción y coordinación documental. Esta evidencia no cubre pérdida de conexión durante commit, crash del proceso, login, recepción de contenido ni REC-01/02.
