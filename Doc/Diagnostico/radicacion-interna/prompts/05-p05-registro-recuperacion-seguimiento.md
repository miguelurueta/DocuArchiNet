# Prompt P05 para registro, recuperación y entrega del seguimiento

Estado: consolidado el 2026-11-26, preparado para refinamiento y ejecución; aplicación sin implementar por redactar este archivo.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: P02, P03 y P04. Orden interno obligatorio: RI07 → RI08 → RI09.

Este prompt reúne las subfases RI07, RI08, RI09 de la exploración. Los identificadores RI conservan trazabilidad de tareas/funciones; no son prompts adicionales a ejecutar. Reducir archivos no elimina verificaciones ni autoriza saltar precondiciones internas.

- **RI07:** G01 y las decisiones ya consumidas por identidad, plantilla y documentos. Trazabilidad: RQ-RI-03, RQ-RI-06, RQ-RI-07, RQ-RI-08, RQ-RI-09.
- **RI08:** G05, G08 y política de reintento de efectos externos. Trazabilidad: RQ-RI-07, RQ-RI-08, RQ-RI-09, RQ-RI-11.
- **RI09:** G05, G09, G10; G11 solo para plazo, conservando políticas existentes. Trazabilidad: RQ-RI-03, RQ-RI-06, RQ-RI-08, RQ-RI-09, RQ-RI-11.

Refinar al abrir cada subfase solo las decisiones que consume; conservar acuerdos ya registrados y continuar trabajo independiente. La instrucción de ejecutar el prompt autoriza su alcance acordado, no decisiones funcionales todavía pendientes ni pruebas del ambiente sin autorización. Versionar contratos antes de cambiar consumidores.

## OBJETIVO

Registrar una sola vez, completar/reconciliar fases posteriores y entregar el seguimiento al módulo existente de respuestas. No elaborar, enviar, confirmar ni cerrar respuestas.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- D-RI-06: tratar la radicación y sus contratos compartidos como núcleo crítico. Caracterizar el comportamiento antes de modificarlo, justificar cada adaptación/extracción indispensable y conservar numeración, transacciones y consumidores. Un servicio o control nuevo no aísla las clases/tablas comunes ni acredita ausencia de regresiones; no reescribir el motor como parte del cambio visual.
- Límite D-RI-03 confirmado: este módulo radica y crea/entrega el seguimiento; elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente. No trasladar esas funciones a esta pantalla ni rediseñar sus políticas.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

### Subfase RI07

M05/M18 y N03/N09/N11. Punto principal: ClassRadicador.Registra_Radicacion_entrante_interna, reserva FOR UPDATE alrededor de 2700 y transacción hasta Commit. Inventariar dependencias que abren otra conexión.

### Subfase RI08

M19–M23 y N10. Revisar Registra_flujo_documento, Envia_corre_notificacion_radicado_envio_documento, generación/descarga de radicado y estados Flow actuales. La página usa hoy Hidden_ruta_archivo/iframe de descarga.

### Subfase RI09

N11/N12 y M25 para consulta/entrega autorizada. M24/M26, Classgestionrespuesta, Class_system_plantilla_radicado y adaptadores modernos se inspeccionan como consumidores de compatibilidad; no son funciones de respuesta a implementar o modificar en este paquete.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

Crear solo los componentes que correspondan a las subfases de este prompt; no implementar capas de paquetes posteriores.

## TRABAJO REQUERIDO

### Subfase RI07

1. **RI07.1.** Ejecutar antes de modificar el núcleo las pruebas de caracterización previstas en P01 sobre el contrato vigente y sus consumidores afectados, con datos/infraestructura autorizados según corresponda. Registrar comportamiento y límites; no sustituirlos por un test de texto ni por la firma intacta. Separar adaptación WebForms, validación y datos de una orden tipada mediante el cambio mínimo justificado; no asumir que un wrapper resuelve dependencias de controles/sesión o de documentos previos. Revalidar autoridad, remitente, destino, plantilla/configuración y manifiesto exacto inmediatamente antes de la mutación. Preparar revisión no concede permiso permanente.

2. **RI07.2.** Implementar ConfirmarOperacion y reclamación durable por operación/clave/revisión. Igual confirmación devuelve el resultado conocido; clave con payload/versión distintos produce conflicto. Dos peticiones concurrentes no ejecutan el núcleo dos veces.

