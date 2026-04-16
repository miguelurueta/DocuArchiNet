## Context

`SCRUMCORE-123` corresponde a la fase 17 FE de `AppEditor`, enfocada en
introducir una experiencia visual de `Guardar` con dirty state real, sin
persistencia backend en esta etapa.

`AppEditor` ya soporta:
- edicion enriquecida basada en HTML serializado;
- modo controlled/uncontrolled;
- `headerActions` para acciones contextuales del shell;
- toolbar propia para acciones de formato;
- paginacion visual multi-hoja;
- zoom visual;
- contador de pagina;
- imagenes y `PageBreak`.

El problema actual no es de capacidad de edicion, sino de ausencia de una señal
visual de guardado. Hoy el usuario puede modificar contenido, pero no tiene una
referencia clara de si existen cambios pendientes. El nuevo comportamiento debe
resolver eso sin acoplar `AppEditor` a persistencia real ni a una implementacion
especifica de backend.

La referencia principal para esta fase es
`docs/Architecture/AppEditor/17-FE-AppEditor-boton-guardar-dirty-state.md`.

## Goals / Non-Goals

**Goals:**
- Exponer un boton `Guardar` visible fuera de la toolbar de formato.
- Mostrar estado gris cuando no hay cambios pendientes y negro cuando el
  contenido difiere del ultimo baseline guardado.
- Resolver dirty state por comparacion real entre `currentValue` y `savedValue`.
- Introducir una normalizacion reutilizable para HTML vacio equivalente.
- Soportar guardado simulado actualizando baseline local sin backend.
- Dejar una estructura preparada para futura integracion con persistencia real.

**Non-Goals:**
- No implementar llamadas a API, base de datos ni backend.
- No mover la responsabilidad de persistencia dentro de `AppEditor`.
- No mezclar `Guardar` con la toolbar de formato del editor.
- No cambiar la arquitectura de paginacion, zoom o contenido del editor.
- No introducir heuristicas tipo "si escribio algo entonces dirty".

## Decisions

1. **Mantener `AppEditor` agnostico a persistencia**
   - **Decision:** El dirty state y el baseline guardado deben vivir fuera de
     `AppEditor`, en el contenedor consumidor o en un hook de application del
     modulo que lo use.
   - **Rationale:** El brief del ticket lo exige de forma explicita. `AppEditor`
     ya es un componente shared UI; meterle persistencia o estado de guardado
     lo haria demasiado opinionado.
   - **Alternatives considered:** Guardar `savedValue` e `isDirty` dentro del
     componente. Se descarta por acoplar UI compartida y politica de negocio.

2. **Usar `headerActions` como punto natural para el boton `Guardar`**
   - **Decision:** La accion visible de `Guardar` debe integrarse en
     `headerActions` del shell del editor o en el contenedor inmediato, nunca
     dentro de la toolbar de formato.
   - **Rationale:** `headerActions` ya existe precisamente para acciones
     contextuales del shell. El boton `Guardar` pertenece al flujo de pantalla,
     no al set de herramientas de edicion.
   - **Alternatives considered:** Incluir `Guardar` en la toolbar. Se descarta
     por mezclar persistencia de pantalla con comandos del editor.

3. **Dirty state basado en comparacion de baseline**
   - **Decision:** El estado dirty debe derivarse exclusivamente de la
     comparacion entre `normalize(currentValue)` y `normalize(savedValue)`.
   - **Rationale:** Cualquier heuristica mas simple genera falsos positivos o
     falsos negativos, especialmente en un editor HTML donde hay serializaciones
     equivalentes.
   - **Alternatives considered:** Marcar dirty en el primer `onChange` y
     resetearlo manualmente. Se descarta por no representar correctamente el
     contenido real.

