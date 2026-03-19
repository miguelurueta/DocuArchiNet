## 1. Estructura base del componente

- [x] 1.1 Crear la carpeta `src/app/Components/UI/AppModal/` con `AppModal.tsx`, `AppModal.module.css`, `AppModal.test.tsx`, `index.ts` y `README.md`.
- [x] 1.2 Definir `AppModalProps` con TypeScript estricto y herencia controlada desde el dialogo base del proveedor UI usando `Omit<ComponentProps<...>, ...>`.
- [x] 1.3 Exponer `AppModal` desde el barrel correspondiente de la capa UI para habilitar su consumo compartido.

## 2. Implementacion del contrato visual y funcional

- [x] 2.1 Implementar el render base de `AppModal` como wrapper sobre el dialogo del proveedor UI sin acoplar a las vistas a su API visual directa.
- [x] 2.2 Implementar soporte para apertura/cierre, titulo, contenido, acciones primarias/secundarias y callbacks asociados.
- [x] 2.3 Mapear estructura y estilos de overlay, cabecera, cuerpo y footer mediante CSS Modules para mantener consistencia con el design system interno.
- [x] 2.4 Garantizar accesibilidad en semantica de dialogo, titulo asociado, foco inicial y cierre por teclado cuando aplique.
- [x] 2.5 Permitir composicion segura con contenido adicional y configuracion de footer sin romper la experiencia base del modal.

## 3. Documentacion y pruebas

- [x] 3.1 Crear pruebas con Vitest + Testing Library para apertura/cierre, titulo, contenido, acciones, bloqueo por carga y accesibilidad del dialogo.
- [x] 3.2 Agregar identificadores `[SPEC:<SPEC_ID>]` en los tests que cubren los requisitos del nuevo spec `app-modal`.
- [x] 3.3 Redactar `README.md` del componente con descripcion, importacion, API de props, ejemplos de uso y buenas practicas.

## 4. Verificacion final

- [x] 4.1 Ejecutar la suite de pruebas relevante del componente y registrar evidencia de resultados para la documentacion OpenSpec.
- [x] 4.2 Revisar exports, tipado y estilos finales para confirmar que `AppModal` queda desacoplado de dominio y alineado con la arquitectura UI del proyecto.

## Evidencia

- `npm.cmd test -- src/app/Components/UI/AppModal/AppModal.test.tsx` -> `6 passed`
- `npx.cmd tsc -p tsconfig.app.json --noEmit` -> sin errores nuevos en `AppModal`; persisten errores preexistentes del repositorio en `src/api`, `src/modules/dashboard` y `src/modules/radicacion`
