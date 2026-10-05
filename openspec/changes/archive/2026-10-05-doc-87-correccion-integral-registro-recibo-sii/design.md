<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Context

La operación **Registrar tarea para ruta** vive en `workflow/WebFormGestionFlujoTrabajoCamaras.aspx` y usa `js/workflow/WebFormGestionFlujoTrabajoCamaras.js`, `WebServiceWorkflow.asmx` y `ClassGestionTareasFlujoTrabajo`. El recorrido actual falla antes de llamar al ASMX porque los seis controles `.conten_registro_ruta` no incluyen `atrib_campo_beetwen`, atributo que `valida_solicita_datos_control_general_async` desreferencia sin comprobar.

La inspección también confirma riesgos posteriores: la actividad vacía usa valor `0`; `zeroFillFReciboSII` acepta prefijos y longitudes inválidos; el cliente conserva datos derivados después de editar el recibo; la coincidencia de trámite tiene fallback implícito; el endpoint mutante confía en un arreglo genérico del navegador, no comprueba el permiso en servidor y devuelve excepciones; `Registra_tarea_ruta_SII` crea la relación expediente-radicado antes de la transacción; y `Registra_flujo_workflow_externo` concatena datos en SQL.

El cambio funcional continúa centrado en registro de tarea para ruta. Una ampliación explícita posterior corrige el mismo atributo contractual ausente en Registrar tarea para flujo, RUE y virtuales, sin cambiar sus nombres, obligatoriedad, eventos ni serialización. Se preservan importación SII, Radicación Simplificada, adjuntos, `FileUploadHandler_.ashx` y el validador compartido.

## Goals / Non-Goals

### Goals

- Restablecer el contrato HTML local y eliminar la excepción sin relajar código compartido.
- Tratar la última consulta SII válida como contexto autoritativo en cliente y revalidarla en servidor.
- Exigir una actividad positiva y resolver una coincidencia única de trámite usando primero el subtipo del radicado y, cuando venga vacío, el tipo del recibo.
- Autorizar la mutación desde sesión y permiso `UTIL_SII_REGISTRO_TAREA_RUTA`.
- Parametrizar la persistencia y garantizar que ninguna relación quede perdida aunque Workflow y Docuarchi usen bases distintas.
- Evitar duplicados concurrentes mediante una migración versionada y reversible de outbox aprobada.
- Entregar pruebas focales, de regresión y E2E real únicamente con autorización explícita.

### Non-Goals

- Cambiar `valida_solicita_datos_control_general_async` o el contrato de otros formularios.
- Modernizar todas las consultas SQL legacy del módulo.
- Cambiar importación de sellos/ENLASE, adjuntos o el handler compartido.
- Introducir recargas, postbacks, gates nuevos o cambios de esquema fuera de la migración DOC-87 aprobada.
- Ejecutar E2E autenticada o escrituras reales sin autorización del ambiente y los datos.

## Decisions

### D-01 — Completar el contrato HTML en los tres espacios afectados

Los seis controles serializados de `conten_registro_ruta` (`recibo`, `codigo_barras`, `matricula`, `rscocial`, `id_tramite`, `id_actividad`), los ocho controles de `conten_registro_flujo` y los ocho controles de `conten_registro_flujo_tarea_sii` declararán `atrib_campo_beetwen="0"`. No se modificará `general_control_java.js`. Una prueba estructural verificará los conjuntos exactos y que ningún otro atributo de esos controles cambie.

### D-02 — Canonizar el recibo con una función pura específica de ruta

Se reemplazará el uso de `zeroFillFReciboSII` en esta operación por un canonizador específico que reciba prefijo y valor, aplique `trim`, normalice el prefijo permitido y acepte exclusivamente un consecutivo numérico de uno a nueve dígitos o un valor ya canónico de prefijo más nueve dígitos. La salida será siempre prefijo `S` o `R` más nueve dígitos y será idempotente. Los demás consumidores de `zeroFillFReciboSII` quedan intactos.

### D-03 — Separar el contexto consultado de la presentación

El JavaScript mantendrá un único estado privado de consulta con recibo canónico, código de barras/radicado, matrícula opcional, razón social, identificadores permitidos, tipo efectivo de trámite y bandera de vigencia. Cambiar el recibo o prefijo invalidará el estado y limpiará campos derivados. La respuesta SII solo se aceptará si tiene un elemento con radicado y razón social, éxito funcional y catálogo de actividades utilizable. El tipo efectivo será el `subtipotramite` del radicado cuando esté informado y, de lo contrario, el `tipotramite` del recibo. Su comparación con el catálogo ignorará espacios laterales y diferencias de mayúsculas, pero exigirá exactamente una coincidencia; nunca seleccionará la primera opción por posición. Todos los callbacks resolverán controladamente y el bloque `finally` restaurará la interfaz. El registro comparará el recibo visible canonizado con el contexto antes de invocar el ASMX.

### D-04 — Mantener el endpoint y aislar un comando tipado y autorizado

