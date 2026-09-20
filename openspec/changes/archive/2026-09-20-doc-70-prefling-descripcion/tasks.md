<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
# Plan atómico DOC-70

## 1. Contratos y modelo
- [x] 1.1 Extender DTOs aditivamente con plan por item, efectos, requisitos y `Executable`. Área: DTOs. Origen: D-01, RQ-01.
- [x] 1.2 Definir modelos internos y puerto de configuración sin identidad física pública. Área: Services. Origen: D-02, RQ-02.
## 2. Configuración y planificación
- [x] 2.1 Implementar repositorio de configuración de efectos con SELECT parametrizado. Área: Infrastructure/Repositories. Origen: D-03, RQ-03.
- [x] 2.2 Implementar `ImportEffectPlanBuilder` puro para modos, requisitos y efectos. Área: Services. Origen: D-02, RQ-02.
- [x] 2.3 Integrar builder/repositorio en preflight sin SII ni mutaciones. Área: servicio/composición. Origen: D-04, RQ-04.
## 3. Huella y creación
- [x] 3.1 Ampliar fingerprint con contexto y configuración, independiente del orden. Origen: D-05, RQ-05.
- [x] 3.2 Revalidar plan, requisitos y huella antes del lock/persistencia. Origen: D-06, RQ-06.
- [x] 3.3 Implementar códigos seguros diferenciados. Origen: D-07, RQ-07.
## 4. Pruebas y documentación
- [x] 4.1 Probar plan individual/múltiple, contrato aditivo y campos prohibidos. Origen: D-01, RQ-01.
- [x] 4.2 Probar pureza, SQL parametrizado y cero dependencias/llamadas SII. Origen: D-04, RQ-04.
- [x] 4.3 Probar determinismo y rechazo antes de lock/persistencia. Origen: D-05, RQ-05.
- [x] 4.4 Crear documentación canónica, diagramas, matriz, errores y trazabilidad. Origen: D-08, RQ-08.
- [x] 4.5 Ejecutar suite transversal y MSBuild con evidencia saneada. Origen: D-08, RQ-08.
## 5. E2E y cierre
- [x] 5.1 Extender solo `tools/e2e` para plan, fingerprint y auditoría SELECT/telemetría. Origen: D-08, RQ-08.
- [x] 5.2 Ejecutar E2E autorizada y ejecución descartable compatible; restaurar gate. Origen: D-08, RQ-08.
- [x] 5.3 Validar OpenSpec/OPSXJ y reconciliar sin pendientes. Origen: D-08, RQ-08.