3. **RI07.3.** Definir una unidad de trabajo explícita y extraer hacia `RegistrarNucleo` solo lo indispensable para la orden tipada, la asociación durable y la recuperación. Justificar por función el requisito, alternativas, consumidores y evidencia previa/posterior; conservar las reglas existentes sin clonar o reescribir el monolito. Mantener SELECT FOR UPDATE, actualización de consecutivo general o de plantilla y formatos corto/simple/código de barras; no reservar el consecutivo fuera de la transacción. No declarar que la extracción es segura por residir detrás de otro ASMX ni atribuir atomicidad a conexiones que no comparten transacción.

4. **RI07.4.** Insertar datos de plantilla, relaciones documentales de la alternativa elegida, registro general, solicitud de respuesta y auditoría con actor/propietario correctos. Crear seguimiento solo según reglas canónicas de RI03 y sin doble solicitud para la operación.

5. **RI07.5.** Enlazar operación y radicado confirmado en la misma transacción del núcleo cuando comparten recurso transaccional; registrar identificadores necesarios para RI08. Si otras conexiones impiden esa garantía, resolver el límite explícitamente y no declarar atomicidad global.

6. **RI07.6.** Preservar wrapper legacy o migrar todos sus consumidores comprobados. Retirar de la ruta nueva la repetición del método monolítico completo como recuperación. Un fallo después de Commit devuelve registrado con pendientes o incertidumbre consultable.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-03, RQ-RI-06, RQ-RI-07, RQ-RI-08, RQ-RI-09.

### Subfase RI08

1. **RI08.1.** Implementar fases durables y reclamación de ejecución por operación para completar incorporación restante, vinculación/asignación Workflow y notificación. Concretar dependencia/orden de fases para no enviar una solicitud antes de tener documentos utilizables.

2. **RI08.2.** Correlacionar tarea, documentos y efecto de notificación con el radicado/operación. Consultar efectos ya confirmados antes de repetirlos. Si una API legacy devuelve solo YES, agregar evidencia o adaptador mínimo, no asumir que repetirla es seguro.

3. **RI08.3.** Implementar ConsultarResultado y RecuperarPendientes con autoridad específica, revisión y estados diferenciados. Las consultas son lecturas; la recuperación es una operación mutante explícita y nunca vuelve a llamar al registro completo.

4. **RI08.4.** Distinguir rechazo previo, registrado, pendiente documental/Workflow, correo pendiente y resultado incierto. Un timeout de correo con entrega desconocida no permite prometer exactamente una entrega; consultar proveedor o conservar pendiente para atención según política acordada.

5. **RI08.5.** Integrar descarga/impresión auténtica con autorización de operación/documento y salidas existentes; no aceptar rutas físicas del cliente ni sustituir el documento por el TXT de la maqueta.

6. **RI08.6.** Implementar limpieza/caducidad solo de temporales propios no vinculados. Conservar registro, archivos incorporados y resultado al cerrar/reanudar. Preparar recuperación tras reciclado del proceso sin depender de memoria de IIS.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-07, RQ-RI-08, RQ-RI-09, RQ-RI-11.

### Subfase RI09

1. **RI09.1.** Implementar ConsultarSeguimientoAutorizado usando referencia de operación/radicado/plantilla y propietario, evitando autorizar solo por consecutivo. Reflejar creado/pendiente/preparación incompleta con identificadores reales y lectura sin mutación.

2. **RI09.2.** Comprobar la aparición de la obligación en la bandeja existente y su relación con tarea/radicado. Mantener al usuario interno asignado como propietario; tercero e identidad autenticada conservan sus roles respectivos. Corregir únicamente los datos/relaciones de origen que impidan esta entrega.

3. **RI09.3.** Inventariar las precondiciones del consumidor existente de respuestas, incluida plantilla/homologación si las usa. No elegir una nueva modalidad saliente/interna ni implementar otro recorrido de respuesta; documentar la incompatibilidad o configuración ausente como dependencia del consumidor.

