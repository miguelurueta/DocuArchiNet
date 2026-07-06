# PROMPT ARQUITECTÓNICO - Radicación Simplificada

# FE-06 - Bootstrap Inteligente del Módulo y Restauración del Estado Documental

---

# Contexto Arquitectónico

Esta fase debe implementarse respetando las decisiones arquitectónicas previamente adoptadas:

### TD-FE-01

La carga de información pertenece al Composition Root del módulo.

Los componentes de presentación no realizan consultas de inicialización.

---

### TD-FE-02

Existe un único `RadicacionDocumentalContext`.

Todo el estado documental debe obtenerse exclusivamente desde este Context.

No pueden existir stores paralelos.

---

## Objetivo

Implementar el proceso de bootstrap del módulo de Radicación.

Cuando el usuario ingrese al módulo, el sistema debe determinar si existe un trámite documental activo (`estado = 0`), restaurar automáticamente el contexto documental y resolver la navegación inicial.

La restauración debe ocurrir antes del render funcional del módulo.

---

# Problema Actual

Actualmente el módulo inicia sin conocer el estado documental activo.

Como consecuencia:

- Documentos no sabe si debe habilitarse.
- El usuario pierde continuidad del trámite.
- Cada flujo debería reconstruir su propio estado.
- No existe un bootstrap centralizado.

---

# Regla Arquitectónica

La inicialización del módulo pertenece exclusivamente al Startup Guard.

Las páginas nunca deben:

- consultar el estado activo;
- restaurar contexto;
- decidir la navegación inicial.

---

# Composition Root Esperado

```text
RadicacionRoutePage
        │
        ▼
RadicacionDocumentalProvider
        │
        ▼
RadicacionStartupGuard
        │
        ├── verifica estado activo
        ├── restaura contexto documental
        ├── resuelve navegación
        ├── maneja loading
        ├── maneja error
        │
        ▼
RadicacionPage
```

El Provider debe envolver al Startup Guard para que el bootstrap pueda escribir en el `RadicacionDocumentalContext`.

Todo el bootstrap ocurre antes del render funcional del módulo.

---

# Contrato Backend

API:

```text
GET /api/radicacion/pendientes/estado-activo
```

Mantener el contrato DTO ya definido.

No modificar el contrato backend.

Contrato esperado de referencia:

```ts
type RadicacionPendienteEstadoActivoDto = {
  tieneActivoEstado0: boolean;
  idEstadoRadicado?: number | null;
  idRadicado?: number | null;
  consecutivoRadicado?: string | null;
  idTareaWorkflow?: number | null;
  estadoActual?: 0 | null;
  tramite?: string | null;
  remitente?: string | null;
  plantillaId?: number | null;
  tipoPlantillaId?: number | null;
  requiereGestionDocumental: boolean;
  tieneTramiteDocumentalActivoEstado0: boolean;
  destinoPostRegistro: "resumen" | "documentos";
  contextoDocumental?: {
    idGabinete?: number | null;
    nombreGabinete?: string | null;
    idTipoTramite?: number | null;
    nombreTramite?: string | null;
    utilEstadoPendienteRad?: boolean;
  } | null;
};
```

---

# Componentes

Crear o extender únicamente:

```text
src/modules/radicacion/types/radicacionContextoDocumental.types.ts

src/modules/radicacion/services/radicacionPendientes.service.ts

src/modules/radicacion/hooks/useRadicacionEstadoActivo.ts

src/modules/radicacion/context/RadicacionDocumentalContext.tsx

src/modules/radicacion/components/RadicacionStartupGuard.tsx
```

No crear Context adicionales.

Si TD-FE-02 ya creó `src/modules/radicacion/types/radicacionDocumental.types.ts`, reutilizarlo o consolidar el tipo documental en una única ubicación. No duplicar tipos incompatibles.

---

# Responsabilidades

## Startup Guard

Responsable de:

- bootstrap del módulo;
- consultar estado activo;
- restaurar contexto;
- limpiar contexto;
- navegación inicial;
- loading inicial;
- recuperación ante errores.

No contiene UI funcional.

---

## Hook

Responsable únicamente de encapsular la consulta REST.

No conoce navegación.

No conoce UI.

---

## Service

