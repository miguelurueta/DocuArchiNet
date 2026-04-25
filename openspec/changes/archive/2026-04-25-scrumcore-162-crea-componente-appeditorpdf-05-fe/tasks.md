## 1. Artefactos OpenSpec del 05-FE

- [x] 1.1 Definir `design.md` con alcance de integracion full-surface para `AppEditorPdf` en `GestionRespuesta`.
- [x] 1.2 Crear `specs/app-appeditorpdf-05-fe/spec.md` con requerimientos de shell neutro y estabilidad funcional.
- [x] 1.3 Validar coherencia con ticket previo `04-FE` sin cambiar alcance funcional.

## 2. Integracion full-surface en modulo consumidor

- [x] 2.1 Ajustar `GestionRespuestaEditorContainer` para declarar contrato de shell neutro.
- [x] 2.2 Simplificar estilos de `GestionRespuestaMainTabContent.module.css` eliminando capa visual redundante.
- [x] 2.3 Mantener `AppEditorPdf` como superficie principal conservando comportamiento actual de flujo.

## 3. Pruebas y cierre tecnico

- [x] 3.1 Actualizar pruebas de `GestionRespuestaMainTabContent` para reflejar contrato `05-FE`.
- [x] 3.2 Ejecutar suite focal de pruebas (`GestionRespuesta` + `AppEditorPdf`) y registrar evidencia.
- [x] 3.3 Dejar cambio listo para `opsxj:archive`.
