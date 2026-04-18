# SCRUM-137
# Integracion Frontend de estructura de respuestas por `idTareaWf`

## Objetivo

Consumir el endpoint:

```txt
GET /api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea?idTareaWf=:id
```

y usar su resultado para poblar dinamicamente los items visuales de
`GestionRespuesta`, reemplazando la construccion estatica actual de tabs por una
fuente real basada en `idTareaWf`.


## Que significa exactamente el prompt

El backend expone una consulta puntual por `idTareaWf` que devuelve:

```ts
AppResponses<List<RaRespuestaRadicado>>
```

Semantica correcta de consumo:

- `success === true` y `data.length > 0`
  Hay resultados y deben renderizarse.
- `success === true` y `data.length === 0`
  No hay resultados. La UI debe mostrar estado vacio, no error.
- `success === false`
  Hay error controlado. La UI debe mostrar error y evitar asumir datos.

Regla importante:

- no usar `message` para decidir la logica
- usar `success` + `data.length`


## Recomendacion de integracion en la UI

### Estado actual

Hoy `GestionRespuesta` construye tabs estaticos en:

- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`

con dos items fijos:

- `Gestion`
- `Documentos`

### Propuesta recomendada

No mezclaria el contrato del endpoint con el tab fijo `Documentos`.

La forma mas limpia es:

- mantener `Documentos` como tab fijo
- reemplazar el tab fijo `Gestion` por uno o varios tabs dinamicos construidos
  desde `RaRespuestaRadicado[]`

Esto produce una estructura como esta:

```ts
[
  ...responseTabs,
  documentosTab,
]
```

donde cada item de `responseTabs` representa una respuesta asociada a la tarea
de workflow.

### Regla de mapeo sugerida

Para cada `RaRespuestaRadicado`:

- `key`: `idRespuestaRadicado`
- `label`: `asunto` si existe; fallback a `radicado`
- `children`: contenedor de `GestionRespuestaMainTabContent` parametrizado con
  el item seleccionado

Ejemplo conceptual:

```ts
{
  key: String(item.idRespuestaRadicado),
  label: item.asunto?.trim() || item.radicado,
  children: (
    <GestionRespuestaMainTabContent
      respuesta={item}
      idTareaWf={item.idTareaWf}
    />
  ),
}
```


## Comportamiento de UI recomendado

### 1. Loading

Mientras se consulta el endpoint:

- renderizar `AppTabs` solo cuando haya datos listos, o
- mostrar un contenedor placeholder/skeleton encima del shell del tab

No recomendaria renderizar tabs vacios que luego cambian de golpe si el efecto
visual es brusco.

### 2. Empty state

Si:

```ts
success === true && data.length === 0
```

la pantalla debe mostrar explicitamente un estado vacio.

Opciones correctas:

- una vista vacia dentro del area principal de `GestionRespuesta`
- o un tab placeholder tipo `Sin respuestas`

Recomendacion:

- mantener el shell y renderizar una vista vacia en lugar de fabricar tabs falsos

### 3. Error state

Si:

```ts
success === false
```

la pantalla debe mostrar:

- mensaje de error desacoplado
- opcion de reintentar si la UX del modulo lo permite

No se debe caer en:

- tabs vacios sin explicacion
- usar `message` como si implicara exito o vacio


## Contratos frontend a crear

## Tipo sugerido

```ts
export type RaRespuestaRadicado = {
  idRespuestaRadicado: number;
  idTareaWf: number;
  radicado: string;
  asunto: string;
};
```

## Response esperada

```ts
export type SolicitaEstructuraRespuestaIdTareaResponse =
  ApiResponse<RaRespuestaRadicado[]>;
