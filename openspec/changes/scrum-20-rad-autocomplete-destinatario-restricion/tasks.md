## 1. Contrato y capa de datos para autocompletado restringido

- [x] 1.1 Definir tipos TypeScript para request/response de `solicitaAutoCompleteDestinatarioRestriccion` con payload `{ idScript, nombreCampo, valueCampo }`.
- [x] 1.2 Implementar helper para construir payload desde metadata de `camposPlantilla` usando `id_escript` y el texto de busqueda.
- [x] 1.3 Implementar hook reutilizable para consulta de autocomplete con normalizacion de opciones y manejo centralizado de errores.

## 2. Integracion en RadicacionForm para campo token Destinatario_Cor

- [x] 2.1 Conectar el hook al campo `Destinatario_Cor` preservando estructura token `ant-select` en modo multiple.
- [x] 2.2 Mantener atributos declarativos del campo (`required`, `disabled`, `title`, `tooltipAyuda`) tomados de `camposPlantilla`.
- [x] 2.3 Implementar limpieza de sugerencias cuando `valueCampo` sea vacio y conservar estabilidad del estado de tokens seleccionados.

## 3. Validaciones y pruebas

- [x] 3.1 Agregar pruebas unitarias del hook para escenarios de exito y error de API.
- [x] 3.2 Agregar/actualizar pruebas de `RadicacionForm` para carga de items, seleccion de token y limpieza al vaciar entrada.
- [x] 3.3 Verificar cobertura de escenarios del spec y etiquetar pruebas con `[SPEC:<ID>]` donde aplique.

## 4. Verificacion final y cierre tecnico

- [x] 4.1 Ejecutar suite de pruebas del modulo Radicacion y corregir regresiones.
- [ ] 4.2 Validar manualmente el flujo de autocompletado de `Destinatario_Cor` sin romper funcionalidades existentes.
- [ ] 4.3 Dejar evidencia de pruebas y preparar cambio para `opsxj:archive`.
