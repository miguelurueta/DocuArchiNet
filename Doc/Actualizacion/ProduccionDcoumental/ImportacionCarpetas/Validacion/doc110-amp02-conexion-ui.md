# DOC-110 AMP-02 — Conexión de fuentes, servicios e interfaz

Fecha: 2026-10-23. Implementación en repositorio, sin despliegue ni escrituras reales en RA/DA.

## Recorrido conectado

Producción Documental → Administrar fuentes → API ASMX con sesión/CSRF → ServicioFuenteImportacion → MySqlFuenteImportacionRepository → RA.

La página consulta ADMINISTRAR_FUENTES_IMPORTACION en el contexto autenticado y ofrece el acceso independiente del permiso de proveedores. La fachada vuelve a comprobar el permiso en cada operación; el atributo HTML no es autoridad. El guard existente interpreta solicitudes cerradas y controla CSRF también para las nuevas operaciones.

Operaciones: ListarFuentes, ObtenerFuente, HistorialFuente, CrearFuente, GuardarFuente, ActivarFuente, RevocarFuente y ConsultarMetadatosFuente. El servicio verifica revisión antes de metadatos y el repositorio vuelve a comprobarla bajo bloqueo. Activar valida la configuración persistida, no una configuración alternativa enviada por el navegador. Revocar no depende de la disponibilidad del origen.

Metadatos administrativos consulta solo DATABASE() por conexión RA/DA de sesión: tablas/vistas y columnas, páginas de 25, timeout de cinco segundos. Requiere permiso de fuentes y no usa el catálogo ordinario como prerrequisito del alta. No devuelve filas de negocio ni credenciales.

FuenteAdminEditor ofrece búsqueda/listado, conexión, tabla, selección de columnas paginada, modalidad de aislamiento, columna empresarial, motivo, alta inactiva, activación, edición, revocación e historial. Mantiene identidad inmutable, protege cambios pendientes, bloquea controles durante escrituras y descarta metadatos de otra selección/vista. Ante una escritura incierta exige consultar estado; no la reenvía automáticamente. Inicializa junto a los editores existentes y evita diálogos superpuestos.

## Política persistente

Se reemplazaron los tres consumidores de appSettings en WebServiceImportacionCarpetas: metadatos de proveedor, administración de proveedor y consulta de índices. MySqlFuenteImportacionRepository.CargarPolitica lee fuentes activas y columnas por ámbito. No hay fallback ni combinación con Web.config. En resolución de índices se usa Lazy por petición para no consultar por campo; operaciones de perfil sin fuentes no cargan el catálogo.

La política persistida exige coincidencia exacta de columna empresarial para COLUMNA y ausencia para CONEXION. El DTO de política conserva compatibilidad con fixtures anteriores; la carga de BD siempre informa la restricción exacta. El formulario de proveedor sigue consumiendo las mismas operaciones de catálogo, ahora alimentadas por RA.

## Resultados de verificación

| Verificación | Resultado | Límite |
|---|---|---|
| Build ASP.NET completo aislado | PASS | Salida .tmp/doc110-amp02-build, no bin/IIS. |
| Suite Node de perfil/cliente/proveedor y harness .NET | 20 PASS | Incluye nuevas pruebas del servicio de fuentes y política exacta; sin mutaciones reales. |
| Suite Node FuenteAdminEditor | 3 PASS | Acceso por permiso, respuestas RA/DA/RA atrasadas, cambio de tabla limpia columnas y escritura incierta no se repite. |
| Chromium suites guiadas | 3 PASS | Interfaz real con transporte simulado. No demuestra ASMX/BD autenticada. |
| Revisión de composición | Sin lecturas de FuentesAutorizadas en runtime VB | Fuentes pasan a RA en el código, no significa despliegue. |
| SELECT esquema RA 2026-10-23T12:33:48.7134590Z | INCOMPLETO | Existen fuente/columna y columna de permiso; falta ra_integracion_fuente_historial. |

Comandos:

```text
node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc110-perfil-importacion-client.test.cjs tests/doc110-perfil-importacion-editor.test.cjs tests/doc110-proveedor-admin-editor.test.cjs
node --test tests/doc110-fuente-admin-editor.test.cjs
```

Desde tools/e2e:

```text
npx.cmd playwright test tests/doc110-fuentes-guided.spec.cjs tests/doc110-perfil-guided.spec.cjs --config=playwright.config.cjs
```

La suite de rechazo anónimo se amplió con las ocho operaciones de fuentes, pero NO se ejecutó contra IIS en este avance. No atribuir cobertura real de CSRF/login/permisos a los fixtures de transporte. Pendientes: esquema completo, permiso asignado, aceptación autenticada, concurrencia real y rollback del historial. No se registró imp_02_egresos por SQL ni se alteraron sus datos. SII permanece sin cambios.

La conversión de FECHA dd/MM/yyyy H:mm:ss sigue en 14.33. CONSECUTIVO duplicado continúa AMBIGUO. Esta entrega no elimina esos pendientes ni cierra DOC-110.
