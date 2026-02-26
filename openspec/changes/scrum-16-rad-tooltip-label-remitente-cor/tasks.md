## 1. Resolucion de metadata de REMITENTE_COR

- [x] 1.1 Localizar en `RadicacionForm.tsx` el registro `REMITENTE_COR` desde `camposPlantilla` por `name_campo` normalizado.
- [x] 1.2 Conectar el control `data-ident="pl-radicacion-spe-REMITENTE_COR"` para que use la metadata del campo resuelto.

## 2. Atributos declarativos y UX del label

- [x] 2.1 Aplicar `required` y `disabled` del registro `REMITENTE_COR` en el selector tipo token.
- [x] 2.2 Mapear `title_control` al atributo `title` del label de remitente.
- [x] 2.3 Renderizar `tooltipAyuda` junto al label con `span.tooltip-ayuda` e icono de informacion.

## 3. Pruebas y evidencia

- [x] 3.1 Agregar o actualizar pruebas de `RadicacionForm` para validar resolucion de `REMITENTE_COR` y atributos (`required`, `disabled`, `title`, `tooltipAyuda`).
- [x] 3.2 Ejecutar pruebas del modulo de radicacion y registrar evidencia de ejecucion en este archivo.

### Evidencia de pruebas

- Comando ejecutado: `npm.cmd test -- src/modules/radicacion/components/RadicacionForm.spec.test.tsx`
- Resultado: `1 passed`, `16 passed`, `0 failed`.
