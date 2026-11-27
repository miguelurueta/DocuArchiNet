# Prompt 14 — Fecha inicial elegida al iniciar la importación

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior de ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1. Implementa esta mejora sobre el módulo existente de importación por carpetas, conservando sus contratos, autorizaciones y mecanismos de recuperación.

## OBJETIVO Y ENCARGO

Permitir que un perfil configure la fecha inicial del expediente como **«Fecha indicada al importar»**. Al iniciar una importación con ese perfil, el usuario elige una fecha fija que se aplica a todos los expedientes nuevos de ese lote. Otra importación del mismo perfil puede elegir una fecha diferente sin modificar la versión publicada.

La fecha es documental: no equivale a la fecha de registro, a la fecha del archivo ni al reloj del servidor. No deducir un día de un nombre como `ABRIL 2025`, ni completar con el primer día del mes o con «hoy». El ejemplo `2025-04-01` utilizado en las pruebas es sintético y no establece la fecha correcta de un expediente real.

Este archivo es un encargo de implementación futura. Su creación no acredita código implementado ni autoriza despliegues, sustitución de DLL del IIS activo, migraciones o carga real de documentos.

## CONTEXTO Y DEPENDENCIAS

Leer `AGENTS.md`, [guía 00](00-guia-ejecucion-prompts.md), [exploración de esta mejora](Exploracion/14-fecha-inicial-por-importacion.md) y el diseño vigente en `Diseno/`. Revisar el estado real de los componentes 02, 05, 06, 07, 08 y 10–12; no reimplementarlos. El prompt 13 es independiente y no constituye una dependencia.

Antes de editar, conservar los cambios locales ajenos e identificar las rutas y pruebas que corresponden al comportamiento actual. Concretar el inventario según «PRECONDICIONES DE RUTAS». Resolver decisiones técnicas rutinarias con evidencia del código y registrar el resultado; no exigir una aprobación adicional para cada detalle. Consultar al usuario solo si aparece una incompatibilidad que obligue a cambiar el alcance funcional o una restricción vinculante.

## RESTRICCIONES CRÍTICAS

- Mantener .NET Framework 4.6.1, WebForms, VB.NET, ASMX y JavaScript existentes. Sin nueva plataforma, árbol, motor de importación o sistema de permisos.
- Conservar `CONSTANTE`: sigue significando un literal del perfil. No usar una constante vacía, un texto especial o una fecha ficticia para representar el nuevo modo.
- No guardar el valor de cada lote en la configuración o reglas del perfil ni alterar el snapshot de la publicación para introducirlo. No modificar versiones publicadas en sitio; utilizar la administración y versionado existentes.
- No cambiar la semántica de `CARPETA`, `ARCHIVO`, `EXPEDIENTE` ni `PROVEEDOR`. La mejora no habilita proveedores como fuente de campos del expediente ni consultas adicionales para obtener esta fecha.
- No actualizar expedientes existentes, reglas de retención, fecha final, auditoría o metadatos documentales por fuera de sus recorridos actuales. Si un índice ya hereda `FECHA_INICIAL` del expediente, debe seguir recibiendo el valor resuelto mediante ese contrato; comprobarlo como regresión.
- **Almacenamiento intocable:** no editar `workflow/ClassAlmacenamiento.vb` ni las funciones protegidas por D-RI-07. Leer el [diagnóstico de almacenamiento](../../../Diagnostico/radicacion-interna/diagnostico-almacenamiento-metadatos.md) y su baseline; ejecutar `node tools/diagnostico/verify-radicacion-storage-freeze.cjs`. No cambiar su baseline o el verificador para conseguir un resultado correcto.
- No cambiar SII, carga individual, radicación, escritores documentales compartidos ni el contrato público de creación de expedientes. El adaptador de creación debe continuar consumiendo `ValoresPlan`.
- No modificar límites globales de JSON, archivos, tiempos o concurrencia. No relajar la forma cerrada del JSON ni sus controles de autorización, revisión, huella o idempotencia.
- Diseñar sin nuevas tablas o columnas, aprovechando los documentos JSON existentes. Si se demuestra una limitación que lo impide, documentarla y refinar el diseño antes de introducir DDL; no ejecutar SQL automáticamente.

## COMPORTAMIENTO FUNCIONAL

