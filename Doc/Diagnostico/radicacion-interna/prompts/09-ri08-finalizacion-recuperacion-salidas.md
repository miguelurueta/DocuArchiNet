# Prompt RI08 para finalización, recuperación y salidas del radicado

Estado: preparado para refinamiento y ejecución del paquete, no implementado por redactar esta serie.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: RI06, RI07; contratos de RI03/RI04. Puertas de refinamiento: G05, G08 y política de reintento de efectos externos. Trazabilidad: RQ-RI-07, RQ-RI-08, RQ-RI-09, RQ-RI-11.

Al abrir este paquete, contrastar resultados anteriores y cerrar solo sus decisiones necesarias antes del código afectado. No exigir que toda la exploración esté aprobada para empezar análisis independiente. La instrucción de ejecutar el paquete autoriza el trabajo acordado, no elecciones funcionales todavía pendientes ni E2E del ambiente. No volver a pedir acuerdos ya registrados. Registrar los detalles técnicos y adaptar contrato/consumidores si cambia su revisión.

## OBJETIVO

Completar documentos, Workflow, notificación y salidas con resultado verificable, sin volver a crear el núcleo cuando un paso posterior falla.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

M19–M23 y N10. Revisar Registra_flujo_documento, Envia_corre_notificacion_radicado_envio_documento, generación/descarga de radicado y estados Flow actuales. La página usa hoy Hidden_ruta_archivo/iframe de descarga.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

## TRABAJO REQUERIDO

1. **RI08.1.** Implementar fases durables y reclamación de ejecución por operación para completar incorporación restante, vinculación/asignación Workflow y notificación. Concretar dependencia/orden de fases para no enviar una solicitud antes de tener documentos utilizables.

2. **RI08.2.** Correlacionar tarea, documentos y efecto de notificación con el radicado/operación. Consultar efectos ya confirmados antes de repetirlos. Si una API legacy devuelve solo YES, agregar evidencia o adaptador mínimo, no asumir que repetirla es seguro.

3. **RI08.3.** Implementar ConsultarResultado y RecuperarPendientes con autoridad específica, revisión y estados diferenciados. Las consultas son lecturas; la recuperación es una operación mutante explícita y nunca vuelve a llamar al registro completo.

4. **RI08.4.** Distinguir rechazo previo, registrado, pendiente documental/Workflow, correo pendiente y resultado incierto. Un timeout de correo con entrega desconocida no permite prometer exactamente una entrega; consultar proveedor o conservar pendiente para atención según política acordada.

5. **RI08.5.** Integrar descarga/impresión auténtica con autorización de operación/documento y salidas existentes; no aceptar rutas físicas del cliente ni sustituir el documento por el TXT de la maqueta.

6. **RI08.6.** Implementar limpieza/caducidad solo de temporales propios no vinculados. Conservar registro, archivos incorporados y resultado al cerrar/reanudar. Preparar recuperación tras reciclado del proceso sin depender de memoria de IIS.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-07, RQ-RI-08, RQ-RI-09, RQ-RI-11.

## CRITERIOS DE ACEPTACIÓN

- Un fallo posterior al commit conserva el radicado y sus identificadores.
- Recuperar dos veces no duplica documento, tarea ni efecto ya reconocido.
- Correo pendiente no anuncia rechazo del registro; incertidumbre no habilita otro registro.
- Descarga/impresión respetan autoridad y contenido; limpieza no borra documentos incorporados.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

## PRUEBAS OBLIGATORIAS

Inyectar fallos/timeout entre fases y después de sus efectos; reiniciar el proceso y consultar/recuperar. Probar descargas ajenas y expiración. E2E autorizada con carpeta/archivos/cuentas descartables y notificación en canal de prueba aprobado; no mandar correo real a terceros para completar evidencia.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.


## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar código del paquete, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. La parte que dependa de una decisión pendiente se identifica sin declararla concluida. Crear durante su ejecución `Doc/Diagnostico/radicacion-interna/implementacion/RI08-resultado.md` y actualizar contrato/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. No publicar ni modificar un ambiente que carezca de autorización.
