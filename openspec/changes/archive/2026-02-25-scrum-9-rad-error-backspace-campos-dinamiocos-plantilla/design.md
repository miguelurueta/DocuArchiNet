## Context

En radicacion, los campos de plantilla se renderizan de forma dinamica. Al limpiar contenido con la tecla `Backspace` en algunos controles, se reporta un error en consola. El objetivo del cambio es corregir ese comportamiento sin afectar el render declarativo ni los flujos actuales de captura.

## Goals / Non-Goals

**Goals:**
- Eliminar el error de consola asociado a `Backspace` en campos dinamicos.
- Mantener comportamiento de edicion y limpieza de los campos.
- Preservar el flujo actual del formulario y su integracion con hooks existentes.
- Cubrir el escenario con pruebas de comportamiento.

**Non-Goals:**
- Reescribir la arquitectura de renderizado dinamico completa.
- Cambiar API backend o contratos de datos.
- Modificar reglas funcionales fuera del problema de `Backspace`.

## Decisions

### Decision: robustecer manejo de `onChange` en campos dinamicos
Se ajustara la logica de entrada para tolerar valores vacios y eventos de borrado sin asumir estructuras no definidas.

Alternativas:
- Ignorar errores de consola: descartado por riesgo de fallos encadenados.
- Reemplazar todos los controles por implementacion nueva: descartado por costo y riesgo.

### Decision: conservar componentes y hooks actuales
Se aplicaran cambios puntuales sobre los componentes que renderizan campos dinamicos, manteniendo interfaces y patrones vigentes.

Alternativas:
- Extraer un nuevo motor de formularios: descartado por alcance excesivo.

### Decision: validar con pruebas de no regresion
Se agregaran/actualizaran pruebas para confirmar que `Backspace` no rompe el render ni el estado del control.

Alternativas:
- Validacion manual unicamente: descartado por baja repetibilidad.

## Risks / Trade-offs

- [Cambio puntual en eventos podria afectar otro tipo de campo] -> Mitigacion: pruebas por tipo de control y escenarios clave.
- [Error depende de datos reales de plantilla] -> Mitigacion: cubrir tests con mocks representativos y vacios.
- [Regresion de UX al borrar texto] -> Mitigacion: verificar que el campo mantenga estado editable y sin bloqueos.

## Migration Plan

1. Reproducir el error de `Backspace` en test de componente.
2. Ajustar el manejo de eventos/valor en renderer dinamico.
3. Validar ausencia de error y funcionamiento de borrado.
4. Ejecutar pruebas de radicacion y registrar evidencia en `tasks.md`.
5. Si aparece regresion, revertir cambio puntual y aislar caso por tipo de campo.

## Open Questions

- Confirmar el tipo exacto de control donde ocurre con mayor frecuencia (`input`, `autocomplete`, `select`).
- Confirmar si el error solo ocurre con ciertos metadatos de `camposPlantilla`.