1. En la regla de destino `CAMPO_EXPEDIENTE:FECHA_INICIAL`, ofrecer **«Fecha indicada al importar»**. El perfil expresa de dónde se obtendrá el dato, sin almacenar la fecha del lote.
2. Solo un perfil con esa fuente muestra en la ventana de importación un calendario obligatorio: **«Fecha inicial de los expedientes»**, con ayuda: «Se aplicará a todos los expedientes nuevos de esta importación».
3. Mantener el calendario vacío hasta una elección explícita. Transportar una fecha civil canónica `yyyy-MM-dd`, sin hora ni conversiones a UTC que puedan desplazar el día. Aplicar la validez y rango de fecha que ya exige el servidor; no añadir restricciones de negocio sobre fechas futuras o pasadas sin respaldo.
4. Mostrar la fecha efectiva en la revisión previa y al recuperar la preparación. La interfaz debe distinguir un dato recuperado del servidor de una selección aún no persistida.
5. Cambiar fecha, perfil o inventario antes de confirmar invalida la revisión anterior. Una preparación ya iniciada conserva sus datos; para utilizar otra fecha crear una nueva preparación y una nueva clave, sin modificar partes guardadas ni permitir que una respuesta tardía confirme una revisión anterior.
6. Conservar el valor del plan al pausar, reanudar, recargar y reintentar. Es una decisión técnica de consistencia, no una nueva exigencia de edición durante la pausa: el usuario indicó que no necesita esa funcionalidad. Tras confirmar no ofrecer una operación de cambio de fecha del lote.
7. Corregir la presentación de «Opcional» para `FECHA_INICIAL`: el servidor ya exige este campo aunque `Obligatoria` sea falso. Mostrar su obligatoriedad efectiva sin reescribir automáticamente reglas históricas ni cambiar sus huellas.
8. Mantener contraídos, accesibles y operativos los grupos de reglas, el botón visible de agregar, los mensajes detallados y las versiones publicadas de solo lectura.

## CONTRATO Y DISEÑO DE IMPLEMENTACIÓN

Adoptar los siguientes nombres salvo incompatibilidad comprobada con el esquema real; documentar cualquier ajuste y mantenerlo uniforme en todos los consumidores.

### Fuente de la regla

- `FuenteTipo = "IMPORTACION"`, admitido exclusivamente en `CAMPO_EXPEDIENTE:FECHA_INICIAL`.
- `TipoValor = "FECHA"`, `EtapaResolucion = "PREFLIGHT"`, `Extraccion = "COMPLETO"`, `FormatoFecha = "yyyy-MM-dd"`, `LongitudMaxima = 10`.
- Sin proveedor, propiedad de expediente, literal constante, segmentación o extracción de extensión. Normalizar estos controles al elegir el nuevo modo sin modificar reglas de otras fuentes.
- Validar las restricciones en servidor, no solo deshabilitando opciones en la interfaz. La publicación valida la estructura; no exige la fecha de una importación que todavía no existe. El ensayo de ejemplo sí la requiere para resolver esa regla.

### Dato de ejecución

- Añadir `fechaInicialImportacion As String` opcional a `RaizManifiestoImportacionSolicitud` y `FechaInicialImportacion As String` opcional a `RaizManifiestoImportacion`. El cliente copia la única fecha elegida a todas las raíces del lote.
- Añadir el dato opcional equivalente a `EjemploPerfilImportacion` para que el resolvedor use una entrada tipada, sin colocarlo artificialmente en el diccionario `Expediente`. Revisar los decodificadores cerrados y las validaciones de longitud y tipo.
- **Omitir estas propiedades del JSON cuando sean `Nothing`**, con configuración localizada en la propiedad. No cambiar globalmente el serializador. Para perfiles antiguos el cliente también omite la propiedad; no envía cadenas vacías o fechas implícitas.
- En el nuevo modo, exigir una fecha válida y la misma fecha en todas las raíces. La ausencia, un valor inválido o fechas distintas bloquean globalmente la preparación, también en modo parcial. No elegir la primera ni excluir raíces para ocultar la discrepancia. Fuera del nuevo modo, rechazar un valor no nulo enviado para esta finalidad en lugar de utilizarlo para sobrescribir la fecha del perfil.
- Conservar el campo en las proyecciones DTO/modelo y al reconstruir solicitudes. Persistirlo en partes `RAICES` y manifiestos existentes; queda incluido en las huellas de esos documentos y del plan. No introducirlo en `Snapshot.Configuracion`, `Snapshot.Reglas` ni en la comparación `MismaPublicacion`.
- En preparación segmentada, comprobar la consistencia contra **todas** las partes de raíces antes de validar un subconjunto y antes de sellar. La comprobación de una sola parte no demuestra una fecha única por lote. Conservar las revisiones y exclusión actuales frente a partes concurrentes.
- Si la preparación aún no tiene partes de raíces guardadas, la fecha todavía no está persistida: recuperar el identificador no autoriza inventar ese valor. Permitir continuar únicamente con la selección necesaria o una nueva preparación; nunca declarar una fecha recuperada que no existe.
- Extender las respuestas de consulta/revisión que sean necesarias para mostrar el dato persistido, con propiedades opcionales omitidas cuando no aplican. Conservar paginación, límites y alcance de las consultas; no devolver inventarios completos ni guardar la fecha como autoridad en `localStorage`, sesión o variables globales.

