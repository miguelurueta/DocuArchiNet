## 1. Plomería de contexto (`idTareaWf`)

- [x] 1.1 Pasar `idTareaWf` desde `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx` a `DocumentosWorkbench`
- [x] 1.2 Actualizar `DocumentosWorkbench` para aceptar `idTareaWf?: number` y manejar el caso inválido/vacío

## 2. Capa de datos (negocio)

Fuente de verdad (sin backend nuevo): documentos adjuntos cargados en el tab **Gestión** (`AppUpload`) compartidos con el tab **Documentos**.

- [x] 2.1 Crear un contexto/provider para compartir `files` entre tabs
- [x] 2.2 Crear un hook para consumir el contexto (`files`, `setFiles`) desde ambos tabs
- [x] 2.3 Mapear `AppUploadFile` → items del listado (nombre, tamaño, tipo)
- [x] 2.4 Resolver el PDF seleccionado como `objectURL` y revocarlo en cleanup (evitar leaks)
- [ ] 2.5 (Si aplica) Integrar backend real de documentos por `idTareaWf` (listar + URL/bytes + auth)

## 3. Integración UI (Tab Documentos)

- [x] 3.1 Reemplazar el placeholder por `AppVisorPdf` en `DocumentosWorkbench`
- [x] 3.2 Integrar el listado en el rail (reusando `DocumentosList`)
- [x] 3.3 Al seleccionar documento:
  - cargar el PDF en `AppVisorPdf`
  - autocerrar overlay en mobile/tablet (UX)
- [x] 3.4 Manejar caso “no soportado” (documento no PDF o no renderizable) con UI clara
- [x] 3.5 Mantener el comportamiento overlay:
  - backdrop/`inert` (sin `aria-hidden` con foco)
  - autocierre al cambiar de tab (mantener el mecanismo existente)
- [ ] 3.6 Implementar estados `loading/error` si se incorpora descarga real (backend)

## 4. Responsive / estilos

- [x] 4.1 Ajustar `DocumentosWorkbench.module.css` para layout estable (min-height/overflow/scroll interno)
- [ ] 4.2 Validar breakpoints: 360×800, 768×1024, 1024×1366, 901–1122, >=1280

## 5. Tests y validación

- [x] 5.1 Actualizar `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx` según el nuevo comportamiento
- [x] 5.2 Asegurar que los tests del tab/routing no queden rotos (`GestionCorrespondenciaRoute.spec.test.tsx`)
- [ ] 5.3 Ejecutar `npm.cmd test` (o subset) y documentar resultados

## 6. Toolbar (acciones)

- [ ] 6.1 Conectar acciones del toolbar a flujos reales si existen (buscar, vincular, abrir)
- [ ] 6.2 Si una acción no está disponible, dejarla deshabilitada con estado/tooltip (sin romper UI)

