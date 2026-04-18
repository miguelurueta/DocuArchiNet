# Ticket 03 FE

## Titulo

Completar integracion backend, accesibilidad y documentación de uso de `AppInputSelect`

## Objetivo

Cerrar la implementación reusable de `AppInputSelect` con integración backend
segura, accesibilidad, documentación de consumo y ejemplos reales para equipos
frontend. El foco de esta FE es completar la pieza shared lista para usarse,
no crear más archivos de arquitectura.

## Contexto existente

- Arquitectura de referencia: `docs/Architecture/AppInputSelect/AppInputSelect-Architecture.md`
- Implementacion esperada en `src/app/Components/UI/AppInputSelect/`
- Integraciones futuras con formularios, filtros y APIs del proyecto

## Restricciones (obligatorio)

- No romper la API del core
- No acoplar a un endpoint fijo
- No agregar dependencias extra solo para fetch o documentación
- Debe poder consumirse desde formularios y filtros reales

## Reglas de accesibilidad (obligatorio)

- `aria-label` o `aria-labelledby` cuando aplique
- Navegacion por teclado completa
- `loading` y `disabled` distinguibles visualmente
- Mensajes de error y estados vacios comprensibles

## Integracion backend (obligatorio)

- Debe aceptar `fetchOptions`
- Debe tolerar respuestas vacias y errores del backend
- Debe soportar debounce o cancelacion en el contenedor
- Debe exponer adaptadores para mapear DTO backend a opciones del select

## Entregables de implementacion

- integración remota funcional dentro del componente o su capa de adaptación
- helpers o adaptadores para mapear DTO backend a opciones del select
- README en `src/app/Components/UI/AppInputSelect/README.md`
- ejemplos:
  - basico local
  - remoto con backend
  - multiple
  - con formularios
  - con estado vacio custom
- export desde el índice shared correspondiente

## Pruebas obligatorias

- flujo local simple
- flujo remoto con loading y no data
- busqueda con `onSearch`
- error backend sin crash del componente
- sizes `sm/md/lg`

## Criterios de aceptacion

- Documentacion útil para equipos frontend
- Integracion backend claramente definida e implementable
- Accesibilidad y estados cubiertos
- Componente reusable listo para consumo por modulos reales
