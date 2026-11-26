# Prompt RI03 para configuración de plantilla, destinos y reglas de respuesta

Estado: preparado para refinamiento y ejecución del paquete, no implementado por redactar esta serie.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: RI00, RI02. Puertas de refinamiento: G09/G10/G11 para las reglas afectadas; catálogo vigente autorizado. Trazabilidad: RQ-RI-04, RQ-RI-05, RQ-RI-06, RQ-RI-11.

Al abrir este paquete, contrastar resultados anteriores y cerrar solo sus decisiones necesarias antes del código afectado. No exigir que toda la exploración esté aprobada para empezar análisis independiente. La instrucción de ejecutar el paquete autoriza el trabajo acordado, no elecciones funcionales todavía pendientes ni E2E del ambiente. No volver a pedir acuerdos ya registrados. Registrar los detalles técnicos y adaptar contrato/consumidores si cambia su revisión.

## OBJETIVO

Resolver configuración real por identidad de trámite/plantilla, preservando campos dinámicos y separando seguimiento, modalidad, canal y plazo.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Límite D-RI-03 confirmado: este módulo radica y crea/entrega el seguimiento; elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente. No trasladar esas funciones a esta pantalla ni rediseñar sus políticas.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

M06–M15 y N04: Genera_Interface_Radicacion_Entrante, Lista_tipos_documentales_de_radicacion_interna, Seleccion_tipo_tramite/flujo/actividad, Class_tipo_doc_entrante y calendario. El catálogo clásico también es consumidor a preservar.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

## TRABAJO REQUERIDO

1. **RI03.1.** Extraer la preparación de campos y contexto documental para aceptar metadatos de un borrador sin consultar un ID de Producción inexistente. Conservar orden, tipo, scripts, catálogos, citas, valores y obligatoriedad de todas las plantillas utilizadas.

2. **RI03.2.** Implementar ObtenerConfiguracion y ResolverReglasRespuesta por ID con autoridad/plantilla. Mantener separados requiere_respuesta, obliga_respuesta_radicado_tramite y resp_correo_fisico_electronico. Corregir Retorna_estado_respuesta_obligatoria tras inventariar consumidores y conservar un adaptador compatible.

3. **RI03.3.** Mantener filtro de trámite interno y disponibilidad por empresa/plantilla. Identificar trámites clásicos que perderían elegibilidad al cambiar tipo_activo_rad_interno; no activar ni clonar configuración del ambiente sin decisión registrada.

4. **RI03.4.** Resolver ruta/destinos en servidor e invalidar las dependencias necesarias. Una ruta ambigua exige elección válida; no usar la primera opción como autorización o solución de configuración faltante.

5. **RI03.5.** Resolver plazo con DateAgregarLaborales y calendario vigente; decidir cero días antes de cambiar su rama. Preservar/informar cuándo la fecha es editable. Sin seguimiento no fabricar vencimiento de respuesta; no equiparar esa condición a un plazo cero.

6. **RI03.6.** Entregar configuración visible tipada, versión y requisitos de confirmación. Campos extra no se codifican como Placa fija ni se convierten todos en obligatorios por copiar asteriscos del HTML.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-04, RQ-RI-05, RQ-RI-06, RQ-RI-11.

## CRITERIOS DE ACEPTACIÓN

- Se distinguen sin seguimiento, confirmación y respuesta formal, incluso requiere_respuesta=1 con obliga_respuesta=0.
- Las otras radicaciones conservan modalidad/catálogo y contratos públicos.
- Trámite/grupo/flujo/actividad no reutilizan identidades incompatibles.
- Fecha, cita y campos adicionales conservan valores reales después de postback y cambio de origen.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

## PRUEBAS OBLIGATORIAS

Pruebas de comportamiento de lectores canónicos con matrices de ambas columnas, calendario, cero días y distintas plantillas. Integración de la corrección compartida en clásica/Simplificada/interna; un test de texto SQL no acredita el resultado funcional. Inventario de configuración con SELECT autorizado, sin activar datos como parte de una lectura.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.


## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar código del paquete, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. La parte que dependa de una decisión pendiente se identifica sin declararla concluida. Crear durante su ejecución `Doc/Diagnostico/radicacion-interna/implementacion/RI03-resultado.md` y actualizar contrato/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. No publicar ni modificar un ambiente que carezca de autorización.
