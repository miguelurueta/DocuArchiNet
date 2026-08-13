## Context

El generador `opsxj:new` ya convierte un ticket Jira en propuesta, diseño, especificación, tareas y un manifiesto de gobierno. La clasificación `--impact` determina documentación y evidencia, pero no existe una forma declarativa de exigir reglas arquitectónicas reutilizables para una modernización de código legacy. La motivación funcional está en [proposal.md](proposal.md) y el contrato de comportamiento en la especificación delta `legacy-opsxj-governance`.

La política debe ser transversal: el patrón aplica a Workflow, Radicación, Expedientes, Documentos y cualquier otra capacidad legacy. No debe codificar clases, tablas o nombres de un módulo particular.

## Goals / Non-Goals

**Goals:**

- Permitir seleccionar explícitamente un perfil de arquitectura al crear un cambio con `opsxj:new` o `opsxj:orchestrate:new`.
- Sembrar una política consistente en los artefactos OpenSpec y persistir su identidad y versión en el manifiesto de gobierno.
- Hacer verificable que los requisitos del perfil están presentes y que su trabajo asociado se completa antes del cierre.
- Mantener el comportamiento actual de tickets sin perfil y de cambios históricos.

**Non-Goals:**

- No reestructurar ni migrar código de los módulos legacy existentes.
- No imponer el perfil a tickets documentales, correcciones pequeñas o cambios no modernizadores.
- No construir un adaptador universal que invoque métodos legacy por nombre dinámico.
- No inferir automáticamente el perfil a partir del texto de Jira.
- No ejecutar análisis estático universal del código legacy en esta entrega; la validación se limita a gobierno, artefactos, tareas y evidencia.

## Decisions

### 1. Catálogo versionado de perfiles, no reglas globales incondicionales

Se agregará un catálogo `ARCHITECTURE_PROFILE_CATALOG` al tooling con el perfil `enterprise-legacy-modernization` y una versión. Cada perfil declarará:

- nombre, versión y descripción;
- requisitos de artefactos OpenSpec;
- tareas de gobierno obligatorias;
- marcadores de contenido que permitan validar que el perfil se sembró;
- documentos técnicos adicionales y evidencia, solo cuando sean necesarios.

`--profile enterprise-legacy-modernization` será optativo. La ausencia de `--profile` conserva exactamente la generación actual. Un valor no reconocido falla antes de consultar Jira, crear ramas, escribir archivos o ejecutar transiciones remotas.

El perfil de arquitectura no selecciona reglas de lenguaje o framework. Para evitar falsos positivos se introduce, de manera independiente, `--tech-profile` con los valores `legacy-webforms-vb`, `tooling-node`, `frontend-react-ts` y `generic`. Si no se declara, la revisión técnica detecta señales inequívocas de forma conservadora y usa `generic` cuando no haya certeza. Un prompt Web Forms/VB no hereda reglas de React/TypeScript por mencionar UI, servicios o adapters.

**Alternativas descartadas:**

- Configurar la política en `openspec/config.yaml` para todos los cambios. La regla afectaría tickets sin modernización y produciría requisitos irrelevantes.
- Inferir el perfil desde resumen, labels o impacto Jira. Esa inferencia sería ambigua y podría aplicar gobierno incorrecto.

### 2. Alias explícito y una sola implementación del comando

Se añadirá `opsxj:orchestrate:new` al `package.json` y al registro de comandos, delegando en el mismo `runNew` de `opsxj:new`. Ambos comandos aceptarán `--impact` y `--profile`; su resultado debe ser idéntico para los mismos argumentos.

El análisis del perfil ocurre antes de las comprobaciones de Git y de cualquier interacción con Jira. Esto garantiza que un perfil inválido no altere estado local ni externo.

**Alternativa descartada:** crear dos flujos de creación. Duplicaría el manejo de Jira, ramas, manifiestos y pruebas.

### 3. Propagación desde el runner hasta cada artefacto

El perfil se propaga así:

```text
CLI --profile / --tech-profile
   → runNew
   → createProposalFromJira
   → writeProposalFile / writeRefinementArtifacts
   → proposal, design, spec, tasks y manifiesto
```

Los builders añadirán una sección identificable de política al contenido generado. Para `enterprise-legacy-modernization`, esa sección exige:

- separación Presentation → Application → Domain → Infrastructure;
- DTOs y modelos tipados; Presentation sin reglas de negocio;
- infraestructura común de datos reutilizable, repositorios específicos por dominio y consultas parametrizadas;
- prohibición de `GenericRepository` que mezcle dominios;
- Gateway/Adapter tipado por capacidad de negocio como frontera exclusiva entre código moderno y comportamiento legacy;
- ausencia de accesos directos desde Presentation/Application a clases legacy, sesión, controles de interfaz, SQL legacy o cambios de estado;
- convivencia gradual, bandera de funcionalidad, piloto, rollback y pruebas de equivalencia.

La política describe un patrón, no una clase universal. Cada cambio define el Gateway/Adapter concreto para su capacidad, por ejemplo transición de tareas, radicación o expedientes.

