# PROMPT ARQUITECTÓNICO - Radicación Simplificada

# FE-07 - Caso de Uso "Enviar Trámite a Pendiente"

---

# Contexto Arquitectónico

Esta fase debe respetar las decisiones previamente adoptadas:

- TD-FE-01 → Single Source of Truth para carga de datos.
- TD-FE-02 → RadicacionDocumentalContext único.
- FE-05 → Casos de uso para mutaciones.
- FE-06 → Startup Guard responsable del bootstrap.
- TD-FE-04 → Navegación contextual centralizada.

No crear nuevos estados documentales.

No crear stores paralelos.

---

# Objetivo

Implementar el caso de uso que permite enviar un trámite documental activo al estado Pendiente.

La transición funcional es:

```text
Estado 0

↓

Estado 1
```

Este cambio debe realizarse únicamente después de una confirmación explícita del usuario y una respuesta exitosa del backend.

---

# Objetivo Arquitectónico

La interfaz únicamente solicita la operación.

Toda la lógica transaccional pertenece al caso de uso:

```text
useEnviarRadicadoPendiente()
```

---

# Problema Actual

El botón existe actualmente en:

```text
src/modules/radicacion/components/RadicacionForm.tsx
```

Texto actual:

```text
Enviar a Pendientes
```

Pero no debe quedar como acción global siempre disponible.

La acción debe depender exclusivamente del estado documental activo del `RadicacionDocumentalContext`.

---

# Flujo Arquitectónico

```text
Usuario

↓

Botón "Enviar a Pendiente"

↓

Modal de Confirmación

↓

useEnviarRadicadoPendiente()

↓

Service

↓

Backend

↓

RadicacionDocumentalContext

↓

Router / rutas centralizadas

↓

Resumen
```

---

# Regla Funcional

La acción solamente puede ejecutarse cuando exista un trámite documental activo.

```ts
const puedeEnviarAPendiente =
  requiereGestionDocumental === true &&
  tieneTramiteDocumentalActivoEstado0 === true &&
  estadoActual === 0 &&
  idEstadoRadicado > 0;
```

Si la condición no se cumple:

- ocultar la acción (recomendado);
- o deshabilitarla cuando el diseño lo requiera.

Nunca ejecutar la mutación.

---

# Contrato Backend

API:

```text
POST /api/radicacion/pendientes/{idEstadoRadicado}/enviar-pendiente
```

Request:

```ts
type EnviarRadicadoPendienteRequestDto = {
  motivo?: string;
};
```

Response esperado:

```ts
type EnviarRadicadoPendienteResponseDto = {
  idEstadoRadicado: number;
  consecutivoRadicado?: string;
  estadoAnterior: 0;
  estadoActual: 1;
  tieneTramiteDocumentalActivoEstado0: false;
  destinoPostRegistro: "resumen";
  mensaje: string;
};
```

No modificar contratos backend desde esta fase.

---

# Componentes

Crear o extender:

```text
services/
    radicacionPendientes.service.ts

hooks/
    useEnviarRadicadoPendiente.ts

components/
    EnviarPendienteConfirmModal.tsx

context/
    RadicacionDocumentalContext.tsx
```

Si existe un modal de confirmación compartido debe reutilizarse.

No crear lógica de mutation directamente dentro de `RadicacionForm.tsx`.

---

# Responsabilidades

## UI

Responsable únicamente de:

- mostrar acción;
- abrir confirmación;
- representar estados.

No contiene reglas de negocio.

---

## Modal de Confirmación

Responsable únicamente de solicitar la confirmación del usuario.

No ejecuta llamadas HTTP.

---

## useEnviarRadicadoPendiente

Caso de uso oficial para:

- ejecutar la mutación;
- validar respuesta;
- limpiar/desactivar Context mediante la operación oficial del Context;
- refrescar contador;
- refrescar listado;
- resolver navegación.

Toda la lógica del proceso pertenece aquí.

---

## Service

Responsable únicamente del acceso HTTP.

No contiene lógica de negocio.

No se acopla a UI.

---

## Context

Responsable únicamente del estado documental.

Nunca ejecuta mutaciones HTTP.

Debe exponer o reutilizar la operación oficial para dejar el módulo sin trámite documental activo, por ejemplo:

```text
clearContextoDocumental()
```

