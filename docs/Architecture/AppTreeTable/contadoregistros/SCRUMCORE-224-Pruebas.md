# SCRUMCORE-224 - Pruebas

## Alcance
- Unitarias: derivacion de contadores en hook.
- Integracion UI: render contador y wiring de seleccion en workbench.
- Regresion local: sin cambios globales en AppTable/AppTreeTable (API opcional solamente).

## Unitarias ejecutadas
Comando:
```bash
npm.cmd test -- src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx
```

Casos cubiertos:
- `Total` backend.
- `TotalRecords` backend.
- fallback `rows.length`.
- estado vacio (`0`).
- `selectedDocumentsCount`.
- recarga por mutaciones runtime (`agregar_item` / `eliminar_item`).

## Integracion UI ejecutada
Comando:
```bash
npm.cmd test -- src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx
```

Casos cubiertos:
- contador visible en workbench.
- contador con seleccionados.
- propagacion `onSelectionChanged` desde `AppTreeTable` al hook.
- no regresion de render base / visor / rail / overlay.

## Evidencia de ejecucion
Comando combinado ejecutado:
```bash
npm.cmd test -- src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentosTable.test.tsx
```
Resultado: `15 passed`.

## Browser interaction
- Cubierto indirectamente por pruebas de integracion (eventos de seleccion/acciones en componentes mockeados).
- Validacion manual sugerida en ambiente real con datos dinamicos.

## E2E
- Se agrego cobertura en `playwright/gestionCorrespondencia/documentosWorkbench.smoke.spec.ts` para validar presencia del contador en flujo real.
- Ejecucion no realizada en esta sesion por dependencia de credenciales/ambiente real (`PLAYWRIGHT_LOGIN_*`).

## Regresion
- `AppTable`: sin cambios de implementacion.
- `AppTreeTable`: extension opcional `onSelectionChanged`, backward-compatible.
- Documento activo: sin cambios funcionales.
- Seleccion multiple: preservada.

## Matriz de cobertura
- Hook derivacion contador: Alto.
- Workbench visual + wiring: Medio/Alto.
- Runtime integrado backend real: Medio (requiere E2E/manual complementario).
