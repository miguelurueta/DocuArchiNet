# SCRUMCORE-216 - Integracion BackEnd

## 1. Resumen integracion
No aplica. Este ticket no introduce integracion backend directa ni nuevos endpoints.

## 2. Compatibilidad futura con Dynamic UI
La estructura (hooks/adapters + metadata `__tree`) deja preparado el componente para incorporar metadata/acciones backend-driven en el futuro sin romper la API publica.

## 3. Potenciales contratos backend-driven (futuro)
- Config de columnas/acciones por metadata.
- Acciones por fila/columna (menu).

## 4. Riesgos de integracion futura
- Crecimiento de props publicas si se mezcla jerarquia con contrato backend-driven sin adaptadores.

## 5. Estrategia de adaptacion futura
- Mantener adaptadores Tree -> Table como capa de traduccion unica.
- Exponer opt-in para acciones/metadata sin breaking changes.

## 6. Trazabilidad a codigo
- `src/app/Components/UI/AppTreeTable/adapters/mapTreeRowsToAppTableRows.ts`

