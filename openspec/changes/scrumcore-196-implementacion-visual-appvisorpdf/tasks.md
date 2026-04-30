## 1. Plomería de contexto (`idTareaWf`)

- [ ] 1.1 Pasar `idTareaWf` desde `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx` a `DocumentosWorkbench`
- [ ] 1.2 Actualizar `DocumentosWorkbench` para aceptar `idTareaWf?: number` y manejar el caso inválido/vacío

## 2. Capa de datos (negocio)

- [ ] 2.1 Identificar/crear servicio en `src/modules/gestionCorrespondencia/services/` para:
  - listar documentos por `idTareaWf`
  - obtener contenido/URL del PDF por documento
- [ ] 2.2 Crear hook en `src/modules/gestionCorrespondencia/hooks/` para encapsular:
  - estados `loading/error/empty`
  - cache del documento seleccionado (evitar descargas repetidas)
- [ ] 2.3 Tipar modelos DTO del listado/visor sin usar `any`

## 3. Integración UI (Tab Documentos)

- [ ] 3.1 Reemplazar el placeholder “Sin visor ni acciones” por `AppVisorPdf` en `DocumentosWorkbench`
- [ ] 3.2 Integrar el listado real en el rail (reusar `DocumentosList` o reemplazarlo por lista real)
- [ ] 3.3 Al seleccionar documento:
  - cargar el PDF
  - renderizarlo en el visor
  - (opcional) autocerrar overlay en mobile/tablet por UX
- [ ] 3.4 Implementar estados visibles:
  - `loading` (listado/visor)
  - `empty` (sin docs)
  - `error` con acción “Reintentar”
  - `unauthorized/forbidden` (si aplica)
- [ ] 3.5 Mantener el comportamiento overlay:
  - backdrop/`inert` (sin `aria-hidden` con foco)
  - autocierre al cambiar de tab (mantener el mecanismo existente)

## 4. Responsive / estilos

- [ ] 4.1 Ajustar `DocumentosWorkbench.module.css` para layout estable:
  - wrappers con `min-height: 0`
  - evitar overflow horizontal
  - scroll interno del visor
- [ ] 4.2 Validar breakpoints: 360×800, 768×1024, 1024×1366, 901–1122, >=1280

## 5. Tests y validación

- [ ] 5.1 Actualizar `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx` para el flujo real (mock de servicios/hook)
- [ ] 5.2 Asegurar que los tests del tab/routing no queden rotos (`GestionCorrespondenciaRoute.spec.test.tsx`)
- [ ] 5.3 Ejecutar `npm.cmd test` (o subset) y documentar resultados

