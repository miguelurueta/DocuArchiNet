# PROMPT DE DEUDA TÉCNICA - Frontend Radicación

# TD-FE-01 - Unificar fuente de plantilla y eliminar doble carga

## Ticket Asociado

```text
SCRUMCORE-290
```

Este prompt queda asociado a `SCRUMCORE-290` para trazabilidad, rollback y auditoría del ajuste.

## Contexto Arquitectónico

Este trabajo debe realizarse respetando el **Contexto Arquitectónico Maestro del proyecto**, por lo tanto:

- La carga de datos pertenece al **boundary del módulo** (Route o Provider).
- Debe existir una única fuente de verdad (Single Source of Truth) por ciclo de vida del módulo.
- Los componentes de presentación no deben conocer cómo se obtiene la información.
- Debe evitarse la duplicación de consultas, estados y lógica de negocio.
- La solución debe ser backward-compatible y no introducir regresiones.
- No introducir abstracciones innecesarias (YAGNI).

---

## Objetivo

Eliminar la deuda técnica donde la plantilla de radicación es cargada más de una vez durante el montaje del módulo.

Debe existir una única fuente de verdad para la plantilla.

**Regla obligatoria:**

```text
La plantilla de radicación debe cargarse una única vez por montaje del módulo.

La carga pertenece al boundary del módulo (Route o Provider).

Los componentes hijos únicamente consumen la información ya cargada.
```

---

## Problema Actual

Actualmente existe una violación del principio de Single Source of Truth.

```text
RadicacionRoutePage
    │
    ├── useCamposPlantilla()
    │
    ▼
plantilla
    │
    ▼
RadicacionPage
    │
    ├── ignora plantilla
    │
    ▼
RadicacionTabs
    │
    ▼
RadicacionForm
    │
    └── vuelve a ejecutar useCamposPlantilla()
```

Esto genera:

- doble request al backend
- doble estado
- posible inconsistencia
- acoplamiento del formulario al origen de datos
- mayor tiempo de render
- mayor complejidad de testing
- deuda técnica

---

## Evidencia Actual

### Route

```text
src/modules/radicacion/pages/RadicacionRoutePage.tsx

useCamposPlantilla()

↓

mapCamposPlantillaToPlantillaRadicado()

↓

<RadicacionPage plantilla={plantilla} />
```

### Page

```text
src/modules/radicacion/pages/RadicacionPage.tsx

Recibe plantilla

↓

void plantilla

↓

<TabsDocu />
```

La plantilla nunca es utilizada.

### Formulario

```text
src/modules/radicacion/components/RadicacionForm.tsx

↓

useCamposPlantilla()

↓

Nueva consulta al backend
```

---

## Objetivo Arquitectónico

La carga de la plantilla debe vivir únicamente en el Composition Root del módulo.

En este caso el Composition Root recomendado es:

```text
RadicacionRoutePage
```

El formulario debe convertirse en un consumidor puro de datos.

No debe conocer cómo se obtiene la plantilla.

---

## Diseño Esperado

### Estrategia recomendada (obligatoria)

Utilizar composición mediante props.

```text
RadicacionRoutePage
        │
        │ useCamposPlantilla()
        ▼
 plantilla
        │
        ▼
RadicacionPage
        │
        ▼
RadicacionTabs
        │
        ▼
RadicacionForm
```

La plantilla se propaga desde la ruta hasta el formulario.

No existen nuevas consultas.

### Compatibilidad con el formulario actual

El formulario actual consume metadata cruda de `CampoPlantillaDTO[]`, por ejemplo:

- `ilist_row_drowlist`
- `tooltipAyuda`
- `disable_campo`
- `TomPParameterTomSelelect`
- `tbl_control`
- `ComportamientoCampo`

Por lo tanto, para mantener compatibilidad total y evitar regresiones, la ruta puede entregar ambos datos derivados de la misma carga:

```text
RadicacionRoutePage
        │
        │ useCamposPlantilla()
        ▼
 camposPlantilla: CampoPlantillaDTO[]
 plantilla: PlantillaRadicadoDTO
        │
        ▼
RadicacionPage
        │
        ▼
RadicacionTabs
        │
        ▼
RadicacionForm
```

Reglas:

- `camposPlantilla` y `plantilla` deben derivarse de una única llamada a `useCamposPlantilla`.
- `RadicacionForm` puede recibir `camposPlantilla` por props para conservar el comportamiento funcional existente.
- `RadicacionForm` no debe reconstruir, consultar ni conocer el origen de la plantilla.
- No debe introducirse una segunda fuente de verdad: `camposPlantilla` es la data cruda cargada una sola vez y `plantilla` es su DTO transformado.

---

## Estrategia alternativa

Únicamente si existe una necesidad real de compartir la plantilla entre múltiples consumidores independientes.

