# Prompt RI02 para operación durable, contexto y autoridad

Estado: preparado para refinamiento y ejecución del paquete, no implementado por redactar esta serie.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: RI00; RI01 para integrar apertura existente sin sus defectos. Puertas de refinamiento: G01, G03, G07, G08. Trazabilidad: RQ-RI-01, RQ-RI-02, RQ-RI-07, RQ-RI-09.

Al abrir este paquete, contrastar resultados anteriores y cerrar solo sus decisiones necesarias antes del código afectado. No exigir que toda la exploración esté aprobada para empezar análisis independiente. La instrucción de ejecutar el paquete autoriza el trabajo acordado, no elecciones funcionales todavía pendientes ni E2E del ambiente. No volver a pedir acuerdos ya registrados. Registrar los detalles técnicos y adaptar contrato/consumidores si cambia su revisión.

## OBJETIVO

Crear la operación propia y sus fronteras de autoridad para ambas entradas, con revisión y estado durable suficientes para concurrencia y recuperación.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Límite D-RI-03 confirmado: este módulo radica y crea/entrega el seguimiento; elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente. No trasladar esas funciones a esta pantalla ni rediseñar sus políticas.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

M01/M03/M08/M10 y N01/N02/N03/N12. Adaptar ButtonRadicar_Click sin alterar controles del árbol. Incorporar componentes acotados bajo Modelo/RadicacionInterna/, DTOs/RadicacionInterna/, Services/RadicacionInterna/ e Infrastructure/Repositories/RadicacionInterna/.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

## TRABAJO REQUERIDO

1. **RI02.1.** Implementar ContextAdapter que resuelve actor/empresa desde autenticación válida y autoriza operación/expediente/documento/plantilla por recurso. Mantener checks específicos de expediente abierto, nivel y radicación; no usar el remitente para elevar permisos.

2. **RI02.2.** Implementar AbrirDesdeProduccion y AbrirCargaDirecta con referencias normalizadas y operación durable. Para Producción capturar principal/anexos y revalidarlos, excluyendo principal duplicado. Para entrada nueva no exigir haber visitado el árbol ni heredar su sesión.

3. **RI02.3.** Entregar migración versionada del estado mínimo aprobado en RI00, índices de pertenencia/revisión y vínculo único de confirmación. Preparar aplicación y reversión antes de aplicar a un ambiente; no crear esquema desde Page_Load.

4. **RI02.4.** Implementar ObtenerAutorizada, ActualizarBorrador y PrepararConfirmacion con concurrencia optimista, revisión/configuración/manifiesto vigentes y errores de conflicto recuperables. Una lectura no confirma ni actualiza auditoría de dominio.

5. **RI02.5.** Implementar cancelación únicamente de borradores no registrados y política de caducidad acordada. Mantener referencias a efectos persistidos; ni cancelar ni cambiar origen puede borrar documentos incorporados.

6. **RI02.6.** Exponer los transportes mínimos de apertura/consulta/actualización y proteger petición/sesión según el patrón compatible del repo. Publicar referencias opacas y datos visibles; no SQL, rutas físicas, sesión completa ni excepciones crudas.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-01, RQ-RI-02, RQ-RI-07, RQ-RI-09.

## CRITERIOS DE ACEPTACIÓN

- Dos pestañas mantienen contextos independientes aunque compartan sesión.
- Modificar referencias o revisión no permite operar sobre recursos ajenos.
- Sesión nueva abre carga directa con configuración autorizada.
- Repetir apertura/cancelación según contrato no produce documentos ni consecutivos.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

## PRUEBAS OBLIGATORIAS

Pruebas de repositorio con revisiones en conflicto, revalidación de permisos, datos fuera de rango y cancelación concurrente. Verificar migración/índices en base local descartable y compilación. Integración autorizada de ambas aperturas sin escritura documental; no usar datos de otro módulo como autoridad.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.


## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar código del paquete, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. La parte que dependa de una decisión pendiente se identifica sin declararla concluida. Crear durante su ejecución `Doc/Diagnostico/radicacion-interna/implementacion/RI02-resultado.md` y actualizar contrato/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. No publicar ni modificar un ambiente que carezca de autorización.
