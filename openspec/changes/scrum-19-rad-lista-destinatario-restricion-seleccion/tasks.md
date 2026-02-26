## 1. Hook de consulta de estructura de restriccion por tramite

- [x] 1.1 Crear hook `useEstructuraRelacionTipoRestriccion` con `GET /api/tramite/tramites/solicitaEstructuraRelacionTipoRestriccion` y query param tipado por `idValue`.
- [x] 1.2 Implementar normalizacion defensiva de respuesta para evitar fallos con payload parcial o vacio.
- [x] 1.3 Integrar manejo de error controlado con fallback seguro sin bloquear el formulario.

## 2. Integracion en flujo de seleccion de tramite

- [x] 2.1 Capturar `idValue` de `Descripcion_Documento` en `onChange` y actualizar estado fuente (`selectedTramiteId`).
- [x] 2.2 Disparar consulta de estructura de restriccion al cambiar tramite cuando exista `idValue` valido.
- [x] 2.3 Evitar llamada al endpoint cuando `idValue` sea nulo/vacio y limpiar estado derivado.

## 3. Validaciones y pruebas

- [x] 3.1 Agregar pruebas unitarias del hook para escenarios de exito, error y respuesta vacia.
- [x] 3.2 Agregar/actualizar pruebas de `RadicacionForm` para verificar llamada GET con parametro correcto al seleccionar tramite.
- [x] 3.3 Verificar no regresion en controles relacionados (`RE_flujo_trabajo` y restricciones de destinatario) y etiquetar pruebas con `[SPEC:<ID>]`.

## 4. Verificacion final y entrega

- [x] 4.1 Ejecutar suite de pruebas de Radicacion y corregir regresiones.
- [ ] 4.2 Validar manualmente seleccion de tramite y consulta de estructura de restriccion en UI.
- [ ] 4.3 Dejar evidencia en OpenSpec y preparar archivado del cambio.
