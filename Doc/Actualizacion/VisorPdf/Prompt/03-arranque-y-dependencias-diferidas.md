# Prompt 03 — Arranque y carga diferida de dependencias del visor

## Contexto de ejecución OPSXJ

Este prompt corresponde a una Jira independiente. Ejecuta `opsxj:new` al iniciar para crear su OpenSpec autónomo. Inspecciona la implementación y evidencia del Prompt 02, pero no asumas que fue completado ni reutilices su cambio.

## Rol esperado

Actúa como arquitecto frontend senior especializado en carga web legacy, compatibilidad de librerías y reducción segura de JavaScript crítico.

## Fuentes obligatorias

- Exploración del visor y métricas vigentes.
- `pdfjs/pdf_anotate_view/index.html` completo.
- `script.js`, `pdf_viewer_anotate.js`, `extend_fabric.js`, scripts Wacom, impresión, Bootstrap, jQuery y jQuery UI referenciados.
- `GestionDocumental-Docuarchi.net.vbproj` para recursos desplegados.

## Objetivo

Reducir el costo de la primera apertura eliminando duplicados y sacando del camino crítico las capacidades opcionales, sin perder visualización, grafos, firma, impresión o compatibilidad WebForms.

## Inventario obligatorio previo

En `design.md`, crea una tabla por recurso con: ruta/URL, versión, símbolo aportado, consumidor real, momento necesario, duplicidad, política local/CDN e integridad. Verifica expresamente:

- múltiples jQuery UI;
- Popper duplicado;
- scripts PDF.js worker/sandbox/entry cargados como scripts normales;
- Font Awesome múltiple;
- Wacom y `signature_sdk.js` duplicados;
- Prettify, jsPDF, Fabric e impresión.

## Implementación obligatoria

1. Definir un núcleo mínimo para visualizar PDF y renderizar la primera página.
2. Eliminar referencias duplicadas solo después de probar el propietario de cada símbolo.
3. Cargar Fabric, Wacom, jsPDF, impresión, configuración de grafo y herramientas equivalentes cuando se active su capacidad.
4. Mantener una única versión compatible de cada dependencia.
5. Preferir recursos ya versionados en el repositorio cuando existan y documentar cualquier CDN que permanezca.
6. Evitar que una dependencia opcional ausente impida visualizar el PDF; la capacidad afectada debe fallar de forma localizada y comprensible.
7. Conservar orden determinista, CSP/integridad existentes y registro en `.vbproj` de recursos nuevos.

## Restricciones y antirregresión

- No actualizar versiones mayores durante esta tarea salvo requisito demostrado y pruebas específicas.
- No retirar Wacom, firma, impresión o grafos por considerarlos poco usados.
- No agregar bundler, SPA o framework nuevo.
- No introducir recursos remotos adicionales.
- No modificar todavía transporte del PDF o persistencia de grafos.

## Pruebas obligatorias

- Contrato estático que detecte duplicados prohibidos e inventario requerido.
- Matriz: solo visualización, grafo, firma personal, Wacom disponible/no disponible, impresión y descarga.
- Arranque con una dependencia opcional simulada como fallida.
- E2E de lectura con medición de `domInteractive`, primer canvas, errores y solicitudes.
- Comparación explícita contra 1.679 ms de `domInteractive` en la primera carga y 92–98 ms en recargas.

## Criterios de aceptación

- El visor básico no depende de cargar herramientas opcionales.
- No existen cargas duplicadas conocidas.
- Todas las capacidades habilitadas conservan comportamiento.
- La primera apertura mejora y no aparecen errores nuevos.

## Documentación y entrega

Registra inventario antes/después, decisiones de versiones, matriz y métricas en `Doc/Actualizacion/VisorPdf/Implementacion/<JIRA>/`.

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
Ejecutar pruebas Node focales, validación sintáctica JavaScript y E2E con Playwright; ejecutar MSBuild únicamente si se toca código compilado y registrar comandos y resultados.

## Documentacion tecnica
Actualizar el paquete documental canonico del ticket.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

Exigir validación sintáctica JavaScript, pruebas Node focales y, si se modifica código compilado, MSBuild; registrar el resultado.

Exigir pruebas focales con la infraestructura Node existente; no agregar un framework de pruebas innecesario.

Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.

Declarar que código + E2E + validación autorizada + evidencia saneada son una única unidad de entrega dentro del mismo cambio; no crear una tarea o entrega E2E independiente.

Reutilizar exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; prohibir login, arnés, proyecto Playwright, configuración o `.env` paralelos.

Antes de autenticar, exigir lectura de `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos/tareas descartables expresamente autorizados.

Exigir secretos efímeros, prohibir exponer/imprimir/persistir credenciales, cookies, tokens y cadenas de conexión, usar verificaciones solo `SELECT` y conservar evidencia saneada.

Exigir cobertura E2E, cuando aplique, de autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.

Respetar feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; no cerrar sin validación autorizada, registrar bloqueo explícito y prohibir mocks, simulaciones, resultados inventados y evidencia ficticia.
