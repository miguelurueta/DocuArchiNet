# Prompt P04 para carga e incorporación de documentos

Estado: consolidado el 2026-11-26, preparado para refinamiento y ejecución; aplicación sin implementar por redactar este archivo.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: P01 y P02. Orden interno obligatorio: RI05 → RI06. No depende de P03.

Este prompt reúne las subfases RI05, RI06 de la exploración. Los identificadores RI conservan trazabilidad de tareas/funciones; no son prompts adicionales a ejecutar. Reducir archivos no elimina verificaciones ni autoriza saltar precondiciones internas.

- **RI05:** G03, G06, G07, G08. Trazabilidad: RQ-RI-02, RQ-RI-07, RQ-RI-09, RQ-RI-10.
- **RI06:** G01, G06, G07 y política de incorporación de Q-CD-06. Trazabilidad: RQ-RI-01, RQ-RI-02, RQ-RI-07, RQ-RI-09, RQ-RI-11.

Refinar al abrir cada subfase solo las decisiones que consume; conservar acuerdos ya registrados y continuar trabajo independiente. La instrucción de ejecutar el prompt autoriza su alcance acordado, no decisiones funcionales todavía pendientes ni pruebas del ambiente sin autorización. Versionar contratos antes de cambiar consumidores.

## OBJETIVO

Reutilizar el cargador existente para recibir archivos reales por operación e incorporarlos de forma definitiva según la alternativa documental acordada. D-RI-04 aprueba adaptar `LoadFilePERSON` / `IniLoadPerson` conservando la interfaz compacta; el modo de recepción temporal y su procesamiento previo son nuevos. Mantener Producción y los adjuntos clásicos/Simplificados compatibles.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Límite D-RI-03 confirmado: este módulo radica y crea/entrega el seguimiento; elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente. No trasladar esas funciones a esta pantalla ni rediseñar sus políticas.
- D-RI-04 aprobada: reutilizar el control de carga y adaptar su modo previo al registro. Conservar los modos y valores predeterminados de los consumidores actuales; no duplicar todo el cargador ni imponer otra ventana compleja sobre la UI aprobada.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

### Subfase RI05

M27 y N06/N07/N12/N13: `generic_control/FileUploadHandler.js`, `LoadFilePERSON`, `IniLoadPerson`, `_EnviaArchivoServidor` y `_RegistraArchivoInterfaz`; nuevo `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb` para el procesamiento previo. Referencias de diseño: ImportacionCarpetasUpload/ServicioRecepcionImportacion; FileUploadHandler_.ashx permanece compatible para otros eventos. El nuevo handler es transporte de recepción, no un segundo cargador visual.

### Subfase RI06

M10/M19/M20/M21 y N08. Revisar Almacena_documento_radicado_externo, Retorna_parametros_almacenamiento_documento_radicado_interno, Actualiza_documento_produccio_radicado, PreAlmacenaDocumentoProduccion y AlmacenamientoDocumentoProduccionDocumental.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

Crear solo los componentes que correspondan a las subfases de este prompt; no implementar capas de paquetes posteriores.

## TRABAJO REQUERIDO

### Subfase RI05

1. **RI05.1.** Reutilizar `LoadFilePERSON` / `IniLoadPerson` y agregar un modo previo por operación, revisión y referencia de archivo/rol. Adaptar de forma acotada el endpoint de `_EnviaArchivoServidor` y la recepción de resultados de `_RegistraArchivoInterfaz`, conservando destino/callbacks predeterminados de los modos actuales. El nuevo resultado identifica una recepción temporal y no inserta filas como si ya existiera un documento definitivo. Implementar multipart autorizado; rechazar uploads de otro usuario/ámbito, operación cancelada/registrada o manifiesto fuera de versión. Sesión y protección de petición se resuelven en el transporte; comprobar bloqueo de sesión de ASP.NET al transmitir/consultar progreso para no prometer paralelismo que el handler serializa.

2. **RI05.2.** Reservar recepción y ruta interna única antes de escribir stream, con cuotas y límites acordados por archivo/total/cantidad. Verificar extensión/tipo real/integridad de contenido y las reglas de conversión previstas; el MIME o accept del navegador no basta.

3. **RI05.3.** Implementar escritura temporal aislada, huella y cierre verificable. Una recepción parcial o respuesta perdida se consulta antes de reenviar; procesar duplicados y nombres coincidentes según contrato, nunca como sobrescritura por nombre original.

4. **RI05.4.** Implementar ConsultarRecepcion y PrepararManifiesto con estados por archivo, principal explícito y anexos sin doble rol. Integrar selección múltiple y estados del control existente en la UI aprobada, aislando archivos/respuestas por instancia y operación para evitar interferencias de sus variables globales. Confirmar solo la versión recibida y validada; un éxito HTTP no equivale a un documento incorporado.

