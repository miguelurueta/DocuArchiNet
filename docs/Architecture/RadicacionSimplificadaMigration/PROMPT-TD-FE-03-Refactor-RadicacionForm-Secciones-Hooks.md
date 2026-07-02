# PROMPT DE DEUDA TECNICA - Frontend Radicacion
# TD-FE-03 - Refactor quirurgico de RadicacionForm por secciones y hooks

## Objetivo

Reducir el acoplamiento de `RadicacionForm.tsx` sin cambiar comportamiento funcional.

El componente actualmente mezcla:

- layout;
- carga de campos;
- controles especializados;
- autocompletes;
- remitente/destinatario;
- tramite/flujo;
- modal de usuario;
- footer;
- normalizacion de opciones.

## Principio

Refactor incremental, con pruebas verdes en cada corte.

No reescribir todo el formulario.

## Alcance

Extraer piezas conservando comportamiento:

```txt
components/RadicacionFormFooter.tsx
components/RadicacionTramiteSection.tsx
components/RadicacionRemitenteSection.tsx
components/RadicacionDestinatarioSection.tsx
components/RadicacionMetadataSection.tsx
hooks/useRadicacionTramiteSelection.ts
hooks/useRadicacionFormOptions.ts
utils/radicacionOptionMappers.ts
```

La extraccion debe priorizar:

1. footer;
2. tramite/flujo;
3. remitente/destinatario;
4. mappers de opciones;
5. tipos de props.

## Tipado

Reducir usos de:

```txt
any
unknown casts repetidos
```

Crear tipos especificos para:

```txt
AntD Select option normalizada
Menu item usado por tags
Campo con id_escript
Opcion backend idValue/id_value/Value/value_campo
```

No bloquear el refactor por contratos backend inconsistentes, pero centralizar la tolerancia en mappers.

## Criterios de Aceptacion

- `RadicacionForm.tsx` baja de responsabilidad, aunque no necesariamente de tamano perfecto.
- Footer queda separado y testeado.
- Mappers de opciones no viven inline dentro del JSX principal.
- No cambia el comportamiento visible del formulario.
- Tests existentes de `RadicacionForm.spec.test.tsx` siguen pasando.
- Nuevos tests cubren hooks/mappers extraidos.

## Fuera de Alcance

- conectar registro real;
- cambiar diseno visual;
- implementar pendientes;
- implementar documentos.
