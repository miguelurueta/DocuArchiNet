## 1. Definicion de constante y tipos

- [x] 1.1 Crear tipo/DTO para `CDeRelacionEstadoRetriccionDto` con estructura estable y tipada.
- [x] 1.2 Crear la constante `CDeRelacionEstadoRetriccionDto` en modulo reutilizable del dominio radicacion.
- [x] 1.3 Exportar la constante y su tipo para consumo desde `RadicacionForm.tsx`.

## 2. Integracion en RadicacionForm

- [x] 2.1 Integrar la lectura de `CDeRelacionEstadoRetriccionDto` en el flujo del campo destinatario.
- [x] 2.2 Aplicar reglas derivadas sin duplicar logica inline y sin romper comportamiento actual.
- [x] 2.3 Conservar atributos declarativos existentes del formulario (`required`, `disabled`, `title`, `tooltipAyuda`).
- [x] 2.4 Manejar estructuras incompletas o invalidas de forma controlada sin romper render.

## 3. Pruebas y evidencia

- [x] 3.1 Agregar/actualizar pruebas unitarias para validar definicion y uso de `CDeRelacionEstadoRetriccionDto`.
- [x] 3.2 Cubrir escenarios de comportamiento normal y estructura invalida en destinatario.
- [x] 3.3 Ejecutar pruebas del modulo de radicacion y registrar evidencia en este archivo.

### Evidencia de pruebas

- Comando ejecutado: `npm.cmd test -- src/modules/radicacion/hooks/useRelacionEstadoRestriccionDestinatario.spec.test.ts src/modules/radicacion/components/RadicacionForm.spec.test.tsx`
- Resultado: `2 files passed`, `22 tests passed`, `0 failed`.
