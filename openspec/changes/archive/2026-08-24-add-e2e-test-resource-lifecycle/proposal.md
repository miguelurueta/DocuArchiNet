## Why

Las E2E reales hoy dependen de tareas seleccionadas manualmente y descubren algunos prerrequisitos de negocio después de abrir una sesión o iniciar una transición. Esto eleva el costo de cada corrida y hace que la infraestructura reutilizable quede limitada a DOC-32 en lugar de servir a cualquier tipo de prueba integrada.

## What Changes

- Incorporar un ciclo de vida común para recursos E2E: identificación, preflight de solo lectura, reserva exclusiva, evidencia y liberación o restauración verificable.
- Definir contratos registrados por tipo de prueba y adaptadores acotados que describan los recursos y prerrequisitos de negocio sin habilitar SQL, comandos ni secretos arbitrarios desde perfiles.
- Hacer que los escenarios mutantes fallen antes de abrir sesión cuando el recurso no está disponible, está reservado o no cumple los prerrequisitos declarados.
- Adaptar la E2E de envío Workflow para consumir un recurso reservado en lugar de depender de la selección manual de una tarea descartable.
- Conservar la ejecución real como acción expresamente autorizada; las pruebas locales y el preflight no deberán requerir navegador, cuentas ni base de datos reales.

## Capabilities

### New Capabilities

- `e2e-test-resource-lifecycle`: Contrato transversal para reservar, validar, usar y liberar recursos descartables de cualquier escenario E2E.

### Modified Capabilities

- `e2e-enviar-usuario-workflow`: La ejecución autorizada de envío a usuario usa recursos Workflow reservados y prerrequisitos verificados antes de mutar.

## Impact

- Adaptadores, registro y orquestación en `tools/e2e`, incluidos sus perfiles no sensibles y evidencia saneada.
- Pruebas unitarias de ciclo de vida y pruebas de política sin servicios reales.
- Documentación operativa para preparar, reservar y liberar recursos por cada DOC o tipo de prueba.
- La configuración de ambientes de prueba deberá exponer únicamente adaptadores y recursos registrados; no se persistirán secretos ni se ejecutarán comandos arbitrarios.
