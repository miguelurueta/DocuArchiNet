# Prompt RI05 para recepción real y temporal de principal y anexos

Estado: preparado para refinamiento y ejecución del paquete, no implementado por redactar esta serie.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: RI02, RI03. Puertas de refinamiento: G03, G06, G07, G08. Trazabilidad: RQ-RI-02, RQ-RI-07, RQ-RI-09, RQ-RI-10.

Al abrir este paquete, contrastar resultados anteriores y cerrar solo sus decisiones necesarias antes del código afectado. No exigir que toda la exploración esté aprobada para empezar análisis independiente. La instrucción de ejecutar el paquete autoriza el trabajo acordado, no elecciones funcionales todavía pendientes ni E2E del ambiente. No volver a pedir acuerdos ya registrados. Registrar los detalles técnicos y adaptar contrato/consumidores si cambia su revisión.

## OBJETIVO

Convertir la selección local de la maqueta en recepción de archivos verificable por operación, sin crear todavía un radicado ni fingir almacenamiento definitivo.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

N06/N07/N12; nuevo generic_control/RadicacionInternaUpload.ashx y .ashx.vb. Referencias de diseño: ImportacionCarpetasUpload/ServicioRecepcionImportacion; FileUploadHandler_.ashx permanece compatible para otros eventos.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

## TRABAJO REQUERIDO

1. **RI05.1.** Implementar multipart autorizado por operación, revisión y referencia de archivo/rol. Rechazar uploads de otro usuario/ámbito, operación cancelada/registrada o manifiesto fuera de versión. Sesión y protección de petición se resuelven en el transporte; comprobar bloqueo de sesión de ASP.NET al transmitir/consultar progreso para no prometer paralelismo que el handler serializa.

2. **RI05.2.** Reservar recepción y ruta interna única antes de escribir stream, con cuotas y límites acordados por archivo/total/cantidad. Verificar extensión/tipo real/integridad de contenido y las reglas de conversión previstas; el MIME o accept del navegador no basta.

3. **RI05.3.** Implementar escritura temporal aislada, huella y cierre verificable. Una recepción parcial o respuesta perdida se consulta antes de reenviar; procesar duplicados y nombres coincidentes según contrato, nunca como sobrescritura por nombre original.

4. **RI05.4.** Implementar ConsultarRecepcion y PrepararManifiesto con estados por archivo, principal explícito y anexos sin doble rol. Confirmar solo la versión recibida y validada; un éxito HTTP no equivale a un documento incorporado.

5. **RI05.5.** Implementar RetirarArchivo y caducidad segura de temporales no vinculados; invalidar el manifiesto/revisión afectados. Una lectura tardía, cancelación o limpieza no resucita ni borra archivos de otra operación o ya incorporados.

6. **RI05.6.** Conservar la recepción autenticada/salida de error y controles compartidos. No reutilizar ni relajar el ID de estado requerido por ADJUNTARADICACION, ni convertir el evento clásico en preparación previa.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-02, RQ-RI-07, RQ-RI-09, RQ-RI-10.

## CRITERIOS DE ACEPTACIÓN

- Existe recepción verificable y consultable para cada archivo de ambos roles.
- Repeticiones, cancelación, respuesta perdida y dos pestañas no mezclan ni duplican la misma recepción.
- Nombres, límites, tipo real y permisos se validan en servidor.
- No se reserva consecutivo ni se registra Workflow/seguimiento al subir un temporal.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

## PRUEBAS OBLIGATORIAS

Pruebas con streams truncados, extensión/MIME falsos, nombres equivalentes, límites, reenvío y cancelación concurrente; comprobar cleanup y publicación del handler. Mantener tests/doc90-radicacion-classic-attachment.test.cjs y tests/radicacion-simple-attachment-upload-signatures.test.cjs. E2E autorizada con upload real y SELECT de control, identificando los temporales descartables.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.


## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar código del paquete, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. La parte que dependa de una decisión pendiente se identifica sin declararla concluida. Crear durante su ejecución `Doc/Diagnostico/radicacion-interna/implementacion/RI05-resultado.md` y actualizar contrato/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. No publicar ni modificar un ambiente que carezca de autorización.
