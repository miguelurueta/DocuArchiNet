<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira, DOC-36 y el contexto de código. Origen: D-01, RQ-01
- [x] 1.2 Ajustar propuesta, design y spec con decisiones, riesgos y compatibilidad definitivos. Origen: D-02, RQ-02

## 2. Implementación

- [x] 2.1 Registrar la presentación sin feature gate, crear trigger, modal y adaptadores JavaScript exclusivos de Usuario anterior. Origen: D-01, RQ-01
- [x] 2.2 Retirar ruta legacy y conectar preview, confirmación, ejecución, timeout y actualización localizada sin tocar operaciones vecinas. Origen: D-03, RQ-03

## 3. Pruebas

- [x] 3.1 Agregar pruebas CJS de bootstrap, ausencia de postback, contratos, estados accesibles y aislamiento. Origen: D-04, RQ-04
- [x] 3.2 Ejecutar pruebas focales y la compilación disponible; registrar evidencia y limitaciones. Origen: D-05, RQ-05
- [x] 3.3 Registrar perfil DOC-37 no sensible, adaptador de recursos y etapas E2E reutilizando los controles DOC-36. Origen: D-06, RQ-06
- [x] 3.4 Agregar prueba Playwright UI DOC-37 y pruebas locales de política/orquestador sin iniciar una sesión autenticada. Origen: D-06, RQ-06
- [x] 3.5 Adaptar el runner y perfil DOC-37 a una sola tarea seleccionada por etapa, usando el preview vigente como única fuente de actividad. Origen: D-06, RQ-06
- [x] 3.6 Verificar localmente el aislamiento de etapas, contratos y limpieza de secretos tras la adaptación. Origen: D-06, RQ-06
- [x] 3.7 Establecer la precondición de tarea mediante el comando oficial de la bandeja, antes de las huellas E2E y sin simular sesión o campos ocultos. Origen: D-06, RQ-06

## 4. Cierre

- [x] 4.1 Validar OpenSpec y la trazabilidad de refinement. Origen: D-01, RQ-01
- [x] 4.2 Documentar el diff final, contratos, compatibilidad y evidencia de no regresión. Origen: D-05, RQ-05
- [x] 4.3 Validar OpenSpec, documentación y evidencia local de la arquitectura E2E reutilizable. Origen: D-06, RQ-06
- [x] 4.4 Ejecutar y registrar las etapas E2E autorizadas de GESTOR, una por una, con tareas descartables distintas. Origen: D-06, RQ-06
