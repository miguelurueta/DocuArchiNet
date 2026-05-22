## Context

En `DocumentosWorkbench`, el listado de documentos se renderiza con `AppTreeTable` y actualmente usa un esquema donde el desplazamiento vertical ocurre en un contenedor externo del modulo. Con esa estrategia, el header de columnas puede desplazarse y dejar de ser visible durante la navegacion por listas largas.

El flujo funcional exige dos comportamientos simultaneos:
- Visualizacion de un unico documento activo por evento de click en fila/celda.
- Seleccion multiple por checkbox para acciones masivas (por ejemplo, eliminacion).

Restricciones:
- No cambiar backend ni rutas.
- No introducir regresiones en `onActionTriggered` ni en visualizacion de documento.
- El alcance debe permanecer local a `AppTreeTable`/`DocumentosWorkbench`, sin impacto global en otros consumidores de `AppTable`.

## Goals / Non-Goals

**Goals:**
- Mantener el header visible de forma robusta durante el scroll en `DocumentosWorkbench`.
- Definir la estrategia de layout/scroll recomendada para `AppTreeTable` en este contexto.
- Preservar la separacion entre:
  - documento activo visualizado (single highlight)
  - seleccion por checkbox (multi para bulk actions).

**Non-Goals:**
- Redisenar globalmente `AppTable`.
- Cambiar contratos backend ni payloads de action/query existentes.
- Implementar nuevas acciones de negocio fuera del scope de header/contador/UX de lista.
- Implementar contador de documentos/registros (queda en otro SCRUM dedicado).

## Decisions

1. Estrategia robusta de header persistente
Se adopta para `DocumentosWorkbench` una estrategia de scroll interno del grid en lugar de depender de scroll externo con `autoHeight`.
Razon:
- El header fijo es comportamiento nativo y mas estable del grid cuando el viewport de scroll es interno.
- Reduce dependencia de soluciones CSS tipo sticky que pueden degradarse con cambios de layout.

2. Scope local del ajuste
La configuracion de layout/scroll para header persistente se limita al uso de `AppTreeTable` en `DocumentosWorkbench`.
Razon:
- Evita efectos secundarios en otros modulos que consumen `AppTable`.
- Mantiene controlado el impacto de UX.

3. Preservacion de semantica de seleccion
Se mantiene la separacion actual:
- Click para visualizar define el documento activo (y su resaltado).
- Checkbox define seleccion multiple para operaciones masivas.
Razon:
- Evita ambiguedad para el usuario y mantiene coherencia con el flujo de negocio.

4. Accesibilidad y legibilidad de lista documental
La propuesta mantiene foco en navegacion y lectura:
- Header visible durante scroll.
- Acciones de fila accesibles.
Razon:
- Mejora descubribilidad y control en listados extensos.

## Risks / Trade-offs

- [Riesgo] Cambio de estrategia de scroll podria requerir ajustes finos de altura del panel en breakpoints.
Mitigacion: definir altura del contenedor del listado y validar desktop/tablet.

- [Trade-off] Solucion robusta de scroll interno implica mayor ajuste de layout que un sticky CSS rapido.
Mitigacion: mantener el cambio encapsulado en `DocumentosWorkbench`.

## Migration Plan

1. Ajustar configuracion de presentacion de `AppTreeTable` en `DocumentosWorkbench` para estrategia de scroll interno robusta.
2. Configurar el contenedor del panel/lista para soportar altura fija y evitar doble scroll conflictivo.
3. Validar manual y por pruebas la persistencia del header y no-regresion de seleccion/visualizacion/acciones.

## Open Questions

- Se requiere alinear alguna restriccion visual adicional de header persistente con design system?
