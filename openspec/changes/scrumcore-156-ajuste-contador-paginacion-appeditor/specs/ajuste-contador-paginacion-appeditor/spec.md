## ADDED Requirements

### Requirement: Contador de pagina estable y desacoplado del motor de repaginacion
El sistema SHALL calcular y mostrar el contexto de pagina actual de `AppEditor`
sin depender de mutaciones destructivas del documento ni de una sincronizacion
fragil entre scroll, seleccion y `pageBreak` automaticos.

#### Scenario: La pagina actual se calcula de forma estable durante scroll
- **WHEN** el usuario navega un documento en `paginationMode="visual"`
- **THEN** el sistema SHALL derivar la pagina actual desde un modelo estable de
  layout y scroll sin jitter ni rebotes visibles del contador

#### Scenario: La pagina actual no salta durante typing
- **WHEN** el usuario escribe cerca del final de una hoja
- **THEN** el contador SHALL mantenerse coherente con la hoja visual activa sin
  cambiar de forma erratica durante la escritura

### Requirement: El contador debe integrarse como soporte UX discreto
El sistema SHALL exponer el contador de pagina como una ayuda visual compacta,
sin competir con la edicion ni introducir acoplamientos innecesarios con la
toolbar o el flujo de contenido.

#### Scenario: El contador se presenta en una ubicacion compacta y consistente
- **WHEN** `AppEditor` se renderiza en modo visual paginado
- **THEN** el contador SHALL mostrarse como un elemento discreto y legible,
  alineado con la experiencia general del editor

#### Scenario: El contador no interfiere con la edicion
- **WHEN** el usuario interactua con el contenido, scroll o seleccion
- **THEN** el contador SHALL permanecer informativo y no bloquear acciones de
  escritura, foco o seleccion

### Requirement: El ajuste del contador no debe degradar la experiencia de hojas
El sistema SHALL preservar la visual de hojas en `paginationMode="visual"`
mientras ajusta el comportamiento del contador de pagina.

#### Scenario: Las hojas siguen visibles mientras el contador se ajusta
- **WHEN** el editor se usa en modo visual
- **THEN** la representacion de paginas SHALL conservarse y el contador SHALL
  reflejar esa estructura sin forzar una hoja infinita sin referencia visual

#### Scenario: El contador sigue la hoja visible activa
- **WHEN** el usuario cambia de hoja por scroll o por continuidad de escritura
- **THEN** el contador SHALL actualizarse hacia la hoja actualmente visible de
  forma consistente

### Requirement: El contador no debe introducir regresiones en modos ni capacidades existentes
El sistema SHALL introducir el ajuste de contador sin romper zoom, modo
continuo, serializacion HTML, imagenes, listas ni el contrato reusable de
`AppEditor`.

#### Scenario: Modo continuo permanece sin contador paginado forzado
- **WHEN** `AppEditor` se usa con `paginationMode="none"`
- **THEN** el sistema MUST NOT forzar contador de pagina ni logica de contexto
  paginado sobre el flujo continuo

#### Scenario: Zoom visual sigue siendo compatible
- **WHEN** el usuario modifica el zoom visual en modo paginado
- **THEN** el contador SHALL mantenerse alineado con la hoja efectiva sin
  desincronizarse del layout visible

#### Scenario: Serializacion HTML permanece limpia
- **WHEN** el contenido del editor se guarda o vuelve a cargarse
- **THEN** el HTML SHALL permanecer libre de metadata espuria derivada solo del
  calculo del contador de pagina
