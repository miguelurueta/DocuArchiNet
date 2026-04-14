## ADDED Requirements

### Requirement: Validacion final de accesibilidad de AppEditor
El sistema SHALL validar que `AppEditor` mantenga una experiencia accesible en su toolbar, superficie editable y estados auxiliares, sin romper la API definida en fases previas.

#### Scenario: Toolbar navegable y etiquetada
- **WHEN** el usuario navega por teclado sobre la toolbar de `AppEditor`
- **THEN** los grupos de acciones y los botones icon-only SHALL exponer roles y `aria-label` consistentes

#### Scenario: Integracion accesible de label, helper y error
- **WHEN** una vista provee `label`, `helperText` y `error`
- **THEN** `AppEditor` SHALL mantener asociacion accesible entre esos elementos y la superficie editable

#### Scenario: Estados blocked accesibles
- **WHEN** `AppEditor` se renderiza con `disabled` o `readOnly`
- **THEN** la superficie editable y la toolbar SHALL conservar semantica accesible y feedback coherente

### Requirement: Documentacion consolidada del componente
El sistema SHALL contar con un `README.md` completo para `AppEditor`, alineado con el comportamiento real del componente y con ejemplos de uso representativos.

#### Scenario: README con modos de uso
- **WHEN** el desarrollador consulta `src/app/Components/UI/AppEditor/README.md`
- **THEN** encuentra descripcion, props, ejemplo basico, ejemplo controlado, ejemplo con `disabled`/`readOnly`, buenas practicas y limitaciones conocidas

### Requirement: Cobertura de pruebas por capas
El sistema SHALL mantener pruebas separadas por capas (`presentation`, `application`, `infrastructure`) para validar comportamiento, regresion y estabilidad del componente.

#### Scenario: Pruebas de presentation
- **WHEN** se ejecutan las pruebas de `AppEditor` y `AppEditorToolbar`
- **THEN** se valida render, estados visibles, toolbar, accesibilidad y comportamientos observables sin depender de internals de Tiptap

#### Scenario: Pruebas del hook
- **WHEN** se ejecutan las pruebas de `useAppEditor`
- **THEN** se valida sincronizacion controlled/uncontrolled, estabilidad de contenido y estados `disabled`/`readOnly`

### Requirement: Integracion shared UI estable
El sistema SHALL mantener export publico de `AppEditor` y compatibilidad con el resto de la capa shared UI sin introducir regresiones de integracion.

#### Scenario: Export estable
- **WHEN** un consumidor importa `AppEditor` desde `src/app/Components/UI/index.ts`
- **THEN** el componente SHALL permanecer disponible sin romper exports compartidos existentes

#### Scenario: Integracion con formulario o contenedor real
- **WHEN** `AppEditor` se usa dentro de un formulario o layout real del proyecto
- **THEN** el componente SHALL conservar submit, layout y serializacion de contenido sin afectar otros componentes UI
