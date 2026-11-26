# Prompt RI09 para entrega del seguimiento al módulo existente de respuestas

Estado: preparado para refinamiento y ejecución del paquete, no implementado por redactar esta serie.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: RI03, RI04, RI07, RI08. Puertas de refinamiento: G05, G09, G10; G11 solo para plazo, conservando políticas existentes. Trazabilidad: RQ-RI-03, RQ-RI-06, RQ-RI-08, RQ-RI-09, RQ-RI-11.

Al abrir este paquete, contrastar resultados anteriores y cerrar solo sus decisiones necesarias antes del código afectado. No exigir que toda la exploración esté aprobada para empezar análisis independiente. La instrucción de ejecutar el paquete autoriza el trabajo acordado, no elecciones funcionales todavía pendientes ni E2E del ambiente. No volver a pedir acuerdos ya registrados. Registrar los detalles técnicos y adaptar contrato/consumidores si cambia su revisión.

## OBJETIVO

Comprobar que la obligación creada al radicar queda disponible para el módulo existente de respuestas con referencia, modalidad, plazo y responsable correctos. Este módulo no elabora, envía, confirma ni cierra respuestas.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Límite D-RI-03 confirmado: este módulo radica y crea/entrega el seguimiento; elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente. No trasladar esas funciones a esta pantalla ni rediseñar sus políticas.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

N11/N12 y M25 para consulta/entrega autorizada. M24/M26, Classgestionrespuesta, Class_system_plantilla_radicado y adaptadores modernos se inspeccionan como consumidores de compatibilidad; no son funciones de respuesta a implementar o modificar en este paquete.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

## TRABAJO REQUERIDO

1. **RI09.1.** Implementar ConsultarSeguimientoAutorizado usando referencia de operación/radicado/plantilla y propietario, evitando autorizar solo por consecutivo. Reflejar creado/pendiente/preparación incompleta con identificadores reales y lectura sin mutación.

2. **RI09.2.** Comprobar la aparición de la obligación en la bandeja existente y su relación con tarea/radicado. Mantener al usuario interno asignado como propietario; tercero e identidad autenticada conservan sus roles respectivos. Corregir únicamente los datos/relaciones de origen que impidan esta entrega.

3. **RI09.3.** Inventariar las precondiciones del consumidor existente de respuestas, incluida plantilla/homologación si las usa. No elegir una nueva modalidad saliente/interna ni implementar otro recorrido de respuesta; documentar la incompatibilidad o configuración ausente como dependencia del consumidor.

4. **RI09.4.** Verificar que modalidad, plazo, identidad de remitente, propietario y documentos publicados se leen correctamente desde el módulo existente. El criterio de este paquete termina en seguimiento disponible y utilizable; no exige elaborar o cerrar una respuesta.

5. **RI09.5.** Ejecutar regresión focal de los contratos existentes de Workflow si el registro los afecta, sin modificar políticas de terminación, transferencia, aprobación, firma o respuesta. Un defecto externo se registra por separado, sin ampliar este cambio.

6. **RI09.6.** Distinguir radicación completada de obligación pendiente de respuesta. Mostrar información o enlace autorizado al módulo existente cuando proceda; no agregar aquí botones para responder, adjuntar una respuesta, enviarla, confirmarla ni cerrarla.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-03, RQ-RI-06, RQ-RI-08, RQ-RI-09, RQ-RI-11.

## CRITERIOS DE ACEPTACIÓN

- Sin seguimiento no se crea obligación; con seguimiento se conservan modalidad, plazo y propietario.
- La obligación aparece una sola vez y es accesible para el responsable en el módulo existente.
- La lectura de seguimiento no elabora, envía, confirma ni cierra respuestas.
- Los contratos y políticas del módulo de respuestas/Workflow permanecen compatibles; el tercero no queda como usuario responsable.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

## PRUEBAS OBLIGATORIAS

Pruebas focales de las dos columnas, consulta autorizada y datos/relaciones de entrega. Mantener tests/workflow-user-send.test.cjs y tests/workflow-group-send.test.cjs como regresión según impacto. E2E autorizada de radicación sin seguimiento y con cada modalidad hasta disponibilidad/lectura de la obligación en el módulo existente. No exigir elaborar, enviar o cerrar respuestas como aceptación de este alcance.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.


## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar código del paquete, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. La parte que dependa de una decisión pendiente se identifica sin declararla concluida. Crear durante su ejecución `Doc/Diagnostico/radicacion-interna/implementacion/RI09-resultado.md` y actualizar contrato/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. No publicar ni modificar un ambiente que carezca de autorización.
