<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## 1. Línea base y límites

- [x] 1.1 Verificar que el alcance DOC-38 no incorpora cambios de producción, configuración, datos ni contratos. Origen: D-01, RQ-01
- [x] 1.2 Reunir las evidencias y decisiones vigentes de DOC-36 y DOC-37 para la matriz de verificación. Origen: D-02, RQ-02

## 2. Contrato, seguridad y aislamiento

- [x] 2.1 Ejecutar y registrar pruebas focales de preview, historial, token, permiso, auto-devolución, lock y revalidación. Origen: D-03, RQ-03
- [x] 2.2 Ejecutar análisis estático y pruebas de adaptador, auditoría y ausencia de componentes de respuestas. Origen: D-04, RQ-04

## 3. UI y no regresión

- [x] 3.1 Ejecutar pruebas de UI de usuario anterior: confirmación, cancelación, bloqueo, espera, accesibilidad, responsive y restauración de bandeja. Origen: D-05, RQ-05
- [x] 3.2 Ejecutar la comparación focal con actividad anterior, continuar flujo, enviar a usuario y enviar a grupo. Origen: D-06, RQ-06
- [x] 3.3 Ejecutar la compilación disponible y registrar advertencias históricas, resultado y límites reproducibles. Origen: D-02, RQ-02

## 4. QA y decisión de salida

- [x] 4.1 Realizar QA manual no autenticada solo con autorización vigente; declarar explícitamente exclusiones de E2E autenticada, carga y despliegue. Origen: D-02, RQ-02
- [x] 4.2 Actualizar `04-pruebas-y-evidencia.md` y `00-indice.md` con la matriz, correlaciones saneadas, riesgos y escenarios aprobados o fallidos. Origen: D-07, RQ-07
- [x] 4.3 Emitir recomendación para 05 o registrar ticket de corrección con evidencia reproducible ante un control crítico fallido. Origen: D-07, RQ-07

## 5. Validación documental

- [x] 5.1 Ejecutar `opsxj:refine --sync`, validación OpenSpec y validación OPSXJ después de registrar la evidencia. Origen: D-07, RQ-07