### Resolución y creación

En `ResolverRaiz`, pasar la fecha de la raíz al ejemplo y resolverla con la nueva fuente. Guardar el resultado canónico como `CAMPO_EXPEDIENTE:FECHA_INICIAL` en `Resultado.Elementos` de ámbito `RAIZ`, conservando procedencia y tipo. Mantener la validación de fecha inicial menor o igual que fecha final cuando esta exista.

La confirmación, ejecución, verificación y recuperación utilizan los valores persistidos del plan. No recalcular esta fecha usando el reloj, el perfil más reciente, el navegador o el proveedor. `ComandoCreacionExpedienteImportacion` debe continuar leyendo `campos("FECHA_INICIAL").Valor`; verificar la aceptación de la nueva procedencia mediante el validador común, sin modificar el escritor legacy para admitirla.

## PRECONDICIONES DE RUTAS

Las rutas son existentes al redactar este prompt. Revisarlas no significa que todas deban editarse. No ampliar el cambio a archivos compartidos sin una necesidad demostrada.

Inventariar los archivos exactos antes de editar en `Diseno/design.md`, con responsabilidad, consumidores y condición existente/nuevo. Registrar explícitamente cualquier archivo VB nuevo en `GestionDocumental-Docuarchi.net.vbproj`; no basta con colocarlo en disco. Mantener un solo inventario enlazado desde la exploración.

| Área | Archivos y puntos relevantes |
|---|---|
| Contratos | `DTOs/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasDtos.vb`, `PreparacionSegmentadaDtos.vb` en ese directorio; `Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasModels.vb` y `PerfilImportacionModels.vb`. |
| Validación y ensayo | `Services/ProduccionDocumental/ImportacionCarpetas/ResolvedorReglasPerfilImportacion.vb`, `ValidadorPerfilImportacion.vb`, `PerfilImportacionJson.vb`, `ServicioPerfilImportacion.vb` y `CoordinadorProveedorIndices.vb`. Revisar copia de ejemplos y mensajes, sin cambiar consultas de proveedor. |
| Preparación | `ServicioPreparacionImportacion.vb`: `CalcularPlan`, `CalcularRaicesSegmentadas`, `ResolverRaiz`; `ServicioPreparacionSegmentadaImportacion.vb`: `Registrar`, `Validar`, `Sellar` y consultas, dentro del directorio de servicios anterior. |
| Persistencia e integridad | `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Preparacion.vb`; `CanonizadorManifiestoImportacion.vb`, `PlanSegmentadoImportacionJson.vb` y `PerfilImportacionJson.vb` en servicios. Verificar recorrido JSON de ida y vuelta sin cambiar algoritmos de huella. |
| Consumidores | `ServicioRecepcionImportacion.vb`, `CoordinadorImportacionCarpetas.Valores.vb` y `ServicioConsultaImportacion.vb` en servicios; `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComandoCreacionExpedienteImportacion.vb`. Conservar contratos de creación y reconciliación. |
| Interfaz | `js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js`, `ImportacionCarpetas.js`, `PreparacionSegmentadaImportacion.js`, sus estilos y clientes existentes; `Gestion/WebFormProducionDocumental.aspx` para versiones de recursos si corresponde. |
| Entradas y proyecto | `webservice/WebServiceImportacionCarpetas.asmx`, `webservice/WebServiceImportacionCarpetas.asmx.vb`, `webservice/WebServiceImportacionCarpetas.PreparacionSegmentada.vb`, su composición y `GestionDocumental-Docuarchi.net.vbproj`. Mantener endpoints, forma cerrada de solicitudes y autenticación actuales. |

