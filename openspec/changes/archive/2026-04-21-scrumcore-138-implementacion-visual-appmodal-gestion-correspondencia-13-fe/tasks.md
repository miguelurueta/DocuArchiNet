## 1. Componente modal base (UI desacoplada)

- [x] 1.1 Crear carpeta `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/`
- [x] 1.2 Implementar `ReasignarRespuestaModal.tsx` usando `AppModal` como contenedor principal
- [x] 1.3 Definir props tipadas del componente (open, onClose, radicado, nota, users, callbacks)
- [x] 1.4 Integrar `AppInputTags` para responsables con render chip y callbacks de agregar/remover
- [x] 1.5 Integrar acciones `Cancelar` y `Enviar` con `AppButton` sin logica de negocio

## 2. Estilos y responsive

- [x] 2.1 Crear `ReasignarRespuestaModal.module.css` con layout vertical, header, nota y acciones
- [x] 2.2 Ajustar comportamiento desktop con modal centrado y ancho medio estable
- [x] 2.3 Ajustar comportamiento tablet con padding reducido y controles al 100%
- [x] 2.4 Ajustar comportamiento mobile para evitar overflow horizontal y mantener legibilidad
- [x] 2.5 Garantizar `max-height` fija con scroll interno cuando el contenido exceda el viewport

## 3. Integracion desde Gestion Correspondencia

- [x] 3.1 Agregar estado de apertura/cierre y contexto del radicado en el contenedor del modulo
- [x] 3.2 Conectar handler de opcion `Reasignar Tramite` del dropdown para abrir el modal
- [x] 3.3 Mantener intacta la implementacion base de tabla (sin cambios de columnas/render/paginacion)
- [x] 3.4 Conectar callbacks de cierre y envio a handlers de UI del contenedor (sin API)

## 4. Accesibilidad basica

- [x] 4.1 Garantizar focus inicial al abrir el modal en control interactivo relevante
- [x] 4.2 Validar cierre por tecla `Escape`
- [x] 4.3 Asociar encabezado con etiquetado accesible del dialogo (`aria-labelledby`)
- [x] 4.4 Marcar iconografia decorativa con `aria-hidden` y labels legibles en controles

## 5. Pruebas y verificacion

- [x] 5.1 Crear `ReasignarRespuestaModal.test.tsx` con pruebas de apertura/cierre
- [x] 5.2 Probar render de `AppInputTags` y callbacks de eliminacion/agregado de tags
- [x] 5.3 Probar visibilidad y accion de botones `Cancelar` y `Enviar`
- [x] 5.4 Probar seccion de nota y contenido de radicado en header
- [x] 5.5 Ejecutar pruebas focalizadas del modulo y registrar evidencia en este archivo

## 6. Evidencia

- [x] 6.1 Registrar rutas de archivos creados/modificados
- [x] 6.2 Registrar comando de pruebas ejecutado y resultado final
- [ ] 6.3 Confirmar cumplimiento de responsive en desktop/tablet/mobile

### Evidencia registrada

- Archivos creados:
  - `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/ReasignarRespuestaModal.tsx`
  - `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/ReasignarRespuestaModal.module.css`
  - `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/ReasignarRespuestaModal.test.tsx`
  - `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/index.ts`
- Archivos modificados:
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
  - `src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx`
- Comandos ejecutados:
  - `npm.cmd run -s test -- --run src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/ReasignarRespuestaModal.test.tsx` -> `4 passed`
  - `npm.cmd run -s test -- --run src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx` -> `10 passed`
  - `npm.cmd run -s build` -> `build successful`
