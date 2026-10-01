<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## 1. Caracterización y fronteras

- [x] 1.1 Reproducir mediante prueba el fallo donde `DAT_ADIC_TAR` entrega radicado vacío antes de consultar la plantilla. Origen: D-01, RQ-01
- [x] 1.2 Proteger con prueba el handler y JavaScript del módulo, y el aislamiento de la inserción compartida a `ADJUNTARADICACION`. Origen: D-05, RQ-05
- [x] 1.3 Inventariar por bloque funcional todas las llamadas de diez y doce argumentos a `UploadSaveFile`. Origen: D-04, RQ-04

## 2. Contexto autoritativo

- [x] 2.1 Crear `ContextoAdjuntoRadicacion`, `ResultadoAdjuntoRadicacion` e `IContextoAdjuntoRadicacionRepository` con contratos inmutables. Origen: D-02, RQ-02
- [x] 2.2 Implementar `MySqlContextoAdjuntoRadicacionRepository.ObtenerAutorizado` con consulta parametrizada y validaciones de pertenencia e identidad. Origen: D-01, RQ-01
- [x] 2.3 Implementar `ServicioAdjuntoRadicacion.Adjuntar` y validar la discrepancia del radicado informativo antes de almacenar. Origen: D-02, RQ-02

## 3. Preparación y almacenamiento

- [x] 3.1 Crear `ConstruirDatosCamposIndiceGabineteConRadicado` sin referencia a `SolicitaRadicadoTareaWorkflow`. Origen: D-03, RQ-03
- [x] 3.2 Crear `PreAlmacenaDocumentosRadicacionConContexto` y reutilizar una sola vez `AlmacenaDocumentosRadicacion`. Origen: D-03, RQ-03
- [x] 3.3 Separar `UploadSaveFile` en sobrecargas no opcionales de diez y doce argumentos y conectar el servicio solo en `ADJUNTARADICACION`. Origen: D-04, RQ-04
- [x] 3.4 Retirar el fallback tardío y el argumento opcional del recorrido histórico sin cambiar su semántica. Origen: D-04, RQ-04

## 4. Pruebas locales y regresión

- [x] 4.1 Probar registro inexistente, pertenencia inválida, radicado autoritativo vacío, discrepancia y valor informativo vacío. Origen: D-01, RQ-01
- [x] 4.2 Probar resolución única, aislamiento entre cargas y materialización completa del contexto. Origen: D-02, RQ-02
- [x] 4.3 Probar constructor con radicado explícito y reutilización del almacenamiento existente. Origen: D-03, RQ-03
- [x] 4.4 Probar resolución de sobrecargas y no regresión de `GESTION_RESPUESTA`, `WORKFLOWSELECCION`, `PRODUCCION`, Enlace, SII y sellos. Origen: D-04, RQ-04
- [x] 4.5 Probar la conservación de `stru_datos_image_lista`, `uploadFiles` y la inserción JavaScript sin recarga. Origen: D-05, RQ-05
- [x] 4.6 Ejecutar la suite focal, `bootstrap-table-global-contract.test.cjs`, compilación completa y `git diff --check`. Origen: D-06, RQ-06

## 5. E2E y documentación

- [x] 5.1 Integrar en `tools/e2e` la suite real, validador, iniciador interactivo, perfil de ejemplo y comando focal. Origen: D-06, RQ-06
- [x] 5.2 Con autorización explícita, ejecutar los escenarios positivo y negativo con consultas `SELECT` y cierre que compruebe el gate retirado/apagado y sin listas de audiencia. Origen: D-06, RQ-06
- [x] 5.3 Completar el paquete documental único de DOC-85 con diagnóstico, diseño, contratos, matriz de regresión y evidencia saneada. Origen: D-05, RQ-05
- [x] 5.4 Validar OpenSpec/OPSXJ y registrar cualquier bloqueo operacional que impida declarar el ticket cerrado. Origen: D-06, RQ-06
- [x] 5.5 Sustituir la fábrica privada de DOC-85 por `RadicacionModuleConnectionFactory`, `AdoNetDataExecutor` y el resolver de sesión existentes, con prueba de composición. Origen: D-07, RQ-07