**Alternativas descartadas:**

- Incluir `WorkflowLegacyExecutorAdapter` en la política. Haría el perfil dependiente de Workflow.
- Crear `GenericLegacyAdapter`. Ocultaría contratos, permisos y trazabilidad detrás de invocaciones no tipadas.

### 4. Manifiesto ampliado y validación documental bloqueante

`opsxj-governance.json` incluirá un contrato documental por archivo generado: nombre, secciones requeridas, marcadores de identidad y restricciones de cierre. También incluirá, cuando se haya solicitado, un bloque `architectureProfile` con nombre, versión y lista de marcadores/requisitos generados.

El manifiesto podrá incluir también `technologyProfile`; es informativo y permite que el revisor seleccione reglas compatibles, sin convertirlo en una obligación de arquitectura ni en una inferencia del módulo de negocio.

`opsxj:validate` aplicará a todo cambio con manifiesto la validación documental bloqueante:

1. confirmar que existe cada documento técnico requerido;
2. confirmar que cada documento conserva sus secciones requeridas y un contenido mínimo distinto de la plantilla;
3. rechazar marcadores de trabajo no terminado: `TBD`, comentarios/instrucciones de plantilla y checkboxes abiertos `- [ ]`;
4. confirmar que Ticket, cambio OpenSpec y clasificación coinciden con el manifiesto;
5. confirmar que las tareas OpenSpec están completas, la revisión fue confirmada y la evidencia exigida está vigente;
6. devolver un check independiente por archivo y regla incumplida, sin modificar Jira, GitHub ni código de negocio.

Cuando exista `architectureProfile`, se agregan sus checks específicos: presencia de las secciones de política en propuesta, diseño, especificación y tareas, más la documentación técnica adicional que el perfil declare.

Los contratos documentales se definen junto a las plantillas de cada clasificación de impacto, no mediante reglas genéricas de texto sobre todos los archivos. Esto permite que, por ejemplo, UI, servicios, integración y pruebas tengan secciones estrictas pero pertinentes. Los marcadores de perfil verifican que la política fue incluida en el cambio. No pretenden demostrar por análisis estático que una implementación futura no tenga una llamada prohibida; esa verificación se realizará mediante tareas, revisión técnica, pruebas y evidencia del ticket.

**Alternativa descartada:** bloquear por expresiones regulares de nombres `Class*`, `Session` o controles WebForms sobre todo el repositorio. Produciría falsos positivos en código legado que debe permanecer y no permite conocer la frontera de cada módulo.

### 5. Compatibilidad de manifiestos y archivos existentes

Los manifiestos anteriores no tendrán bloque de perfil. En ese caso el validador conservará el comportamiento actual. Los cambios nuevos sin `--profile` también tendrán el manifiesto actual, sin controles adicionales de perfil.

La estructura de manifiesto se extiende de forma aditiva y versionable; los consumidores existentes deben ignorar propiedades desconocidas.

## Risks / Trade-offs

- [La política se vuelva texto sin efecto] → Persistir marcadores, validar artefactos y convertir reglas en tareas obligatorias antes del cierre.
- [Documentación vacía o plantilla no terminada] → Validación bloqueante de secciones, contenido mínimo, metadatos, `TBD`, instrucciones de plantilla y checklists abiertos.
- [Falsos positivos por validación textual] → Definir contratos por tipo documental generado y no inspeccionar el código legacy globalmente.
- [Perfiles excesivamente rígidos para tickets pequeños] → Mantener el perfil explícito y opcional.
- [Divergencia entre los dos comandos de inicio] → Ambos delegan en el mismo handler y comparten pruebas de paridad.
- [Cambios de manifiesto rompan artefactos históricos] → Aplicar campos aditivos y activar reglas solo si existe `architectureProfile`.
- [Reglas de datos provoquen una abstracción genérica inadecuada] → Exigir infraestructura compartida, pero repositorios propios por dominio.

## Migration Plan

1. Implementar el catálogo de perfiles, el parser `--profile` y el alias de comando, con pruebas de argumentos válidos e inválidos.
2. Propagar el perfil a los builders de propuesta, diseño, especificación, tareas y manifiesto.
3. Extender la generación documental y la validación con contratos estrictos por archivo para todos los cambios con manifiesto.
4. Extender la validación con el bloque `architectureProfile` cuando el perfil esté declarado.
5. Crear pruebas de generación y validación documental/perfil, y pruebas de compatibilidad sin perfil/manifiesto histórico.
6. Ejecutar la suite de `tools/opsxj`; publicar el perfil como opción de uso para nuevos tickets modernizadores.
7. Si surge una regresión, retirar el uso del perfil de nuevos tickets o desactivar sus checks de perfil; los cambios ya creados y el comportamiento base permanecen intactos.

## Open Questions

- Ninguna. Los requisitos de arquitectura se aplican como perfil explícito y las decisiones de capacidad concreta se documentarán dentro de cada cambio que lo adopte.