4. **Introducir una normalizacion compartida de HTML**
   - **Decision:** La comparacion debe apoyarse en una funcion de normalizacion
     compartida que trate como equivalentes `""`, `<p></p>` y `<p><br></p>`,
     ademas de aplicar trimming basico.
   - **Rationale:** El editor puede producir distintas representaciones de
     contenido vacio. Sin normalizacion, el dirty state parpadearia o quedaria
     activado sin un cambio real.
   - **Alternatives considered:** Comparar el HTML bruto. Se descarta por ser
     fragil frente a vacios equivalentes y pequeñas variaciones de serializacion.

5. **Simular guardado actualizando baseline local**
   - **Decision:** En esta fase, `Guardar` solo debe actualizar `savedValue`
     con el `currentValue` ya normalizado o derivado localmente, sin efectos
     externos de persistencia.
   - **Rationale:** El ticket pide dejar la UX y la estructura listas antes de
     conectar backend real.
   - **Alternatives considered:** Introducir desde ya un mock de API. Se
     descarta por agregar complejidad que el ticket explicitamente no necesita.

6. **Preparar un contrato futuro sin implementarlo**
   - **Decision:** El diseño debe anticipar una extension futura tipo
     `saveDraft?: (html: string) => Promise<void>`, pero sin cablearla todavia.
   - **Rationale:** El cambio debe quedar listo para evolucionar hacia backend
     sin rehacer la logica de dirty state o la ubicacion del boton.
   - **Alternatives considered:** No pensar en contrato futuro hasta el ticket
     backend. Se descarta porque obligaria a rehacer la interfaz del contenedor.

7. **Sincronizar baseline con cambios externos**
   - **Decision:** Si el valor controlado cambia externamente por una carga de
     datos o refresh, ese nuevo valor debe pasar a ser el baseline guardado y
     resetear `isDirty`.
   - **Rationale:** En una integracion real el usuario recibira contenido desde
     backend. Si ese cambio no sincroniza baseline, la UI mostraria dirty sin
     que el usuario haya editado nada.
   - **Alternatives considered:** Mantener baseline fijo despues de montar el
     componente. Se descarta por generar falsos positivos tras rehidratacion.

## Risks / Trade-offs

- [Riesgo] Si la normalizacion de HTML vacio es insuficiente, el boton puede
  cambiar de gris a negro sin cambios reales.
  Mitigacion: centralizar la normalizacion y cubrirla con pruebas especificas.

- [Riesgo] Implementar dirty state dentro del shared component haria dificil
  conectar futuros flujos con backend y distintos modulos consumidores.
  Mitigacion: forzar desde el diseño que la logica viva fuera de `AppEditor`.

- [Riesgo] Cambios externos de `value` pueden confundirse con ediciones del
  usuario si no se sincroniza `savedValue`.
  Mitigacion: tratar cambios externos como nueva fuente de verdad del baseline.

- [Riesgo] El boton `Guardar` podria invadir visualmente el shell del editor si
  no se integra bien con `headerActions`.
  Mitigacion: usar el slot ya previsto para acciones contextuales del encabezado.

## Migration Plan

- Definir una funcion compartida de normalizacion para HTML serializado del
  editor.
- Implementar un contenedor consumidor o hook de application que gestione
  `currentValue`, `savedValue`, `isDirty` y el guardado simulado.
- Integrar el boton `Guardar` a traves de `headerActions` o el shell inmediato
  del modulo consumidor.
- Asegurar que cambios externos de `value` reseteen baseline y dirty state.
- Ampliar pruebas para estado inicial, dirty state, guardado simulado,
  re-edicion y equivalencia de HTML vacio.

## Open Questions

- ¿Conviene dejar desde ya un hook reusable del tipo `useEditorSaveState` en la
  capa application del modulo consumidor, o basta con resolverlo localmente en
  el primer contenedor que use el flujo?
- ¿El estado visual negro del boton debe convivir despues con variantes
  adicionales como `saving` o `error`, o ese refinamiento se reserva por
  completo para la fase backend?
