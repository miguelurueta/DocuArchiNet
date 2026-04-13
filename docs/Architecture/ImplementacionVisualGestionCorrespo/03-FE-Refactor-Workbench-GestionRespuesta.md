# PROMPT ARQUITECTONICO

Titulo:
Refactor del primer tab de GestionRespuesta integrando AppToolbar y AppUpload existentes

Rol esperado:
Arquitecto de software senior frontend (React 19, TypeScript estricto, Clean Architecture, componentes reutilizables enterprise)

Objetivo:
Refactorizar el contenido del primer tab de `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx` para adoptar una estructura tipo workbench documental, reutilizando `AppToolbar` y `AppUpload` existentes, sin alterar el sistema de tabs.

Contexto existente:
- La pagina `GestionRespuesta.tsx` ya existe.
- El sistema de tabs ya esta implementado.
- Componentes reutilizables disponibles:
  - `AppToolbar`
  - `AppUpload`
- No se debe crear una nueva pantalla ni reemplazar tabs.
- Se reorganiza solo el contenido renderizado dentro del primer tab.

Objetivo funcional del primer tab:
El primer tab debe renderizar una composicion interna con:
- zona de informacion
- toolbar reutilizando `AppToolbar`
- editor principal
- panel derecho colapsable de herramientas
- zona inferior de adjuntos reutilizando `AppUpload`

Alcance:
- Mantener `GestionRespuesta.tsx` como pagina orquestadora.
- Mantener el contrato actual de tabs.
- Reutilizar `AppToolbar` y `AppUpload` existentes.
- Extraer el contenido del primer tab a un componente desacoplado, mantenible y testeable.
- No afectar el contenido ni comportamiento actual del segundo tab.

Ubicacion base:
- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`

Ubicacion esperada para la nueva composicion:
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/`
- o una ruta equivalente alineada con la arquitectura actual del proyecto.

Estructura esperada:
- `GestionRespuesta.tsx`
  - conserva tabs existentes
  - delega el contenido del primer tab a un componente dedicado

Componente principal sugerido:
- `GestionRespuestaMainTabContent`

Subcomponentes sugeridos:
- `GestionRespuestaInfoHeader`
- `GestionRespuestaEditorContainer`
- `GestionRespuestaRightToolsPanel`

Componentes existentes a reutilizar:
- `AppToolbar`
- `AppUpload`

Layout esperado (obligatorio):
- Estructura base en grid o flex con tres zonas verticales:
  - header informativo
  - cuerpo workbench
  - zona inferior de adjuntos
- Cuerpo workbench con dos columnas:
  - izquierda: editor principal (zona dominante)
  - derecha: panel de herramientas colapsable
- Breakpoints:
  - desktop: 2 columnas (panel visible)
  - tablet: panel derecho colapsable por default
  - mobile: panel derecho colapsado + tabs con overflow horizontal
- Scroll:
  - editor principal con scroll interno
  - panel derecho con scroll interno si hay overflow

Comportamiento del panel derecho:
- Estado inicial: expandido en desktop, colapsado en mobile
- Debe permitir expandir/colapsar con control visible (icono o boton)
- El colapso no debe eliminar el contenido (solo ocultarlo)

Reglas obligatorias:
- No usar `any`
- No logica de negocio en componentes presentacionales
- Mantener separacion estricta de responsabilidades
- No duplicar componentes shared ya existentes
- No acoplar componentes shared al modulo
- El layout debe ser estable, claro y preparado para evolucion
- El refactor debe ser incremental y sin romper funcionalidad existente
- Estilos solo via CSS Modules del modulo (no estilos globales)

Interacciones minimas esperadas:
- El primer tab renderiza el nuevo layout interno
- `AppToolbar` se integra como barra de acciones del workspace
- El editor central ocupa la zona principal
- El panel derecho permite expandir/colapsar
- `AppUpload` permanece integrado en la parte inferior del primer tab
- El segundo tab no se ve afectado

Pruebas obligatorias:
- Unit tests
- UI integration tests
- Browser interaction tests cuando aplique:
  - render del primer tab
  - integracion visible de `AppToolbar`
  - integracion visible de `AppUpload`
  - colapso/expansion del panel derecho
  - no afectacion del segundo tab

Pruebas minimas sugeridas (tests actuales):
- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx`
- Agregar specs nuevas si se crean componentes dedicados

Criterios de aceptacion:
- `GestionRespuesta.tsx` sigue siendo la pagina principal
- los tabs actuales siguen funcionando
- el primer tab adopta la nueva estructura tipo workbench
- `AppToolbar` y `AppUpload` se reutilizan correctamente
- no se crean duplicados innecesarios de componentes shared
- el codigo queda desacoplado, mantenible y testeable
- no se rompe la experiencia actual del usuario

Instruccion final:
Generar la implementacion completa del refactor sobre el primer tab existente, reutilizando los componentes shared ya disponibles y respetando la arquitectura actual del proyecto.