En ese caso podrá implementarse:

```text
RadicacionPlantillaProvider

- plantilla
- camposPlantilla
- loading
- error
```

No implementar Provider si solamente existe un flujo lineal.

No utilizar simultáneamente:

- Props
- Context

Debe elegirse una única estrategia.

---

## Responsabilidades Esperadas

### RadicacionRoutePage

Responsable de:

- cargar plantilla
- manejar loading
- manejar error
- transformar DTO
- entregar la plantilla
- entregar `camposPlantilla` cuando el formulario necesite metadata cruda para mantener compatibilidad

No contiene lógica del formulario.

---

### RadicacionPage

Responsable de:

- composición del módulo
- distribuir la plantilla
- distribuir `camposPlantilla` si se usa la estrategia compatible por props

No realiza consultas.

---

### RadicacionTabs

Responsable únicamente de propagar la información.

No consulta backend.

---

### RadicacionForm

Responsable únicamente de:

- renderizar campos
- validaciones
- interacción del usuario

No consulta backend.

No ejecuta useCamposPlantilla.

No conoce el origen de la plantilla.

Si requiere metadata cruda para conservar comportamiento actual, debe recibir `camposPlantilla` desde arriba.

---

### useCamposPlantilla

Debe permanecer únicamente en el boundary del módulo.

No debe utilizarse desde componentes internos.

---

## Alcance

Modificar únicamente lo necesario para eliminar la duplicidad de carga.

Incluye:

- utilizar realmente la prop plantilla en RadicacionPage
- propagar plantilla hacia RadicacionTabs
- propagar plantilla hacia RadicacionForm
- propagar `camposPlantilla` cuando sea necesario para conservar comportamiento del formulario
- eliminar el uso innecesario de useCamposPlantilla en el formulario
- eliminar console.log del hook
- mantener comportamiento funcional

---

## Restricciones

No modificar:

- flujo de radicación
- registro contra backend
- documentos
- pendientes
- validaciones del formulario
- comportamiento funcional existente

No introducir:

- breaking changes
- dependencias nuevas
- estados duplicados
- consultas duplicadas
- lógica de negocio en componentes Shared

---

## Principios Arquitectónicos Obligatorios

Aplicar:

- Single Source of Truth
- Separation of Concerns
- Smart Route / Dumb Components
- Composition Root
- Dependency Inversion
- Clean Architecture
- Backward Compatibility

---

## Criterios de Aceptación

### Arquitectura

- Existe una única fuente de verdad para la plantilla.
- La carga ocurre únicamente una vez por montaje.
- El formulario deja de conocer el mecanismo de carga.
- La responsabilidad de carga pertenece al Route (o Provider si fue justificado).
- Si se propaga `camposPlantilla`, debe provenir de la misma carga única de la ruta.

---

### Funcionales

- RadicacionPage utiliza realmente plantilla.
- RadicacionTabs recibe plantilla.
- RadicacionForm recibe plantilla desde arriba.
- RadicacionForm recibe `camposPlantilla` desde arriba si lo requiere para mantener comportamiento actual.
- RadicacionForm deja de llamar useCamposPlantilla.
- No existen dobles consultas.
- No existen regresiones.

---

### Backend

Debe existir únicamente una llamada a:

```text
/api/PlantillaRadicado/listaPlantilla
```

por montaje del módulo.

---

### Calidad

Eliminar:

```ts
console.log(data)
```

de:

```text
useCamposPlantilla
```

---

## Testing Obligatorio

### Unitarios

Validar:

- propagación de plantilla
- propagación de `camposPlantilla` cuando aplique
- render del formulario usando props
- ausencia de carga duplicada
- transformación correcta del DTO

---

### Integración

Validar:

- Route → Page
- Page → Tabs
- Tabs → Form
- la metadata cruda requerida por el formulario llega desde la carga única del Route

---

### Regresión

Validar que:

- el flujo de radicación continúa funcionando
- no existen cambios visibles para el usuario
- no aparecen errores de consola
- no existen errores TypeScript
- build exitoso
- lint limpio

---

## Entregables Esperados

1. Lista de archivos modificados.

2. Resumen técnico:

- antes vs después
- eliminación de la doble carga
- estrategia utilizada
- justificación arquitectónica

3. Resultado de pruebas ejecutadas.

4. Riesgos residuales.

5. Próximos pasos sugeridos.

---

## Instrucción Final

Refactorizar el flujo de carga de la plantilla de radicación para garantizar una única fuente de verdad durante el montaje del módulo, trasladando la responsabilidad de carga al boundary (Route o Provider cuando esté justificado), propagando la información mediante composición, eliminando consultas duplicadas y manteniendo compatibilidad total con el comportamiento actual, sin introducir breaking changes ni regresiones.
