# Jira Context - DOC-92

## Summary

SEGUNDO-FACTOR-PERSISTENCIA

## Description

> # 02 — Persistencia transaccional del challenge
> 
> ## ROL ESPERADO
> 
> Actúa como arquitecto de datos y desarrollador senior MySQL/VB.NET.
> 
> ## OBJETIVO
> 
> Implementar, en el OpenSpec independiente de la Jira actual, el endurecimiento compatible de `docuarchi.ra_auth_second_factor_challenge` y el repositorio atómico requerido por Login 2FA.
> 
> ## CONTEXTO
> 
> Lee la exploración cerrada y verifica la fundación entregada por la tarea 01. Antes de escribir DDL/SQL, confirma nombres y tipos mediante artefactos versionados o una consulta `information_schema` de solo lectura expresamente autorizada. No reutilices una autorización antigua.
> 
> ## PRECONDICIONES DE RUTAS
> 
> Inventaría primero en `design.md` los archivos exactos, clases, interfaces implementadas y firmas. Ubica:
> 
> - implementaciones MySQL exclusivamente en `Infrastructure/Repositories/Login/SegundoFactor/`;
> - extensiones indispensables de contratos únicamente en `Modelo/Login/SegundoFactor/`;
> - preflight, apply, rollback y README SQL en `Doc/Actualizacion/Login/Implementacion/<JIRA>/Sql/`;
> - pruebas en `tests/` y harness/validación estructural en `tools/validation/` cuando corresponda;
> - altas de `.vb` en `GestionDocumental-Docuarchi.net.vbproj`.
> - `webservice/WebServiceLoginSegundoFactor.asmx(.vb)` queda expresamente fuera del alcance de esta tarea de persistencia.
> 
> No colocar repositorios en `Services`, SQL en `webservice` o Session/`HttpContext` en Infrastructure. No crear una carpeta global de migraciones ajena al paquete Jira.
> 
> ## REQUISITOS POSITIVOS Y CONTRATOS
> 
> 1. Crear migración idempotente, verificación previa/posterior y rollback documentado. Las extensiones serán aditivas y compatibles con los consumidores actuales del Core.
> 2. Incorporar columnas para propósito, vínculo de sesión no reversible, estado, identificador de llave HMAC, conteo/último envío, fechas terminales/actualización y versión de esquema. Los nuevos registros dejan `AuthPayloadJson = NULL`.
> 3. Incorporar índices que soporten: identidad canónica + propósito + estado; vínculo de sesión + estado; y limpieza por estado/expiración. No eliminar `uq_challengeid` ni `IX_ra_auth_sfc_authuserid` sin evidencia y migración coordinada.
> 4. Implementar el repositorio con `IDataExecutor`/`ITransactionFactory` y SQL parametrizado. No leer `HttpContext` o `Session` dentro de Infrastructure.
> 5. Implementar operaciones transaccionales para crear, marcar enviado/fallo, adquirir verificación `SENT -> FINALIZING`, incrementar intento/bloquear, completar/fallar finalización, revocar al reenviar y expirar.
> 6. Aplicar bloqueo de fila (`SELECT ... FOR UPDATE` o mecanismo equivalente demostrado) y condiciones de estado en cada `UPDATE`. Una sola solicitud puede adquirir `FINALIZING`.
> 7. Definir consulta/DDL de limpieza de estados terminales con retención de 30 días, sin crear scheduler oculto ni ejecutarla automáticamente.
> 
> ## Reglas de datos
> 
> - `AuthUserId` nuevo: `empresa:módulo:tipo:idInterno`; no login ni correo.
> - Registros legacy con propósito/estado nulos nunca son elegibles para el flujo nuevo.
> - `CodeHash` activo no puede ser nulo desde Application; una incompatibilidad con filas legacy se resuelve con condición/versionado, no con una restricción destructiva improvisada.
> - El intento incorrecto incrementa una sola vez y el quinto cambia a `BLOCKED` en la misma transacción.
> - El código correcto cambia a `FINALIZING`; `Consumed = 1` solo en `COMPLETED`.
> - Reenvío autorizado revoca el challenge anterior antes de dejar elegible el nuevo.
> 
> ## RESTRICCIONES CRITICAS
> 
> - No ejecutar migraciones ni limpieza en ambientes compartidos sin autorización explícita actual.
> - No cambiar datos de módulos, usuarios, tareas, auditoría o configuración SMTP.
> - No persistir contraseña, OTP, nonce crudo, SessionID, correo, conexión o payload de autenticación.
> - No usar concatenación SQL ni el helper legacy `conect` para el repositorio nuevo.
> 
> ## PRUEBAS OBLIGATORIAS
> 
> Cubre migración desde el esquema inspeccionado, reaplicación idempotente, convivencia con una fila legacy, cada transición válida/inválida, expiración, cinco intentos, reenvío, aislamiento de identidades iguales entre empresas/módulos y carreras con dos verificaciones simultáneas donde exactamente una adquiere `FINALIZING`.
> 
> Las pruebas unitarias no reemplazan la integración transaccional. E2E no aplica en esta tarea: es solo infraestructura, sin recorrido de usuario ni endpoint; valida el límite Repository con integración real. Si se autoriza un esquema descartable, ejecútala allí y registra solo evidencia saneada; si no, deja el caso automatizado y el bloqueo explícito. Ejecuta MSBuild y las regresiones de la tarea 01.
> 
> ## CRITERIOS DE ACEPTACION
> 
> - DDL, rollback y repositorio son versionados, parametrizados y compatibles.
> - La concurrencia queda demostrada con una prueba real o marcada honestamente como bloqueo pendiente.
> - No se tocó ningún dato real sin autorización.
> - El OpenSpec de esta Jira traza `D-XX`/`RQ-XX` a migración, métodos y pruebas.
> 
> ## DOCUMENTACION TECNICA
> 
> Actualiza documentación existente de esquema y estados; registra contrato de cada método, parámetros, retorno, transacción y transición. Conserva evidencia en `Doc/Actualizacion/Login/Implementacion/<JIRA>/`.
> 
> ## ENTREGABLE FINAL
> 
> Entrega archivos, DDL/rollback, firmas del repositorio, pruebas unitarias e integración con rutas/casos, comandos, códigos de salida, plan de despliegue y riesgo residual.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DOBLE, FACTOR, PERSISTENCIA
