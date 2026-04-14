## Context

`SCRUMCORE-99` busca integrar el componente shared `AppEditor` dentro de
`GestionRespuestaEditorContainer`, que hoy actua como shell visual del area
principal del workbench y todavia renderiza un placeholder en lugar de un editor
real. La integracion debe ocurrir dentro de `editorSurface`, preservando el
layout del modulo y la coexistencia con `GestionRespuestaRightToolsPanel`.

Existe una guia especifica del ticket en
`docs/Architecture/AppEditor/04-FE-AppEditor-integracion-gestion-respuesta.md`,
ademas de la implementacion shared de `AppEditor` ya consolidada en fases
previas.

## Goals / Non-Goals

**Goals:**
- Reemplazar el placeholder de `editorSurface` por `AppEditor`.
- Mantener `GestionRespuestaEditorContainer` como shell visual del modulo.
- Usar `AppEditor` en modo controlled dentro del flujo `gestionRespuesta`.
- Preservar layout, responsive y panel lateral del workbench.
- Agregar pruebas de integracion y regresion representativas.

**Non-Goals:**
- No reemplazar todo `GestionRespuestaEditorContainer` por `AppEditor`.
- No acoplar la logica interna de `AppEditor` al modulo `gestionCorrespondencia`.
- No introducir reglas de negocio nuevas del flujo de respuesta.
- No alterar la API compartida de `AppEditor` salvo que sea imprescindible para un modo embebido y sin regresion.

## Decisions

1. **Mantener el shell actual del contenedor**
   - **Decision:** `GestionRespuestaEditorContainer` seguira siendo el wrapper visual que pinta titulo, descripcion y recuadro principal.
   - **Rationale:** El ticket define explicitamente que solo debe reemplazarse el contenido de `editorSurface`, no todo el container.
   - **Alternatives considered:** Sustituir por completo el container con `AppEditor`. Se descarta porque duplicaria responsabilidades y alteraria el layout actual del modulo.

2. **Embebido de AppEditor sin header duplicado**
   - **Decision:** `AppEditor` se integrara en modo embebido dentro de `editorSurface`, evitando repetir el header que ya renderiza el container.
   - **Rationale:** El shell del modulo ya comunica contexto; duplicarlo dentro del editor produciria ruido visual y una jerarquia incorrecta.
   - **Alternatives considered:** Renderizar `AppEditor` completo con `title` y `description`. Se descarta por duplicacion visual.

3. **Estado controlled ubicado en el modulo**
   - **Decision:** El valor del editor vivira en `GestionRespuestaMainTabContent` o en el contenedor, pero en un solo punto de verdad.
   - **Rationale:** El ticket exige modo controlled y prohíbe duplicar estado.
   - **Alternatives considered:** Usar `defaultValue` o dejar el estado dentro de `AppEditor`. Se descarta porque rompe el control esperado del flujo.

4. **Preservar layout del workbench como restriccion primaria**
   - **Decision:** Cualquier ajuste de estilos debe respetar `workbenchBody`, `editorContainer`, `editorSurface` y la convivencia con el panel lateral.
   - **Rationale:** El mayor riesgo no es funcional sino estructural: romper el layout principal del modulo.
   - **Alternatives considered:** Reescribir el layout del workbench para acomodar el editor. Se descarta por sobrealcance.

## Risks / Trade-offs

- [Riesgo] `AppEditor` podria duplicar header o paddings dentro del recuadro existente.
  Mitigacion: integrarlo en modo embebido y revisar cuidadosamente la jerarquia visual.

- [Riesgo] El editor puede afectar la altura o el scroll del workbench.
  Mitigacion: conservar el control del tamano en `editorSurface` y validar scroll interno.

- [Riesgo] El estado controlled puede ubicarse en un punto que genere re-renders amplios.
  Mitigacion: mantener un unico estado local y evitar props innecesarias que desestabilicen el arbol.

- [Riesgo] El panel lateral derecho puede verse afectado por cambios de anchura o overflow.
  Mitigacion: agregar pruebas de integracion del layout con el panel colapsado y expandido.

## Migration Plan

- Mantener `GestionRespuestaEditorContainer` como shell.
- Introducir `AppEditor` dentro de `editorSurface`.
- Subir el estado controlled al punto minimo necesario del modulo.
- Ajustar pruebas del modulo y del editor para cubrir la integracion.
- Registrar evidencia de regresion y calidad en `tasks.md`.

## Open Questions

- ¿El modo embebido de `AppEditor` requiere una prop explicita o basta con renderizarlo sin `title` ni `description`?
- ¿Conviene que el estado controlled viva en `GestionRespuestaMainTabContent` por claridad, aunque el contenedor sea quien renderiza el editor?
