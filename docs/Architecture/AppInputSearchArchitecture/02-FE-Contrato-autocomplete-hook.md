# Ticket 02 FE

## Titulo

Contrato generico de autocomplete + integracion con hook externo

## Objetivo

Definir el contrato generico de request/response y conectar AppInputSearch con un hook/servicio que consume API y entrega options/loading/error.

## Restricciones (obligatorio)

- mapper vive en hook/servicio, no en AppInputSearch
- no duplicar normalizacion por modulo
- AppInputSearch no consume API

## Contratos (obligatorios)

```ts
type AppAutocompleteRequest = {
  query: string;
  fieldName?: string;
  fieldControl?: string;
  context?: Record<string, unknown>;
  searchFields?: string[];
};

type AppAutocompleteOption = {
  value: string;
  label: string;
  meta?: Record<string, unknown>;
};
```

## Reglas de implementacion (obligatorio)

- soportar mapeo de endpoints conocidos
- normalizar response a `{ value, label }`
- loading visible sin bloquear input
- error no rompe el control

## Pruebas obligatorias

- normalizacion de response con variantes de keys
- loading visible sin bloqueo
- error mantiene control usable

## Criterios de aceptacion

- contrato generico definido y documentado
- adaptadores request/response listos
- integracion funcional con hook externo
