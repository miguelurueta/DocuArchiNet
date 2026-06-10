## Why

CREA-COMPONENTE-APPGUIDETOUR. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-235.
- Se formaliza la propuesta OpenSpec para implementar `AppGuideTour` a partir del ticket Jira.
- Se define la capability `crea-componente-appguidetour` como parte de la capa UI reutilizable.
- Se integra el primer tour en `AppVisorEmbedPdf` usando los botones reales actuales de `AppPdfToolbar`.
- Se conserva el contexto funcional del ticket como base, aclarando que controles mencionados por Jira pero no existentes actualmente en la toolbar no se crean en este ticket.

## Current AppPdfToolbar Scope

El tour obligatorio de `AppVisorEmbedPdf` debe cubrir los controles reales visibles en el toolbar actual:

- Abrir/cerrar thumbnails.
- Zoom out.
- Indicador de zoom actual.
- Zoom in.
- Reset zoom.
- Rotar izquierda.
- Rotar derecha.
- Abrir/cerrar firmas.
- Bloquear/desbloquear firma.
- Eliminar firma seleccionada.
- Print.
- Export.
- Ayuda / Guia interactiva.

Adicionalmente, puede cubrir overlays visibles del visor si existen en DOM durante el recorrido:

- Paginacion.
- Ir arriba.

No se agregan como parte de este ticket botones nuevos de Search, Fit Width o Fit Page, porque no existen como controles explicitos en `AppPdfToolbar` al momento del refinamiento. Si producto requiere esos botones, corresponde a un cambio funcional separado.

## Jira Details

