# PROMPT ARQUITECTÓNICO
Migrar primer modulo consumidor a `AppTableExport`

## Rol esperado

Arquitecto de software senior y desarrollador frontend React
(React 19 + TypeScript estricto + integracion enterprise)

## Objetivo

Adoptar `AppTableExport` en el primer modulo real consumidor para validar la arquitectura reusable end-to-end.

## Problema actual

Aunque exista la infraestructura, falta validar su uso real en una pantalla del sistema.

## Objetivo funcional

Conectar el primer modulo a:

- exportacion de pagina actual
- exportacion de seleccionados
- exportacion total si existe backend disponible

## Alcance

- reemplazar wiring ad hoc de exportacion
- conectar datasource local y/o server-side
- validar UX y contratos en un modulo real

## No alcance

- no rediseñar la pantalla completa
- no convertir este ticket en definicion de arquitectura base
- no hardcodear la solucion dentro del modulo

## Reglas funcionales

- la pantalla debe usar `AppTableExport`
- la configuracion debe declararse desde el modulo
- la pieza reusable debe seguir viviendo en `src/app/Components/UI/AppTable/`

## Reglas tecnicas

- el modulo consumidor no debe duplicar logica de exportacion
- debe pasar datasource y capacidades requeridas
- debe respetar el query state actual si usa `allMatching`

## Archivos esperados

- modulo consumidor elegido
- integracion con `AppTableExport`
- pruebas de integracion

## Riesgos a evitar

- volver a acoplar exportacion al modulo
- resolver casos especiales dentro del componente reusable
- mezclar responsabilidades de pantalla con infraestructura shared

## Pruebas obligatorias

- render del trigger en pantalla real
- exportacion local funcionando
- exportacion total si existe capacidad server-side
- no regresion de paginacion y busqueda

## Criterios de aceptación

- primer modulo migrado sin wiring ad hoc
- `AppTableExport` validado en uso real
- arquitectura reusable confirmada

## Conclusión

Este ticket debe venir al final, cuando contratos, pieza reusable e integracion server-side ya existan.