```


## Distribucion tecnica recomendada

### 1. Servicio

Crear un service del modulo para encapsular el endpoint:

```txt
src/modules/gestionCorrespondencia/services/solicitaEstructuraRespuestaIdTarea.service.ts
```

Responsabilidad:

- recibir `idTareaWf`
- ejecutar `GET`
- devolver `ApiResponse<RaRespuestaRadicado[]>`

Ejemplo conceptual:

```ts
clienteApi.get<ApiResponse<RaRespuestaRadicado[]>>(
  "/api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea",
  { params: { idTareaWf } }
);
```

### 2. Tipos

Crear tipos del dominio:

```txt
src/modules/gestionCorrespondencia/types/estructuraRespuestaIdTarea.types.ts
```

Responsabilidad:

- contrato `RaRespuestaRadicado`
- tipo response
- posibles helpers tipados del estado UI

### 3. Hook de consulta

Crear un hook de consulta del modulo:

```txt
src/modules/gestionCorrespondencia/hooks/useEstructuraRespuestaIdTarea.ts
```

Responsabilidad:

- orquestar React Query
- recibir `idTareaWf`
- exponer:
  - `items`
  - `loading`
  - `error`
  - `isEmpty`

Forma recomendada:

```ts
const query = useQuery<ApiResponse<RaRespuestaRadicado[]>, AxiosError>({
  queryKey: ["gestion-correspondencia", "estructura-respuesta", idTareaWf],
  queryFn: () => getEstructuraRespuestaByIdTarea(idTareaWf),
  enabled: Number.isFinite(idTareaWf) && idTareaWf > 0,
});
```

### 4. Adaptador de tabs

Crear un mapper/adaptador UI:

```txt
src/modules/gestionCorrespondencia/adapters/mapRespuestaRadicadoToTabs.tsx
```

Responsabilidad:

- transformar `RaRespuestaRadicado[]` a `AppTabItem[]`
- preservar `Documentos` como tab fijo
- mantener las reglas de fallback de label

### 5. Pagina orquestadora

Actualizar:

```txt
src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx
```

Responsabilidad:

- obtener `idTareaWf` desde el contexto correcto
- consumir `useEstructuraRespuestaIdTarea`
- renderizar loading / empty / error
- pasar `items` ya resueltos a `AppTabs`

### 6. Contenido del tab

Posible ajuste en:

```txt
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx
```

Responsabilidad:

- recibir props del item seleccionado:
  - `respuesta`
  - `idTareaWf`
- usar esos datos para header, metadata o estado futuro

No pisaria este archivo si no hace falta. Solo lo tocaria si el contenido del
tab necesita conocer la respuesta activa.


## De donde sacar `idTareaWf`

Este es el punto critico del cambio.

La integracion depende de identificar la fuente real de `idTareaWf`.

Opciones probables:

- parametro de ruta actual
- estado del registro seleccionado desde la bandeja
- prop inyectada al abrir `GestionRespuesta`

Mi recomendacion tecnica:

- resolver `idTareaWf` en el contenedor de pagina
- no enterrarlo en `GestionRespuestaMainTabContent`

Si `GestionRespuesta` hoy no recibe ese dato, entonces probablemente habria que
tocar tambien:

```txt
src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx
```

o el punto donde se construye el panel secundario.


## Archivos que tocaria

## Nuevos

- `src/modules/gestionCorrespondencia/types/estructuraRespuestaIdTarea.types.ts`
- `src/modules/gestionCorrespondencia/services/solicitaEstructuraRespuestaIdTarea.service.ts`
- `src/modules/gestionCorrespondencia/hooks/useEstructuraRespuestaIdTarea.ts`
- `src/modules/gestionCorrespondencia/adapters/mapRespuestaRadicadoToTabs.tsx`
- `src/modules/gestionCorrespondencia/tests/solicitaEstructuraRespuestaIdTarea.service.test.ts`
- `src/modules/gestionCorrespondencia/tests/useEstructuraRespuestaIdTarea.test.tsx`

## Probablemente modificados

- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`
- `src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx`
- `src/modules/gestionCorrespondencia/README.md`

## Posibles, segun de donde salga `idTareaWf`

- `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx`
- `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
- algun adapter de navegacion o evento de tabla que hoy abre `GestionRespuesta`


## Flujo de implementacion recomendado

1. Tipar `RaRespuestaRadicado`
2. Crear el service del endpoint
3. Crear el hook de consulta
4. Confirmar de donde sale `idTareaWf`
5. Mapear la respuesta a `AppTabItem[]`
6. Actualizar `GestionRespuesta.tsx`
7. Ajustar el contenido del tab si necesita props dinamicas
8. Agregar pruebas de service, hook y render de tabs


## Riesgos

- no tener identificado aun el origen real de `idTareaWf`
- mezclar tabs estructurales (`Documentos`) con tabs de datos sin una regla clara
- asumir que `message` controla vacio o error
- asumir que siempre llega al menos una respuesta


## Decision recomendada

Si me pides implementarlo despues, yo lo haria asi:

- `Documentos` queda como tab fijo
- los items devueltos por el endpoint reemplazan el tab fijo `Gestion`
- `GestionRespuesta.tsx` pasa a ser el orquestador de query + estados UI
- `GestionRespuestaMainTabContent` recibe props del item seleccionado solo si realmente las necesita

Eso mantiene el diseño actual, usa el patron de servicios/hooks del repo y
evita acoplar la UI directamente al endpoint.
