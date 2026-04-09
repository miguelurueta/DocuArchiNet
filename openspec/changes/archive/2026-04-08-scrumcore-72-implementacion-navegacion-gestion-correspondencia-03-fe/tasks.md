## 1. Consolidacion contractual

- [x] 1.1 Verificar que `proposal.md` modifica la capability real `gestion-correspondencia` y no deja una capability artificial derivada del nombre Jira.
- [x] 1.2 Verificar que la delta spec describe el shell actual como panel superpuesto con bandeja principal montada y retorno visible.
- [x] 1.3 Corregir lenguaje residual de `Drawer`, `dialog` o modal en los artefactos OpenSpec de este cambio cuando contradiga el contrato actual.

## 2. Regresion de routing

- [x] 2.1 Revisar `GestionCorrespondenciaRoute.spec.test.tsx` para confirmar cobertura de ruta base, subruta secundaria, deep link y retorno a la bandeja.
- [x] 2.2 Agregar o ajustar pruebas para afirmar que la region secundaria no usa `role="dialog"` y que el shell observable sigue presente.
- [x] 2.3 Agregar o ajustar pruebas para afirmar que la region principal permanece montada y visible bajo el panel secundario.
- [x] 2.4 Verificar que la accion `Volver a la bandeja` sigue siendo visible y devuelve a la ruta base.

## 3. Regresion de pagina principal y detalle contextual

- [x] 3.1 Revisar `GestionCorrespondenciaRoutePage.test.tsx` para asegurar cobertura de loading inicial, error y render resuelto de la bandeja principal.
- [x] 3.2 Confirmar que `GestionRespuesta` sigue tratandose como contenido contextual del shell y no como controlador de navegacion.
- [x] 3.3 Ajustar copy o asserts visibles solo si existe una inconsistencia real entre pruebas, README y comportamiento actual del modulo.

## 4. Verificacion final

- [x] 4.1 Ejecutar las pruebas focales del modulo de navegacion y route page.
- [x] 4.2 Validar `openspec validate scrumcore-72-implementacion-navegacion-gestion-correspondencia-03-fe --strict`.
- [x] 4.3 Validar que el cambio no introduce refactor funcional innecesario del shell ni de `GestionRespuesta`.
