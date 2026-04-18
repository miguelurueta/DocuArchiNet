## Context

El proyecto ya cuenta con un componente `AppTabs` (fase 01) como wrapper de Ant Design para pestanas reutilizables. El ticket `SCRUMCORE-84` busca completar la fase 02 FE, estabilizando el contrato, documentando su uso y preparando el terreno para adopciones futuras sin romper el flujo actual ni introducir acoplamientos con modulos concretos.

Actualmente no hay uso extensivo de `Tabs` en modulos del repositorio, por lo que esta fase debe enfocarse en consolidar la API reusable, su documentacion y cobertura de pruebas para habilitar integraciones futuras.

## Goals / Non-Goals

**Goals:**
- Consolidar el contrato reusable del componente de tabs (API estable, tipada y consistente con la fase 01).
- Documentar el componente con ejemplos de uso y reglas de integracion.
- Asegurar cobertura de pruebas de comportamiento para los escenarios clave del contrato (controlado/no controlado, bloqueo, accesibilidad).
- Mantener la implementacion encapsulada (sin estilos globales, sin dependencias nuevas).

**Non-Goals:**
- No reescribir el router ni flujos de navegacion para tabs.
- No migrar modulos existentes a `AppTabs` en esta fase, salvo que el alcance lo especifique explicitamente.
- No introducir dependencias externas ni cambios de arquitectura UI.

## Decisions

1. **Reutilizar `AppTabs` como componente unico**
   - **Decision:** Mantener `AppTabs` como el componente base y evitar crear un wrapper paralelo tipo `AppApptabs02Fe`.
   - **Rationale:** Reduce duplicacion, evita fragmentar la API y mantiene continuidad con la fase 01.
   - **Alternatives considered:** Crear un nuevo componente `AppApptabs02Fe` y dejar `AppTabs` como legado. Se descarta por riesgo de divergencia y sobrecosto de mantenimiento.

2. **Mantener el contrato tipado y controlado/no controlado**
   - **Decision:** Preservar la API existente (`items`, `activeKey`, `defaultActiveKey`, `beforeChange`, `disabled`) como base del contrato de fase 02.
   - **Rationale:** Es el nucleo del comportamiento reusable y ya esta validado por pruebas de fase 01.
   - **Alternatives considered:** Simplificar la API para la fase 02. Se descarta porque reduce flexibilidad y afecta adopcion futura.

3. **Documentacion y ejemplo de integracion**
   - **Decision:** Completar README del componente con ejemplos representativos, sin acoplarlo a modulos de negocio.
   - **Rationale:** Facilita adopcion sin introducir dependencias de dominio ni cambios en rutas.
   - **Alternatives considered:** Integrar el componente en un modulo real en esta fase. Se pospone hasta que exista un flujo de negocio claro.

## Risks / Trade-offs

- [Risk] Ambiguedad entre el nombre del ticket y el componente real (AppTabs vs AppApptabs02Fe) -> Mitigacion: documentar explicitamente que `AppTabs` es el componente oficial y alinear naming en specs/tasks.
- [Risk] Falta de adopcion inmediata -> Mitigacion: dejar ejemplos claros y contrato estable para futuras migraciones.
- [Risk] Regresion en accesibilidad o foco -> Mitigacion: reforzar pruebas de comportamiento existentes y mantener atributos ARIA.
