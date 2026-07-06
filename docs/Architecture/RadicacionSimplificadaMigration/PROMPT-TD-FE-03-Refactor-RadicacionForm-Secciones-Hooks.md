# PROMPT DE DEUDA TÉCNICA - Frontend Radicación

# TD-FE-03 - Descomposición Arquitectónica de RadicacionForm

---

# Contexto Arquitectónico

Esta deuda técnica debe implementarse respetando las decisiones previamente adoptadas:

- TD-FE-01 → Single Source of Truth.
- TD-FE-02 → Context documental único.
- TD-FE-04 → Navegación desacoplada.
- TD-FE-05 → UI Reset separado del Workflow.
- FE-05 → Casos de uso para mutaciones.
- FE-06 → Startup Guard.
- FE-07 → Mutaciones transaccionales.

Este refactor aplica Smart Components mediante composición.

No cambiar comportamiento funcional.

No introducir nuevas funcionalidades.

---

# Objetivo

Reducir el acoplamiento interno de `RadicacionForm.tsx` mediante una descomposición incremental basada en composición.

El objetivo no es reducir únicamente el tamaño del archivo, sino separar responsabilidades claramente.

---

# Problema Actual

Actualmente `RadicacionForm` concentra responsabilidades de múltiples capas:

```text
Layout

↓

Render

↓

Estado UI

↓

Normalización

↓

Selección de trámite

↓

Remitente

↓

Destinatario

↓

Autocompletes

↓

Footer

↓

Modales

↓

Mapeos

↓

Tipos
```

Esto dificulta:

- pruebas unitarias;
- reutilización;
- mantenimiento;
- lectura;
- evolución del formulario.

---

# Evidencia Actual

El archivo:

```text
src/modules/radicacion/components/RadicacionForm.tsx
```

contiene componentes internos y lógica especializada como:

```text
BaseSelectUsuarios
SelectRemitente
SelectDestinatario
SelectRemitenteToken
SelectDestinatarioToken
resolveCampoIdScript
```

También contiene mappers inline para variantes de contrato backend:

```text
idValue
id_value
value
Value
value_campo
```

y casts/tipos amplios:

```text
any
unknown
```

El footer está embebido y contiene acciones que pertenecen a otros casos de uso:

```text
Limpiar
Enviar a Pendientes
Radicar
Documentos IA
```

---

# Objetivo Arquitectónico

Transformar `RadicacionForm` en un componente orquestador.

El componente principal únicamente debe:

- componer secciones;
- conectar hooks;
- distribuir props.

Toda lógica especializada debe migrarse gradualmente.

---

# Arquitectura Objetivo

```text
RadicacionForm

│

├── useRadicacionFormOptions()

├── useRadicacionTramiteSelection()

│

├── RadicacionMetadataSection

├── RadicacionTramiteSection

├── RadicacionRemitenteSection

├── RadicacionDestinatarioSection

├── RadicacionFormFooter

│

└── utils/
      radicacionOptionMappers
```

---

# Estrategia

Aplicar refactor incremental.

Cada extracción debe finalizar con pruebas verdes.

No realizar un Big Bang Refactor.

---

# Orden de Extracción

## Primera etapa

Footer

```text
RadicacionFormFooter
```

Debe contener únicamente:

- botones;
- acciones recibidas por props;
- layout.

No lógica del formulario.

No casos de uso.

No mutaciones.

No limpieza directa.

Las acciones del footer deben recibirse como callbacks desde `RadicacionForm` o desde hooks especializados definidos por otros prompts:

```text
handleClearRadicacionForm
handleEnviarPendiente
handleRadicar
handleDocumentosIa
```

---

## Segunda etapa

Trámite / Flujo

```text
RadicacionTramiteSection
```

Toda la lógica de selección debe migrar al hook:

```text
useRadicacionTramiteSelection()
```

Este hook debe coordinar:

```text
selectedTramiteId
hasUserChangedTramite
flujoOptions
isLoadingFlujosRelacionados
limpieza de flujo al cambiar trámite
```

sin conocer backend documental, rutas ni contexto documental.

---

## Tercera etapa

Remitente

Destinatario

Cada sección debe encapsular únicamente la UI correspondiente.

Los componentes inline actuales deben extraerse o quedar encapsulados detrás de secciones:

```text
BaseSelectUsuarios
SelectRemitente
SelectDestinatario
SelectRemitenteToken
SelectDestinatarioToken
```

---

## Cuarta etapa

Mappers

Extraer completamente:

```text
idValue

id_value

value

Value

value_campo

resolveCampoIdScript
```

