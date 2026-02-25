## 1. Integracion de metadatos en campo de fecha

- [x] 1.1 Ubicar en `RadicacionForm` el campo de plantilla `name_campo = "FECHALIMITERESPUESTA"` y derivar `label`, `title_control` y `tooltipAyuda` con fallback seguro.
- [x] 1.2 Construir el label del campo "Fecha Límite Respuesta" con patron de tooltip existente (`tooltip-ayuda`, `aria-describedby`, `data-tooltip-id`) cuando `tooltipAyuda` tenga valor.
- [x] 1.3 Mantener el `DatePicker` actual y asegurar que conserva atributos declarativos existentes, incluyendo `aria-describedby` asociado al tooltip cuando aplique.

## 2. Validacion por pruebas

- [x] 2.1 Agregar/actualizar tests en `RadicacionForm.spec.test.tsx` para cubrir `[SPEC]` del tooltip/title en `FECHALIMITERESPUESTA`.
- [x] 2.2 Verificar por tests que el campo de fecha mantiene su comportamiento actual (control de fecha y atributos declarativos) tras integrar metadatos.

## 3. Evidencia y cierre del cambio

- [x] 3.1 Ejecutar pruebas relevantes de radicación y confirmar que pasan.
- [x] 3.2 Registrar evidencia de ejecución de tests dentro del cambio OpenSpec.

## Evidencia de pruebas

Fecha: 2026-02-24

Comando ejecutado:
> npm.cmd test -- src/modules/radicacion/components/RadicacionForm.spec.test.tsx --run

Resultado:
- Test Files: 1 passed
- Tests: 7 passed
- Incluye cobertura de `[SPEC:RAD-006]` y `[SPEC:RAD-007]` para `FECHALIMITERESPUESTA`
