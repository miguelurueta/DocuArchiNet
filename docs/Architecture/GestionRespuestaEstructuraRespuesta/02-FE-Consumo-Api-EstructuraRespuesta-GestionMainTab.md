# SCRUMCORE-129
# Consumo Enterprise de `solicita-estructura-respuesta-id-tarea` en `GestionRespuestaMainTab`

## Resumen Ejecutivo

Esta FE tiene como objetivo integrar el endpoint:

```txt
GET /api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea?idTareaWf=:id
```

en el componente:

```txt
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx
```

para obtener la estructura de respuesta asociada a una tarea de workflow y
almacenarla en una variable reutilizable llamada:

```ts
estrucTuraRespuesta
```

Esa estructura debe reemplazar el `metadata` actualmente fijo del
`GestionRespuestaInfoHeader`.


## Objetivo Funcional

Al cargar el tab principal de gestión, el frontend debe:

- consultar la API por `idTareaWf`
- normalizar el response
- guardar la estructura útil en `estrucTuraRespuesta`
- usarla para renderizar:
  - `Radicado`
  - `Remitente`
  - `Trámite`

La salida funcional esperada es esta:

```tsx
<GestionRespuestaInfoHeader
  metadata={[
    { label: "Radicado", value: estrucTuraRespuesta?.Radicado ?? "-" },
    { label: "Remitente", value: estrucTuraRespuesta?.Destinatario ?? "-" },
    { label: "Trámite", value: estrucTuraRespuesta?.TramiteDocumento ?? "-" },
  ]}
/>
```


## Alcance

### Incluye

- consumo HTTP del endpoint por `idTareaWf`
- normalización del response del backend
- almacenamiento reutilizable en `estrucTuraRespuesta`
- reemplazo del header fijo en `GestionRespuestaMainTabContent.tsx`
- manejo de `loading`, vacío y error
- cobertura mínima de pruebas del flujo

### No Incluye

- cambios en el endpoint backend
- submit de formularios
- persistencia adicional
- rediseño general del workbench
- tabs dinámicos adicionales


## Contexto Actual

Hoy `GestionRespuestaMainTabContent.tsx` renderiza este bloque fijo:

```tsx
<GestionRespuestaInfoHeader
  metadata={[
    { label: "Origen", value: "Bandeja de correspondencia" },
    { label: "Estado", value: "Pendiente de validacion" },
    { label: "SLA", value: "4 horas restantes" },
  ]}
/>
```

Ese bloque no está conectado al contexto real de la tarea de workflow.

El requerimiento de negocio exige reemplazarlo por metadata real proveniente de
la API `solicita-estructura-respuesta-id-tarea`.


## Contrato Backend Relevante

### Endpoint

- Método: `GET`
- Ruta: `/api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea`
- Querystring: `idTareaWf`

### Response

```ts
ApiResponse<RaRespuestaRadicado[]>
```

### Semántica de Consumo

- `success === true` y `data.length > 0`
  Existe estructura utilizable.
- `success === true` y `data.length === 0`
  No hay resultados. La UI debe mostrar fallback.
- `success === false`
  Hay error controlado. La UI no debe asumir datos.

Regla de implementación:

- no usar `message` para lógica
- usar `success`, `data` y el tamaño de la lista


## Decisión de Arquitectura

### 1. El consumo vive en `GestionRespuestaMainTabContent`

Esta decisión se toma porque el requerimiento es explícito respecto al lugar
donde debe iniciarse la consulta: cuando carga el componente de
`gestionRespuestaMainTab`.

### 2. `estrucTuraRespuesta` será el modelo UI estable

No se debe acoplar directamente el render al DTO crudo del backend. La UI debe
trabajar con un modelo frontend estable llamado:

```ts
GestionRespuestaEstructuraRespuesta
```

y la variable final consumida en el componente debe llamarse:

```ts
estrucTuraRespuesta
```

### 3. La normalización se hará fuera del JSX

No se debe resolver el shape del backend directamente dentro del render. La
adaptación debe vivir en un mapper o en un hook del módulo.


## Modelo Frontend Recomendado

```ts
export type SolicitaEstructuraRespuestaBackendItem = {
  Radicado?: string;
  Destinatario?: string;
  TramiteDocumento?: string;
  radicado?: string;
  destinatario?: string;
  tramiteDocumento?: string;
};

export type GestionRespuestaEstructuraRespuesta = {
  Radicado: string;
  Destinatario: string;
  TramiteDocumento: string;
};
```

Razonamiento:

- soporta inconsistencias de casing del backend
- desacopla el componente del shape crudo
- deja una estructura reutilizable para futuras operaciones


## Diseño Técnico Propuesto

### Capa de tipos

Archivo nuevo:

```txt
src/modules/gestionCorrespondencia/types/gestionRespuestaEstructura.types.ts
```

Responsabilidades:

- tipar el item crudo del backend
- tipar el modelo final `GestionRespuestaEstructuraRespuesta`

### Capa de servicio

Archivo nuevo:

```txt
src/modules/gestionCorrespondencia/services/solicitaEstructuraRespuestaIdTarea.service.ts
```

Responsabilidades:

- encapsular la llamada HTTP
- recibir `idTareaWf`
- devolver `ApiResponse<SolicitaEstructuraRespuestaBackendItem[]>`

