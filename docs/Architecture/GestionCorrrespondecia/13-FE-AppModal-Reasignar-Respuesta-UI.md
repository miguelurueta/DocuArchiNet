# PROMPT ARQUITECTONICO Ticket FE 13

# Implementar AppModal “Reasignar Respuesta” (UI + Responsive + A11y)

## Rol esperado

Arquitecto frontend senior (React, TypeScript estricto, Design System, UI responsive, accesibilidad).

## Objetivo

Implementar el componente `ReasignarRespuestaModal` usando **AppModal** como contenedor principal e integrando **AppInputTags** y **AppButton**, replicando la UI definida en la arquitectura y asegurando comportamiento responsive en Desktop/Tablet/Mobile.

Este modal se abrirá cuando el usuario haga click en la opción **“Reasignar Trámite”** del menú desplegable de opciones (dropdown) dentro de la tabla del módulo **Gestión Correspondencia** (la conexión del dropdown se cubre en el Ticket FE 14).

## Contexto existente

- Documento tecnico (referencia obligatoria):
  - `docs/Architecture/GestionCorrrespondecia/AppModal-Reasignar-Respuesta.md`
- Design System:
  - `src/app/Components/UI/AppModal/`
  - `src/app/Components/UI/AppInputTags/`
  - `src/app/Components/UI/AppButton/`
- Modulo:
  - `src/modules/gestionCorrespondencia/`

## Ubicacion esperada

```txt
src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/
  ReasignarRespuestaModal.tsx
  ReasignarRespuestaModal.module.css
  ReasignarRespuestaModal.test.tsx
  index.ts (opcional)
```

## Restricciones obligatorias

- no usar `any`
- no logica de negocio (sin API, sin mutaciones, sin side-effects del dominio)
- no estilos globales (solo CSS Modules)
- no modificar `AppModal`
- no acoplar el modal a la tabla (sin dependencias directas de columnas/rows)

## Pruebas unitarias obligatorias

- renderiza cuando `open=true`
- “Cancelar” llama `onClose`
- “Enviar” llama `onSubmit`
- render de nota
- `AppInputTags` renderiza tags y permite eliminar (dispara callbacks)

## Criterios de aceptacion

- UI coincide con la arquitectura (header, input tags, nota card, acciones)
- responsive sin rupturas y con scroll interno cuando aplique
- accesible (focus inicial, teclado, labels/aria)
- pruebas en verde