Responsable del acceso HTTP.

No contiene lógica de negocio.

---

## Context

Responsable de almacenar el estado documental.

No consulta backend.

---

## RadicacionPage

Debe asumir que el módulo ya fue inicializado.

No conoce bootstrap.

---

# Flujo Esperado

```text
Usuario entra al módulo

↓

RadicacionDocumentalProvider

↓

Startup Guard

↓

GET estado-activo

↓

¿Existe estado = 0 y requiereGestionDocumental = true?

    SI
        ↓
    actualizar Context

        ↓
    navegar automáticamente

        ↓
    Documentos

NO

↓

limpiar Context

↓

Resumen

↓

Formulario disponible
```

---

# Regla Funcional

Documentos solamente puede habilitarse cuando:

```text
estadoActual == 0

AND

requiereGestionDocumental == true

AND

tieneTramiteDocumentalActivoEstado0 == true

AND

idEstadoRadicado > 0
```

Ningún otro dato puede habilitar el flujo documental.

Esta regla debe mantenerse alineada con TD-FE-02.

---

# Guard Documental

El acceso a Documentos debe depender exclusivamente del Context.

No validar utilizando:

- consecutivo;
- gabinete;
- workflow;
- idRadicado.

---

# Estados del Bootstrap

El Startup Guard debe manejar:

- inicializando;
- verificando estado;
- restaurando;
- activo encontrado;
- sin activo;
- error;
- reintentando.

Evitar render parcial del formulario cuando exista una navegación inmediata.

---

# Navegación y Separación con TD-FE-04

FE-06 debe resolver la navegación inicial usando el mecanismo disponible en el módulo en el momento de implementación.

Si aún no existen rutas hijas definitivas o tabs semánticas, no deben rediseñarse como parte de esta fase.

La creación, normalización o limpieza de rutas como:

```text
/dashboard/radicacion/registro/:idEstadoRadicado
/dashboard/radicacion/registro/:idEstadoRadicado/documentos
```

y la redefinición de keys semánticas de tabs pertenecen a TD-FE-04.

FE-06 debe dejar el bootstrap preparado para consumir esas rutas cuando existan, sin bloquear la restauración del contexto documental.

---

# Integraciones Futuras

Este bootstrap será consumido por:

- FE-05
- FE-07
- FE-08
- cualquier otra funcionalidad que dependa del estado documental.

---

# Restricciones

No implementar:

- listado de pendientes;
- tomar pendiente;
- enviar pendiente;
- digitalización;
- upload;
- visor;
- rediseño visual de tabs;
- rutas hijas definitivas.

---

# Principios Arquitectónicos

Aplicar:

- Single Source of Truth.
- Composition Root.
- Smart Bootstrap.
- Smart Guard.
- Separation of Concerns.
- Clean Architecture.
- Open/Closed.
- Backward Compatibility.

---

# Testing

## Unitarios

- hook;
- context;
- startup guard.

## Integración

- startup → context;
- startup → navegación;
- startup → tabs.

## Regresión

- build;
- lint;
- TypeScript;
- navegación;
- consola limpia.

---

# Criterios de Aceptación

- El módulo consulta estado-activo al iniciar.
- La restauración ocurre antes del render funcional.
- El Provider envuelve al Startup Guard.
- El Context queda sincronizado.
- Documentos únicamente se habilita mediante el Context.
- Documentos no se habilita si `requiereGestionDocumental !== true`.
- No existen stores paralelos.
- No existen consultas duplicadas.
- No existen regresiones.

---

# Entregables

1. Archivos modificados.

2. Resumen técnico:

- bootstrap;
- startup guard;
- restauración;
- navegación;
- integración con TD-FE-02.

3. Resultado de pruebas.

4. Riesgos residuales.

5. Próximas fases que consumirán este bootstrap.

---

# Instrucción Final

Implementar un proceso de bootstrap inteligente para el módulo de Radicación mediante un Startup Guard que centralice la inicialización, restaure automáticamente el estado documental activo utilizando el `RadicacionDocumentalContext`, resuelva la navegación inicial antes del render funcional y deje preparada la infraestructura para las siguientes fases sin introducir duplicidad de estado, consultas innecesarias ni breaking changes.
