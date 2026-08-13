# legacy-opsxj-governance Specification

## Purpose

Establece un gobierno local y verificable para cambios del tooling `opsxj` en la aplicación legacy, respetando WebForms, VB.NET y un único repositorio.

## Requirements

### Requirement: Clasificación declarativa de impacto legacy
El sistema SHALL clasificar cada cambio nuevo de `opsxj` en una categoría de impacto soportada y SHALL asociar a esa categoría la documentación técnica y los tipos de evidencia requeridos.

#### Scenario: Ticket WebForms

- **WHEN** se inicia un ticket con impacto `webforms_ui`
- **THEN** el cambio declara documentación de UI y flujo WebForms, y exige evidencia de QA manual reproducible

#### Scenario: Impacto desconocido

- **WHEN** se solicita una categoría que no pertenece al catálogo
- **THEN** el comando falla antes de crear artefactos y muestra las categorías válidas

### Requirement: Paquete técnico generado por ticket
El sistema SHALL generar un manifiesto de gobierno dentro del cambio OpenSpec y el paquete documental técnico bajo `Doc/Tecnica/Opsxj/<change-name>/` al iniciar un ticket.

#### Scenario: Inicio de ticket válido

- **WHEN** `opsxj:new` obtiene correctamente el contexto del ticket
- **THEN** crea propuesta, artefactos OpenSpec, manifiesto y los documentos definidos por su clasificación

### Requirement: Validación local trazable
El sistema SHALL validar localmente las tareas OpenSpec, la confirmación de revisión, los documentos requeridos y la evidencia vigente para el SHA evaluado.

#### Scenario: Evidencia vigente y completa

- **WHEN** todos los requisitos están completos y cada evidencia exigida tiene resultado `pass` para el SHA actual
- **THEN** `opsxj:validate` informa estado PASS sin modificar Jira, GitHub ni el código de negocio

#### Scenario: Evidencia ausente o vencida

- **WHEN** falta evidencia requerida o su SHA no coincide con el SHA evaluado
- **THEN** `opsxj:validate` informa estado FAIL e identifica el requisito pendiente

### Requirement: Compatibilidad con cambios históricos
El sistema SHALL permitir validar y archivar cambios OpenSpec históricos que no cuenten con manifiesto de gobierno, sin convertir esa ausencia en un bloqueo retroactivo.

#### Scenario: Cambio sin manifiesto

- **WHEN** se evalúa un cambio existente sin manifiesto de gobierno
- **THEN** la validación informa que aplica compatibilidad histórica y no agrega requisitos nuevos
