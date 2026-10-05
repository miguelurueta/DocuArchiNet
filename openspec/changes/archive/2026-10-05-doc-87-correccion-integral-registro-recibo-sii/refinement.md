<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-87-correccion-integral-registro-recibo-sii

## Fuente y alcance

- Ticket: `DOC-87` — CORRECCION-INTEGRAL-REGISTRO-RECIBO-SII
- Cambio OpenSpec: `doc-87-correccion-integral-registro-recibo-sii`
- Fuente Jira: `specs/correccion-integral-registro-recibo-sii/jira-context.md`
- Perfil tecnológico comprobado: ASP.NET Web Forms, VB.NET/.NET Framework, JavaScript legacy, ASMX y MySQL.
- Alcance: operación Registrar tarea para ruta; quedan fuera los otros formularios Workflow, importaciones, adjuntos y handlers compartidos.

## Contexto inspeccionado

- `workflow/WebFormGestionFlujoTrabajoCamaras.aspx`: seis controles `.conten_registro_ruta` carecen de `atrib_campo_beetwen`; los formularios vecinos usan otros espacios.
- `js/workflow/WebFormGestionFlujoTrabajoCamaras.js`: `Button_registro_actividad_ruta`, `zeroFillFReciboSII`, consulta SII y registro ASMX muestran estado mutable, fallback de trámite y desreferencias directas.
- `js/java_general/general_control_java.js`: el validador compartido exige el atributo; se preserva sin cambios.
- `webservice/WebServiceWorkflow.asmx.vb`: el endpoint mutante deserializa un arreglo genérico, no autoriza server-side y expone `ex.Message`.
- `workflow/ClassGestionTareasFlujoTrabajo.vb`: actividad `0`, consultas/insert concatenados y transacción posterior a la relación expediente-radicado.
- `workflow/Class_Listado_Actividades_workflow.vb`: la opción vacía usa valor `0`.
- `workflow/Class_permisos_usuarios_workflow.vb`: existe el permiso `UTIL_SII_REGISTRO_TAREA_RUTA` reutilizable desde sesión.
- `Gestion/ClassRaRelacionRadicadoExternoExpediente.vb`: la relación se consulta e inserta con conexión propia fuera de la transacción de tarea.
- `Integracionccv/Class_ConSultaRecibo.vb` y `Class_ws_tipotramitesii_determina_gabinete.vb`: existen las fuentes server-side para reconstruir recibo y trámite.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Completar el atributo obligatorio en los seis controles de ruta y, por ampliación explícita, en los ocho controles de flujo y ocho de RUE/virtual; preservar el validador compartido | `workflow/WebFormGestionFlujoTrabajoCamaras.aspx`; `js/java_general/general_control_java.js` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Usar canonizador puro específico de ruta con prefijo S/R y nueve dígitos | `js/workflow/WebFormGestionFlujoTrabajoCamaras.js::zeroFillFReciboSII` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Mantener instantánea autoritativa, invalidarla ante edición y resolver el tipo efectivo con subtipo o tipo del recibo | `registro-tarea-ruta-sii.js`; `Service_REST_solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Conservar URL ASMX, adaptar a comando mínimo, autorizar por sesión y transportar recibo/radicado para revalidar la misma fuente en servidor | `Service_registro_tarea_ruta_sii`; `ConsultaAutoritativaRegistroRutaSii`; `Class_ConSultaRecibo` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Confirmar tarea y outbox en Workflow, detectar recibos históricos en la tabla autoritativa, materializar la relación idempotente y serializar por ruta/recibo | repositorios, consulta previa parametrizada y despachador específicos DOC-87; APIs legacy intactas | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Versionar recursos y exigir pruebas focales, regresión y E2E autorizada con política de gate | `WebFormGestionFlujoTrabajoCamaras.aspx`; `tests/`; `tools/e2e/AGENT-RUNBOOK.md` | D-06 | RQ-06 | Origen: D-06, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Los seis controles se validan sin excepción | WHEN se acepta el formulario THEN se serializan exactamente seis campos | No tocar validador ni formularios vecinos |
| RQ-02 | Cada recibo aceptado queda como S/R más nueve dígitos | WHEN se canoniza una entrada válida THEN la salida es idempotente; entradas ambiguas se rechazan | Aplicar solo a ruta |
| RQ-03 | UI y consulta confirmada no pueden divergir; matrícula y subtipo son opcionales | WHEN cambia recibo/prefijo o faltan radicado/razón social THEN se invalida; WHEN falta subtipo THEN se usa el tipo del recibo y se exige coincidencia única normalizada | No confiar en controles disabled ni seleccionar por posición |
| RQ-04 | Solo sesión autorizada registra datos reconstruidos | WHEN falta sesión/permiso o se manipula payload THEN se rechaza sin escribir | Mantener URL ASMX y compatibilidad de entrada acotada |
| RQ-05 | No existen SQL de valores concatenados, duplicados de recibo ni trabajo remoto perdido | WHEN el recibo ya está en `F_W_E_REGISTROPUBLICO` se rechaza; WHEN falla Workflow hay rollback local; WHEN falla Docuarchi queda outbox reintentable | Consulta previa parametrizada bajo bloqueo; validar tabla dinámica; APIs legacy intactas |
| RQ-06 | Éxito visible y no regresión demostrada | WHEN suites/compilación/E2E autorizada concluyen THEN existe evidencia saneada | E2E queda bloqueada sin autorización; gate siempre false |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` aparece en `design.md`, `spec.md` y al menos una tarea con `Origen: D-XX, RQ-XX`.
2. Ninguna tarea puede ampliar el alcance a funciones compartidas sin prueba de consumidores y decisión separada.
3. La documentación y la evidencia no pueden contener credenciales, cookies, conexiones, respuestas SII completas ni datos personales.
4. La aprobación de este refinamiento autoriza planificación e implementación local; no autoriza E2E real ni cambios de esquema.

## Resultado del refinamiento

- Estado: aprobado para planificación e implementación local.
- E2E real: condicionada a autorización explícita y al runbook.
- Cambio de esquema: fuera de alcance; cualquier necesidad detiene la implementación para nueva aprobación.
