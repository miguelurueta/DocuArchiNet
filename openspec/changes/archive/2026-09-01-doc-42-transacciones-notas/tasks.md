<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## 1. Fundamentos y contratos

- [x] 1.1 Verificar base de DTOs, modelos, puertos y lecturas DOC-41; añadir métodos ASMX de escritura sin modificar WebForms, consumidores legacy ni gates. Origen: D-01, RQ-01
- [x] 1.2 Completar mapeo DTO/modelo/resultado de creación, actualización y eliminación, sin aceptar identidad, grupo, ruta, actividad ni fecha del cliente. Origen: D-01, RQ-01
- [x] 1.3 Validar `idSolicitudCliente` como UUID y conservar validaciones de contenido BMP, NUL, longitud y versión en servicio. Origen: D-03, RQ-02

## 2. Persistencia transaccional

- [x] 2.1 Diseñar e implementar preflight no destructivo de motor, columnas, índices y almacenamiento de idempotencia, con retorno `Unavailable` sin intento de escritura. Origen: D-06, RQ-05
- [x] 2.2 Preparar migración por esquema revisable y reversible para InnoDB, índices y tabla de idempotencia; no aplicarla sin inspección `SELECT` y autorización explícita. Origen: D-06, RQ-05
- [x] 2.3 Implementar reserva idempotente única por tarea, autor y UUID, creación parametrizada y recuperación de respuesta original durante 30 días en una transacción. Origen: D-03, RQ-02
- [x] 2.4 Implementar ETag SHA-256 canónico y `UPDATE` condicionado por nota, tarea, propietario, estado y versión, sin patrón comprobar-y-mutuar separado. Origen: D-02, RQ-03
- [x] 2.5 Implementar `DELETE` físico condicionado por nota, tarea, propietario, estado y versión, sin recuperación ni exposición posterior de contenido. Origen: D-04, RQ-03
- [x] 2.6 Registrar auditoría de metadatos, longitudes y SHA-256 en misma transacción que cada mutación, con rollback y liberación determinista ante excepción. Origen: D-05, RQ-04

## 3. Pruebas y documentación

- [x] 3.1 Agregar pruebas de servicio para contexto ausente, tarea/ruta inválida, contenido/UUID inválido y ausencia de escrituras tras rechazo. Origen: D-02, RQ-01
- [x] 3.2 Agregar pruebas de repositorio para reintento, tarea/nota cruzada, propietario distinto, conflicto, cambio de estado, error de auditoría y rollback total. Origen: D-05, RQ-04
- [x] 3.3 Ajustar pruebas de contratos para exigir endpoints de escritura, SQL parametrizado, versión segura y ausencia de tarea de sesión. Origen: D-01, RQ-03
- [x] 3.4 Actualizar matriz y documentación de Notas con condición atómica, idempotencia, preflight, auditoría privada, rollback y límites MySQL 5.1. Origen: D-06, RQ-05
- [x] 3.5 Ejecutar suites locales afectadas y build VB.NET; registrar comandos y evidencia saneada sin base real. Origen: D-07, RQ-06
- [x] 3.6 Reutilizar `tools/e2e/E2E-TEST` para DOC-42: seguir su `AGENTS.md`, crear el adaptador declarativo, registro, perfil no sensible y pruebas de plataforma para escritura, idempotencia, conflicto y concurrencia; extender sólo el despacho genérico y la reserva local indispensables para esas etapas, conservar el arnés legado y no ejecutar una E2E real sin autorización. Origen: D-07, RQ-06
- [x] 3.7 Con autorización independiente de ambiente, cuenta y tarea descartable, ejecutar E2E con controles `SELECT`, evidencia saneada y gate apagado; si falta, registrar bloqueo. Origen: D-07, RQ-06

## 4. Cierre técnico

- [x] 4.1 Verificar que no se habilitan consumidores, UI, gates, usuarios ni grupos y documentar rollback de despliegue y migración. Origen: D-01, RQ-06
- [x] 4.2 Validar OpenSpec y trazabilidad OPSXJ antes de solicitar revisión, publicación o cierre. Origen: D-07, RQ-06
