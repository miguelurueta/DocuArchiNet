## 1. Conexion de accion de dropdown con modal

- [x] 1.1 Identificar handler de acciones de fila en `GestionCorrespondencia.tsx`
- [x] 1.2 Conectar `actionId` de reasignacion (`reasignar_tramite` / `reasignar_tramite_menu`) para abrir `ReasignarRespuestaModal`
- [x] 1.3 Mantener flujo existente de `gestionar_tramite` para navegacion al detalle

## 2. Contexto de fila y estado local de modal

- [x] 2.1 Agregar/usar estado local para `open`, `rowContext` y `users`
- [x] 2.2 Resolver `radicado` y `nota` desde la fila con fallback seguro
- [x] 2.3 Conectar callbacks de cierre/envio en modo UI-only (sin API)

## 3. Restricciones de no-regresion

- [x] 3.1 Verificar que no se modifica AppTable base ni sus contratos
- [x] 3.2 Verificar que no cambian columnas, paginacion, query o render principal de tabla
- [x] 3.3 Verificar que el cambio quede localizado en contenedor y pruebas del modulo

## 4. Pruebas y evidencia

- [x] 4.1 Verificar escenario de apertura de modal por `reasignar_tramite` en test de pagina
- [x] 4.2 Ejecutar suite focalizada de `GestionCorrespondencia.test.tsx`
- [x] 4.3 Registrar resultado de pruebas y archivos de evidencia

## Evidencia

- Implementacion ubicada en:
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
  - `src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx`
- Escenario validado:
  - `abre el modal de reasignacion cuando la accion es reasignar_tramite`
- Comando ejecutado:
  - `npm.cmd run -s test -- --run src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx`
- Resultado:
  - `1 file passed`, `10 tests passed`