## SECUENCIA DE TRABAJO

1. Capturar con datos sintéticos la serialización y huellas actuales de un perfil y planes previos, esquemas 1 y 2; fijar esas expectativas antes de ampliar modelos. Inventariar también las partes de preparación y el estado recuperable anterior al cambio.
2. Implementar fuente y contratos opcionales, validación, ensayo y resolución; probar sus recorridos positivos y rechazos sin efectos documentales.
3. Integrar proyecciones, persistencia segmentada, consultas y recuperación; demostrar conservación del valor y de las huellas anteriores. No pasar a creación con un plan cuya fecha no esté comprobada.
4. Integrar editor, calendario, revisión y control de respuestas tardías con los mecanismos existentes de invalidación. Resolver operaciones inciertas por consulta, sin repetir automáticamente escrituras.
5. Compilar, ejecutar regresión aplicable y preparar evidencia y guía de publicación/retorno. No desplegar ni ejecutar pruebas reales sin la autorización correspondiente.

## CRITERIOS DE ACEPTACIÓN Y PRUEBAS OBLIGATORIAS

| ID | Escenario | Resultado exigido |
|---|---|---|
| FI-01 | Perfil anterior con `CONSTANTE`, `CARPETA` u otras fuentes admitidas. | Mismas entradas, resolución y comportamiento; no aparece calendario nuevo ni se exige parámetro adicional. |
| FI-02 | Documentos históricos sin la propiedad nueva: perfil, plan clásico, plan segmentado y partes en curso. | Deserializar, volver a serializar y verificar con las mismas huellas previas. No regenerar expectativas históricas para acomodar un cambio accidental. |
| FI-03 | Crear borrador, guardar, publicar, seleccionar y ensayar la nueva fuente. | Estructura válida sin fecha al publicar; ensayo con fecha explícita resuelve, sin ella informa el campo requerido; reglas publicadas continúan de solo lectura. |
| FI-04 | Dos importaciones del mismo perfil, dos fechas distintas y varias raíces cada una. | Cada lote conserva su fecha uniforme; el perfil, su snapshot y la otra ejecución no cambian. |
| FI-05 | Fecha ausente, imposible, fuera del rango admitido, con hora o de tipo JSON incorrecto. | Rechazo comprensible antes de confirmar; sin fecha implícita ni efectos. Probar año bisiesto y ausencia de desplazamiento por zona horaria. |
| FI-06 | Fecha diferente entre raíces, incluso separadas en partes no consecutivas; modo parcial y completo. | Bloqueo global de preparación/sellado; no seleccionar una fecha ni omitir raíces. Valor enviado a un perfil que no usa el modo: rechazo sin sobrescritura. |
| FI-07 | Preparación clásica y segmentada del mismo inventario, con varios fragmentos `RAICES` y `ARCHIVOS`. | Mismos valores de fecha por raíz; el dato sobrevive a proyecciones, persistencia, canonización, fragmentación y reconstrucción. Respetar límites de tamaño. |
| FI-08 | Pausa y recuperación antes/después de guardar raíces, pérdida de respuesta y reintento idempotente. | No inventar fecha donde aún no hay persistencia. Recuperar la guardada, conservar revisiones y no duplicar planes, expedientes o documentos. |
| FI-09 | Cambiar fecha/perfil/inventario con una preparación en curso o una respuesta tardía. | Invalidar la revisión y separar la nueva preparación; no reutilizar clave, consentimiento o confirmación anterior ni mezclar fechas entre partes. |
| FI-10 | Fecha inicial posterior a la final; índice ya configurado para heredar fecha inicial. | Conservar rechazo de rango; el índice sigue heredando el valor confirmado por su recorrido existente, sin nuevas reglas automáticas. |
| FI-11 | Confirmar y ejecutar con adaptadores de prueba; repetir creación/verificación/reanudación. | Se recibe exactamente la fecha del plan; no se actualizan expedientes existentes ni se vuelve a resolver desde el reloj. Escrituras inciertas conservan sus barreras. |
| FI-12 | UI con teclado, móvil, reglas largas y un perfil publicado. | Calendario etiquetado, obligatoriedad efectiva y error asociado al campo; fecha visible en revisión; colapsado, foco, botón agregar y lectura de publicados conservados. |
| FI-13 | Usuario sin permiso o con preparación/plan de otro ámbito o propietario; JSON desconocido. | Rechazos actuales conservados, sin filtración de fechas/inventarios ajenos ni relajación de contratos. |
| FI-14 | Regresión de importación de egresos, carga individual y congelación de almacenamiento. | Evidencia apropiada al entorno; escritor y funciones protegidas sin cambios. No declarar regresión real aprobada únicamente por pruebas con dobles. |

