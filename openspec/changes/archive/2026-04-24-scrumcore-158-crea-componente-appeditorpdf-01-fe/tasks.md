## 1. Baseline del componente shared

- [x] 1.1 Crear estructura del componente en `src/app/Components/UI/AppEditorPdf/` con separacion por capas (presentation/application/domain/infrastructure segun aplique).
- [x] 1.2 Normalizar naming tecnico a `AppEditorPdf` en archivos, exports y referencias internas del cambio.
- [x] 1.3 Definir API publica inicial del componente (modo controlled/uncontrolled, readOnly y callbacks explicitos) con TypeScript estricto.

## 2. Contrato y boundaries de arquitectura

- [ ] 2.1 Implementar boundaries para mantener `AppEditorPdf` agnostico al dominio de `gestionCorrespondencia`.
- [ ] 2.2 Garantizar que la integracion con modulos consumidores ocurra solo por props/callbacks tipados, sin acoplamiento por DOM global.
- [ ] 2.3 Verificar que la implementacion cumple baseline UX del core: scroll continuo unico, sin flicker, sin saltos de cursor, sin perdida de seleccion.

## 3. Pruebas y calidad

- [x] 3.1 Crear pruebas unitarias del contrato base y estados principales del componente.
- [ ] 3.2 Crear pruebas de integracion UI para validar comportamiento estable de edicion y estados readOnly/controlado.
- [ ] 3.3 Ejecutar suite objetivo con Vitest y documentar evidencia de ejecucion vinculada al SPEC del cambio.

## 4. Documentacion y trazabilidad

- [ ] 4.1 Actualizar documentacion tecnica del componente en `docs/Components/AppEditorPdf/` alineada con este change.
- [ ] 4.2 Registrar decisiones finales de implementacion y desviaciones aprobadas respecto a `design.md`.
- [ ] 4.3 Preparar resumen de cierre del change con alcance implementado, riesgos residuales y siguientes dependencias de tickets.
