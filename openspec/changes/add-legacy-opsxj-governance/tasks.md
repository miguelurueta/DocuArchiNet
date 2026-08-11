## 1. Gobierno y generación de artefactos

- [x] 1.1 Crear el catálogo declarativo de impactos legacy y sus documentos/evidencias obligatorias.
- [x] 1.2 Generar manifiesto de gobierno y paquete técnico desde `opsxj:new`.
- [x] 1.3 Incorporar la opción explícita `--impact` con validación de categorías soportadas.
- [x] 1.4 Agregar pruebas unitarias de clasificación, documentación y compatibilidad histórica.

## 2. Validación y evidencia local

- [x] 2.1 Implementar evaluación local de documentos, tareas, revisión OpenSpec y evidencia por SHA.
- [x] 2.2 Exponer `opsxj:validate` con salida legible y JSON opcional.
- [x] 2.3 Exponer `opsxj:validation:evidence` para registrar resultados verificables por ticket.
- [x] 2.4 Agregar pruebas de evidencia ausente, vigente y vencida.

## 3. Cierre controlado y compatibilidad

- [x] 3.1 Integrar la evaluación de gobierno como compuerta previa de `opsxj:archive`.
- [x] 3.2 Mantener explícitamente la excepción de compatibilidad para cambios OpenSpec históricos sin manifiesto.
- [x] 3.3 Renombrar la revisión de prompts a revisión técnica neutral y conservar el alias compatible.
- [x] 3.4 Actualizar README, ayuda y ejemplo de configuración del `opsxj` existente.

## 4. Verificación y adopción

- [x] 4.1 Añadir CI de solo lectura para validar OpenSpec y las pruebas del tooling, sin acciones Jira/GitHub.
- [x] 4.2 Ejecutar la suite del tooling y la validación estricta del cambio OpenSpec.
- [ ] 4.3 Ejecutar un ticket de ensayo real hasta `opsxj:validate` y documentar la evidencia QA/E2E disponible para WebForms.
