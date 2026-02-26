## 1. Consumo de flujos relacionados por tramite

- [x] 1.1 Crear o ajustar servicio/hook reutilizable para consumir `/api/tramite/tramites/empsolicitaListaflujosRelacionadosTramite` con `idTipoDocEntrante`.
- [x] 1.2 Centralizar manejo de errores del consumo en la capa axios/query usada por el modulo.
- [x] 1.3 Normalizar `idValue` seleccionado en `Descripcion_Documento` y habilitar consulta solo cuando sea valido.

## 2. Integracion en RadicacionForm

- [x] 2.1 Capturar `onChange` del campo `data-ident="pl-radicacion-spe-Descripcion_Documento"` para actualizar el id de tramite seleccionado.
- [x] 2.2 Poblar opciones de `data-ident="pl-radicacion-spe-RE_flujo_trabajo"` con la respuesta (`idValue`, `Value`) de la API.
- [x] 2.3 Limpiar opciones y valor de `RE_flujo_trabajo` cuando `idValue` sea null, la API retorne vacio o falle.
- [x] 2.4 Conservar atributos declarativos existentes del campo flujo (`required`, `disabled`, `title`, `tooltipAyuda`).

## 3. Pruebas y evidencia

- [x] 3.1 Agregar o actualizar pruebas unitarias para validar consulta por `idTipoDocEntrante`, carga de opciones y limpieza en casos null/vacio/error.
- [x] 3.2 Verificar que los errores de API se manejan de forma controlada sin romper el flujo de formulario.
- [x] 3.3 Ejecutar pruebas del modulo de radicacion y registrar evidencia en este archivo.

### Evidencia de pruebas

- Comando ejecutado: `npm.cmd test -- src/modules/radicacion/hooks/useFlujosRelacionadosTramite.spec.test.ts src/modules/radicacion/components/RadicacionForm.spec.test.tsx`
- Resultado: `2 files passed`, `21 tests passed`, `0 failed`.