Reutilizar las pruebas y arneses existentes, en especial `tests/doc110-perfil-importacion-behavior.test.cjs`, `tests/doc110-perfil-importacion-editor.test.cjs`, `tests/doc110-perfil-importacion-client.test.cjs`, `tests/doc111-importacion-carpetas-client.test.cjs`, `tests/doc120-preparacion-segmentada.test.cjs`, `tests/doc120-preparacion-tamano.test.cjs` y `tests/doc120-segmentada-browser.test.cjs`. Ampliar los escenarios de fecha donde corresponda; no crear pruebas que solo comprueben presencia de cadenas en el código para acreditar persistencia o comportamiento.

Para JavaScript ejecutar `node --check` sobre los archivos cambiados y pruebas de comportamiento pertinentes. Para VB compilar con MSBuild y la configuración real del proyecto; un arnés aislado no sustituye la compilación de la aplicación.

La E2E es parte integral del mismo cambio: código, pruebas, validación autorizada y evidencia saneada forman la unidad de entrega. Reutilizar exclusivamente la infraestructura existente de `tools/e2e`: autenticación, configuración, validadores, reporteros y utilidades. No crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión según FI-01–FI-14.

Antes de E2E autenticado, carga o escritura real, leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` y obtener autorización explícita para ambiente, cuentas y datos descartables. Este prompt no la concede. Usar secretos efímeros; no imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión. Consultas de control solo `SELECT`; evidencia saneada y datos de ejecución en ubicaciones excluidas.

Respetar los gates de seguridad existentes sin habilitarlos arbitrariamente para probar. No introducir flags o listas de usuarios/grupos para activar la experiencia moderna. No cerrar sin la validación requerida; registrar bloqueo explícito cuando falte ambiente o autorización. Mocks y simulaciones son evidencia local complementaria y no sustituyen la integración real pendiente.

## PUBLICACIÓN Y COMPATIBILIDAD

No habilitar el uso de la nueva fuente hasta disponer del backend y los recursos cliente compatibles. Versionar los recursos cambiados según el mecanismo existente. Una aplicación antigua con JSON cerrado o sin reconocimiento de `IMPORTACION` no es un destino seguro para planes nuevos: documentar cómo conservar su lectura y recuperación antes de un retorno de versión. No eliminar el soporte mientras queden preparaciones o lotes nuevos pendientes; no convertir sus reglas o fechas silenciosamente para hacerlos legibles por el código antiguo.

## DOCUMENTACIÓN TÉCNICA

Actualizar la exploración y el diseño vigente con el contrato final y una tabla de funciones creadas/modificadas: función, ruta, ubicación, parámetros y responsabilidad/retorno. Documentar fecha requerida, recuperación, publicación y retorno en las guías existentes; guardar evidencia saneada bajo `Validacion/` y datos de ejecución locales excluidos. No duplicar documentación del módulo ni convertir ejemplos en datos operativos.

## ENTREGABLE FINAL

Entregar archivos cambiados, contrato final, decisiones refinadas, evidencia por escenario FI-01–FI-14 y comandos con resultado. Separar `EJECUTADO_OK`, `FALLIDO` y `NO_EJECUTADO` con motivo. Mantener pendientes las verificaciones reales sin ambiente o autorización; no presentarlas como aprobadas.

Registrar los cambios propios en la carpeta principal según `AGENTS.md` y comunicar el estado frente al remoto, sin incluir cambios ajenos o secretos. La mejora no se declara operativa de extremo a extremo mientras falten compilación o verificaciones de integración requeridas.
