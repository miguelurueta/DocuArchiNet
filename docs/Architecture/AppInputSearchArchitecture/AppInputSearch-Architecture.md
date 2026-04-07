# Arquitectura Maestra: AppInputSearch (Autocomplete + Debounce + Loading)

## Objetivo

Definir una arquitectura reusable para `AppInputSearch` que estandarice la busqueda con soporte de autocomplete, debounce, minLength, loading y contrato generico para consumo de APIs, sin acoplarse a modulos concretos.

## Alcance

Aplica a:

- AppInputSearch como control reusable
- contenedores de consulta (AppTableQueryWrapper u otros)
- pantallas con busqueda local o server-side

No aplica a:

- redisenio visual general del sistema
- definicion de endpoints de negocio

## Resumen de arquitectura

Frontend

- AppInputSearch: UI + semantica de eventos
- Hook/servicio del modulo: consumo de API + normalizacion de options
- Contenedor: decide cuando consultar y como actualizar query state

Backend

- endpoints de autocomplete (segun campo)
- contratos heterogeneos adaptados por mapper

## Principios

- Control reusable, no acoplado
- Debounce configurable, minLength obligatorio cuando aplica
- Autocomplete soportado via AutoComplete + Input de AntD
- Loading no bloquea el input
- Contrato generico de request/response con adaptadores por endpoint

## Contrato base

- onChange: siempre
- onSearch: Enter + debounce + click icono (si length >= minLength)
- onClear: limpieza del input

## UX clave

- icono de busqueda es interactivo
- loading visible en icono y/o dropdown
- error manejado fuera del control

## CSS y tamanos

- alinear con AppInput (border radius 12px, sombras, focus, error, disabled)
- variantes size: sm | md | lg

## Plan sugerido

1. Definir contrato y props
2. Base UI con AntD AutoComplete + Input
3. Implementar semantica de eventos + debounce
4. Integrar consumo de API via hook externo
5. Estilos y tamanos alineados a AppInput
6. Pruebas unitarias e integracion
