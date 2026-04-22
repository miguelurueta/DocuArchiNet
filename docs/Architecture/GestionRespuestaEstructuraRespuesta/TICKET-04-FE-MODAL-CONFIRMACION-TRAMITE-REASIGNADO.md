# Ticket Jira - 04 FE
# Modal de confirmacion "Tramite Reasignado" con AppModal

## Summary

Gestion de Correspondencia: mostrar modal de confirmacion "Tramite Reasignado" con `AppModal` al enviar reasignacion valida

## Tipo sugerido

Story

## Componente / Modulo

- Modulo: `gestionCorrespondencia`
- Componente origen:
  `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/ReasignarRespuestaModal.tsx`
- Nuevo componente:
  `src/modules/gestionCorrespondencia/components/modalTramiteReasignado/TramiteReasignadoModal.tsx`

## Contexto

Despues de validar el formulario de reasignacion y pulsar `Enviar`, la UI debe presentar un modal de confirmacion desacoplado, centrado y consistente con el Design System, usando componentes shared (`AppModal`, `AppButton`) y sin introducir logica de negocio.

Referencia de arquitectura:

- `docs/Architecture/GestionRespuestaEstructuraRespuesta/04-FE-Modal-Confirmacion-Tramite-Reasignado-AppModal.md`

## Description (para Jira)

Implementar un modal de confirmacion visual para el flujo de reasignacion de tramite en Gestion de Correspondencia.

El modal debe abrirse al finalizar exitosamente la validacion/envio del formulario en `ReasignarRespuestaModal` y debe mostrar:

- Titulo centrado: `Tramite Reasignado`
- Icono de exito junto al titulo
- Informacion:
  - `Usuario Asignado: <valor>`
  - `Radicado: <valor>`
- Boton primario `Aceptar` centrado (full width en mobile)

Requisitos tecnicos:

- usar `AppModal` y `AppButton`
- componente reusable en:
  `src/modules/gestionCorrespondencia/components/modalTramiteReasignado/`
- CSS Modules obligatorio
- sin `any`
- sin modificar `AppModal`
- sin estilos globales
- sin logica de negocio

## Criterios de aceptacion

1. Cuando `open=true`, el modal `TramiteReasignadoModal` se renderiza correctamente.
2. El modal muestra texto visible de:
   - `Usuario Asignado`
   - `Radicado`
3. El boton `Aceptar` ejecuta `onClose`.
4. El modal se abre al enviar reasignacion valida desde `ReasignarRespuestaModal`.
5. El layout se mantiene estable en desktop, tablet y mobile.
6. En mobile, el boton principal se renderiza en ancho completo.
7. El foco inicial cae en el boton `Aceptar` al abrir el modal.
8. Navegacion por teclado y cierre accesible se mantienen operativos.

## Tareas tecnicas sugeridas

1. Crear componente `TramiteReasignadoModal.tsx` con props tipadas:
   - `open`
   - `usuarioAsignado`
   - `radicado`
   - `onClose`
2. Crear `TramiteReasignadoModal.module.css` con reglas responsive.
3. Integrar estado de apertura/cierre en `ReasignarRespuestaModal.tsx`.
4. Pasar payload de confirmacion (`usuarioAsignado`, `radicado`) desde el submit valido.
5. Implementar foco inicial en `Aceptar`.
6. Crear pruebas unitarias del modal.
7. Ajustar pruebas de integracion de `ReasignarRespuestaModal` para validar apertura del modal de confirmacion.

## Fuera de alcance

- Integracion backend adicional
- Cambios de endpoint o contrato API
- Refactor de `AppModal`
- Cambios globales de tema o estilos compartidos

## Definicion de terminado (DoD)

- Codigo implementado y tipado en TypeScript estricto
- Pruebas unitarias/integracion pasando para el flujo de confirmacion
- Sin regresiones en flujo actual de reasignacion
- Documentacion alineada al archivo de arquitectura referenciado
