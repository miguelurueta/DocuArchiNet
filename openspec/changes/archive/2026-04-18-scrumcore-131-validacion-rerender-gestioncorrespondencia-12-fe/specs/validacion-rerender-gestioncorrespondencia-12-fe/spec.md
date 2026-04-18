## ADDED Requirements

### Requirement: Auditoria tecnica focalizada de GestionCorrespondencia
El sistema SHALL contar con una auditoria tecnica focalizada de `GestionCorrespondencia` que cubra exclusivamente `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx` y su arbol inmediato de consumo.

#### Scenario: Alcance tecnico acotado
- **WHEN** se ejecute la auditoria del cambio `SCRUMCORE-131`
- **THEN** el analisis SHALL revisar el componente objetivo, sus hijos directos y los props, callbacks, objetos y hooks que fluyen desde ese componente

#### Scenario: Componentes y contratos incluidos
- **WHEN** se documente el alcance de la revision
- **THEN** el entregable SHALL incluir `AppToolbar`, `AppInputSearch`, `AppButton`, `AppContent`, `AppTableQueryWrapper`, `AppTableExport`, `AppTable`, `useWorkflowInboxAutocomplete` y `GestionCorrespondenciaTableResult`

### Requirement: Diagnostico de rerender basado en evidencia
El sistema SHALL diferenciar entre hallazgos confirmables por lectura de codigo y hallazgos que requieren medicion para demostrar impacto real.

#### Scenario: Hallazgo confirmable por codigo
- **WHEN** la causa del rerender pueda demostrarse por identidad referencial, estado local o composicion inline visible en `GestionCorrespondencia.tsx`
- **THEN** el analisis SHALL marcar el hallazgo como confirmable por lectura de codigo

#### Scenario: Hallazgo dependiente de medicion
- **WHEN** el impacto dependa del comportamiento interno de hijos, memoizacion real o frecuencia observable en runtime
- **THEN** el analisis SHALL clasificarlo como requeriente de profiling y no como problema confirmado

### Requirement: Identificacion de causas tecnicas concretas de rerender
El sistema SHALL identificar causas tecnicas concretas de rerender dentro del flujo de `GestionCorrespondencia`.

#### Scenario: Estado local que amplia el rerender
- **WHEN** `selectedRows` o `searchDraft` cambien y disparen rerender del page component
- **THEN** el analisis SHALL explicar que partes del arbol reciben un nuevo ciclo de render y si ese efecto es esperado o evitable

#### Scenario: Props y closures inestables
- **WHEN** existan callbacks recreadas, objetos inline, JSX inline o derivados nuevos por render
- **THEN** el analisis SHALL documentar la pieza afectada, la causa tecnica y el impacto esperado sobre hijos directos como `AppTableExport` o `AppTable`

#### Scenario: Datos y contratos posiblemente inestables
- **WHEN** `table.rows`, `table.columns`, `table.queryState` o funciones del contrato `table` puedan cambiar de referencia sin cambio semantico
- **THEN** el analisis SHALL indicar el riesgo, su origen probable y si la confirmacion requiere revisar el hook proveedor o hacer profiling

### Requirement: Clasificacion explicita de rerenders y falsos positivos
El sistema SHALL clasificar los rerenders detectados en categorias operables para evitar sobre-optimizacion.

#### Scenario: Rerender esperado
- **WHEN** un rerender responda a escritura de busqueda, seleccion de filas, carga de datos o navegacion esperada
- **THEN** el analisis SHALL marcarlo como rerender esperado y explicar por que no constituye necesariamente un problema

#### Scenario: Rerender sospechoso o claramente evitable
- **WHEN** una pieza reciba nuevas referencias sin aportar valor funcional
- **THEN** el analisis SHALL clasificarla como sospecha razonable o como rerender claramente evitable segun la evidencia disponible

#### Scenario: Falso positivo comun en React
- **WHEN** una optimizacion aparente aumente complejidad sin beneficio observable o se base en una suposicion comun pero imprecisa
- **THEN** el analisis SHALL descartarla explicitamente como falso positivo comun en React

### Requirement: Recomendaciones priorizadas con costo, beneficio y riesgo
El sistema SHALL producir recomendaciones priorizadas y accionables para optimizar rerenders sin degradar mantenibilidad.

#### Scenario: Recomendacion accionable
- **WHEN** el analisis detecte una oportunidad real de mejora
- **THEN** cada recomendacion SHALL incluir severidad, zona afectada, causa tecnica, impacto esperado, costo de implementacion, beneficio esperado y riesgo de regresion

#### Scenario: Optimizacion no justificada
- **WHEN** una optimizacion dependa de `React.memo`, `useMemo` o `useCallback` sin evidencia clara de impacto
- **THEN** el analisis SHALL indicar explicitamente que no vale la pena implementarla

### Requirement: Estrategia de medicion y validacion
El sistema SHALL definir como validar cada optimizacion propuesta antes y despues de cualquier refactor.

#### Scenario: Profiling dirigido
- **WHEN** un hallazgo requiera medicion
- **THEN** el plan SHALL sugerir uso de React DevTools Profiler y, si aplica, `why-did-you-render`, indicando que comportamiento deberia observarse

#### Scenario: Validacion funcional sin regresion
- **WHEN** se proponga una optimizacion sobre `GestionCorrespondencia`
- **THEN** la estrategia SHALL indicar que validar con tests existentes, que tests nuevos conviene agregar y que comparar antes y despues en profiling
