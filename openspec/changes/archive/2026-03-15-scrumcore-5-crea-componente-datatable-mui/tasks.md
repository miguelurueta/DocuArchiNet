## 1. Estructura base del componente

- [x] 1.1 Crear la carpeta `src/app/Components/UI/AppDataTableMui/` con `AppDataTableMui.tsx`, `AppDataTableMui.module.css`, `AppDataTableMui.test.tsx`, `index.ts` y `README.md`.
- [x] 1.2 Definir `AppDataTableMuiProps` y tipos relacionados con TypeScript estricto y herencia controlada desde `DataGrid` usando `Omit<ComponentProps<...>, ...>`.
- [x] 1.3 Exponer `AppDataTableMui` desde el barrel correspondiente de la capa UI para habilitar su consumo compartido.

## 2. Implementacion del contrato visual y funcional

- [x] 2.1 Implementar el render base de `AppDataTableMui` como wrapper sobre `@mui/x-data-grid` sin acoplar a las vistas a la API completa del proveedor.
- [x] 2.2 Implementar soporte para columnas, filas, loading, estado vacio, seleccion de filas y paginacion base.
- [x] 2.3 Mapear estilos del contenedor, encabezado y overlay vacio para mantener consistencia con el design system interno.
- [x] 2.4 Garantizar accesibilidad de la grilla, nombre accesible configurable y soporte observable de teclado heredado del control base.
- [x] 2.5 Permitir composicion segura con definiciones de columna y renderers personalizados sin romper la experiencia base del componente.

## 3. Documentacion y pruebas

- [x] 3.1 Crear pruebas con Vitest + Testing Library para render de columnas/filas, loading, estado vacio, nombre accesible y seleccion.
- [x] 3.2 Agregar identificadores `[SPEC:<SPEC_ID>]` en los tests que cubren los requisitos del nuevo spec `app-datatable-mui`.
- [x] 3.3 Redactar `README.md` del componente con descripcion, importacion, API de props, ejemplos de uso y buenas practicas.

## 4. Verificacion final

- [x] 4.1 Ejecutar la suite de pruebas relevante del componente y registrar evidencia de resultados para la documentacion OpenSpec.
- [x] 4.2 Revisar exports, tipado y estilos finales para confirmar que `AppDataTableMui` queda desacoplado de dominio y alineado con la arquitectura UI del proyecto.

## Evidencia

- `npm.cmd test -- src/app/Components/UI/AppDataTableMui/AppDataTableMui.test.tsx` -> `6 passed`
- `npx.cmd tsc -p tsconfig.app.json --noEmit` -> sin errores nuevos en `AppDataTableMui`; persisten errores preexistentes del repositorio en `src/api`, `src/modules/dashboard` y `src/modules/radicacion`
