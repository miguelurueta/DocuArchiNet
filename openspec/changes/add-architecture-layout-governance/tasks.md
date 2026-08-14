## 1. Catálogo y contrato arquitectónico

- [ ] 1.1 Crear el catálogo versionado de rutas, raíces retiradas y restricciones de capas en `tools/opsxj/scripts/lib/architectureLayoutCatalog.js`. Origen: D-01, RQ-01
- [ ] 1.2 Crear la norma técnica `Doc/Arquitectura/convenciones/estructura-modular.md` alineada al catálogo, con rutas, límites, ejemplos y tratamiento de excepciones. Origen: D-01, RQ-02
- [ ] 1.3 Agregar pruebas Vitest para normalización de módulo/caso de uso, derivación de rutas y rechazo de catálogos inválidos. Origen: D-01, RQ-01

## 2. Manifiesto y creación de cambios

- [ ] 2.1 Extender el manifiesto OPSXJ a versión 4 con la sección `architecture`, incluyendo `scope`, módulo, caso de uso, raíces permitidas, frontera legacy y excepciones. Origen: D-02, RQ-01
- [ ] 2.2 Extender `opsxj:new` y `opsxj:orchestrate:new` con `--module`, `--use-case` y `--architecture-scope`; validar pares obligatorios y el requisito del perfil de modernización. Origen: D-02, D-05, RQ-01
- [ ] 2.3 Preservar manifestos v2/v3 y cambios no modulares como `NOT_APPLICABLE`, con pruebas de regresión de compatibilidad. Origen: D-02, D-03, RQ-03

## 3. Gobierno y validación estructural

- [ ] 3.1 Implementar `architectureGovernanceService.js` para validar esquema, rutas declaradas, raíces retiradas, límites de Web Forms, Shared, repositorios y fronteras legacy. Origen: D-03, RQ-03
- [ ] 3.2 Implementar el modelo de excepción acotada (`rule`, `paths`, `reason`, `approvedBy`, `expiresOn`) y sus bloqueos por expiración, comodines o campos incompletos. Origen: D-04, RQ-03
- [ ] 3.3 Integrar la auditoría de arquitectura en `opsxj:orchestrate:refine` y `opsxj:technical-review` / `opsxj:prompt-review`, sin alterar sus alias compatibles. Origen: D-05, RQ-02
- [ ] 3.4 Integrar los checks estructurados en `opsxj:validate` y crear `tools/validation/Verify-ArchitectureStructure.ps1` como gate local/CI. Origen: D-03, D-05, RQ-03
- [ ] 3.5 Agregar pruebas Vitest de implementación conforme, ruta incompatible, dependencia Shared, frontera legacy, excepción vigente y excepción vencida. Origen: D-03, D-04, RQ-02, RQ-03

## 4. Documentación, verificación y adopción

- [ ] 4.1 Actualizar `tools/opsxj/README.md` con la convención, comandos, scopes, excepciones, compatibilidad histórica y secuencia de gates. Origen: D-05, RQ-01, RQ-02
- [ ] 4.2 Ejecutar Vitest, `Verify-ArchitectureStructure.ps1`, `opsxj:technical-review`, `opsxj:validate` con fixtures y `openspec validate --strict`; registrar resultados y limitaciones. Origen: D-03, D-05, RQ-02, RQ-03
