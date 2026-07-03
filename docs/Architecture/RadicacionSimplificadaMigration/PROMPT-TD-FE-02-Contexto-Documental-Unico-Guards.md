# PROMPT DE DEUDA TÉCNICA - Frontend Radicación

# TD-FE-02 - Contexto Documental Único y Guards del Módulo

---

# Contexto Arquitectónico

Este trabajo debe respetar el Contexto Arquitectónico Maestro del proyecto.

Principios aplicables:

- Single Source of Truth.
- Composition Root.
- Smart Context / Dumb Components.
- Separation of Concerns.
- Clean Architecture.
- Backward Compatibility.

El estado documental del módulo debe administrarse desde un único punto y no distribuirse mediante estados locales en páginas o componentes.

---

# Objetivo

Crear la infraestructura base que gobernará el estado documental completo del módulo de Radicación.

Esta deuda técnica constituye el punto de integración común para los desarrollos posteriores relacionados con:

- Pendientes
- Tomar Pendiente
- Enviar a Pendiente
- Restauración de estado
- Gestión Documental

Su propósito es garantizar que todos los flujos compartan una única fuente de verdad respecto al estado documental.

---

# Problema Actual

Actualmente existen múltiples problemas de arquitectura:

```text
RadicacionTabs siempre habilita Documentos.

↓

CapDocument siempre renderiza.

↓

No existe contexto documental compartido.

↓

Cada flujo deberá resolver su propio estado.
```

Consecuencias:

- no existe gobierno del estado documental;
- Documentos puede abrirse en estados inválidos;
- no existe integración común entre módulos;
- difícil incorporación de pendientes;
- riesgo de inconsistencias futuras.

---

# Regla Arquitectónica Obligatoria

Debe existir un único contexto documental para todo el módulo.

```text
Documentos solamente puede activarse cuando exista un trámite documental activo
con estado = 0.
```

No deben existir estados documentales duplicados en páginas o componentes.

---

# Objetivo Arquitectónico

Introducir un Context especializado para el estado documental.

A diferencia de la plantilla de radicación (TD-FE-01), este estado sí debe vivir en un Context porque:

- es transversal al módulo;
- será consumido por múltiples páginas;
- será utilizado por múltiples flujos;
- sobrevivirá a navegación interna;
- constituye un estado compartido del proceso de negocio.

---

# Arquitectura Esperada

```text
RadicacionRoutePage
        │
        ▼
RadicacionDocumentalProvider
        │
        ▼
RadicacionPage
        │
        ▼
RadicacionTabs
        │
        ├──────────────┐
        ▼              ▼
Resumen      RadicacionDocumentosGuard
                     │
                     ▼
                 CapDocument
```

Todo el estado documental debe provenir exclusivamente del Provider.

---

# Componentes Esperados

Crear o consolidar:

```text
src/modules/radicacion/context/RadicacionDocumentalContext.tsx

src/modules/radicacion/hooks/useRadicacionDocumentalContext.ts

src/modules/radicacion/components/RadicacionDocumentosGuard.tsx

src/modules/radicacion/components/RadicacionStartupGuard.tsx
```

Si el contrato crece o se comparte entre provider, guards, tabs y pruebas, extraer tipos a:

```text
src/modules/radicacion/types/radicacionDocumental.types.ts
```

---

# Contrato del Contexto

Debe soportar como mínimo:

```ts
type RadicacionDocumentalState = {
  idEstadoRadicado: number | null;

  idRadicado?: number | null;

  consecutivoRadicado?: string | null;

  idTareaWorkflow?: number | null;

  estadoActual?: 0 | 1 | null;

  requiereGestionDocumental: boolean;

  tieneTramiteDocumentalActivoEstado0: boolean;

  destinoPostRegistro?: "resumen" | "documentos";
};

type RadicacionDocumentalContextValue = RadicacionDocumentalState & {
  setContextoDocumental: (
    value: RadicacionDocumentalState
  ) => void;

  clearContextoDocumental: () => void;
};
```

El Context debe ser la única fuente de verdad del estado documental.

El estado inicial debe representar ausencia de trámite documental activo.

---

# Integraciones Esperadas

El Context será el punto único de integración para futuras deudas técnicas.

### FE-05

Tomar Pendiente

- inicializa el contexto.

---

### FE-06

Restauración

- reconstruye el contexto.

---

### FE-07

Enviar a Pendiente

- limpia completamente el contexto.

---

### RadicacionTabs

Debe leer exclusivamente el Context para determinar:

- habilitar pestaña Documentos;
- deshabilitar pestaña;
- navegación disponible.

No debe mantener estado propio.

