## Why

DOC-41 dejó disponible el recorrido moderno de lectura de Notas Workflow. Sus contratos ya reservan solicitudes y puertos de creación, actualización y eliminación, pero el transporte sólo publica lecturas y el repositorio responde `Unavailable` para toda mutación. DOC-42 completa únicamente esa capa de escritura sin migrar consumidores WebForms ni activar la experiencia moderna.

## What Changes

- Publicar en el ASMX especializado los contratos tipados `CrearNota`, `ActualizarNota` y `EliminarNota`, usando sólo el contexto autenticado de servidor y la tarea explícita.
- Implementar persistencia parametrizada, condicionada y transaccional para creación idempotente, actualización por versión y borrado físico auditado.
- Añadir un preflight fail-closed para cada esquema MySQL 5.1 y una migración revisable/reversible que sólo pueda aplicarse con autorización posterior.
- Extender las pruebas focales y reutilizar las suites de escritura y concurrencia ya existentes bajo `tools/e2e`; una corrida con escritura requerirá su propia autorización de ambiente, cuenta y tarea descartable.
- Actualizar la documentación y matriz de pruebas de Notas. No se cambiarán páginas WebForms, consumidores, gates, endpoints legacy ni datos de un ambiente durante este refinamiento.

## Goals / Non-Goals

**Goals**

- Garantizar que la autorización efectiva de cada mutación se comprueba dentro de la misma operación condicionada que escribe.
- Evitar duplicados por reintento de creación, conflictos silenciosos y auditoría parcial.
- Mantener respuestas funcionales seguras y no revelar existencia ni contenido a un actor no autorizado.

**Non-Goals**

- Cambiar `workflow/Webworkflow.aspx`, sus code-behind, HTML, UI o gates.
- Retirar o envolver `Class_anotacion_tarea`, alterar contratos legacy o migrar consumidores.
- Ejecutar una migración, escritura de base de datos o E2E autenticada sin una autorización nueva y específica.

## Decisions

1. **D-01 — Frontera moderna aislada.** Las mutaciones se agregan sólo al ASMX especializado y a las capas Workflow existentes; la tarea llega como `idTarea` explícito y nunca proviene de sesión.
2. **D-02 — Contexto y autorización atómicos.** Identidad, grupo, ruta, actividad y fecha se derivan del contexto autorizado y del snapshot de tarea del servidor. La escritura condiciona nota, tarea, actor, estado operativo y versión en la misma unidad de persistencia.
3. **D-03 — Creación idempotente.** `clientRequestId` será un UUID ligado de manera única a tarea y autor; durante 30 días el reintento devuelve la misma respuesta sin una segunda nota ni una segunda auditoría.
4. **D-04 — Concurrencia y borrado.** Actualizar y eliminar requieren el ETag SHA-256 esperado, calculado por .NET y conservado en un libro de versiones InnoDB independiente de la respuesta original de idempotencia. Eliminar es físico, sólo para el propietario y se audita atómicamente; el contenido eliminado no queda operativo ni histórico.
5. **D-05 — Transacción y privacidad.** Nota, idempotencia y auditoría se confirman juntas o se revierten juntas. La auditoría conserva metadatos, longitud y SHA-256, nunca texto completo.
6. **D-06 — Preflight y esquema.** En MySQL 5.1 toda escritura falla como `Unavailable` si el preflight no verifica InnoDB, `TEXT utf8`, índices requeridos, almacén InnoDB de idempotencia y libro InnoDB de versiones. La migración es revisable y reversible, pero no se aplicará sin autorización explícita.
7. **D-07 — Evidencia integrada y segura.** Las pruebas locales no abren MySQL; la E2E reutiliza exclusivamente `tools/e2e`, usa controles `SELECT` y queda bloqueada hasta recibir autorización de escritura para ambiente, cuenta y tarea descartable.

## Risks / Trade-offs

- Los esquemas conocidos aún pueden conservar `ANOTACION_TAREA` en MyISAM; habilitar escritura sin la migración autorizada rompería la garantía de rollback, por lo que el contrato falla cerrado.
- La versión debe derivarse de una representación canónica de campos persistidos; no se acepta una versión calculada o enviada por el cliente.
- Algunas instalaciones MySQL 5.1 no habilitan `SHA2()` para consultas SQL; el libro de versiones evita esa dependencia en lecturas y mutaciones sin sustituir SHA-256 por un algoritmo menor.
- Las E2E de escritura alteran estado y auditoría incluso si eliminan la nota de prueba; una autorización previa de lectura no las habilita.

## Migration Plan

1. Implementar y validar localmente contratos, servicio, repositorio, preflight y migración sin conectarse a un ambiente.
2. Con autorización de consultas de sólo lectura, inspeccionar cada esquema objetivo y confirmar la migración propuesta.
3. Obtener autorización explícita e independiente antes de aplicar una migración por esquema, verificando rollback y el preflight posterior.
4. Con autorización explícita de una tarea descartable, ejecutar las E2E de escritura y concurrencia por el runner existente; registrar sólo evidencia saneada.
5. Mantener gates apagados y consumidores legacy intactos.

## Open Questions

No hay decisiones de negocio abiertas: DP-01, DP-03, DP-04, DP-05 y DP-07 están resueltas en el modelo de requisitos de Notas. La precondición operacional es obtener autorizaciones separadas para inspección, migración y E2E de escritura.
