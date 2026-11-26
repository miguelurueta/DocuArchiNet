# Prompt RI07 para núcleo de registro e idempotencia de confirmación

Estado: preparado para refinamiento y ejecución del paquete, no implementado por redactar esta serie.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior especialista en ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, JavaScript/jQuery y MySQL. Conserva contratos y datos existentes y produce una entrega comprobable del alcance de este paquete.

## CONTEXTO Y DEPENDENCIAS

Leer [contexto común](contexto-comun.md), [guía](00-guia-ejecucion.md) y [mapa de funciones](../mapa-funciones-implementacion.md). Dependencias: RI02, RI03, RI04, RI06. Puertas de refinamiento: G01 y las decisiones ya consumidas por identidad, plantilla y documentos. Trazabilidad: RQ-RI-03, RQ-RI-06, RQ-RI-07, RQ-RI-08, RQ-RI-09.

Al abrir este paquete, contrastar resultados anteriores y cerrar solo sus decisiones necesarias antes del código afectado. No exigir que toda la exploración esté aprobada para empezar análisis independiente. La instrucción de ejecutar el paquete autoriza el trabajo acordado, no elecciones funcionales todavía pendientes ni E2E del ambiente. No volver a pedir acuerdos ya registrados. Registrar los detalles técnicos y adaptar contrato/consumidores si cambia su revisión.

## OBJETIVO

Extraer un núcleo transaccional comprobable que produzca un único radicado y seguimiento para la misma confirmación, preservando numeración y datos actuales.

## RESTRICCIONES CRÍTICAS

- Leer el contexto común y el código vigente; actuar solo en el alcance refinado. Las funciones N son propuestas y las M existen en el código; no confundir sus estados ni inventar su implementación.
- Mantener WebForms/VB.NET/.NET Framework 4.6.1, plantillas dinámicas y contratos de Producción, clásica, Simplificada y Workflow. No modificar el OpenSpec DOC-127 ni su trabajo concurrente.
- Límite D-RI-03 confirmado: este módulo radica y crea/entrega el seguimiento; elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente. No trasladar esas funciones a esta pantalla ni rediseñar sus políticas.
- Actor/empresa/permisos provienen de autoridad autenticada. El remitente no sustituye al ejecutor ni al responsable; IDs visibles, campos ocultos, rutas y nombres de tabla del cliente no son autoridad.
- No repetir una escritura incierta ni anunciar éxito global por HTTP 200/YES. Mantener revisión, correlación, resultado durable y compatibilidad con los efectos ya confirmados.
- No ejecutar E2E real ni carga ni mutaciones del ambiente sin autorización específica. No guardar secretos ni añadir activación por usuarios/grupos o `WorkflowCentroTrabajoModernActive`.

## FUNCIONES Y ARCHIVOS AFECTADOS

M05/M18 y N03/N09/N11. Punto principal: ClassRadicador.Registra_Radicacion_entrante_interna, reserva FOR UPDATE alrededor de 2700 y transacción hasta Commit. Inventariar dependencias que abren otra conexión.

## PRECONDICIONES DE RUTAS

