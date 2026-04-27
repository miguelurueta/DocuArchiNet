## 1. Contrato FE-09 y reutilizacion del PageBreak

- [x] 1.1 Definir el contrato reusable para insercion manual de `PageBreak` en `AppEditorPdf`.
- [x] 1.2 Reutilizar el comando existente de `AppEditor` para insertar el nodo persistido sin duplicar logica.
- [x] 1.3 Exponer el control de salto manual de forma opcional para consumidores de `AppEditorPdf`.

## 2. Integracion visual y de navegacion

- [x] 2.1 Integrar la accion de salto manual en la toolbar o slot reusable de `AppEditorPdf`.
- [x] 2.2 Verificar que el cursor pueda seguir escribiendo antes y despues del salto manual.
- [x] 2.3 Asegurar que los saltos manuales actuen como fronteras duras en la paginacion visual.

## 3. Persistencia y reglas del nodo

- [x] 3.1 Confirmar la serializacion estable del nodo `data-page-break="true"`.
- [x] 3.2 Evitar inserciones de saltos manuales consecutivos en la misma posicion.
- [x] 3.3 Validar que el contenido con `PageBreak` sobreviva round-trip HTML/render.

## 4. Pruebas y validacion

- [x] 4.1 Crear/actualizar pruebas con Vitest + Testing Library para insercion y persistencia del salto manual.
- [x] 4.2 Agregar pruebas de cursor y frontera visual alrededor del `PageBreak`.
- [x] 4.3 Ejecutar `npm.cmd run test -- --run` y `npm.cmd run spec:validate`, dejando evidencia en el cambio.

## Evidencia (2026-04-27)

- `npm.cmd run spec:validate`: OK.
- `npm.cmd run test -- --run`: no ejecutable en este entorno. Falla al cargar `vite.config.ts` con `Error: spawn EPERM` desde `esbuild` (Vite).