`Service_registro_tarea_ruta_sii` conservará su URL para limitar impacto, pero su adaptador aceptará el comando de ruta con `recibo`, `id_tramite` e `id_actividad`; durante la transición podrá extraer esos tres campos del arreglo legacy. Código de barras, matrícula y razón social nunca serán autoridad del navegador. Matrícula y subtipo pueden venir vacíos desde SII: la matrícula se conserva opcional y el trámite se valida server-side contra el mismo tipo efectivo usado por el cliente (`subtipotramite` no vacío o, como respaldo, `tipotramite` del recibo). La consulta autoritativa transportará ambos objetos SII hasta el repositorio para evitar fuentes divergentes. El servidor obtendrá el usuario de `Session("Id_Usuario_Workflow")`, comprobará `UTIL_SII_REGISTRO_TAREA_RUTA`, rechazará identificadores no positivos, volverá a consultar el recibo SII y resolverá trámite, gabinete, ruta y sede desde catálogos server-side. Las respuestas públicas usarán códigos/mensajes saneados y las excepciones completas no cruzarán el ASMX.

### D-05 — Persistir mediante transacción local y outbox idempotente entre bases

La exploración confirmó dos recursos transaccionales distintos: la tarea usa `Dbase_Conction_Mysql` (Workflow) y la relación expediente-radicado usa `Dbase_Conction_Mysql_DA` (Docuarchi). No se fingirá atomicidad distribuida. En una transacción local de Workflow se confirmarán registro público, inicio, datos adicionales, estado, log aplicable y un evento duradero en `workflow_registro_ruta_sii_outbox`. Solo después del commit, un despachador específico intentará materializar idempotentemente la relación en Docuarchi y confirmará el evento en Workflow. Una falla remota conservará el evento reintentable y devolverá el estado público `REGISTERED_RELATION_PENDING`; nunca se declarará relación confirmada si no existe.

Todos los valores irán por `MySqlParameter`; el único identificador dinámico de tabla procederá de la ruta resuelta en servidor y deberá satisfacer un patrón cerrado. El repositorio adquirirá un bloqueo MySQL nombrado por ruta/recibo, comprobará primero el outbox y también consultará la tabla autoritativa `F_W_E_REGISTROPUBLICO` dentro de la sección, y liberará el bloqueo en `Finally`. Si el recibo ya existe responderá `ALREADY_REGISTERED` sin insertar. Así detectará tanto registros DOC-87 como recibos históricos o creados por consumidores legacy sin modificar el esquema de `F_W_E_REGISTROPUBLICO`. Un reintento con evento pendiente podrá reconciliar la relación, pero conservará un código público de recibo ya registrado y nunca informará un alta nueva. Las APIs legacy compartidas permanecerán intactas y ningún consumidor vecino será redirigido.

### D-06 — Liberar con evidencia focal y compatibilidad observable

Se agregarán pruebas que ejecuten las funciones reales del cliente y un harness VB.NET para autorización, reconstrucción autoritativa, parametrización, rollback y concurrencia. La página versionará solo los recursos JavaScript modificados y mostrará confirmación inequívoca de éxito, con protección de doble envío. La E2E real reutilizará `tools/e2e` y quedará formalmente bloqueada si no hay autorización, cuenta y recibo descartable. Al finalizar cualquier ejecución autorizada, `WorkflowCentroTrabajoModernActive` permanecerá en `false`, con listas de usuarios y grupos vacías.

## Risks / Trade-offs

- La reconsulta SII en la mutación agrega latencia y dependencia externa, pero evita registrar datos manipulados u obsoletos. El cliente debe mostrar un error recuperable sin escribir.
- El bloqueo nombrado depende de una conexión MySQL viva; debe liberarse en `Finally` y usar timeout acotado. Si el motor no lo soporta, la operación falla cerrada.
- La consistencia entre bases es eventual: la tarea puede quedar confirmada con relación pendiente. El outbox conserva el trabajo, permite reintento idempotente y hace observable el estado sin compensaciones destructivas.
- La frontera específica agrega clases, pero evita alterar `Registra_flujo_workflow_externo`, utilizado por otros recorridos. Los archivos nuevos deberán incluirse en el `.vbproj`.
- Mantener compatibilidad temporal con el arreglo legacy amplía el adaptador ASMX, no el dominio. Las pruebas deben demostrar que los campos derivados recibidos son ignorados.
- La prueba E2E muta datos reales; por política solo puede ejecutarse con autorización explícita y evidencia mediante `SELECT`.

## Migration Plan

1. Incorporar pruebas de caracterización y contrato antes del cambio funcional.
2. Aplicar el contrato HTML local y el canonizador/estado específico de ruta.
3. Aplicar la migración versionada de outbox en Workflow y ejecutar su preflight de solo lectura.
4. Introducir modelos, servicio, repositorios y despachador específicos; adaptar el endpoint existente.
5. Versionar recursos y ejecutar suites focales, regresión y compilación.
6. Desplegar sin gate adicional. Para rollback, detener primero la versión aplicativa; el SQL solo elimina la tabla si no contiene eventos.
7. Ejecutar E2E real solo después de leer el runbook y recibir autorización explícita; registrar bloqueo si faltan prerrequisitos.

## Open Questions

No quedan decisiones funcionales abiertas: el usuario aprobó la alternativa de outbox versionado al confirmarse que Workflow y Docuarchi tienen conexiones independientes. La compatibilidad del motor y la disponibilidad de datos E2E son verificaciones de preflight; una falla bloquea la etapa correspondiente y no autoriza una alternativa insegura.