4. **RI09.4.** Verificar que modalidad, plazo, identidad de remitente, propietario y documentos publicados se leen correctamente desde el módulo existente. El criterio de este paquete termina en seguimiento disponible y utilizable; no exige elaborar o cerrar una respuesta.

5. **RI09.5.** Ejecutar regresión focal de los contratos existentes de Workflow si el registro los afecta, sin modificar políticas de terminación, transferencia, aprobación, firma o respuesta. Un defecto externo se registra por separado, sin ampliar este cambio.

6. **RI09.6.** Distinguir radicación completada de obligación pendiente de respuesta. Mostrar información o enlace autorizado al módulo existente cuando proceda; no agregar aquí botones para responder, adjuntar una respuesta, enviarla, confirmarla ni cerrarla.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-03, RQ-RI-06, RQ-RI-08, RQ-RI-09, RQ-RI-11.

## CRITERIOS DE ACEPTACIÓN

### Subfase RI07

- Doble clic y dos peticiones concurrentes producen un solo radicado/obligación.
- Se conservan todos los formatos y ámbitos de consecutivo frente a consumidores anteriores.
- Rollback anterior al commit no se confunde con un registro confirmado.
- Respuesta perdida alrededor del commit se resuelve consultando la asociación durable, sin otro consecutivo.
- Cada modificación compartida tiene necesidad y alcance mínimo documentados, caracterización previa y comparación posterior; los consumidores vigentes conservan sus invariantes y no se reescribió el motor por el cambio visual.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

### Subfase RI08

- Un fallo posterior al commit conserva el radicado y sus identificadores.
- Recuperar dos veces no duplica documento, tarea ni efecto ya reconocido.
- Correo pendiente no anuncia rechazo del registro; incertidumbre no habilita otro registro.
- Descarga/impresión respetan autoridad y contenido; limpieza no borra documentos incorporados.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

### Subfase RI09

- Sin seguimiento no se crea obligación; con seguimiento se conservan modalidad, plazo y propietario.
- La obligación aparece una sola vez y es accesible para el responsable en el módulo existente.
- La lectura de seguimiento no elabora, envía, confirma ni cierra respuestas.
- Los contratos y políticas del módulo de respuestas/Workflow permanecen compatibles; el tercero no queda como usuario responsable.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

No declarar completado este prompt si una subfase requerida conserva decisiones o verificaciones pendientes. Registrar qué se completó, qué falta y qué trabajo independiente puede continuar; no repetir permisos ni acuerdos ya registrados.

## PRUEBAS OBLIGATORIAS

### Subfase RI07

Ejecutar la caracterización anterior al cambio y repetirla después sobre los consumidores afectados, comparando datos, numeración, permisos, documentos y seguimiento. Pruebas transaccionales con DB local descartable y fallos justo antes/después de commit, concurrencia y configuración de consecutivos. Probar reutilización del resultado, payload en conflicto y atomicidad de relación de operación. Incluir el recorrido vigente de Producción y los otros consumidores realmente afectados por la extracción, no solo la entrada nueva. Integración autorizada con SELECT por referencia de operación; verificar documentos, registro y obligación únicos. Si falta evidencia o autoridad de ambiente, registrar el límite y no declarar segura/aceptada la integración.

### Subfase RI08

Inyectar fallos/timeout entre fases y después de sus efectos; reiniciar el proceso y consultar/recuperar. Probar descargas ajenas y expiración. E2E autorizada con carpeta/archivos/cuentas descartables y notificación en canal de prueba aprobado; no mandar correo real a terceros para completar evidencia.

### Subfase RI09

Pruebas focales de las dos columnas, consulta autorizada y datos/relaciones de entrega. Mantener tests/workflow-user-send.test.cjs y tests/workflow-group-send.test.cjs como regresión según impacto. E2E autorizada de radicación sin seguimiento y con cada modalidad hasta disponibilidad/lectura de la obligación en el módulo existente. No exigir elaborar, enviar o cerrar respuestas como aceptación de este alcance.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.

## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar el código de las subfases, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. Crear durante la ejecución `Doc/Diagnostico/radicacion-interna/implementacion/P05-resultado.md`, con estado y evidencia por subfase RI, y actualizar contratos/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. Publicar/desplegar solo si la autorización incluye el ambiente correspondiente. Este módulo no gestiona respuestas.
