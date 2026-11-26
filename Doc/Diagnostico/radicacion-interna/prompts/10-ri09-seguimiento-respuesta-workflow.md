# Prompt RI09 para seguimiento y gestión de respuesta según trámite

Estado: preparado para refinamiento y ejecución del paquete, no implementado por redactar esta serie.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: RI03, RI04, RI07, RI08. Puertas de refinamiento: G05, G09, G10, G11. Trazabilidad: RQ-RI-03, RQ-RI-06, RQ-RI-08, RQ-RI-09, RQ-RI-11.

Al abrir este paquete, contrastar resultados anteriores y cerrar solo sus decisiones necesarias antes del código afectado. No exigir que toda la exploración esté aprobada para empezar análisis independiente. La instrucción de ejecutar el paquete autoriza el trabajo acordado, no elecciones funcionales todavía pendientes ni E2E del ambiente. No volver a pedir acuerdos ya registrados. Registrar los detalles técnicos y adaptar contrato/consumidores si cambia su revisión.

## OBJETIVO

Hacer operativa la obligación creada por el registro: bandeja, consulta, confirmación/respuesta formal y cierre compatibles con cada política de Workflow.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

M24–M26 y N11/N12; Class_ra_respuesta_radicado, Classgestionrespuesta, Class_system_plantilla_radicado y adaptadores de requisitos modernos de transición/envío a usuario/grupo.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

## TRABAJO REQUERIDO

1. **RI09.1.** Implementar ConsultarSeguimientoAutorizado usando referencia de operación/radicado/plantilla y propietario, evitando autorizar solo por consecutivo. Reflejar creado/pending/preparación incompleta con identificadores reales.

2. **RI09.2.** Integrar la aparición del seguimiento en la bandeja y su relación con tarea/radicado. Mantener al usuario interno asignado como propietario; tercero e identidad autenticada conservan sus roles respectivos.

3. **RI09.3.** Reutilizar confirmación y respuesta formal del motor tras resolver G09. Si se mantiene RADICACIONSALIENTE, validar plantilla predeterminada y homologación específica; no escoger silenciosamente el primer trámite de respuesta. Si se elige otro recorrido interno, implementarlo de extremo a extremo y ajustar salidas/pruebas.

4. **RI09.4.** Caracterizar y probar Verifica_respuesta_radicado_sin_respuesta para propietario y no propietario. Mantener coherencia al reasignar/devolver/cambiar grupo, con auditoría de los cambios necesarios.

5. **RI09.5.** Probar por separado terminación/conector, envío a usuario y envío a grupo. El adaptador de grupo tiene requisitos diferentes; aplicar la política acordada sin introducir un bloqueo universal ni quitar controles de respuesta/aprobación existentes.

6. **RI09.6.** Distinguir registro de origen completado de obligación pendiente, respuesta elaborada, confirmada y reversada. Adjuntar el principal o enviar un correo de apertura no da por contestada la solicitud.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-03, RQ-RI-06, RQ-RI-08, RQ-RI-09, RQ-RI-11.

## CRITERIOS DE ACEPTACIÓN

- Cada modalidad aparece y se gestiona en el recorrido real que corresponde.
- Propietario y bandeja siguen siendo coherentes después de transferencias.
- La respuesta formal utiliza una homologación revisada y genera la salida acordada.
- Los tipos de transición preservan su política y el remitente tercero no queda como usuario responsable.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

## PRUEBAS OBLIGATORIAS

Pruebas focales de las dos columnas, consulta autorizada y cambios de propietario. Mantener tests/workflow-user-send.test.cjs y tests/workflow-group-send.test.cjs. E2E autorizada de sin seguimiento, confirmación y respuesta formal hasta cierre y reversión aplicable, con aprobación/firma/canal real de prueba cuando la configuración lo exija.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.


## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar código del paquete, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. La parte que dependa de una decisión pendiente se identifica sin declararla concluida. Crear durante su ejecución `Doc/Diagnostico/radicacion-interna/implementacion/RI09-resultado.md` y actualizar contrato/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. No publicar ni modificar un ambiente que carezca de autorización.
