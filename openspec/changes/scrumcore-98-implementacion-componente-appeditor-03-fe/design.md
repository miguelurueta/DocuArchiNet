## Context

`SCRUMCORE-98` corresponde a la fase 03 FE de `AppEditor`, enfocada en
consolidar accesibilidad, documentacion, pruebas e integracion final del
componente ya implementado en las fases 01 y 02. El componente existe en
`src/app/Components/UI/AppEditor/`, por lo que esta fase no busca ampliar su
alcance funcional sino elevar su nivel de calidad y prepararlo para uso
productivo en la capa shared UI.

La referencia principal es
`docs/Architecture/AppEditor/03-FE-AppEditor-accessibility-testing.md`, junto
con la implementacion actual del componente y sus pruebas existentes.

## Goals / Non-Goals

**Goals:**
- Consolidar accesibilidad basica y avanzada del componente.
- Completar y alinear el README con el comportamiento real de `AppEditor`.
- Reforzar pruebas por capas para toolbar, shell del editor y `useAppEditor`.
- Verificar integracion limpia del componente con exports shared y contextos reales de uso.
- Registrar evidencia de validacion para dejar el componente listo para cierre del ticket.

**Non-Goals:**
- No cambiar el contrato base del componente.
- No introducir un editor distinto de Tiptap ni nuevas dependencias fuera del alcance.
- No modificar `domain` o `infrastructure` salvo que surja un defecto bloqueante.
- No abrir una migracion masiva de modulos consumidores en esta fase.

## Decisions

1. **Fase orientada a consolidacion y no a nuevas features**
   - **Decision:** Priorizar pruebas, accesibilidad, README e integracion por encima de nuevas capacidades funcionales.
   - **Rationale:** Las fases 01 y 02 ya resolvieron core y UI/UX; esta fase debe asegurar robustez y readiness para produccion.
   - **Alternatives considered:** Agregar nuevas herramientas o extensiones al editor. Se descarta por ampliar el alcance sin necesidad.

2. **Mantener separacion estricta por capas en el testing**
   - **Decision:** Las pruebas seguiran separadas entre `presentation` y `application`, evitando acoplarlas a internals de Tiptap.
   - **Rationale:** La documentacion de la fase 03 exige respetar Clean Architecture tambien en pruebas.
   - **Alternatives considered:** Centralizar pruebas end-to-end del editor en un solo archivo. Se descarta porque mezcla responsabilidades y dificulta diagnostico.

3. **Completar la documentacion como contrato operativo**
   - **Decision:** El README de `AppEditor` debe pasar de descripcion minima a documentacion de uso real con variantes y limites conocidos.
   - **Rationale:** En una shared UI, la documentacion es parte del contrato y reduce integraciones incorrectas.
   - **Alternatives considered:** Mantener el README actual y depender de OpenSpec. Se descarta porque OpenSpec no reemplaza la documentacion de consumo del componente.

4. **Validar integracion shared antes de cierre**
   - **Decision:** Verificar exports y, cuando sea posible, una integracion representativa dentro de un formulario o contenedor real.
   - **Rationale:** La calidad del componente no depende solo de pruebas unitarias; tambien depende de que no rompa layout, submit o consumo desde la capa UI compartida.
   - **Alternatives considered:** Cerrar el ticket solo con pruebas focalizadas del componente. Se descarta por insuficiente para la fase 03.

## Risks / Trade-offs

- [Riesgo] La cobertura de pruebas puede seguir siendo funcional pero no capturar todos los detalles de accesibilidad avanzada.
  Mitigacion: reforzar assertions de roles, labels, estados y orden de foco en presentation.

- [Riesgo] La documentacion puede desviarse del comportamiento real si el README se actualiza sin verificar implementacion.
  Mitigacion: alinear README despues de revisar props y ejemplos reales del componente.

- [Riesgo] La integracion con layouts o formularios reales puede revelar acoplamientos no visibles en pruebas aisladas.
  Mitigacion: incluir al menos una validacion de integracion representativa antes de cierre.

- [Riesgo] Existen errores TypeScript heredados en el repo que pueden contaminar la validacion global.
  Mitigacion: registrar explicitamente los residuos ajenos al componente y aislar la evidencia focalizada de `AppEditor`.

## Migration Plan

- Partir de la implementacion actual de `AppEditor`.
- Revisar README y completar ejemplos y restricciones de uso.
- Extender pruebas focalizadas de `AppEditor`, `AppEditorToolbar` y `useAppEditor`.
- Verificar export publico y una integracion representativa.
- Registrar evidencia final en `tasks.md`.

## Open Questions

- ¿La validacion de integracion debe quedarse en test representativo o se espera una adopcion real en un modulo del producto durante esta fase?
- ¿Conviene dejar explicitadas limitaciones conocidas de shortcuts o screen readers en el README actual?