RadicacionTabs no calcula reglas de negocio; únicamente consume flags derivados del Context para habilitar, deshabilitar o redirigir.

---

# Guard Documental

Implementar:

```text
RadicacionDocumentosGuard
```

Debe permitir renderizar únicamente cuando:

```text
estadoActual === 0

AND

requiereGestionDocumental === true

AND

tieneTramiteDocumentalActivoEstado0 === true

AND

idEstadoRadicado > 0
```

El flag `tieneTramiteDocumentalActivoEstado0` puede ser derivado del contexto o persistido desde backend, pero debe mantenerse coherente con:

```text
requiereGestionDocumental === true
estadoActual === 0
idEstadoRadicado > 0
```

---

# Comportamiento

Si la condición NO se cumple:

- no renderizar CapDocument;
- no inicializar gabinete documental;
- no cargar digitalización;
- no realizar consultas asociadas;
- mostrar estado no disponible o redirigir a Resumen.

---

# Responsabilidades

## Provider

Responsable de:

- almacenar estado documental;
- restaurar estado;
- limpiar estado;
- exponer operaciones.

No contiene UI.

---

## Hook

Responsable únicamente de consumir el contexto.

No contiene lógica de negocio.

---

## Guard

Responsable de decidir si el flujo documental puede renderizarse.

No conoce backend.

No conoce lógica de workflow.

---

## Tabs

Responsable únicamente de mostrar la navegación.

No decide reglas documentales.

Debe consumir únicamente el Context o flags derivados del Context para reflejar navegación disponible.

---

## CapDocument

Debe asumir que si fue renderizado ya pasó todas las validaciones.

No debe volver a validar estado documental.

---

# Separación con TD-FE-04

TD-FE-02 prepara infraestructura de contexto y guards.

La creación, normalización o limpieza de rutas hijas, keys semánticas de tabs y rutas como:

```text
/dashboard/radicacion/registro/:idEstadoRadicado
/dashboard/radicacion/registro/:idEstadoRadicado/documentos
```

queda fuera de alcance y será cubierta por TD-FE-04.

TD-FE-02 solo debe dejar los puntos de integración necesarios para que esas rutas puedan consumir el contexto documental cuando existan.

---

# Restricciones

No implementar:

- consumo backend;
- upload;
- digitalización;
- AppTable;
- workflow;
- pendientes;
- rediseño visual de tabs;
- rutas hijas definitivas.

Esta deuda únicamente prepara la infraestructura.

---

# Principios Arquitectónicos

Aplicar obligatoriamente:

- Single Source of Truth.
- Smart Context.
- Separation of Concerns.
- Composition Root.
- Clean Architecture.
- Backward Compatibility.
- Open/Closed Principle.

---

# Criterios de Aceptación

## Arquitectura

- Existe un único contexto documental.
- No existen estados documentales duplicados.
- Los componentes consumen exclusivamente el Context.
- Existe un único punto de integración para futuras funcionalidades.
- El Provider envuelve el módulo desde el composition root.
- El guard centraliza el acceso a documentos.

---

## Funcionales

- Documentos no puede abrirse sin estado 0.
- Documentos no puede abrirse si `requiereGestionDocumental !== true`.
- CapDocument no renderiza en estados inválidos.
- RadicacionTabs habilita Documentos únicamente mediante el Context.
- El Guard controla completamente el acceso.
- CapDocument no inicializa digitalización cuando no hay contexto válido.

---

## Testing

### Unitarios

Validar:

- contexto inicial;
- setContextoDocumental();
- clearContextoDocumental();
- selector del hook;
- guard con estado válido;
- guard con estado inválido;
- guard bloquea cuando `requiereGestionDocumental` es false.

---

### Integración

Validar:

- Provider → Tabs;
- Provider → Guard;
- Guard → CapDocument;
- RadicacionTabs refleja estado disabled/enabled desde Context.

---

### Regresión

Validar:

- navegación actual;
- build;
- TypeScript;
- lint;
- consola limpia.

---

# Entregables Esperados

1. Lista de archivos creados/modificados.

2. Resumen técnico:

- antes vs después;
- arquitectura del Context;
- estrategia del Guard;
- punto de integración futuro.

3. Resultado de pruebas.

4. Riesgos residuales.

5. Próximas deudas técnicas que consumirán este Context.

---

# Instrucción Final

Implementar una infraestructura documental única para el módulo de Radicación mediante un Context especializado y Guards de acceso, garantizando que el estado documental tenga una única fuente de verdad, que los componentes de presentación permanezcan desacoplados del flujo documental y que el módulo quede preparado para integrar Pendientes, Restauración y Gestión Documental sin introducir breaking changes ni regresiones.
