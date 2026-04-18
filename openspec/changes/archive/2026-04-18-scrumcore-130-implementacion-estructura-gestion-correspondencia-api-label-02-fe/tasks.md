## 1. Contratos y acceso a datos

- [x] 1.1 Crear los tipos del response backend y del modelo UI `GestionRespuestaEstructuraRespuesta`
- [x] 1.2 Crear el service `solicitaEstructuraRespuestaIdTarea.service.ts` usando `clienteApi`
- [x] 1.3 Implementar la normalización del payload hacia `estrucTuraRespuesta`

## 2. Orquestación del estado

- [x] 2.1 Crear `useEstructuraRespuestaIdTarea.ts` para consumir la API por `idTareaWf`
- [x] 2.2 Exponer desde el hook: `estrucTuraRespuesta`, `loading`, `error` e `isEmpty`
- [x] 2.3 Confirmar y documentar la fuente real de `idTareaWf` antes de integrar el consumo en `GestionRespuestaMainTabContent`

## 3. Integración visual

- [x] 3.1 Integrar el hook en `GestionRespuestaMainTabContent.tsx`
- [x] 3.2 Reemplazar el `metadata` fijo de `GestionRespuestaInfoHeader` por `Radicado`, `Remitente` y `Trámite`
- [x] 3.3 Aplicar fallback seguro cuando no existan datos o haya error
- [x] 3.4 Garantizar que la lógica de UI use `success`, `data` y `data.length`, sin depender de `message`

## 4. Validación

- [x] 4.1 Agregar pruebas del service para el endpoint `solicita-estructura-respuesta-id-tarea`
- [x] 4.2 Agregar pruebas del hook `useEstructuraRespuestaIdTarea`
- [x] 4.3 Actualizar `GestionRespuestaMainTabContent.test.tsx` para validar el header dinámico
- [x] 4.4 Ejecutar la suite relevante y registrar evidencia en este archivo

## Evidencia

- 2026-04-16: `node .\node_modules\vitest\vitest.mjs --run src\modules\gestionCorrespondencia\tests\solicitaEstructuraRespuestaIdTarea.service.test.ts src\modules\gestionCorrespondencia\tests\useEstructuraRespuestaIdTarea.test.tsx src\modules\gestionCorrespondencia\tests\GestionRespuestaMainTabContent.test.tsx src\modules\gestionCorrespondencia\routes\GestionCorrespondenciaRoute.spec.test.tsx`
- Resultado: `15 tests passed`
- 2026-04-18: `node .\node_modules\vitest\vitest.mjs --run src\modules\gestionCorrespondencia\tests\solicitaEstructuraRespuestaIdTarea.service.test.ts src\modules\gestionCorrespondencia\tests\useEstructuraRespuestaIdTarea.test.tsx src\modules\gestionCorrespondencia\tests\GestionRespuestaMainTabContent.test.tsx src\modules\gestionCorrespondencia\routes\GestionCorrespondenciaRoute.spec.test.tsx`
- Resultado: `15 tests passed`
