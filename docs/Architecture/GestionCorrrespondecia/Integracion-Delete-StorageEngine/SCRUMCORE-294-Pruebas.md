# SCRUMCORE-294 - Pruebas

## Unit tests requeridos

- `DocumentosWorkbench.test.tsx`
- `useGestionRespuestaDocumentosTable.test.tsx`
- `documentosWorkbenchActionMapper.test.ts`
- `documentosWorkbenchResponseAdapter.test.ts`

## Casos cubiertos

- La accion `eliminar_item` se dispara desde la fila correcta.
- La tabla conserva compatibilidad con filas legacy sin `CanDelete`.
- El backend puede bloquear el delete por negocio o autorizacion.
- La UI usa `UserMessage` antes que `Message` y `message`.
- El documento activo se limpia cuando la fila borrada estaba abierta.
- El listado se refresca despues de exito.
- El flujo `ver_documento` sigue funcionando despues del cambio.

## Validacion manual

- Abrir Gestion Respuesta.
- Ubicar un documento con accion de fila `eliminar_item`.
- Ejecutar delete y observar:
  - mensaje segun precedencia;
  - refresh del listado;
  - cleanup del visor si era el documento activo.

## Comandos sugeridos

```powershell
npx.cmd vitest run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx
```

```powershell
npx.cmd vitest run src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts
```

```powershell
npx.cmd eslint src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.ts
```

## Evidencia esperada

- Validacion de OpenSpec `--strict`.
- Tests unitarios de action mapping y workbench.
- Verificacion manual en UI de la tabla con accion de delete visible.
