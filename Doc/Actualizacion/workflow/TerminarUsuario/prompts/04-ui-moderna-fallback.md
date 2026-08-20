# 04 — Interfaz moderna y fallback Web Forms

## ROL ESPERADO

Actúa como desarrollador senior de ASP.NET Web Forms y JavaScript legacy accesible.

## OBJETIVO

Conectar solo el comando **Enviar a usuario** a los endpoints ya implementados, con búsqueda paginada, confirmación accesible y fallback legacy exacto.

## CONTEXTO OBLIGATORIO

- Requiere 03 aprobado y contratos de preview/ejecución disponibles.
- Leer `00-contexto-obligatorio.md`, evidencia de 03, CSS/componentes de confirmación existentes y exploración arquitectónica.
- Habilita 05 únicamente cuando no haya listeners, estado ni payload compartido con Continuar flujo.

## REQUISITOS POSITIVOS

- Registrar trigger y bootstrap detrás del gate existente, con adaptador JavaScript exclusivo de usuario.
- Consumir `PreviewEnviarUsuario` y `EjecutarEnvioUsuario`; aplicar debounce, páginas, descarte de respuesta obsoleta e invalidación de selección antigua.
- Reutilizar confirmación, foco, trampa de foco, teclado, Escape, ARIA, responsive, cancelación, doble clic y mensaje de éxito correlacionado.
- Tras éxito, actualizar solo tarea afectada, visor y contador mediante componentes modernos existentes.

## RESTRICCIONES CRÍTICAS

- No crear ASMX, gate, framework, bundler, modal alterno ni autorización en JavaScript.
- No invocar controles ocultos, motor, handlers, SQL, `Cambia_Estado`, reasignación de respuesta ni endpoints/payload de Continuar flujo.
- Con gate inactivo, no cambiar enlace, postback ni modal legacy de `ImageButtonEnviarUsuario`.
- No ejecutar E2E autenticado ni activar gate.

## REGLAS DE ANTIRREGRESIÓN

- Enviar a usuario y Continuar flujo no comparten selectores, eventos, estado, `IdConector` ni requests.
- El fallback Web Forms es el recorrido exacto cuando el gate está inactivo.

## CRITERIOS DE ACEPTACIÓN

- Modal y búsqueda representan solo JSON autorizado, soportan teclado/foco/Escape y no materializan la lista completa.
- Error, bloqueo, cancelación y respuesta obsoleta restauran contexto sin iniciar flujo legacy de reasignación.

## PRUEBAS OBLIGATORIAS

Agregar pruebas CJS de contratos, eventos aislados, búsqueda, debounce, páginas, respuesta obsoleta, vacío, error, selección, éxito, bloqueo, cancelación, doble clic, teclado, foco y fallback. Ejecutar MSBuild y pruebas focales; registrar evidencia. No E2E.

## DOCUMENTACIÓN TÉCNICA

Actualizar `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md`, `04-pruebas-y-evidencia.md` y diagramas necesarios con selectores, UI, accesibilidad, fallback y relevo a 05.

## ENTREGABLE FINAL

Reportar ticket, archivos UI, pruebas, compilación y evidencia de fallback/no regresión. No activar gate ni realizar QA autenticado.
