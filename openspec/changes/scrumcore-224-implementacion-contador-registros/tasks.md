## 1. Derivacion automatica de contador (hook/adapters)

- [x] 1.1 Extender `useGestionRespuestaDocumentosTable` para exponer `totalDocumentsCount` derivado de source-of-truth actual.
- [x] 1.2 Implementar estrategia de fallback de total: `Total` -> `TotalRecords` -> `rows.length`.
- [x] 1.3 Asegurar que post-mutacion runtime la fuente principal pase a `rows/treeRows` actuales.
- [x] 1.4 Exponer `selectedDocumentsCount` derivado automaticamente de la seleccion actual, sin contador mutable.
- [x] 1.5 Evitar estado duplicado/mutable y garantizar memoizacion de derivados.

## 2. Integracion en DocumentosWorkbench

- [x] 2.1 Consumir `totalDocumentsCount` y `selectedDocumentsCount` en `DocumentosWorkbench`.
- [x] 2.2 Renderizar contador contextual en formato acordado (`Documentos (N)` / `Documentos (N) · Seleccionados (M)`).
- [x] 2.3 Mantener estabilidad visual en loading/empty/error sin flicker.
- [x] 2.4 Verificar que documento activo y acciones no alteren el conteo fuera de cambios reales de lista/seleccion.

## 3. No-regresion funcional y de arquitectura

- [x] 3.1 Confirmar cero cambios backend/endpoints/contratos.
- [x] 3.2 Confirmar que no hay cambios globales en `AppTable`/`AppTreeTable`.
- [x] 3.3 Confirmar que no se modifica logica de Dynamic UI, documento activo ni acciones.
- [x] 3.4 Confirmar que no existe lógica manual de incremento/decremento (`++`/`--`) para el contador.

## 4. Pruebas unitarias obligatorias

- [x] 4.1 Caso `Total` backend.
- [x] 4.2 Caso `TotalRecords` backend.
- [x] 4.3 Fallback `rows.length`.
- [x] 4.4 Fallback vacio (`Documentos (0)`).
- [x] 4.5 `selectedDocumentsCount` correcto.
- [x] 4.6 Recalculo automatico con mutaciones runtime (agregar/eliminar).

## 5. Pruebas de integracion UI obligatorias

- [x] 5.1 Contador visible en `DocumentosWorkbench`.
- [x] 5.2 Contador actualizado al cargar datos.
- [x] 5.3 Contador de seleccionados actualizado con seleccion multiple.
- [x] 5.4 Agregar filas actualiza contador automaticamente.
- [x] 5.5 Eliminar filas actualiza contador automaticamente.

## 6. Pruebas browser/E2E/regresion

- [x] 6.1 Browser interaction: seleccion multiple actualiza contador automatico.
- [x] 6.2 Browser interaction: `agregar_item` y `eliminar_item` sincronizan contador sin lógica manual.
- [x] 6.3 E2E: lista con datos, vacia, seleccion multiple, agregar/eliminar, contador correcto. (No aplica para este ticket)
- [x] 6.4 Regresion: `AppTable`, `AppTreeTable`, documento activo y seleccion permanecen estables.

## 7. Documentacion obligatoria

- [x] 7.1 Crear `docs/Architecture/AppTreeTable/contadoregistros/SCRUMCORE-224-Arquitectura.md`.
- [x] 7.2 Crear `docs/Architecture/AppTreeTable/contadoregistros/SCRUMCORE-224-Implementacion-Detallada.md`.
- [x] 7.3 Crear `docs/Architecture/AppTreeTable/contadoregistros/SCRUMCORE-224-Integracion-BackEnd.md`.
- [x] 7.4 Crear `docs/Architecture/AppTreeTable/contadoregistros/SCRUMCORE-224-Pruebas.md`.
- [x] 7.5 Crear `docs/Architecture/AppTreeTable/contadoregistros/SCRUMCORE-224-Metadata.md`.

## 8. Validacion OpenSpec y cierre

- [x] 8.1 Ejecutar `openspec validate scrumcore-224-implementacion-contador-registros`.
- [x] 8.2 Registrar evidencia de comandos/tests y resumen de diff final.
