# Prompt 01 — Núcleo de importación y registro de adaptadores

Actúa como implementador senior de ASP.NET WebForms, JavaScript y accesibilidad. Lee completa la exploración y el preview indicados en `README.md`, inspecciona el código vigente y crea o continúa un cambio OpenSpec antes de modificar código productivo.

## Objetivo

Construir el núcleo frontend genérico de **Importar documentos desde servicio**, desacoplado de SII, con resolución del proveedor configurado y registro explícito de adaptadores.

## Implementa

- Un orquestador de UI con estados cerrado, resolviendo proveedor, consultando, vacío, resultados, preparando, ejecutando, reconciliando, completado y error.
- Un contrato de adaptador basado en capacidades: selección múltiple, vista previa, descarga, tipología, requisitos adicionales y acciones permitidas.
- Un registro que resuelva el adaptador por identidad canónica del proveedor; no uses una cadena de `if` dispersos.
- Apertura desde `ctw-document-action-service`, conservando `btnloadservice` como puente temporal según el gate.
- Manejo explícito de proveedor sin configuración, conocido aún no migrado y desconocido.
- Modal estable, foco inicial, restauración de foco, navegación por teclado y regiones `aria-live`.

## Restricciones

- El núcleo no puede referenciar `CIncripcionSII`, caché SII, libro, registro, matrícula, acto, noticia ni código de barras.
- No cambies endpoints mutadores ni inventes respuestas backend.
- Un proveedor desconocido nunca debe dirigirse a SII.
- Mantén el recorrido anterior cuando el gate esté desactivado.

## Aceptación

- Pruebas focales demuestran resolución de adaptador, capacidades y fallos seguros.
- Con el gate apagado se conserva el comportamiento anterior.
- Con el gate encendido existe una sola entrada visible.
- El núcleo puede probarse con un adaptador falso local sin red ni secretos.

## Correcciones opsxj:prompt-review

Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.

## Rol esperado
Definir el rol tecnico esperado para ejecutar el ticket.

## Objetivo
Describir el objetivo funcional y tecnico verificable.

## Restricciones criticas
- No introducir cambios fuera del alcance declarado.
- No romper comportamiento existente ni contratos publicos.

## Criterios de aceptacion
- El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.

## Contexto obligatorio
Listar archivos, modulos, servicios, hooks, adapters y documentacion que deben leerse antes de implementar.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar el paquete documental canonico del ticket.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

## Requisitos positivos
- Implementar el comportamiento esperado con contratos tipados y responsabilidades claras.
- Mantener la integracion sobre los puntos de extension existentes del repo.
- Dejar evidencia de pruebas y documentacion tecnica actualizada.

## Reglas de ubicacion de codigo
- Si se construye una app reusable o componente compartido, ubicarlo bajo `src/app/Components/<NombreComponente>/` o la ruta compartida equivalente existente.
- Si se implementa comportamiento de modulo funcional, ubicarlo bajo `src/modules/<modulo>/components/`, `hooks/`, `services/`, `adapters/` o `types/` segun responsabilidad.
- Adaptarse a la estructura existente del repo antes de crear carpetas nuevas.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.

Cuando el ticket afecte un flujo completo de usuario, navegacion, integracion entre vistas, persistencia de estado u operacion transaccional, exigir E2E real con Playwright; si no aplica, documentar justificacion formal y evidencia manual.
