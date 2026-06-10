# SCRUMCORE-219 - Implementacion Detallada

## Archivos modificados

- `gestionRespuestaEstructura.types.ts`
- `mapEstructuraRespuesta.ts`
- `useEstructuraRespuestaIdTarea.ts`
- `mapEstructuraRespuesta.test.ts`
- `useEstructuraRespuestaIdTarea.test.tsx`

## Tipos agregados

`SolicitaEstructuraRespuestaBackendItem` soporta:

- `idRespuestaRadicado?: string | number`
- `IdRespuestaRadicado?: string | number`
- `ID_RESPUESTA_RADICADO?: string | number`
- `id_respuesta_radicado?: string | number`

`GestionRespuestaEstructuraRespuesta` expone:

- `idRespuestaRadicado?: string | number`

## Reglas de mapping

`mapEstructuraRespuesta` resuelve el campo con esta precedencia:

1. `idRespuestaRadicado`
2. `IdRespuestaRadicado`
3. `ID_RESPUESTA_RADICADO`
4. `id_respuesta_radicado`

Los campos existentes se mantienen:

- `Radicado`
- `Destinatario`
- `TramiteDocumento`

## Fallback strategy

Si ninguna variante existe, el mapper no agrega la propiedad opcional. El acceso a `idRespuestaRadicado` retorna `undefined`.

No se usan:

- `0`
- string vacio
- `NaN`

## Capas

- `types`: compatibilidad DTO y modelo normalizado.
- `adapters`: normalizacion unica.
- `hooks`: consumo del modelo normalizado sin casing backend.
- `tests`: cobertura mapper/hook y compatibilidad legacy.
