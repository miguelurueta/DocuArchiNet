<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## 1. Estado y composición

- [x] 1.1 Crear `importar-servicio-web-requirements.js` con estados y habilitación cerrada. Origen: D-02, RQ-02; D-06, RQ-06
- [x] 1.2 Crear `importar-servicio-web-preparation.js` para colección individual/múltiple. Origen: D-01, RQ-01
- [x] 1.3 Crear `importar-servicio-web-intent-client.js` reutilizando API, preflight, huella e idempotencia. Origen: D-03, RQ-03; D-04, RQ-04
- [x] 1.4 Deduplicar confirmaciones y traducir `PREFLIGHT_STALE`. Origen: D-04, RQ-04; D-06, RQ-06

## 2. UI y accesibilidad

- [ ] 2.1 Agregar popup, selectores, resumen, cancelar y confirmar en `Webworkflow.aspx`. Origen: D-02, RQ-02; D-05, RQ-05
- [x] 2.2 Extender solo `importar-servicio-web-modern.css` con layout responsive. Origen: D-05, RQ-05
- [ ] 2.3 Integrar acciones individual/múltiple sin preflight implícito desde `Guardar todas`. Origen: D-01, RQ-01; D-05, RQ-05
- [ ] 2.4 Mostrar catálogo/plan autoritativos sin `ExpedientId` y restaurar contexto/foco. Origen: D-03, RQ-03; D-05, RQ-05; D-06, RQ-06

## 3. Registro y límites

- [x] 3.1 Registrar scripts en orden en code-behind y `.vbproj` bajo el gate existente. Origen: D-04, RQ-04; D-05, RQ-05
- [ ] 3.2 Probar invariancias contra segundo transporte, ejecución, persistencia y cambios legacy. Origen: D-04, RQ-04; D-06, RQ-06

## 4. Pruebas

- [x] 4.1 Crear `importar-servicio-web-preparation.test.cjs`. Origen: D-01, RQ-01; D-05, RQ-05
- [x] 4.2 Crear `importar-servicio-web-preflight-contract.test.cjs`. Origen: D-02, RQ-02; D-03, RQ-03; D-06, RQ-06
- [x] 4.3 Crear `importar-servicio-web-intent-client.test.cjs`. Origen: D-04, RQ-04; D-06, RQ-06
- [ ] 4.4 Ejecutar focales, regresión y build; E2E real solo con autorización. Origen: D-05, RQ-05; D-06, RQ-06

## 5. Documentación

- [ ] 5.1 Crear paquete canónico `DOC-75-preparacion-individual-multiple/` y `Diagramas/`. Origen: D-01, RQ-01; D-05, RQ-05
- [ ] 5.2 Documentar B03/B09/B11, estados, riesgos, rollout, rollback y funciones. Origen: D-03, RQ-03; D-06, RQ-06
- [ ] 5.3 Registrar evidencia y validar OpenSpec/Opsxj. Origen: D-06, RQ-06