Antes de editar, inventariar archivos exactos, clases, funciones y firmas en el contrato de RI00, junto con sus consumidores y dependencias. Reutilizar las ubicaciones existentes y crear solo responsabilidades justificadas: modelos/puertos en `Modelo/RadicacionInterna/`, DTOs en `DTOs/RadicacionInterna/`, servicios en `Services/RadicacionInterna/`, repositorios en `Infrastructure/Repositories/RadicacionInterna/` y adaptadores de Session/IO en `Infrastructure/RadicacionInterna/`. Servicio web si hace falta en `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`; upload propio en `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Registrar explícitamente cada alta Compile/Content en `GestionDocumental-Docuarchi.net.vbproj`; no crear capas vacías ni una API por cada función interna.

## TRABAJO REQUERIDO

1. **RI07.1.** Separar adaptación WebForms, validación y datos de una orden tipada. Revalidar autoridad, remitente, destino, plantilla/configuración y manifiesto exacto inmediatamente antes de la mutación. Preparar revisión no concede permiso permanente.

2. **RI07.2.** Implementar ConfirmarOperacion y reclamación durable por operación/clave/revisión. Igual confirmación devuelve el resultado conocido; clave con payload/versión distintos produce conflicto. Dos peticiones concurrentes no ejecutan el núcleo dos veces.

3. **RI07.3.** Extraer RegistrarNucleo con una unidad de trabajo explícita. Mantener SELECT FOR UPDATE, actualización de consecutivo general o de plantilla y formatos corto/simple/código de barras; no reservar el consecutivo fuera de la transacción.

4. **RI07.4.** Insertar datos de plantilla, relaciones documentales de la alternativa elegida, registro general, solicitud de respuesta y auditoría con actor/propietario correctos. Crear seguimiento solo según reglas canónicas de RI03 y sin doble solicitud para la operación.

5. **RI07.5.** Enlazar operación y radicado confirmado en la misma transacción del núcleo cuando comparten recurso transaccional; registrar identificadores necesarios para RI08. Si otras conexiones impiden esa garantía, resolver el límite explícitamente y no declarar atomicidad global.

6. **RI07.6.** Preservar wrapper legacy o migrar todos sus consumidores comprobados. Retirar de la ruta nueva la repetición del método monolítico completo como recuperación. Un fallo después de Commit devuelve registrado con pendientes o incertidumbre consultable.

Relacionar cada tarea con las funciones/rutas de este paquete y con su caso de prueba en el resultado de ejecución. Si al contrastar el código una extracción exige más de una entrega revisable, descomponerla antes de editar; conservar la trazabilidad RQ-RI-03, RQ-RI-06, RQ-RI-07, RQ-RI-08, RQ-RI-09.

## CRITERIOS DE ACEPTACIÓN

- Doble clic y dos peticiones concurrentes producen un solo radicado/obligación.
- Se conservan todos los formatos y ámbitos de consecutivo frente a consumidores anteriores.
- Rollback anterior al commit no se confunde con un registro confirmado.
- Respuesta perdida alrededor del commit se resuelve consultando la asociación durable, sin otro consecutivo.
- Cambios y evidencia corresponden al alcance, con consumidor/versiones trazables; no hay una dependencia pendiente presentada como implementada.

## PRUEBAS OBLIGATORIAS

Pruebas transaccionales con DB local descartable y fallos justo antes/después de commit, concurrencia y configuración de consecutivos. Probar reutilización del resultado, payload en conflicto y atomicidad de relación de operación. Integración autorizada con SELECT por referencia de operación; verificar documentos, registro y obligación únicos.

Ejecutar pruebas unitarias/focales de comportamiento real, integración de límites afectados y compilación MSBuild compatible con .NET Framework 4.6.1 si cambia VB.NET/proyecto. Registrar comando, resultado y límite; existencia de archivos o coincidencias de texto no acreditan dominio. Comprobar las suites candidatas antes de usarlas.

E2E o validación manual reproducible con evidencia forman parte integral del mismo cambio funcional y de su aceptación, no una entrega futura que permita declararlo terminado. Reutilizar la infraestructura existente de `tools/e2e`, autenticación y configuración. No crear login, arnés, proyecto Playwright ni `.env` paralelos. Antes de autenticar leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos descartables expresamente autorizados.

Usar secretos efímeros; no imprimir, guardar ni persistir credenciales, cookies, tokens o cadenas de conexión. Los controles serán solo SELECT y la evidencia saneada excluirá datos personales/documentales sensibles. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada según el alcance. No habilitar arbitrariamente flags o gates existentes. No cerrar la aceptación sin validación autorizada: registrar bloqueo explícito de la aceptación integrada pendiente y evidencia local real, sin mocks, simulaciones ni evidencia ficticia para acreditar el ambiente.


## DOCUMENTACIÓN TÉCNICA

Ubicar y actualizar documentación existente de esta capacidad y sus consumidores afectados; si falta un documento necesario, registrar su ruta y crearlo dentro del alcance. Actualizar el resultado del paquete y los contratos afectados: función anterior/nueva, ruta, entrada/retorno, autoridad, efectos, errores, consumidores y pruebas. Documentar la secuencia funcional paso a paso de éxito, rechazo y recuperación del paquete, con estados, límites de transacción y efectos comprobables. Documentar esquema/configuración/publicación cuando se toquen; no guardar datos de ejecución sensibles ni copiar documentación de otras capacidades como evidencia propia. Mantener el registro de decisiones en las exploraciones y evitar contradicciones entre paquetes.

## ENTREGABLE FINAL

Entregar código del paquete, altas de proyecto/migración justificadas y pruebas de comportamiento con evidencia y límites. La parte que dependa de una decisión pendiente se identifica sin declararla concluida. Crear durante su ejecución `Doc/Diagnostico/radicacion-interna/implementacion/RI07-resultado.md` y actualizar contrato/guía. Conservar trabajo ajeno, verificar diff y registrar únicamente la entrega autorizada en Git; comunicar sincronización remota. No publicar ni modificar un ambiente que carezca de autorización.
