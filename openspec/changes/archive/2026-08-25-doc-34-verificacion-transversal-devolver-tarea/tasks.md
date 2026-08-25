<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05 -->
## 1. Refinamiento aprobado

- [x] 1.1 Consolidar el alcance no mutante, las restricciones de ambiente y la evidencia previa de DOC-32/DOC-33. Origen: D-01, RQ-01
- [x] 1.2 Trazar decisiones de contrato, UI, no regresión y recomendación en design, spec y tasks. Cobertura: D-02, D-03, D-04, D-05, RQ-02, RQ-03, RQ-04, RQ-05. Origen: D-02, RQ-02

## 2. Verificación local de contrato y capas

- [x] 2.1 Ejecutar la compilación disponible y las pruebas CJS/VB focales de devolución, registrando comandos, resultado y duración sin datos sensibles. Cobertura: D-02, D-03, RQ-02, RQ-03. Origen: D-01, RQ-01
- [x] 2.2 Revisar estáticamente preview, repositorio, servicio y adaptador para confirmar fuente entrante Ruta/Flujo, universo autorizado, cursor, límite, token y lock. Cobertura: D-03, RQ-03. Origen: D-02, RQ-02
- [x] 2.3 Comparar contratos y pruebas de Continuar flujo, Enviar a usuario, Enviar a grupo y Usuario anterior, señalando cualquier diferencia reproducible. Origen: D-05, RQ-05

## 3. Verificación de interfaz y QA no autenticada

- [x] 3.1 Ejecutar las pruebas CJS de UI, confirmación y políticas para preview, selección, error, bloqueo de respuesta y accesibilidad. Origen: D-04, RQ-04
- [x] 3.2 Realizar QA manual no autenticada del shell visual en escritorio y móvil; documentar que el disparador requiere tarea seleccionada y que los escenarios dinámicos se cubren con CJS y evidencia E2E previa. Cobertura: D-04, RQ-04. Origen: D-01, RQ-01
- [x] 3.3 Confirmar por revisión que no hay evaluación del feature gate ni postback, handler o fallback Web Forms alcanzable para devolución. Origen: D-04, RQ-04

## 4. Evidencia y decisión de fase

- [x] 4.1 Actualizar `00-indice.md` y `04-pruebas-y-evidencia.md` del paquete `DebolverTarea` con matriz saneada, riesgos y correlaciones. Origen: D-05, RQ-05
- [x] 4.2 Registrar escenarios aprobados, fallidos o asociados a corrección y emitir una recomendación única para fase 04. Cobertura: D-05, RQ-05. Origen: D-03, RQ-03
- [x] 4.3 Validar estrictamente el cambio OpenSpec antes de solicitar revisión de cierre. Cobertura: D-05, RQ-05. Origen: D-01, RQ-01
