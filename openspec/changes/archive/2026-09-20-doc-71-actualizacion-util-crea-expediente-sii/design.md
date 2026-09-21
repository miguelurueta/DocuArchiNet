<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->

## Context

La importación moderna bloquea el documento cuando `util_Estado_Crea_ExpedienteSII=0`, porque una bandera representa obligatoriedad y autorización de creación. DOC-71 separa almacenamiento documental de efectos de expediente sin alterar el recorrido legacy.

## Goals / Non-Goals

**Goals:** modelar el modo autoritativo, permitir importación sin expediente, persistir `NoAplica`, completar coherentemente y conservar DOC-67 para la rama creadora.

**Non-Goals:** modificar funciones legacy, reconstruir/refoliar históricos, inferir el modo desde SII o agregar fallback de identidad del propietario.

## Decisions

### D-01 — Modo autoritativo explícito
La configuración expondrá `SinExpediente` o `GestionarExpediente`, derivado exclusivamente del trámite. `CreacionAutomaticaHabilitada` seguirá protegiendo la creación física.

### D-02 — Plan válido sin expediente
El coordinador devolverá un plan exitoso sin destinos físicos cuando el modo sea `SinExpediente`. El orquestador almacenará normalmente y no tratará la ausencia deliberada como error.

### D-03 — Estado NoAplica
`EstadoEfectoExpedienteImportacion` incorporará `NoAplica`. Repositorios y DTOs lo serializarán por nombre, manteniendo compatibilidad con filas históricas.

### D-04 — Índices documentales independientes del expediente
La resolución de datos para índices ocurre antes de bifurcar efectos físicos: MERCANTIL/ESAL consultan `consultarExpedienteMercantil` por matrícula y RUP consulta `consultarExpedienteProponente` por proponente. Después, la rama 0 omite caché, búsqueda, creación y vínculo del expediente local, pero ejecuta almacenamiento y actualización de índices documentales para todo documento del mismo `ENLASE`. El gateway actualiza por `ID` + `ENLASE`, filtra campos mediante `DETALLE_GABIENETE`/`INFORMATION_SCHEMA`, ajusta longitud y tipo y relee los valores. Nunca cambia `ID_EXPEDIENTE`.

NIT/cédula y razón social no son precondiciones universales: algunas respuestas SII legítimas no los incluyen incluso después de consultar expediente. El adaptador omitirá de la escritura los valores ausentes y nunca sobrescribirá índices históricos con vacío. La identidad consultable sí es obligatoria y se normaliza desde la consulta de expediente SII.

La respuesta usada para construir inscripciones y la respuesta de metadatos del sello pueden exponer
campos distintos. Después de almacenar todos los items y antes de procesar relacionados, el orquestador
completará solo los campos vacíos de cada `InscripcionImportacion` con `MetadatosSii` del item enlazado por
`ClaveInscripcion`. No reemplazará valores existentes. La consulta de expediente es la fuente autoritativa
para matrícula/proponente y completa NIT/cédula y razón social cuando estén disponibles. Los índices
documentales no aceptan `NoAplica`: `Confirmado` queda reservado para una escritura con postlectura física
satisfactoria; una identidad no consultable bloquea el plan antes de almacenar.

### D-05 — Finalización según plan
El cierre exige `Confirmado` para almacenamiento e índices documentales. Resolución, vínculo y caché de expediente aceptan `NoAplica` únicamente en modo `SinExpediente`. No se crea evidencia física ficticia.

### D-06 — Fingerprint coherente
El modo y su configuración participarán en el fingerprint DOC-70. Cualquier cambio invalida creación/reutilización antes del lock o persistencia.

### D-07 — Rama con expediente intacta
Para `GestionarExpediente` se preservan sujeto → caché → buscar/crear → almacenar → vincular → índices → caché, con las postcondiciones existentes.

### D-08 — Verificación proporcional al riesgo
Pruebas estructurales, unitarias, persistencia, regresión y E2E autorizadas demostrarán ambas ramas, idempotencia, ausencia de llamadas prohibidas y restauración del gate.

## Risks / Trade-offs

- El nuevo estado exige revisar predicados que comparan exclusivamente con `Confirmado`.
- Un plan sin destinos puede romper coordinadores que asumen una colección no vacía; el universo documental por `ENLASE` debe procesarse de forma independiente.
- Actualizar por `ENLASE` sin `ID_EXPEDIENTE` amplía el alcance físico; se mitiga iterando IDs descubiertos, usando `WHERE ID=@imageId AND ENLASE=@radicado`, exigiendo una fila y releyendo.
- Los verificadores históricos que exigen expediente para todos los items deben hacerse sensibles al modo sin relajar la rama 1.

## Migration Plan

No requiere DDL si los estados son texto compatible. El despliegue es aditivo y reversible mediante el gate moderno; la rama legacy permanece intacta. Se validará longitud de columnas antes de liberar.

## Open Questions

- Confirmar que las columnas de estado admiten `NoAplica` en esquemas objetivo.
- Identificar recurso descartable real con configuración 0 para E2E; su ausencia bloquea evidencia, no las pruebas automatizadas.
