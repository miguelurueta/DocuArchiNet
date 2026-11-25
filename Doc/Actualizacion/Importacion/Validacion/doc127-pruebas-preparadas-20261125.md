# DOC-127 — Pruebas preparadas y verificación local

**55/55 pruebas Node PASS**, cero fallos; el harness compilado pasó **7996 comprobaciones nativas** y su regresión heredada. Corte base `fe2fa911a2e36a2d5424035b9265dde1f4ca0feb` más las huellas exactas de los ocho archivos del JSON. Estos conteos se solapan: no representan8051 pruebas independientes. **22/33 tareas**: programar y probar localmente los recorridos no acredita una ejecución nueva en IIS/MySQL.

## Qué quedó creado

| Grupo | Cobertura preparada y comprobada | Nivel de ejecución |
| --- | --- | --- |
| Plantillas |12 combinaciones: histórica/nueva × XLS/XLSX × crear/agregar/reemplazar; hoja, clave, revisión, IDs, ruta y mapeos NULL; roundtrip por servicio/repositorio real | Servicios VB reales, ADO sintético |
| Cambio de autoridad |36 casos: permiso, usuario, empresa, módulo, conexión e identidad física × tres modos × antes de escribir/después de INSERT antes del COMMIT | Servicio/writer reales, transacciones ADO sintéticas |
| Capacidad |18 recorridos de preparar/confirmar/consultar/replay: dos formatos ×1000/5000/10000 × tres modos; consentimiento de reemplazo, resultado y confirmación sin nueva recepción | Suite E2E real evaluada con transporte/controles sintéticos |
| Rechazos de capacidad | Formato incorrecto o filas incompletas detienen confirmar en12 cruces; permiso perdido detiene antes del login | Transporte sintético |
| Seguridad E2E | CSRF ausente/inválido, conexión/plantilla/carga ajenas e ID inexistente; códigos exactos, sin datos de la carga ajena y lectura válida al terminar | Programada; helpers y guardas pasan localmente; pendiente ventana real |
| Asistente E2E | Alta y carga desde XLS o XLSX; formato y extensión deben coincidir con el scope autorizado; se conserva el recorrido DOC126 anterior | Programada y validada localmente; XLS real pendiente de ventana |
| Libros |18 archivos de capacidad y hashes de nueve fixtures históricos comprobados con el lector del producto | Archivos sintéticos reales; sin MySQL/HTTP |

Los casos de autoridad tardía demuestran que llegaron al escritor y que se conserva el contenido completo tras el rechazo. En CREAR se comprueba expresamente que el DDL propio puede sobrevivir al rollback DML; no se borra la estructura para fingir reversión. La revocación de permiso en estos casos es sintética: ninguna cuenta real fue modificada.

## Cómo ejecutar la comprobación local

Desde la carpeta principal:

```powershell
npm.cmd run test:doc127:local
```

El comando compila el harness VB/C# en .tmp/, ejecuta los validadores y transportes sintéticos existentes y comprueba los18 libros con el lector de producto. No pide contraseñas, conecta a una base real, instala una DLL ni importa datos reales. Usa los compiladores .NET y Python ya disponibles localmente.

## Qué falta para ejecutar las pruebas reales

Los selectores y controles existentes se reutilizan; el listado Playwright conserva13 escenarios, sin ejecutarlos. Para la autoridad se usa --excel-ui-read con un scope nuevo que declare acceptanceIssue=DOC-127 y authorityProbes, además de DOC127_AUTHORITY_E2E_AUTHORIZED=YES y la autorización de lectura existente. El runner rechaza la falta del permiso adicional antes de pedir credenciales. La lectura anterior consumida no autoriza estos intentos adicionales.

| Referencia del scope authorityProbes | Condición que se comprueba |
| --- | --- |
| authorizationReference | Aprobación expresa del nuevo alcance |
| foreignConexionId / foreignPlantillaId | Conexión distinta de la autorizada y plantilla que pertenece a ella |
| foreignUsuarioId / foreignLoadId | Otro usuario y una carga existente de ese propietario, misma empresa y conexión de prueba |
| missingLoadId | ID distinto cuya ausencia se comprueba por SELECT |

Tres SELECT previos verifican los recursos antes del primer login. El recorrido utiliza sólo AbrirCargaExcel, ListarPlantillas, ConsultarPlantilla y ConsultarResultado; no prepara, confirma ni guarda plantillas. Conserva los controles de32 tablas/configuración que se declaren en el scope. Empresa manipulada y revocación física mantienen cobertura local o requieren su mecanismo/ventana específicos; no se simula haberlas ejecutado realmente.

El asistente reutiliza --excel-ui-assistant. Para XLS se declara acceptanceIssue=DOC-127, assistant.formato=XLS y un archivo .xls; para XLSX, formato/extensión correspondientes. Exige su autorización propia DOC126_ASSISTANT_E2E_AUTHORIZED=YES y un destino nuevo expresamente autorizado. Este recorrido sí crea una plantilla y confirma1000 filas; preparar el código no concede esos efectos.

Las seis celdas de capacidad reutilizan --excel-capacity y sus scopes por ventana, con cuenta, plantilla, destino, archivo, filas previas, efectos y presupuesto concretos. No se rellenan esos valores por suposición ni se reabren scopes consumidos. Las credenciales se introducen sólo en consola privada y memoria.

## Estado de las tareas

| Tareas pendientes | Lo que ya está preparado | Dependencia para completar aceptación |
| --- | --- | --- |
|2.4| SQL29 de EXPLAIN entregado | Plan manual del servidor y revisión |
|5.1| Lectura real previa PASS; seis rechazos reales preparados,36 cambios de autoridad y guardas previas locales PASS | Recursos/ventana para los casos adicionales y evidencia de preparación; revocación física sólo autorizada |
|5.2|12 casos de plantillas PASS y asistente E2E ampliado a ambos formatos | Cruces reales pendientes y evidencia histórica compatible |
|6.1–6.6|18 recorridos programados y comprobados con transporte sintético;18 libros leídos | Ventanas autorizadas y mediciones reales de tiempo/memoria/resultado |
|7.1| Regresión focal y casos adversos preparados; sin nuevo defecto de producto demostrado | Revisar hallazgos de la aceptación integral |
|7.5| OpenSpec/OPSXJ y evidencias trazables | Completar aceptación y revisión/evidencias por SHA antes de cerrar |

La DLL, página, configuración y assets publicados conservan las huellas de la lectura real previa. Sólo se modifican pruebas, soporte E2E y el comando local; no se despliega producto. El soporte/suite cambiaron después de aquella lectura: su PASS histórico se conserva con su SHA original y no acredita haber ejecutado estos casos nuevos. Datos de ejecución y logs quedan en .tmp/, excluidos de Git.
