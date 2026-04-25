## 1. Integracion de AppEditorPdf en consumidor GestionRespuesta

- [x] 1.1 Sustituir imports de `AppEditor` por `AppEditorPdf` y companion APIs en `GestionRespuestaMainTabContent`.
- [x] 1.2 Reemplazar uso JSX del editor para montar `AppEditorPdf` como superficie principal.
- [x] 1.3 Mantener props de layout/paginacion y acciones de guardado sin cambiar comportamiento funcional.

## 2. Estabilidad de comportamiento en modulo consumidor

- [x] 2.1 Verificar que flujo de pasos, panel lateral, adjuntos y modal se mantiene estable.
- [x] 2.2 Ajustar pruebas del modulo para reflejar integracion `AppEditorPdf`.
- [x] 2.3 Ejecutar suite focal de tests del modulo y validar no regresion.

## 3. Cierre y trazabilidad del cambio

- [x] 3.1 Actualizar tareas marcando lo ejecutado.
- [x] 3.2 Consolidar cambios OpenSpec + codigo en commit del branch `feature/SCRUMCORE-161`.
- [x] 3.3 Archivar change para abrir PR y comentar Jira.
