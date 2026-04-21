## 1. Cobertura unitaria del modal

- [x] 1.1 Verificar render de `ReasignarRespuestaModal` con `open=true`
- [x] 1.2 Verificar callbacks de acciones (`Cancelar` -> `onClose`, `Enviar` -> `onSubmit`)
- [x] 1.3 Verificar render de nota y radicado en el modal
- [x] 1.4 Verificar interaccion de `AppInputTags` para eliminacion de tags (`onRemoveUser`)

## 2. Cobertura de integracion en Gestion Correspondencia

- [x] 2.1 Verificar apertura del modal al disparar accion `reasignar_tramite`
- [x] 2.2 Agregar validacion explicita de cierre por `Cancelar` tras la apertura
- [x] 2.3 Confirmar que la prueba sigue sin modificar la implementacion base de AppTable

## 3. Ejecucion y evidencia

- [x] 3.1 Ejecutar prueba focalizada del modal de reasignacion
- [x] 3.2 Ejecutar prueba focalizada de la pagina `GestionCorrespondencia`
- [x] 3.3 Registrar resultados de ejecucion en este archivo

## Evidencia

- Archivos de pruebas:
  - `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/ReasignarRespuestaModal.test.tsx`
  - `src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx`
- Comandos:
  - `npm.cmd run -s test -- --run src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/ReasignarRespuestaModal.test.tsx`
  - `npm.cmd run -s test -- --run src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx`
- Resultado esperado:
  - Suites en verde para flujo de reasignacion (modal + integracion)