5. **RI05.5.** Implementar RetirarArchivo y caducidad segura de temporales no vinculados; invalidar el manifiesto/revisión afectados. Una lectura tardía, cancelación o limpieza no resucita ni borra archivos de otra operación o ya incorporados.

6. **RI05.6.** Conservar la recepción autenticada/salida de error y controles compartidos. No reutilizar ni relajar el ID de estado requerido por ADJUNTARADICACION, ni convertir el evento clásico en preparación previa.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-02, RQ-RI-07, RQ-RI-09, RQ-RI-10.

### Subfase RI06

1. **RI06.1.** Implementar la alternativa G01 elegida: A revalida expediente/tipología y obtiene registros reales de Producción al incorporar; B prepara almacenamiento/relaciones independientes con clasificación posterior acordada. No pasar cero por una firma que exige un documento previo.

2. **RI06.2.** Normalizar origen Producción frente a recepción nueva mediante manifiesto autorizado. Para existentes verificar expediente, gabinete, vigencia y relación de radicación; para nuevos exigir recepción íntegra de la operación y política de clasificación.

3. **RI06.3.** Reutilizar almacenamiento físico, índices, gabinetes, firmas y conversiones reales. Si hace falta contexto explícito, agregar sobrecarga/adaptador acotado preservando la firma y pruebas de los consumidores actuales.

4. **RI06.4.** Persistir reclamación y resultado por archivo con referencia durable al documento/incorporación. Consultar evidencia antes de repetir un efecto cuya respuesta se perdió; si el proveedor no permite determinarlo, dejar incertidumbre y recuperación controlada.

5. **RI06.5.** Publicar ConsultarIncorporacion y ReconciliarIncorporacion para preparación incompleta. Mantener el principal explícito, los anexos y la caracterización real; los folios no se inventan a partir del tamaño o fecha de modificación del archivo.

6. **RI06.6.** Definir el estado cuando los documentos quedaron incorporados pero falla el núcleo posterior. No borrar automáticamente datos definitivos como si fueran temporales ni exigir al usuario volver a subir lo ya reconocido.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-01, RQ-RI-02, RQ-RI-07, RQ-RI-09, RQ-RI-11.

## CRITERIOS DE ACEPTACIÓN

### Subfase RI05

- Existe recepción verificable y consultable para cada archivo de ambos roles.
- La carga usa el componente existente con modo previo, configuración de envío acotada y resultado temporal; los modos/callbacks actuales mantienen su comportamiento. La presentación conserva la interfaz compacta aprobada.
- Repeticiones, cancelación, respuesta perdida y dos pestañas no mezclan ni duplican la misma recepción.
- Nombres, límites, tipo real y permisos se validan en servidor.
- No se reserva consecutivo ni se registra Workflow/seguimiento al subir un temporal.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

### Subfase RI06

- Ambos orígenes entregan referencias válidas sin depender de la selección mutable del árbol.
- Fallo/timeout después de guardar conserva evidencia y evita duplicar el mismo documento.
- Solo se incorporan archivos del manifiesto y roles confirmados.
- La variante sin expediente, si fue elegida, funciona sin simular registros de Producción.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

No declarar completado este prompt si una subfase requerida conserva decisiones o verificaciones pendientes. Registrar qué se completó, qué falta y qué trabajo independiente puede continuar; no repetir permisos ni acuerdos ya registrados.

## PRUEBAS OBLIGATORIAS

### Subfase RI05

Pruebas con streams truncados, extensión/MIME falsos, nombres equivalentes, límites, reenvío y cancelación concurrente; comprobar cleanup y publicación del handler. Cubrir endpoint/callback predeterminados frente al nuevo modo, dos instancias/pestañas sin estado mezclado y respuesta temporal sin inserción definitiva en la lista. Mantener tests/doc90-radicacion-classic-attachment.test.cjs y tests/radicacion-simple-attachment-upload-signatures.test.cjs, añadiendo regresión de Producción y otros consumidores efectivamente afectados. E2E autorizada con el control reutilizado y upload real, con SELECT de control e identificación de temporales descartables.

### Subfase RI06

Pruebas de adaptadores con fallos antes/después del efecto, varios gabinetes, firmas/conversión y referencias ajenas. Integración de almacenamiento real con archivos descartables y consultas solo SELECT de control; verificar relaciones/índices y ausencia de duplicados al reconciliar.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.

## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar el código de las subfases, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. Crear durante la ejecución `Doc/Diagnostico/radicacion-interna/implementacion/P04-resultado.md`, con estado y evidencia por subfase RI, y actualizar contratos/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. Publicar/desplegar solo si la autorización incluye el ambiente correspondiente. Este módulo no gestiona respuestas.
