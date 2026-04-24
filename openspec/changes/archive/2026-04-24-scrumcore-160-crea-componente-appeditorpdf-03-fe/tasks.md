## 1. Endurecimiento de accesibilidad en wrapper AppEditorPdf

- [x] 1.1 Definir estrategia de resolucion de nombre accesible (`aria-label` -> `label` string -> fallback).
- [x] 1.2 Implementar resolucion accesible en `AppEditorPdf` sin romper contrato existente.
- [ ] 1.3 Verificar que el wrapper sigue desacoplado de logica de dominio.

## 2. Cobertura de pruebas focalizadas

- [x] 2.1 Ajustar pruebas unitarias de `AppEditorPdf` para validar forwarding de `aria-label` y fallback.
- [x] 2.2 Mantener pruebas de compatibilidad del contrato controlado y composicion de className.
- [x] 2.3 Ejecutar suite focal `AppEditorPdf.test.tsx` y registrar evidencia.

## 3. Cierre y trazabilidad del cambio

- [ ] 3.1 Actualizar tareas marcando lo ejecutado en la iteracion.
- [ ] 3.2 Consolidar cambios OpenSpec + implementacion en commit del branch `feature/SCRUMCORE-160`.
- [ ] 3.3 Preparar archive del cambio para apertura de PR y comentario en Jira.
