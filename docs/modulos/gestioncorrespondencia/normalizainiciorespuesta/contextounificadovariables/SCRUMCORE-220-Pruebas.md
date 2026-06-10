# SCRUMCORE-220 - Pruebas

## Unitarias

Archivo: `src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentos.test.tsx`

Cobertura:

- fallback fuera del provider;
- props transversales expuestas;
- normalizacion de `nombreGabinete`;
- preservacion de `files/setFiles`;
- idempotencia por `idTareaWf`;
- `reloadGabinete`;
- `gabineteError`;
- proteccion contra response stale.

## Integracion

Archivo: `src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx`

Cobertura:

- `useGestionRespuestaDocumentosTable` consume contexto;
- no consulta gabinete directamente;
- conserva counters de documentos;
- conserva seleccion;
- conserva acciones runtime;
- bloquea query cuando falta radicado;
- propaga error cuando gabinete indica radicado inexistente;
- incluye `Radicado` en request de listado;
- mantiene proteccion anti-stale en cambio de tarea.

## Browser interaction

Validacion esperada:

- abrir detalle de gestion respuesta;
- confirmar metadata de estructura;
- cambiar a tab Documentos;
- verificar que listado/visor siguen operando;
- validar que no se duplican requests de gabinete por re-render;
- validar que reload explicito puede refrescar gabinete sin romper render.

## E2E / regresion

Flujos a cubrir en suite E2E o validacion manual:

- GestionRespuesta abre con `idTareaWf` valido.
- DocumentosWorkbench carga listado con gabinete contextual.
- AppVisorEmbedPdf carga documento seleccionado.
- Adjuntos mantiene `files/setFiles`.
- Estado de error de gabinete no rompe UI.

## Matriz de cobertura

| Requisito | Prueba |
| --- | --- |
| Contexto expone datos transversales | `useGestionRespuestaDocumentos.test.tsx` |
| `files/setFiles` compatible | `useGestionRespuestaDocumentos.test.tsx` |
| Idempotencia por `idTareaWf` | `useGestionRespuestaDocumentos.test.tsx` |
| `reloadGabinete` | `useGestionRespuestaDocumentos.test.tsx` |
| Cancelacion/stale guard | `useGestionRespuestaDocumentos.test.tsx`, `useGestionRespuestaDocumentosTable.test.tsx` |
| Tabla sin fetch duplicado de gabinete | `useGestionRespuestaDocumentosTable.test.tsx` |
| Error no rompe render | `useGestionRespuestaDocumentos.test.tsx` |
| Build sin errores TS | `npm run build` |

## Evidencia ejecutada

```bash
npx vitest run src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentos.test.tsx src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx
```

Resultado: 2 archivos, 17 tests, todos pasan.

```bash
npx vitest run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx
```

Resultado: 1 archivo, 6 tests, todos pasan.

Validacion adicional intentada:

```bash
npx vitest run src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx
```

Resultado: `DocumentosWorkbench.test.tsx` pasa. `GestionCorrespondenciaRoute.spec.test.tsx` mantiene fallos de expectativas historicas del test y requiere mocks adicionales del listado de documentos para evitar requests reales en jsdom. `GestionRespuestaMainTabContent.test.tsx` mantiene una expectativa de mensaje de gate no observada. Estos fallos no corresponden a errores TypeScript ni a cambios visuales de SCRUMCORE-220.

```bash
npm run build
```

Resultado: build correcto. Vite reporta warning conocido de chunks mayores a 500 kB, sin error de TypeScript.

```bash
npx openspec validate scrumcore-220-implementacion-contexto-trasversal-unificado-gestion-respuesta --strict
```

Resultado: change OpenSpec valido.
