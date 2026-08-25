## Context

El orquestador actual de `tools/e2e` ya centraliza perfiles estrictos, captura efímera de secretos, autorización por etapa, cierre y evidencia para DOC-32. Sin embargo, las tareas descartables, sus comprobaciones dinámicas y su preparación se expresan como campos propios de Workflow. Véanse `proposal.md` y las especificaciones de este cambio para el alcance funcional.

## Goals / Non-Goals

**Goals:**

- Añadir un núcleo de ciclo de vida que sirva a cualquier E2E sin conocer tareas, actividades ni consultas de Workflow.
- Ejecutar preflight no mutante y una reserva antes de cada etapa mutante autorizada.
- Impedir la reutilización accidental de un recurso consumido hasta que su adaptador confirme una nueva generación preparada.
- Mantener perfiles sin secretos, adaptadores registrados y las suites propietarias de cada escenario.

**Non-Goals:**

- Crear, restaurar o eliminar datos de negocio de forma genérica.
- Permitir que perfiles proporcionen SQL, comandos, rutas ejecutables o proveedores de reserva.
- Sustituir el token y preview vigente que siguen siendo necesarios inmediatamente antes de un endpoint mutante.
- Garantizar exclusión entre máquinas sin un proveedor de reserva compartido registrado.

## Decisions

### D-01 — Registro de contratos y adaptadores, no comportamiento enviado por perfil

Se creará un registro transversal de contratos de recurso. Cada contrato declara sus roles lógicos (`execution`, `concurrency` u otros), el descriptor no sensible que acepta, el adaptador de preflight, su política de consumo y el proveedor de reserva. El perfil solo referencia un recurso mediante el esquema ya aprobado por el contrato.

Los adaptadores reciben un contexto efímero y devuelven una estructura mínima: disponibilidad, código saneado, clave de reserva y huella de generación. No pueden ejecutar comandos proporcionados por el operador ni leer consultas que no estén registradas en código. Esta decisión conserva la extensibilidad para una futura prueba no Workflow sin crear un intérprete de perfiles.

Se descarta una única interfaz con campos universales como `taskId` o `activity`: impondría el modelo Workflow a pruebas de otros dominios. También se descarta cargar módulos desde el perfil, porque permitiría código arbitrario.

### D-02 — Preflight antes de reservar y antes de la etapa mutante

La secuencia será: validar argumentos, perfil, autorizaciones y controles de cierre; capturar secretos una sola vez; ejecutar los preflights de lectura requeridos; adquirir las reservas; ejecutar las etapas propietarias; y cerrar en `finally` las reservas, secretos y controles de integridad.

El preflight puede usar los adaptadores de solo lectura autorizados por el contrato, incluso si requieren el contexto efímero de la corrida. No abre un endpoint mutante ni modifica estado o auditoría. Una falla de disponibilidad detiene la etapa asociada antes de abrir su sesión autenticada. El preview vigente dentro de la suite propietaria no se elimina: sigue protegiendo contra cambios entre el preflight y el envío.

Se descarta inferir los prerrequisitos únicamente a partir de una respuesta de ejecución rechazada; llega tarde y consume la tarea de prueba.

### D-03 — Reservas con ámbito explícito y estado de consumo

El núcleo usa una abstracción de almacén de reservas con operaciones atómicas de adquirir, renovar, finalizar y liberar. La implementación inicial será un almacén de archivos local con marcadores opacos, apropiado para corridas en el mismo espacio de trabajo. Un contrato que requiera exclusión entre equipos deberá declarar un proveedor compartido registrado; si no está disponible, la corrida fallará cerrada en vez de afirmar una garantía que no puede cumplir.

El marcador no contiene secretos ni identificadores de negocio legibles. La evidencia conserva solo alias de recurso, códigos, tiempos y huellas. Tras una operación mutante exitosa, el estado de consumo guarda la huella de generación final. El recurso no vuelve a estar disponible hasta que el preflight del adaptador observe una generación distinta, producida por la preparación autorizada del ambiente. Las etapas no mutantes liberan la reserva al cierre.

Se descarta liberar incondicionalmente tras una transición: permitiría reutilizar una tarea ya consumida. También se descarta restaurar datos Workflow desde el núcleo: cada restauración sería una operación de negocio específica y potencialmente riesgosa.

### D-04 — Adaptadores propietarios conservan la semántica funcional

La migración de DOC-32 añade un adaptador Workflow que conoce los roles de sus dos tareas, las consultas registradas de solo lectura y las condiciones de selección aplicables. El adaptador entrega el recurso reservado a las suites DOC-32 mediante el entorno efímero existente; las suites mantienen el preview, token, destino, aserciones y evidencia propios.

Un nuevo tipo de prueba solo añade su contrato y adaptador, además de sus pruebas de política. No debe modificar el motor de reservas ni reutilizar campos DOC-32. Durante la migración se aceptan los campos actuales de perfil DOC-32 como representación compatible del descriptor de recurso; la plantilla documentará la forma declarativa nueva cuando esté disponible.

### D-05 — Evidencia y cierre siguen siendo transversales

El resultado de preflight, la adquisición, el consumo y la liberación se agregan a evidencia saneada por etapa. Todo recurso adquirido se finaliza en `finally`, incluso ante timeout, error de hijo o interrupción. La limpieza de secretos y los controles de gate y páginas legacy existentes permanecen como el cierre externo de la secuencia.

Se descarta delegar este cierre a cada adaptador: repetiría errores y permitiría que una nueva prueba olvide liberar un recurso.

## Risks / Trade-offs

- [El estado del recurso cambia entre preflight y ejecución] → Las suites conservan su preview y token vigentes; una respuesta de concurrencia o versión obsoleta sigue tratándose como bloqueo seguro.
- [Una reserva local no protege una corrida desde otro equipo] → Los contratos compartidos exigen un proveedor de reserva compartido; si no existe, el escenario no se declara apto para mutación concurrente.
- [Una tarea mutante no se puede restaurar automáticamente] → El adaptador la marca consumida y exige una nueva generación preparada antes de reutilizarla.
- [El preflight añade controles] → Se mantiene no mutante, acotado por contrato y se ejecuta solo para etapas solicitadas, evitando corridas completas que terminarían en un rechazo de negocio.

## Migration Plan

1. Implementar el núcleo de contrato, preflight y reserva local, con pruebas unitarias que no requieran ambiente real.
2. Integrar DOC-32 mediante un adaptador Workflow y mantener compatibilidad temporal con sus campos actuales de perfil.
3. Documentar el registro de un nuevo escenario y cómo elegir un proveedor de reserva compartido cuando aplique.
4. Validar las pruebas locales y de política; realizar una nueva E2E solo con autorización explícita y recursos preparados.
5. Para rollback, desactivar el uso del ciclo de recursos en el registro DOC-32 y conservar los comandos específicos existentes; no se modifica ni restaura automáticamente ningún dato de negocio.
