## Context

`SCRUMCORE-96` busca implementar el core reusable de `AppEditor` como componente
shared de UI, tomando como referencia la superficie actual usada en
`GestionRespuesta` y la arquitectura definida en
`docs/Architecture/AppEditor/AppEditor-Architecture.md` y
`docs/Architecture/AppEditor/01-FE-AppEditor-core.md`.

Actualmente el repositorio solo tiene un contenedor visual acoplado al modulo en
`src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaEditorContainer.tsx`.
Ese contenedor resuelve encabezado, superficie principal y scroll interno, pero
no expone una API reusable ni un editor enriquecido desacoplado del dominio.

## Goals / Non-Goals

**Goals:**
- Crear `AppEditor` en `src/app/Components/UI/AppEditor/` como componente shared reusable.
- Implementar una base de edicion enriquecida con Tiptap MIT y API tipada.
- Mantener separacion por capas (`domain`, `application`, `infrastructure`, `presentation`).
- Soportar layout dominante, scroll interno, modos controlado/no controlado y toolbar base.
- Dejar el componente listo para adopcion progresiva en modulos consumidores.

**Non-Goals:**
- No incorporar reglas de negocio ni persistencia remota.
- No acoplar el componente a `GestionRespuesta` ni a un modulo especifico.
- No resolver en este ticket la fase completa de UI/UX dark mode documentada en `02-FE-AppEditor-ui-ux.md`.
- No integrar aun proveedores externos distintos de Tiptap ni extensiones con licencias no MIT.

## Decisions

1. **Implementar `AppEditor` en la capa shared UI**
   - **Decision:** El componente vivira en `src/app/Components/UI/AppEditor/` y no dentro de `src/modules`.
   - **Rationale:** El ticket define un reusable shared; mantenerlo fuera de los modulos evita acoplamiento y facilita adopcion transversal.
   - **Alternatives considered:** Extender `GestionRespuestaEditorContainer` dentro del modulo. Se descarta porque perpetua dependencias de dominio y complica reutilizacion.

2. **Separar la solucion por capas y aislar Tiptap**
   - **Decision:** La integracion con Tiptap quedara encapsulada en `infrastructure`, la logica de orquestacion en `application/useAppEditor`, y la UI en `presentation`.
   - **Rationale:** La documentacion del ticket prohibe el uso directo de Tiptap en `presentation/AppEditor.tsx`; esta separacion mantiene testabilidad y control de dependencias.
   - **Alternatives considered:** Implementar todo en un unico `AppEditor.tsx`. Se descarta por mezclar UI, estado y proveedor de editor en un solo punto.

3. **Preservar el patron estructural del contenedor actual**
   - **Decision:** `AppEditor` reutilizara el patron de encabezado contextual + superficie principal con scroll interno, evolucionandolo a un editor enrichido reusable.
   - **Rationale:** Ese patron ya existe en `GestionRespuestaEditorContainer` y responde al layout esperado del workbench.
   - **Alternatives considered:** Diseñar un editor full-screen o sin encabezado. Se descarta porque rompe la referencia arquitectonica ya aprobada.

4. **Soportar API controlada y no controlada**
   - **Decision:** El contrato del componente soportara `value` + `onChange` y tambien estado interno cuando `value` no se provea.
   - **Rationale:** El ticket exige ambas modalidades y estabilidad frente a cambios externos sin perder cursor ni historial.
   - **Alternatives considered:** Forzar un modelo exclusivamente controlado. Se descarta porque reduce flexibilidad de adopcion y complica casos locales simples.

5. **Cubrir un toolbar base alineado al alcance core**
   - **Decision:** El toolbar incluira formato basico, listas, headings, alineacion, undo/redo, enlaces e imagenes como parte del core.
   - **Rationale:** Esas capacidades estan explicitamente pedidas en la documentacion del ticket y definen la viabilidad del componente como editor reusable.
   - **Alternatives considered:** Dejar toolbar minimo y posponer enlaces/imagenes. Se descarta porque dejaria incompleto el alcance funcional esperado de la fase core.

## Risks / Trade-offs

- [Riesgo] Integrar Tiptap puede introducir complejidad de estado y sincronizacion controlada.
  Mitigacion: encapsular la instancia en `useAppEditor` y cubrir controlled/uncontrolled con pruebas dedicadas.

- [Riesgo] El alcance documental mezcla layout reusable y rich text editor completo.
  Mitigacion: usar `AppEditor-Architecture.md` para el layout y `01-FE-AppEditor-core.md` para el contrato y capacidades del editor.

- [Riesgo] La referencia de UI/UX avanzada incluye dark mode y refinamientos visuales fuera del core.
  Mitigacion: limitar este ticket a una base visual consistente y dejar extensiones visuales avanzadas para la siguiente fase.

- [Riesgo] Insercion de imagenes y links puede empujar decisiones de UX no cerradas.
  Mitigacion: implementar interacciones basicas y documentar cualquier restriccion pendiente en `tasks.md` o en futuras fases.

## Migration Plan

- Crear `AppEditor` como componente nuevo en la capa shared.
- Mantener `GestionRespuestaEditorContainer` sin reemplazo inmediato durante la fase core.
- Validar el contrato del nuevo componente con pruebas unitarias antes de migrar consumidores.
- Dejar la integracion del primer modulo consumidor como trabajo posterior o fase siguiente.

## Open Questions

- ¿La insercion de imagenes debe limitarse a URLs/remotas o incluir carga local desde la primera version?
- ¿El toolbar de enlaces e imagenes requiere UI modal/popover especifica o basta una interaccion minima para esta fase?
