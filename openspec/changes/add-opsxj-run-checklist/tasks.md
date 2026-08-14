## 1. Registro persistente y contrato local

- [x] 1.1 Implementar `runChecklistService.js` con esquema v1, normalización de ticket, etapas permitidas y escritura append-only atómica en `.opsxj/runs/<ISSUE-KEY>.json`.
- [x] 1.2 Implementar lectura tolerante y resolución de la última entrada aplicable por etapa y SHA, incluyendo `STALE` y archivo ausente o inválido.
- [x] 1.3 Agregar pruebas unitarias del servicio para éxito, error, preservación de historial, cambio de SHA, archivo corrupto y ausencia de secretos en la estructura registrada.

## 2. Gobierno, revisión y validación

- [x] 2.1 Integrar el registro persistente de revisión en `opsxj:validate`, conservando `OPSXJ_OPENSPEC_REVIEW_CONFIRMED` y `OPSXJ_OPENSPEC_REVIEWED_BY` como entrada compatible.
- [x] 2.2 Hacer que la compuerta de gobierno acepte únicamente la revisión persistida para el SHA actual cuando la variable temporal no esté presente y devuelva un bloqueo explícito para revisión desactualizada.
- [x] 2.3 Registrar el resultado de `new`, `refine` y `validate`, incluidos los fallos que ya pueden asociarse a un ticket o cambio resuelto, sin cambiar sus códigos de salida.
- [x] 2.4 Agregar pruebas de runner y gobierno para confirmación durable, SHA desactualizado, compatibilidad temporal y persistencia de fallos.

## 3. Checklist de estado

- [x] 3.1 Implementar en `opsxjStatusService` el modelo ordenado `new`, `refine`, `review`, `validate`, `archive`, `pull_request` y `close`, combinando el registro local con OpenSpec, Git, GitHub y Jira.
- [x] 3.2 Exponer `checklist` en la salida JSON y representarlo en la salida de texto sin eliminar `checks`, `status` ni `nextAction` existentes.
- [x] 3.3 Agregar pruebas de estado para ticket activo, SHA desactualizado, ticket archivado con PR/Jira finalizados y ticket histórico sin registro.

## 4. Integración de archivo y cierre

- [x] 4.1 Registrar éxito o fallo de `opsxj:archive` después de sus operaciones reales, sin anunciar éxito cuando el registro no pueda persistirse.
- [x] 4.2 Registrar éxito o fallo de `opsxj:close` después de verificar PR y actualizar Jira, preservando el detalle disponible ante una falla parcial.
- [x] 4.3 Agregar pruebas de integración del runner para archivo/cierre exitosos y fallidos, comprobando que no cambian los efectos remotos ni los mensajes establecidos.

## 5. Compatibilidad, documentación y verificación

- [x] 5.1 Ignorar `.opsxj/runs/` en Git y documentar formato, semántica por SHA, recuperación y limitaciones en `tools/opsxj/README.md`.
- [x] 5.2 Ejecutar `npm.cmd --prefix tools/opsxj test`, `openspec validate add-opsxj-run-checklist --strict` y revisar que la salida JSON previa sigue siendo consumible.
