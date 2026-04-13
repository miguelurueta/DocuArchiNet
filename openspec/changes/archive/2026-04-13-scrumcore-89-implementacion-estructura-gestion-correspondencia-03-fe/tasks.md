## 1. Refactor estructural del primer tab (workbench enterprise)

- [x] 1.1 Extraer el contenido del primer tab a `GestionRespuestaMainTabContent` con interfaz de props tipada (sin `any`)
- [x] 1.2 Crear subcomponentes desacoplados en `components/gestionRespuestaMainTab/`:
      `GestionRespuestaInfoHeader`, `GestionRespuestaEditorContainer`, `GestionRespuestaRightToolsPanel`
- [x] 1.3 Mantener `GestionRespuesta.tsx` como orquestador de tabs y contrato actual, sin impacto al segundo tab
- [x] 1.4 Asegurar que los subcomponentes sean presentacionales (sin logica de negocio), preparados para tests aislados

## 2. Integracion de componentes shared (reuso enterprise)

- [x] 2.1 Insertar `AppToolbar` como barra de acciones principal del workbench (zona superior del cuerpo)
- [x] 2.2 Mantener `AppUpload` en la zona inferior del primer tab con el mismo contrato actual
- [x] 2.3 Verificar que no se duplican componentes shared ni se acopla su implementacion al modulo

## 3. Layout workbench y comportamiento del panel derecho

- [x] 3.1 Implementar layout base con 3 zonas verticales: header informativo, cuerpo workbench, zona inferior
- [x] 3.2 Definir cuerpo workbench en 2 columnas: editor dominante + panel derecho de herramientas
- [x] 3.3 Implementar panel derecho colapsable con control visible (boton/icono) y estado controlado
- [x] 3.4 Estado inicial: expandido en desktop, colapsado en mobile; tablet colapsado por defecto
- [x] 3.5 Garantizar scroll interno del editor y del panel derecho sin afectar el scroll global del tab
- [x] 3.6 Asegurar estabilidad de layout (sin saltos al colapsar/expandir)

## 4. Responsive y accesibilidad enterprise

- [x] 4.1 Ajustar breakpoints: desktop 2 columnas, tablet panel colapsado, mobile panel colapsado
- [x] 4.2 Tabs con overflow horizontal en mobile sin romper el contenedor
- [x] 4.3 Controles del panel derecho con `aria-expanded`, `aria-controls` y foco visible
- [x] 4.4 Mantener semantica clara en header y zonas (labels o roles donde aplique)

## 5. Estilos locales y no regresion

- [x] 5.1 Estilos solo via CSS Modules del modulo (sin estilos globales)
- [x] 5.2 Validar que no se rompe el shell master-detail, navegacion ni el segundo tab
- [x] 5.3 Revisar consistencia visual con el design system actual (espaciado, tipografia, alturas)

## 6. Pruebas y evidencia (enterprise-ready)

- [x] 6.1 Unit tests: render del primer tab y subcomponentes (sin dependencia de tabs)
- [x] 6.2 Integration tests: presencia de `AppToolbar`, `AppUpload` y comportamiento de colapso/expansion
- [x] 6.3 UI interaction tests: toggle del panel derecho y persistencia de contenido oculto
- [x] 6.4 Validar que el segundo tab sigue intacto en pruebas de ruta existente
- [x] 6.5 Registrar evidencia de ejecucion de tests en el change OpenSpec

## Evidencia de pruebas

- `node .\node_modules\vitest\vitest.mjs --run src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/app/Components/UI/AppUpload/AppUpload.test.tsx src/app/Components/UI/AppTabs/AppTabs.test.tsx` (2026-04-13)
- Validacion visual: shell master-detail, segundo tab y estilos en desktop/mobile revisados (2026-04-13)
