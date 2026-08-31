## Context

Los arneses E2E actuales ya cuentan con piezas seguras para sesión Workflow, TTY, ODBC, recursos y saneamiento, pero cada DOC las ensambla mediante scripts propios. La plataforma se incorpora de forma aditiva en `tools/e2e`; la motivación y el contrato observable están definidos en `proposal.md` y `specs/workflow-e2e-platform/spec.md`.

## Goals / Non-Goals

**Goals:**

- Ofrecer un único punto de entrada para escenarios E2E declarativos y perfiles no sensibles.
- Reutilizar los helpers actuales y hacer obligatorios sus límites de seguridad, cierre y evidencia.
- Separar kernel compartido, adaptadores de DOC y perfiles de ambiente.
- Migrar `notes-read` como prueba piloto sin cambiar su resultado funcional ni su comando vigente.

**Non-Goals:**

- Reemplazar de una vez los arneses DOC-32, DOC-33 u otros DOC.
- Introducir cuentas, contraseñas, SQL libre, cadenas de conexión o datos de negocio en perfiles o registros.
- Ejecutar una E2E real como parte de la implementación sin una nueva autorización explícita.
- Implementar un coordinador distribuido de recursos; la primera versión protege el espacio de trabajo local y falla cerrada si un escenario exige coordinación compartida.

## Decisions

### D-01 — Kernel aditivo con punto de entrada único

Se agregará un ejecutor común y módulos de soporte bajo `tools/e2e/scripts/`, sin reescribir los comandos existentes. El ejecutor recibirá un identificador de escenario y un perfil JSON, validará ambos y coordinará el ciclo completo.

Se elige una incorporación aditiva porque permite probar la plataforma con Notas y conservar una reversión simple. Reemplazar los scripts DOC existentes de inmediato concentraría riesgo operativo y dificultaría aislar regresiones.

### D-02 — Registro cerrado de escenarios, controles y adaptadores

El kernel expondrá un registro en código versionado para escenarios, controles de datos y adaptadores. Cada escenario declara etapas permitidas, autorizaciones, tipo de recurso, controles aprobados y expectativas; cada adaptador declara solo operaciones, payloads permitidos y validaciones de su DOC.

El perfil referencia identificadores de ese registro, no rutas de scripts ni SQL. Esta decisión elimina la ejecución arbitraria desde archivos de configuración. Como alternativa se descartó permitir comandos y consultas parametrizables en perfiles, porque amplía la superficie de secretos, SQL no revisado y ejecución no gobernada.

### D-03 — Perfil JSON no sensible y secretos exclusivamente TTY

El perfil tendrá una lista blanca de campos: escenario, raíz, módulo, ambiente, DSN, recurso, presupuestos, navegador y excepción TLS. El kernel valida esquema, tipos, campos desconocidos y patrones prohibidos antes de abrir navegador, ODBC o sesión.

Los secretos se solicitan solo después de que el preflight no sensible y las autorizaciones hayan sido aceptados. Se inyectan exclusivamente en el entorno del proceso de la corrida y se eliminan en `finally`. No se admite `.env`, almacenamiento persistente ni una alternativa de argumentos. Esto conserva el patrón ya probado por los iniciadores interactivos.

### D-04 — Ciclo común de etapas y controles

El ciclo se ejecuta en este orden:

```text
perfil + escenario -> preflight -> autorizaciones -> TTY efímero
  -> reserva de recurso -> controles antes -> etapa -> controles después
  -> evidencia saneada -> cierre obligatorio
```

Las etapas `anonymous`, `read` y `preview` aplican controles de no mutación cuando corresponden. Las etapas `execution`, `concurrency` y `ui-lock` exigen autorización adicional y recurso descartable. Un fallo, presupuesto vencido o discrepancia de huellas bloquea las etapas posteriores, pero nunca omite el cierre.

Se reutilizarán los contratos locales de reserva existentes para recursos mutantes. Un recurso que requiera coordinación entre equipos se rechazará si no tiene un coordinador registrado, en vez de asumir exclusividad.

### D-05 — Transporte, TLS y saneamiento centralizados

El kernel reutiliza la sesión autenticada y crea el cliente HTTP de cada adaptador a partir del estado de sesión actual. TLS permanece validado por defecto. La excepción para un certificado local autofirmado solo se acepta desde un campo no sensible del perfil y una autorización explícita; se propaga tanto al navegador como al cliente HTTP de la corrida.

La salida de subprocesos pasa por un saneador compartido. Los resultados temporales de Playwright se escriben fuera del repositorio y se eliminan al cerrar. La evidencia persistente se valida contra una lista de campos seguros antes de escribirse. Se descartó confiar en el reporte por defecto de Playwright porque puede incluir contexto de sesión durante un error.

### D-06 — Adaptador piloto de Notas y compatibilidad de comandos

El adaptador `notes-read` registrará las operaciones de listado, consulta y cursor inválido, junto con los controles de estado y auditoría ya aprobados. El nuevo comando de plataforma será adicional; `test:notes:read` seguirá invocando el arnés existente hasta que se compruebe equivalencia y se solicite una migración definitiva.

El adaptador no puede importar ni crear prompts, login, ODBC, reserva, TLS, saneamiento o directorios temporales. Las pruebas de política comprobarán esa frontera mediante importaciones y reglas estáticas.

## Risks / Trade-offs

- [Un kernel compartido puede propagar una regresión] → Mantener el primer lanzamiento aditivo, cubrir políticas y migrar un solo piloto antes de otros DOC.
- [El perfil puede intentar ampliar la ejecución] → Validar campos permitidos y resolver exclusivamente identificadores del registro versionado.
- [Un error de navegador puede revelar contexto de sesión] → Sanear salida, usar directorios temporales y eliminarlos siempre en `finally`.
- [La excepción TLS puede normalizarse fuera del ambiente local] → Mantenerla desactivada por defecto, exigir autorización explícita y no persistirla.
- [Tareas mutantes pueden competir] → Reutilizar reserva local y fallar cerrada sin coordinador compartido.
- [La migración de Notas puede cambiar cobertura existente] → Ejecutar ambos arneses durante el piloto y preservar el comando actual.

## Migration Plan

1. Incorporar registro, validadores, kernel, comando y pruebas de política sin modificar comandos DOC existentes.
2. Agregar perfil de ejemplo y adaptador de lectura de Notas; comprobar equivalencia contra los controles y evidencia actuales.
3. Con autorización explícita, ejecutar el piloto anónimo y de lectura; registrar solo evidencia saneada.
4. Mantener el comando actual de Notas durante el piloto. Migrar otros DOC únicamente mediante cambios posteriores aprobados.
5. Para revertir, retirar el nuevo comando, perfiles y adaptador piloto; los arneses previos continúan operativos y no requieren restauración de datos de negocio.
