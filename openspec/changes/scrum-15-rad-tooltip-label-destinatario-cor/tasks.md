## 1. Resolucion de metadata de Destinatario_Cor

- [x] 1.1 Localizar en `RadicacionForm.tsx` el campo `Destinatario_Cor` desde `camposPlantilla` por `name_campo` (normalizado).
- [x] 1.2 Conectar el selector `data-ident="pl-radicacion-spe-Destinatario_Cor"` para que use metadata del campo resuelto.

## 2. Atributos declarativos y UX de label

- [x] 2.1 Aplicar `required` y `disabled` del registro `Destinatario_Cor` en el control destinatario.
- [x] 2.2 Mapear `title_control` al atributo `title` del label del destinatario.
- [x] 2.3 Renderizar `tooltipAyuda` junto al label con `span.tooltip-ayuda` e icono de información.

## 3. Pruebas y evidencia

- [x] 3.1 Agregar/actualizar pruebas de `RadicacionForm` para validar resolución de `Destinatario_Cor` y atributos (`required`, `disabled`, `title`, `tooltipAyuda`).
- [x] 3.2 Ejecutar pruebas del modulo radicación y registrar resultados en este archivo.

### Evidencia de pruebas

- Comando ejecutado: `npm.cmd test -- src/modules/radicacion/components/RadicacionForm.spec.test.tsx`
- Resultado: `1 passed`, `15 passed`, `0 failed`.