> PROMPT ENTERPRISE — Implementación de AppGuideTour (Driver.js) en AppVisorEmbedPdf
> Rol esperado
> Arquitecto Frontend Senior (React 19, TypeScript Strict, Design System Enterprise, Driver.js, UX Enterprise, Accesibilidad, Testing Enterprise, Clean Architecture)
> Objetivo
> Implementar un sistema de guía interactiva reutilizable basado en Driver.js para AppVisorEmbedPdf mediante un componente enterprise llamado:
> AppGuideTour
> El objetivo es permitir que cualquier usuario pueda entender visualmente las funcionalidades del visor PDF mediante recorridos guiados no intrusivos.
> La implementación debe:
> Utilizar Driver.js como motor de tour.
> 
> Integrarse de forma desacoplada con AppVisorEmbedPdf.
> 
> Ser reusable para otros componentes del Design System.
> 
> No modificar la lógica funcional existente del visor.
> 
> No introducir coupling entre Driver.js y los componentes internos.
> 
> Mantener compatibilidad futura con cualquier componente UI del ecosistema.
> 
> Dependencias
> npm install driver.js
> Contexto
> Actualmente AppVisorEmbedPdf posee múltiples funcionalidades:
> Toolbar principal
> 
> Zoom In
> 
> Zoom Out
> 
> Fit Width
> 
> Fit Page
> 
> Rotación
> 
> Descarga
> 
> Impresión
> 
> Navegación de páginas
> 
> Búsqueda
> 
> Anotaciones
> 
> Firma electrónica
> 
> Sidebar
> 
> Miniaturas
> 
> Herramientas futuras
> 
> No existe actualmente un sistema guiado para explicar dichas funcionalidades.
> Objetivo UX
> Agregar un botón permanente de ayuda en la Toolbar.
> Características:
> Icono: ?
> 
> Tooltip:
> "Ayuda"
> 
> "Guía interactiva"
> 
> Accesible por teclado
> 
> Visible en desktop y mobile
> 
> Integrado visualmente con el Design System
> 
> Al hacer click:
> Iniciar Tour Interactivo
> Ubicación esperada
> src/app/Components/UI/AppGuideTour/
> ├── AppGuideTour.tsx
> ├── AppGuideTour.types.ts
> ├── AppGuideTour.service.ts
> ├── AppGuideTour.adapter.ts
> ├── AppGuideTour.constants.ts
> ├── hooks/
> │   └── useAppGuideTour.ts
> ├── providers/
> │   └── AppGuideTourProvider.tsx
> ├── drivers/
> │   └── DriverJsAdapter.ts
> ├── tests/
> └── index.ts
> Integración:
> src/app/Components/UI/AppVisorEmbedPdf/
> Restricciones obligatorias
> NO modificar lógica existente de:
> Toolbar
> 
> Zoom
> 
> Rotate
> 
> Search
> 
> Download
> 
> Print
> 
> Signature
> 
> Annotation
> 
> Sidebar
> 
> NO introducir lógica Driver.js dentro de:
> plugins
> 
> hooks existentes
> 
> reducers existentes
> 
> NO usar any.
> NO generar dependencia circular.
> NO romper consumers actuales.
> Arquitectura obligatoria
> AppGuideTour debe funcionar como una capa de presentación desacoplada.
> AppVisorEmbedPdf
>         │
>         ▼
> AppGuideTour
>         │
>         ▼
> DriverJsAdapter
>         │
>         ▼
> Driver.js
> Responsabilidades
> AppGuideTour
> Responsable de:
> iniciar tour
> 
> detener tour
> 
> registrar pasos
> 
> manejar eventos visuales
> 
> NO responsable de:
> lógica PDF
> 
> permisos
> 
> anotaciones
> 
> firma
> 
> estado documental
> 
> DriverJsAdapter
> Responsable de:
> encapsular Driver.js
> 
> evitar dependencia directa en consumers
> 
> permitir reemplazo futuro
> 
> useAppGuideTour
> Responsable de:
> exponer API reusable
> 
> gestionar ciclo de vida
> 
> coordinar eventos del tour
> 
> API pública esperada
> type AppGuideTourStep = {
>   id: string;
>   element: string;
>   title: string;
>   description: string;
>   side?: "top" | "bottom" | "left" | "right";
> };
> 
> type AppGuideTourProps = {
>   tourId: string;
>   steps: AppGuideTourStep[];
> };
> Tour obligatorio AppVisorEmbedPdf
> El recorrido debe cubrir todos los elementos visibles de la Toolbar.
> Ejemplo:
> Toolbar principal
> 
> Navegación páginas
> 
> Zoom In
> 
> Zoom Out
> 
> Fit Width
> 
> Fit Page
> 
> Rotar documento
> 
> Buscar texto
> 
> Descargar documento
> 
> Imprimir documento
> 
> Panel lateral
> 
> Miniaturas
> 
> Herramientas de anotación
> 
> Firma electrónica
> 
> Ayuda
> 
> Cada paso debe explicar:
> qué hace
> 
> cuándo usarlo
> 
> impacto funcional
> 
> Botón de ayuda obligatorio
> Agregar en Toolbar:
> [ ? ]
> Requisitos:
> Tooltip "Ayuda"
> 
> Tooltip "Guía interactiva"
> 
> Focus visible
> 
> Keyboard accessible
> 
> Responsive
> 
> No alterar layout existente
> 
> Al hacer click:
> guideTour.start();
> Accesibilidad obligatoria
> WCAG AA
> Incluir:
> navegación teclado
> 
> aria-label
> 
> aria-describedby
> 
> focus visible
> 
> escape para cerrar
> 
> screen reader friendly
> 
> Performance
> Crear Driver.js una sola vez.
> 
> Evitar reinicializaciones innecesarias.
> 
> Memoizar steps.
> 
> No provocar rerenders del visor PDF.
> 
> No afectar renderizado documental.
> 
> Observabilidad
> Registrar eventos:
> guide_started
> guide_completed
> guide_cancelled
> guide_step_changed
> No registrar:
> URLs
> 
> tokens
> 
> información documental sensible
> 
> Pruebas obligatorias
> Unitarias
> render AppGuideTour
> 
> registro de steps
> 
> start()
> 
> stop()
> 
> adapter Driver.js
> 
> hook useAppGuideTour
> 
> Integración
> Toolbar muestra botón ayuda
> 
> click inicia tour
> 
> recorrido completo
> 
> cierre correcto
> 
> Playwright
> Validar:
> botón ayuda visible
> 
> tooltip visible
> 
> apertura del tour
> 
> navegación siguiente/anterior
> 
> finalización tour
> 
> responsive desktop
> 
> responsive tablet
> 
> responsive mobile
> 
> Regresión
> Validar:
> zoom intacto
> 
> rotate intacto
> 
> print intacto
> 
> download intacto
> 
> firma intacta
> 
> anotaciones intactas
> 
> Criterios de aceptación
> Existe AppGuideTour reusable.
> 
> Driver.js encapsulado correctamente.
> 
> AppVisorEmbedPdf integra guía interactiva.
> 
> Toolbar incluye botón de ayuda.
> 
> Tour cubre todos los elementos visibles.
> 
> No existen regresiones funcionales.
> 
> Tests exitosos.
> 
> Compatible con futuras extensiones.
> 
> Documentación Enterprise Obligatoria
> Ruta:
> docs/Components/AppGuideTour/GuiaVisorPDF
> 1. SCRUMCORE-[ID]-Arquitectura.md
> Documentar:
> Objetivo técnico
> 
> Decisiones arquitectónicas
> 
> Separación de responsabilidades
> 
> Integración con AppVisorEmbedPdf
> 
> DriverJsAdapter
> 
> Estrategia reusable
> 
> Diagramas Mermaid obligatorios:
> classDiagram
> Relación entre:
> AppGuideTour
> 
> DriverJsAdapter
> 
> useAppGuideTour
> 
> AppVisorEmbedPdf
> 
> sequenceDiagram
> Flujo:
> Usuario→ Click Ayuda→ AppGuideTour→ DriverJsAdapter→ Driver.js→ Render Tour
> stateDiagram-v2
> Estados:
> idle
> loading
> running
> paused
> completed
> cancelled
> error
> 2. SCRUMCORE-[ID]-Implementacion-Detallada.md
> Documentar:
> Archivos creados
> 
> Archivos modificados
> 
> Props
> 
> Hooks
> 
> Eventos
> 
> Flujo interno
> 
> Integración Toolbar
> 
> Responsive
> 
> Accesibilidad
> 
> Casos borde
> 
> 3. SCRUMCORE-[ID]-Pruebas.md
> Documentar:
> Unit Tests
> 
> Integration Tests
> 
> Playwright
> 
> Evidencias
> 
> Cobertura
> 
> Casos críticos
> 
> Riesgos residuales
> 
> 4. SCRUMCORE-[ID]-Metadata.md
> Incluir:
> Ticket JIRA
> 
> Autor
> 
> Fecha
> 
> Versión
> 
> Branch
> 
> Commit
> 
> Referencias cruzadas
> 
> Estado implementación
> 
> Trazabilidad obligatoria
> Tabla:
> Elemento
> Archivo
> Evidencia
> Estado
> AppGuideTour
> ruta real
> componente
> Completo
> DriverJsAdapter
> ruta real
> adapter
> Completo
> useAppGuideTour
> ruta real
> hook
> Completo
> HelpButton
> ruta real
> toolbar
> Completo
> Playwright
> ruta real
> test
> Completo
> Instrucción final
> Implementar AppGuideTour como componente reusable enterprise basado en Driver.js, integrado en AppVisorEmbedPdf mediante un botón de ayuda en la Toolbar, desacoplado de la lógica PDF, extensible para cualquier componente del Design System, completamente documentado, testeado y mantenible a largo plazo.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: APPGUIDETOUR, COMPONENTE, CREA

## Capabilities

### New Capabilities
- `crea-componente-appguidetour`: Componente reusable `AppGuideTour` para la capa UI compartida del proyecto.

### Modified Capabilities
- 

## Impact

- Nuevo componente compartido en `src/app/Components/UI/AppGuideTour/`.
- Integracion inicial en `src/app/Components/UI/AppVisorEmbedPdf/`.
- Nuevas pruebas de comportamiento para el contrato reusable del componente.
