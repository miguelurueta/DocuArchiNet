# Design

## Context

`SCRUMCORE-130` implementa el consumo de la API
`solicita-estructura-respuesta-id-tarea` dentro del tab principal de
`GestionRespuesta`, concretamente en:

- `src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx`

El objetivo funcional es reemplazar la metadata estática del
`GestionRespuestaInfoHeader` por metadata real derivada de la tarea de workflow
activa.

## Scope Of This FE

Esta FE cubre:

- consumo HTTP por `idTareaWf`
- tipado del response relevante
- normalización del payload hacia un modelo de UI estable
- exposición de una variable reutilizable llamada `estrucTuraRespuesta`
- reemplazo del `metadata` fijo del header
- manejo de loading, vacío y error sin romper la vista

No cubre:

- cambios del backend
- tabs dinámicos adicionales
- submit o persistencia de formularios
- rediseño del workbench

## Design Decisions

### 1. El consumo vive en `GestionRespuestaMainTabContent`

El requerimiento pide explícitamente consumir la API cuando carga el componente
de `gestionRespuestaMainTab`, por lo que la integración debe resolverse en
`GestionRespuestaMainTabContent.tsx` o en un hook consumido directamente por ese
componente.

No se moverá esta responsabilidad a `GestionRespuesta.tsx` salvo que sea
necesario únicamente para inyectar `idTareaWf`.

### 2. `estrucTuraRespuesta` será el contrato reusable de UI

El response crudo del backend no debe entrar directamente al render. Se define
un modelo frontend estable que represente únicamente los datos necesarios para
la UI:

- `Radicado`
- `Destinatario`
- `TramiteDocumento`

La variable final consumida por el componente debe llamarse:

- `estrucTuraRespuesta`

### 3. La normalización se resuelve fuera del JSX

El componente no debe mezclar fallback mapping ni casing del backend dentro del
array `metadata`. La transformación del payload debe resolverse en:

- un adapter dedicado, o
- el hook del módulo

Esto mantiene el render limpio y la lógica reusable.

### 4. La lógica de estado debe separar carga, vacío y error

La integración debe diferenciar claramente tres estados:

- `loading`
- `isEmpty`
- `error`

No se debe usar `message` del backend para decidir la lógica. La fuente de
verdad debe ser:

- `success`
- `data`
- `data.length`

### 5. El header debe tener fallback robusto

Aunque la tarea pide reemplazar el metadata por `estrucTuraRespuesta`, la vista
no debe romperse si:

- el request falla
- no hay resultados
- faltan campos

Por eso el render final debe usar un fallback neutral como `"-"`.

## Technical Approach

### Tipo del backend

Se creará un contrato tipado del item de respuesta del endpoint, soportando
variaciones de casing si el backend no es estable.

### Tipo del modelo UI

Se definirá `GestionRespuestaEstructuraRespuesta` como modelo frontend
normalizado.

### Service

Se creará un service del módulo para encapsular:

- endpoint
- params
- response tipada

### Hook

Se creará un hook del módulo para exponer:

- `estrucTuraRespuesta`
- `loading`
- `error`
- `isEmpty`

### Integración visual

`GestionRespuestaMainTabContent.tsx` consumirá el hook y reemplazará:

- `Origen`
- `Estado`
- `SLA`

por:

- `Radicado`
- `Remitente`
- `Trámite`

## Dependencies

La implementación depende de identificar la fuente real de `idTareaWf`.

El componente no debe inventar este valor. Debe:

- recibirlo por props, o
- resolverlo desde el contexto/ruta/selección actual del módulo

Si ese dato aún no está disponible en la vista, será necesario extender el
contenedor superior.

## Risks

- no contar todavía con una fuente confiable de `idTareaWf`
- acoplar el render al response crudo
- asumir que `data[0]` siempre existe
- mezclar cambios adicionales del workbench en la misma FE

## Result

Al finalizar esta FE, `GestionRespuestaMainTabContent` debe mostrar metadata
real de la estructura de respuesta de workflow a través de `estrucTuraRespuesta`
y dejar ese modelo disponible para futuras operaciones del módulo.
