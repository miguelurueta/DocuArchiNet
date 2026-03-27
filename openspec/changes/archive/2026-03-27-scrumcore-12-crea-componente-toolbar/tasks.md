## 1. Definicion del contrato AppToolbar

- [x] 1.1 Corregir la propuesta del cambio para que el capability refleje `app-toolbar` en lugar del contenido generico heredado de `opsxj:new`
- [x] 1.2 Revisar el spec `app-toolbar` y cerrar las decisiones pendientes sobre regiones opcionales, overflow de acciones y alcance inicial de sticky/filtros

## 2. Implementacion del componente compartido

- [x] 2.1 Crear `src/app/Components/UI/AppToolbar/` con componente, estilos CSS Modules, exports y tipos publicos del contrato
- [x] 2.2 Implementar la composicion por regiones semanticas para titulo, descripcion, breadcrumbs, acciones principales, acciones secundarias y contenido auxiliar
- [x] 2.3 Implementar el comportamiento responsive para desktop/mobile y la estrategia de overflow de acciones sin acoplar la API publica a Ant Design o MUI
- [x] 2.4 Garantizar accesibilidad base del componente, incluyendo orden de foco coherente y soporte para acciones icon-only con nombre accesible

## 3. Pruebas del comportamiento reusable

- [x] 3.1 Crear pruebas Vitest + Testing Library para validar renderizado de encabezado, regiones opcionales y acciones segun el contrato `app-toolbar`
- [x] 3.2 Agregar pruebas para comportamiento responsive u overflow observable y para requisitos de accesibilidad de acciones icon-only
- [x] 3.3 Ejecutar las pruebas del componente y registrar evidencia util para la validacion del cambio

## 4. Adopcion inicial y verificacion

- [x] 4.1 Integrar `AppToolbar` en un consumidor real del repo, preferiblemente `gestionCorrespondencia`, reemplazando el encabezado manual actual
- [x] 4.2 Ajustar o agregar pruebas del consumidor para verificar que la adopcion preserva contexto visual y acciones del modulo
- [x] 4.3 Ejecutar validaciones finales relevantes (`npm test -- --run` o subset equivalente) y dejar el cambio listo para aplicacion o verificacion OpenSpec
