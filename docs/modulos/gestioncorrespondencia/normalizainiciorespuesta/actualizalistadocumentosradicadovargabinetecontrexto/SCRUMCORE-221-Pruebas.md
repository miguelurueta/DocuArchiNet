# SCRUMCORE-221 - Pruebas

## Unitarias

Archivo: `src/modules/gestionCorrespondencia/tests/useListaDocumentosRadicadosTreeTable.test.tsx`

- Validación de consumo de `nombreGabinete` desde contexto transversal.
- Verificación de `queryListaDocumentosRadicados` con `NombreGabinete` proveniente del contexto.
- Verificación de bloqueos funcionales:
  - no consulta documentos en `gabineteLoading`;
  - no consulta documentos sin `NombreGabinete`;
  - `ver_documento` no se ejecuta sin `NombreGabinete`.
- Verificación de que `actionListaDocumentosRadicados` recibe payload con `NombreGabinete`.

## Integracion UI

- `useGestionRespuestaDocumentos` integrado con `useListaDocumentosRadicadosTreeTable` mantiene:
  - estado de loading/error de gabinete,
  - resolución de gabinete en contexto único,
  - operación de listado sin resolver gabinete localmente.

## Browser interaction

- Validar flujo en ejecución:
  - abrir `GestionRespuesta` con tarea válida;
  - activar tab/listado de documentos;
  - confirmar que el listado se carga solo cuando contexto tiene `nombreGabinete`;
  - validar que mensajes de `gabineteLoading`/`gabineteError` no rompen árbol.

## E2E / regresion

- Documento -> visor:
  - listar documento, seleccionar `ver_documento`, abrir con flujo existente.
- Revisión de estabilidad:
  - cambios rápidos de estado transicional de proveedor no rompen selección.
  - el árbol y el visor no muestran regresiones funcionales.
- Duplicación:
 - no introducir fetch de gabinete desde el hook en flujos de listados y acciones.

## Matriz de cobertura

| Requisito | Prueba |
| --- | --- |
| Hook ya no depende de getSolicitaGabinetePorTareaWorkflow | `useListaDocumentosRadicadosTreeTable.test.tsx` |
| consume `nombreGabinete` desde contexto | `useListaDocumentosRadicadosTreeTable.test.tsx` |
| bloqueos por estado de gabinete | `useListaDocumentosRadicadosTreeTable.test.tsx` |
| `ver_documento` usa `NombreGabinete` contextual | `useListaDocumentosRadicadosTreeTable.test.tsx` |
| load/loadChildren mantienen contratos | `useListaDocumentosRadicadosTreeTable.ts` + test |

## Evidencia ejecutada

```bash
npx vitest run src/modules/gestionCorrespondencia/tests/useListaDocumentosRadicadosTreeTable.test.tsx
```

Resultado: 1 archivo, 4 tests, OK.

```bash
npm run build
```

Resultado: pendiente en este ciclo de cierre (recomendado ejecutar en validación final del bloque de merge si el repo no tiene estado global pendiente).
