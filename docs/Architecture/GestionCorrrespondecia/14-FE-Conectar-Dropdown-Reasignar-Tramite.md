# PROMPT ARQUITECTONICO Ticket FE 14

# Conectar opcion “Reasignar Trámite” del dropdown para abrir ReasignarRespuestaModal (sin tocar tabla)

## Rol esperado

Arquitecto frontend senior (React, composition, desacoplo UI).

## Objetivo

Al hacer click en la opcion **“Reasignar Trámite”** dentro del dropdown de acciones de la tabla de **Gestión Correspondencia**, abrir el modal `ReasignarRespuestaModal` con `radicado` (y `nota` preview si aplica) derivados del contexto de la fila, **sin modificar la tabla**.

## Contexto existente

- Documento tecnico:
  - `docs/Architecture/GestionCorrrespondecia/AppModal-Reasignar-Respuesta.md`
- Modulo:
  - `src/modules/gestionCorrespondencia/`

## Restricciones obligatorias

- no modificar el componente base de la tabla
- no cambiar columnas/render/paging/query de la tabla
- no agregar logica de negocio (sin API)
- no usar `any`

## Criterios de aceptacion

- Click “Reasignar Trámite” abre el modal.
- El modal recibe `radicado` correctamente (o fallback definido).
- Cerrar por “Cancelar” y “X”.
- No se cambia la tabla (solo integración/handlers).