o una operación equivalente definida por TD-FE-02.

---

# Flujo Transaccional

```text
Usuario

↓

Confirma operación

↓

POST enviar-pendiente

↓

¿estadoActual == 1?

SI

↓

Actualizar Context

↓

Desactivar Documentos

↓

Refrescar contador

↓

Refrescar listado

↓

Navegar a Resumen

NO

↓

Mantener Context intacto

↓

Mostrar error
```

---

# Integridad del Context

Nunca limpiar el Context antes de una respuesta exitosa.

El Context debe modificarse únicamente cuando backend confirme:

```text
estadoActual = 1
```

Si la respuesta no confirma `estadoActual == 1`, mantener el Context intacto.

Ante cualquier error:

- conservar completamente el estado anterior;
- mantener Documentos activo;
- permitir reintento.

No dejar estados parcialmente actualizados.

No manipular campos aislados del Context desde componentes.

---

# Integración

Tras una operación exitosa:

Actualizar:

- Context documental.
- Contador de pendientes.
- Lista de pendientes (si está abierta).

La información utilizada debe ser la misma consumida por FE-05 y FE-06.

No crear modelos paralelos.

Si el modal/listado de pendientes está abierto:

- refrescar listado;
- el radicado enviado debe aparecer como `estado = 1` después del refresco.

---

# Navegación

Después de una operación exitosa, navegar a Resumen o a la ruta base de Radicación usando las rutas centralizadas o helpers definidos por TD-FE-04.

No hardcodear rutas en componentes.

Ruta conceptual:

```text
/dashboard/radicacion
```

Si las rutas definitivas aún no existen, usar el adapter/helper disponible y dejar el punto de integración tipado.

---

# Estados UI

Representar:

- disponible;
- confirmando;
- enviando;
- éxito;
- error;
- bloqueo;
- reintento.

Mientras exista una mutación activa:

- bloquear reentradas;
- bloquear doble clic;
- mantener el estado visible;
- conservar Context hasta que backend confirme éxito.

---

# Restricciones

No implementar:

- listado de pendientes;
- AppTable;
- backend;
- digitalización;
- visor;
- upload.

No mover lógica de negocio hacia componentes.

No crear stores paralelos.

No limpiar contexto desde UI sin pasar por el caso de uso.

---

# Principios Arquitectónicos

Aplicar:

- Single Source of Truth.
- Smart Hooks / Dumb Components.
- Transactional Use Cases.
- Clean Architecture.
- Backward Compatibility.
- Fail Safe Updates.

---

# Testing

## Unitarios

Validar:

- hook;
- service;
- selector de visibilidad;
- response inesperada sin `estadoActual = 1`.

---

## Integración

Validar:

- Form → Modal;
- Modal → Hook;
- Hook → Context;
- Hook → Router;
- refresco de contador/listado cuando aplique.

---

## Regresión

Validar:

- navegación;
- contador;
- listado;
- build;
- lint;
- TypeScript.

---

# Criterios de Aceptación

- La acción sólo aparece cuando existe estado documental activo.
- La confirmación precede a la mutación.
- Toda la lógica vive en `useEnviarRadicadoPendiente`.
- El Context únicamente se limpia/desactiva tras una respuesta exitosa.
- Si la respuesta no confirma `estadoActual = 1`, el Context queda intacto.
- Documentos queda deshabilitado después de la transición.
- El contador y la lista se actualizan.
- No existen estados inconsistentes.
- No existen stores paralelos.
- No se introducen regresiones.

---

# Entregables

1. Lista de archivos modificados.

2. Resumen técnico:

- caso de uso;
- flujo transaccional;
- actualización del Context;
- integración con FE-05 y FE-06;
- integración con rutas centralizadas de TD-FE-04.

3. Resultado de pruebas.

4. Riesgos residuales.

5. Próximas fases habilitadas.

---

# Instrucción Final

Implementar el caso de uso **Enviar Trámite a Pendiente** encapsulando toda la lógica transaccional en `useEnviarRadicadoPendiente`, garantizando que la transición del estado documental se realice únicamente tras una confirmación del usuario y una respuesta exitosa del backend, manteniendo el `RadicacionDocumentalContext` como única fuente de verdad, preservando la consistencia del módulo y evitando estados parciales, duplicados o regresiones.