Toda tolerancia a contratos backend debe vivir exclusivamente en:

```text
utils/radicacionOptionMappers.ts
```

Nunca dentro del JSX.

---

## Quinta etapa

Tipos

Eliminar gradualmente:

```text
any

unknown

casts repetidos
```

Introduciendo tipos especializados.

---

# Componentes Esperados

```text
components/

RadicacionFormFooter.tsx

RadicacionTramiteSection.tsx

RadicacionRemitenteSection.tsx

RadicacionDestinatarioSection.tsx

RadicacionMetadataSection.tsx

hooks/

useRadicacionTramiteSelection.ts

useRadicacionFormOptions.ts

utils/

radicacionOptionMappers.ts
```

---

# Responsabilidades

## RadicacionForm

Responsable únicamente de:

- composición;
- distribución de props;
- coordinación.

No debe contener lógica especializada.

No debe cargar plantilla si TD-FE-01 ya movió esa responsabilidad al boundary del módulo.

No debe reintroducir `useCamposPlantilla`.

---

## Sections

Responsables únicamente de representar una parte del formulario.

No contienen reglas del proceso documental.

No consultan backend.

No ejecutan mutaciones.

---

## Hooks

Responsables únicamente de la lógica UI de cada sección.

No conocen backend documental.

No realizan mutaciones del proceso.

No navegan.

---

## Mappers

Responsables únicamente de normalizar contratos backend.

Toda tolerancia debe vivir aquí.

Nunca dentro del render.

---

# Integración con otros prompts

## TD-FE-01

El formulario debe consumir datos recibidos desde arriba.

Este refactor no debe reintroducir carga de plantilla ni nuevos hooks de carga dentro del formulario.

## TD-FE-05

`RadicacionFormFooter` debe consumir el callback de limpieza, no implementarlo.

## FE-07

`RadicacionFormFooter` debe consumir el callback de enviar a pendiente, no ejecutar la mutación directamente.

---

# Tipado

Crear tipos específicos para:

- opciones Ant Design;
- opciones backend;
- menú de etiquetas;
- campos dinámicos;
- contratos de selección;
- campos con `id_escript`;
- parámetros de tag render.

Evitar:

```text
any

unknown
```

salvo donde sea estrictamente necesario por integración externa.

Si `unknown` es necesario, debe quedar encapsulado en un mapper o guard tipado, no repetido dentro del JSX.

---

# Restricciones

No modificar:

- comportamiento;
- flujo documental;
- Context;
- navegación;
- backend;
- casos de uso.

No introducir nuevas funcionalidades.

No mover lógica transaccional al footer.

No cambiar diseño visual salvo ajustes mínimos derivados de la extracción.

---

# Principios Arquitectónicos

Aplicar:

- Composition over Inheritance.
- Single Responsibility.
- Presentational Components.
- Smart UI Hooks.
- Centralized Mapping.
- Clean Architecture.
- Backward Compatibility.

---

# Testing

## Unitarios

Validar:

- Footer;
- Sections;
- Hooks;
- Mappers.

---

## Integración

Validar:

- composición completa del formulario;
- interacción entre secciones;
- comportamiento previo;
- callbacks del footer se invocan sin ejecutar lógica propia.

---

## Regresión

Validar:

- build;
- lint;
- TypeScript;
- comportamiento visual;
- pruebas existentes.

---

# Criterios de Aceptación

- `RadicacionForm` reduce significativamente sus responsabilidades.
- El Footer queda completamente desacoplado.
- El Footer recibe acciones por props.
- La lógica de trámite vive en un hook especializado.
- Los mappers dejan de estar embebidos en JSX.
- `resolveCampoIdScript` queda centralizado.
- Los tipos quedan centralizados.
- No cambia el comportamiento funcional.
- No se reintroduce carga de plantilla en el formulario.
- No existen regresiones.
- Las pruebas existentes continúan pasando.
- Existen nuevas pruebas para hooks, mappers y componentes extraídos.

---

# Entregables

1. Lista de archivos modificados.

2. Resumen técnico:

- antes vs después;
- responsabilidades extraídas;
- mejoras de mantenibilidad;
- reducción del acoplamiento.

3. Resultado de pruebas.

4. Riesgos residuales.

5. Próximas oportunidades de descomposición.

---

# Instrucción Final

Refactorizar `RadicacionForm` mediante una estrategia incremental basada en composición, transformándolo en un componente orquestador que delegue la lógica especializada a componentes presentacionales, hooks de UI y mappers centralizados, preservando completamente el comportamiento funcional, manteniendo compatibilidad hacia atrás y evitando regresiones.
