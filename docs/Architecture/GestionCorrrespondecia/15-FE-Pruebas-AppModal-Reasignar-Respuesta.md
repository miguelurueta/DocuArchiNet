# PROMPT ARQUITECTONICO Ticket FE 15

# Pruebas del flujo “Reasignar Trámite” + ReasignarRespuestaModal (UI)

## Rol esperado

Arquitecto frontend senior (Vitest + Testing Library).

## Objetivo

Cubrir con pruebas unitarias/UI el modal y su integración de apertura/cierre desde “Reasignar Trámite”, validando comportamiento observable (sin backend).

## Contexto existente

- `docs/Architecture/GestionCorrrespondecia/AppModal-Reasignar-Respuesta.md`
- `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/`

## Pruebas unitarias obligatorias

- modal abre/cierra
- AppInputTags render + eliminación de tags (callbacks)
- nota render
- botones visibles y funcionales (callbacks)
- integración: click en “Reasignar Trámite” abre modal; cancelar cierra

## Criterios de aceptacion

- suite en verde
- cubre flujo end-to-end UI sin tocar tabla

