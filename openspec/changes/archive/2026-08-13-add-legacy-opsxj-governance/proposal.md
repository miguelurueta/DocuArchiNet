## Why

El `opsxj` existente puede crear y cerrar cambios, pero no expresa el impacto propio de una aplicación ASP.NET WebForms ni exige documentación técnica y evidencia verificable antes del cierre. Se requiere un control local, gradual y aplicable a un único repositorio legacy sin introducir dependencias de React ni automatizaciones remotas implícitas.

## What Changes

- Incorporar un catálogo declarativo de impacto para documentación, UI WebForms, backend VB.NET, handlers/integraciones, base de datos y cambios transversales.
- Hacer que `opsxj:new` genere el paquete documental técnico y un manifiesto de gobierno asociado al cambio OpenSpec.
- Exponer validación local de gobierno y registro de evidencia con ticket, tipo de prueba, referencia y SHA de Git.
- Mantener compatibilidad con cambios OpenSpec históricos que no tengan manifiesto de gobierno.
- Renombrar conceptualmente la revisión de prompts hacia validación técnica neutral, preservando el comando actual como alias compatible.
- Preparar una validación CI de solo lectura; Jira, GitHub, archivos remotos y transiciones permanecerán fuera de CI.

## Capabilities

### New Capabilities

- `legacy-opsxj-governance`: Clasifica el impacto de un cambio legacy, genera documentación técnica y evalúa los requisitos locales de evidencia antes del archivado.

### Modified Capabilities

- 

## Impact

- `tools/opsxj/scripts/lib`, comandos npm y pruebas unitarias del tooling.
- Artefactos OpenSpec nuevos en cada ticket y documentación técnica bajo `Doc/Tecnica/Opsxj/`.
- Evidencia local bajo `.opsxj/evidence/`, sin subir secretos ni ejecutar transiciones Jira/GitHub durante validación.