Patrón esperado:

- uso de `clienteApi`
- sin lógica de render
- sin estado React

### Capa de adaptación

Archivo nuevo recomendado:

```txt
src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.ts
```

Responsabilidades:

- transformar el primer item útil del backend al shape estable de UI
- resolver fallbacks vacíos

Ejemplo conceptual:

```ts
export const mapEstructuraRespuesta = (
  item?: SolicitaEstructuraRespuestaBackendItem,
): GestionRespuestaEstructuraRespuesta => ({
  Radicado: item?.Radicado ?? item?.radicado ?? "",
  Destinatario: item?.Destinatario ?? item?.destinatario ?? "",
  TramiteDocumento: item?.TramiteDocumento ?? item?.tramiteDocumento ?? "",
});
```

### Capa de hook

Archivo nuevo:

```txt
src/modules/gestionCorrespondencia/hooks/useEstructuraRespuestaIdTarea.ts
```

Responsabilidades:

- disparar la consulta al montar
- exponer:
  - `estrucTuraRespuesta`
  - `loading`
  - `error`
  - `isEmpty`

Contrato sugerido:

```ts
type UseEstructuraRespuestaIdTareaResult = {
  estrucTuraRespuesta: GestionRespuestaEstructuraRespuesta | null;
  loading: boolean;
  error: Error | null;
  isEmpty: boolean;
};
```

### Capa de integración visual

Archivo principal a modificar:

```txt
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx
```

Responsabilidades:

- resolver o recibir `idTareaWf`
- consumir el hook
- reemplazar el metadata fijo
- aplicar fallback seguro


## Diseño de Render

### Estado actual

```tsx
<GestionRespuestaInfoHeader
  metadata={[
    { label: "Origen", value: "Bandeja de correspondencia" },
    { label: "Estado", value: "Pendiente de validacion" },
    { label: "SLA", value: "4 horas restantes" },
  ]}
/>
```

### Estado objetivo

```tsx
<GestionRespuestaInfoHeader
  metadata={[
    { label: "Radicado", value: estrucTuraRespuesta?.Radicado ?? "-" },
    { label: "Remitente", value: estrucTuraRespuesta?.Destinatario ?? "-" },
    { label: "Trámite", value: estrucTuraRespuesta?.TramiteDocumento ?? "-" },
  ]}
/>
```

### Regla de fallback

Si no hay datos disponibles:

- usar `"-"` como fallback neutral
- no dejar `undefined` en la UI


## Dependencia Crítica

### Origen de `idTareaWf`

Este punto debe resolverse antes de implementar.

El componente necesita un `idTareaWf` real. Ese dato puede venir de:

- parámetro de ruta
- item seleccionado de la bandeja
- prop inyectada desde el contenedor superior

Regla:

- `GestionRespuestaMainTabContent` no debe inventar el valor
- debe recibirlo o resolverlo desde una fuente existente del flujo

Si hoy no está disponible, habrá que extender el contenedor superior.


## Archivos Impactados

### Nuevos

- `src/modules/gestionCorrespondencia/types/gestionRespuestaEstructura.types.ts`
- `src/modules/gestionCorrespondencia/services/solicitaEstructuraRespuestaIdTarea.service.ts`
- `src/modules/gestionCorrespondencia/hooks/useEstructuraRespuestaIdTarea.ts`
- `src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.ts`

### Modificados

- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`

### Posiblemente Modificados

- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`

Solo si se requiere inyectar `idTareaWf` desde un nivel superior.

### Tests

- `src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx`
- `src/modules/gestionCorrespondencia/tests/solicitaEstructuraRespuestaIdTarea.service.test.ts`
- `src/modules/gestionCorrespondencia/tests/useEstructuraRespuestaIdTarea.test.tsx`


## Flujo de Implementación Recomendado

1. Confirmar fuente real de `idTareaWf`
2. Crear tipos del contrato backend y del modelo UI
3. Crear service HTTP
4. Crear adapter de normalización
5. Crear hook `useEstructuraRespuestaIdTarea`
6. Integrar `estrucTuraRespuesta` en `GestionRespuestaMainTabContent.tsx`
7. Reemplazar el header fijo
8. Agregar fallbacks de vacío y error
9. Cubrir pruebas


## Riesgos

- `idTareaWf` no disponible en el flujo actual
- shape del backend distinto al esperado
- asumir que siempre existe `data[0]`
- acoplar el JSX a propiedades crudas del backend


## Criterios de Aceptación

- la API se consume al cargar `GestionRespuestaMainTabContent`
- existe una variable reutilizable llamada `estrucTuraRespuesta`
- `GestionRespuestaInfoHeader` deja de usar metadata estática
- el header muestra `Radicado`, `Remitente` y `Trámite`
- la UI maneja vacío y error sin romper el componente
- la implementación sigue el patrón `types + service + hook + adapter`


## Decisión Final

Para una implementación enterprise y mantenible, esta tarea debe ejecutarse con
una separación clara entre:

- contrato backend
- normalización frontend
- orquestación de estado
- render del componente

Eso permite cumplir el requerimiento puntual de negocio sin introducir deuda
técnica en `GestionRespuestaMainTabContent.tsx` y deja `estrucTuraRespuesta`
lista para reutilización futura.
