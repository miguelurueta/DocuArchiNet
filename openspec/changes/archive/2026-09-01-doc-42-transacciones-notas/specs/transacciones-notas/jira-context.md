# Jira Context - DOC-42

## Summary

TRANSACCIONES-NOTAS

## Description

> # Prompt 03 — Crear, editar y eliminar con integridad transaccional
> 
> ## Prompt para ejecutar
> 
> ```text
> Aplica primero el contexto común de Prompt/00-guia-de-uso-y-contexto-comun.md. Requiere las fases 01 y 02 revisadas.
> 
> Objetivo: implementar operaciones modernas de escritura de notas con autorización atómica, concurrencia, idempotencia y auditoría consistente. No migrar UI ni habilitar el flujo a usuarios en esta fase.
> 
> Rol esperado: arquitecto y desarrollador senior de ASP.NET Web Forms/VB.NET y MySQL, responsable de diseñar mutaciones transaccionales, reversibles y auditables sin exponer detalles de persistencia.
> 
> Contexto obligatorio: revisa los contratos aprobados de las fases 01 y 02, el gate de contexto, servicios y repositorios Workflow, la infraestructura ADO.NET, mecanismos de transacción y auditoría existentes. Ubica DTOs/modelos/interfaces en las rutas Workflow equivalentes existentes, servicios en `Services/Workflow/`, repositorios en `Infrastructure/Repositories/Workflow/` y transporte ASMX en `webservice/`; no traslada reglas de dominio a páginas `.aspx` o code-behind `.vb`.
> 
> Restricciones críticas:
> - No migrar UI, activar gates, retirar legacy, ejecutar E2E autenticada ni aplicar cambios de esquema contra ambientes reales sin autorización.
> - No usar tarea mutable de sesión, autorización visual, SQL concatenado ni una lectura previa de propietario seguida de una mutación no condicionada.
> - No inventar semántica de borrado, retención, privacidad o idempotencia cuando falta decisión aprobada; registra el bloqueo.
> - Preserva cambios ajenos, libera recursos y no deja auditoría, nota o transacción parcial ante error.
> 
> Pruebas obligatorias: agrega pruebas focales de servicio y repositorio para idempotencia, concurrencia, tareas/notas cruzadas, conflictos, auditoría y rollback transaccional. Integra en este mismo cambio las E2E de escritura autorizada, idempotencia, conflicto y concurrencia controlada que correspondan a los contratos expuestos. Ejecuta MSBuild o `dotnet` si se modifica código VB, y registra comandos, resultados y evidencia saneada; una validación manual solo complementa escenarios no automatizables.
> 
> Documentación técnica: actualiza propuesta OpenSpec, requerimientos y matriz de pruebas bajo `Doc/Actualizacion/workflow/Notas/` con la condición atómica de mutación, estrategia de idempotencia, transacción/auditoría, rutas modificadas, rollback y decisiones bloqueantes. No generes documentos en la raíz.
> 
> Entregable final: entrega las mutaciones condicionadas, contratos/resultados seguros, pruebas y evidencia de compilación, además de las decisiones de negocio pendientes y precondiciones de la fase 04.
> 
> Detalle de contrato: documenta para cada operación el request/response tipado, campos requeridos (`idTarea`, `idNota` cuando corresponda, versión y `clientRequestId`), resultado funcional, códigos seguros, reglas de validación y qué valores se derivan exclusivamente del contexto autorizado en servidor.
> 
> E2E integrada obligatoria: incorpora y cumple `bloque-e2e-integrado-en-modernizacion.md` en este cambio. Cubre autorización, escritura únicamente sobre tareas descartables autorizadas, idempotencia, conflicto de versión, concurrencia, regresión y auditoría con controles `SELECT` y evidencia saneada. Si falta autorización, datos, ambiente o configuración, registra el bloqueo; no simules la E2E ni habilites gates arbitrariamente.
> 
> Controles E2E obligatorios: la E2E es parte integral del mismo cambio y no una tarea o entrega independiente. Reutiliza exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de una E2E autenticada lee `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecútala solo con ambiente, cuentas y datos o tareas descartables expresamente autorizados. Usa secretos efímeros y no exponer, imprimir ni persistir credenciales, cookies, tokens ni cadenas de conexión; las verificaciones son solo `SELECT` y toda evidencia saneada. Cubre, cuando aplique, autorización y control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión. Respeta feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; la implementación no se considera terminada sin validación autorizada y registra bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.
> 
> Antes de escribir código, revisa DP-01, DP-03, DP-04, DP-05 y DP-07 del modelo de requerimientos. Si la semántica de borrado o una decisión indispensable sigue sin aprobarse, registra el bloqueo en la propuesta y no inventes comportamiento irreversible. No uses una migración de esquema hasta inspeccionar el esquema por consultas de solo lectura autorizadas y obtener aprobación para cambiarlo.
> 
> Alcance: RF-03, RF-05, RF-06, RF-10, RF-12, RF-15 a RF-18; RN-01 a RN-07, RN-11 a RN-14 y RN-17; RS-01 a RS-09; 11.3 a 11.5; RNF-08 y RNF-09.
> 
> 1. Implementa CrearNota con idTarea explícito, contenido y clientRequestId. El servidor obtiene actor, fecha, grupo y actividad desde el snapshot de la tarea autorizada; no desde el cliente ni desde una tarea guardada en sesión.
> 2. Implementa idempotencia: un mismo clientRequestId para la misma tarea y autor devuelve la misma nota y evita segunda auditoría. Define la garantía mediante restricción/diseño de persistencia sólo después de validar el esquema y decisión de retención.
> 3. Implementa ActualizarNota y EliminarNota con version esperada. La sentencia o unidad de persistencia debe condicionar en forma atómica idNota + idTarea + actor + estado aplicable + versión. No realices una consulta de propietario y luego un UPDATE/DELETE por idNota solamente.
> 4. Distingue resultados de manera segura: Forbidden, TaskNotActive, NoteNotFound, NotOwner, VersionConflict e InvalidContent. No reveles contenido ni existencia cuando el actor carece de acceso.
> 5. Construye la operación de dominio y auditoría en una única transacción. Usa Using/Try/Catch/Finally para conexión, comandos, lectores y transacción. Rechazos y excepciones no pueden dejar transacción, conexión, nota ni auditoría parcial.
> 6. Audita actor autenticado, tarea validada, nota, operación, correlación/clientRequestId, fecha de servidor, resultado y versión resultante. Para valores anterior/nuevo aplica la decisión aprobada de privacidad; nunca uses ciegamente contenido entregado por cliente como fuente de auditoría de borrado.
> 7. Valida contenido en servidor: obligatorio, máximo aprobado, Unicode, comillas y saltos de línea. Se persiste como texto plano y con parámetros.
> 8. Si es necesario modificar esquema o índices, crea una migración revisable y reversible, con nombre explícito, criterio de rollback y verificación no destructiva. No aplicar contra ambientes reales sin autorización.
> 9. Agrega pruebas de servicio y repositorio para doble clic/reintento, dos pestañas en tareas distintas, conflicto de versión, cambio de estado de tarea, nota cruzada, error de auditoría y salidas tempranas. Integra la E2E indicada, ejecútala solo con autorización y no habilites gates.
> 
> Fuera de alcance: representar las acciones en WebForms, cambiar el HTML aprobado, migrar los consumidores, retirar endpoints legacy y activar una bandera.
> 
> Criterios de aceptación:
> - Ninguna escritura moderna lee Session("ID_TAREA_SELECCIONDA").
> - Crear es idempotente y editar/eliminar son condicionales y atómicos.
> - Actividad y tarea auditada proceden del mismo contexto autorizado.
> - Fallas de auditoría, autorización o persistencia revierten por completo y liberan recursos.
> - La estrategia de borrado sigue una decisión explícita y todos los contratos la respetan.
> 
> Entrega el detalle de la condición de mutación, la estrategia de idempotencia, pruebas y cualquier decisión de negocio todavía bloqueante para la fase 04.
> ```

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: NOTAS, TRANSACCIONES
