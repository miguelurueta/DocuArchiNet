## Context

`SCRUMCORE-111` corresponde a la fase 10 FE de `AppEditor`, enfocada en
simplificar la toolbar eliminando el toggle visible de tema y optimizando la
disposicion responsive para mobile y tablet.

`AppEditor` ya soporta:
- toolbar con acciones de formato, estructura, alineacion, historial e insercion;
- integracion con theming via atributos y estilos;
- comportamiento reusable dentro de modulos como gestion de correspondencia.

El problema actual no es funcional sino de UX y densidad visual:
- el toggle de tema manual ya no se necesita;
- la toolbar ocupa demasiado espacio en anchos reducidos;
- el contenido editable pierde area util en mobile y tablet.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/10-FE-AppEditor-toolbar-responsive-sin-toggle-tema.md`.

## Goals / Non-Goals

**Goals:**
- Eliminar el boton visible de cambio manual `dark/light`.
- Mantener compatibilidad con el sistema de tema global.
- Compactar la toolbar en `667px`, `896px`, `932px` y `1024px`.
- Mantener visibles las acciones principales.
- Colapsar acciones secundarias cuando el espacio lo exija.
- Mejorar el area util del editor sin romper desktop.

**Non-Goals:**
- No rediseñar por completo `AppEditor`.
- No eliminar soporte de tema del componente.
- No introducir breaking changes innecesarios en la API.
- No cambiar comandos del editor ni la instancia de Tiptap.

## Decisions

1. **Quitar solo la affordance visual del tema**
   - **Decision:** Eliminar el boton del toolbar, pero conservar compatibilidad
     con `themeMode` y/o cualquier wiring externo aun usado por el sistema.
   - **Rationale:** El requerimiento es remover control manual, no deshabilitar
     theming. El editor debe seguir respondiendo al tema global.
   - **Alternatives considered:** Eliminar por completo props y estilos de tema.
     Se descarta por riesgo de breaking change y por desacoplarlo del sistema global.

2. **Mantener acciones principales visibles**
   - **Decision:** `bold`, `italic`, `undo` y `redo` deben seguir visibles en
     resoluciones reducidas.
   - **Rationale:** Son las acciones de uso mas frecuente y no deben quedar
     ocultas detras de menus secundarios.
   - **Alternatives considered:** Colapsar toda la toolbar en un solo menu. Se
     descarta por degradar usabilidad y velocidad de edicion.

3. **Colapsar acciones secundarias por grupos**
   - **Decision:** Acciones de menor prioridad como insercion, estructura o
     alineacion podran agruparse o compactarse en dropdowns responsivos.
   - **Rationale:** El toolbar ya usa dropdowns y grupos; extender ese patron es
     mas coherente que crear un layout nuevo.
   - **Alternatives considered:** Wrap libre en multiples filas. Se descarta
     porque genera barras demasiado altas e impredecibles.

4. **Resolver responsive principalmente en `presentation` + CSS**
   - **Decision:** Mantener los comandos donde ya estan y modificar
     composicion/agrupacion en `AppEditorToolbar.tsx` y `AppEditor.module.css`.
   - **Rationale:** El problema es de representacion y layout, no del modelo del
     editor.
   - **Alternatives considered:** Mover decisiones responsive a capas de dominio
     o infraestructura. Se descarta por sobreingenieria.

5. **Proteger foco y estabilidad del editor**
   - **Decision:** Cualquier colapso o boton nuevo debe seguir el patron actual
     del toolbar para no recrear la instancia de Tiptap ni perder foco.
   - **Rationale:** La UX de un editor se rompe rapido si el foco salta o si el
     contenido deja de responder tras interaccion con la barra.
   - **Alternatives considered:** Re-renderizar el toolbar con estrategias mas
     agresivas. Se descarta por riesgo de regresion.

## Risks / Trade-offs

- [Riesgo] Compactar demasiado puede volver ambiguas algunas acciones.
  Mitigacion: mantener iconos claros, labels cortos y priorizacion explicita.

- [Riesgo] Eliminar el toggle podria dejar props de tema aparentemente
  inconsistentes si no se revisa bien el contrato.
  Mitigacion: conservar compatibilidad API y limpiar solo la parte visual/manual.

- [Riesgo] Cambios responsive pueden afectar desktop de forma no intencional.
  Mitigacion: aislar estilos por breakpoints y validar regresion en desktop.

- [Riesgo] Dropdowns adicionales pueden introducir friccion tactil.
  Mitigacion: colapsar solo secundarios y mantener tamanos interactivos adecuados.

## Migration Plan

- Revisar el contrato de tema actual de `AppEditor`.
- Remover el boton visible del tema en `AppEditorToolbar`.
- Ajustar `AppEditor` para no depender del toggle manual.
- Reorganizar grupos de toolbar para responsive.
- Ajustar `AppEditor.module.css` con reglas especificas para mobile y tablet.
- Actualizar pruebas del toolbar y del editor.

## Open Questions

- ¿Conviene un unico dropdown de acciones secundarias o varios colapsos pequeños
  segun grupo funcional?
- ¿En `1024px` la toolbar debe comportarse ya como tablet compacta o mantener un
  layout cercano a desktop?
