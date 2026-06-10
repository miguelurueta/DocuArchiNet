# SCRUMCORE-216 - Pruebas

## 1. Estrategia de pruebas
- Unitarias: adapters y hooks (sin AG Grid real).
- Integracion: smoke con consumer (`DocumentosWorkbench`) para no-regresion.

## 2. Unitarias
- `src/app/Components/UI/AppTreeTable/AppTreeTable.test.tsx`
  - Mock de `AppTable` para evitar dependencia de AG Grid en tests.

## 3. Integracion
- `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`

## 4. QT / calidad
- Verificar que no hay regresiones visuales en consumidores clave.

## 5. Regresion
- Mantener API publica intacta (props existentes).

## 6. Matriz de cobertura
- Flattening: cubierto por tests de render jerarquico (visible rows).
- Expand/collapse: cubierto.
- Loading/error/retry: cubierto.

## 7. Defectos
- N/A

## 8. Conclusion de calidad
Las pruebas unitarias e integracion cubren el refactor sin afectar consumidores principales.

