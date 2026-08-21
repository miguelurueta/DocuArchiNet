# 04 — Interfaz moderna oficial

## ROL ESPERADO

Actúa como desarrollador senior de ASP.NET Web Forms y JavaScript accesible.

## OBJETIVO

Conectar **Devolver a usuario anterior** a los endpoints modernos mediante una confirmación accesible y una única experiencia moderna para todo contexto Workflow válido.

## CONTEXTO OBLIGATORIO

- Requiere 03 aprobado y endpoints de preview/ejecución disponibles.
- Leer `00-contexto-obligatorio.md`, evidencia de 03 y componentes modernos existentes.
- Habilita 05 únicamente si no comparte listeners, estado o payload con otras operaciones.

## REQUISITOS POSITIVOS

- Registrar trigger y bootstrap uniformes con un adaptador JavaScript exclusivo de devolución a usuario anterior.
- Consumir `PreviewDevolverUsuarioAnterior` y `EjecutarDevolverUsuarioAnterior`.
- Presentar exclusivamente el usuario y actividad históricos resueltos por el servidor.
- Reutilizar modal, foco, trampa de foco, teclado, Escape, ARIA, responsive, cancelación, doble clic y mensajes correlacionados.
- Mientras ejecuta, deshabilitar confirmación y cierre que pueda abandonar un resultado pendiente.
- Tras éxito, actualizar solo tarea afectada, visor, contador, listado y scroll horizontal mediante componentes modernos existentes.

## RESTRICCIONES CRÍTICAS

- No crear framework, bundler, selector de destinos, búsqueda, paginación, modal paralelo, banderas de habilitación ni autorización JavaScript.
- No usar controles ocultos, postbacks, `GridView`, `UpdatePanel`, `ModalPopupExtender`, SQL ni handlers Web Forms.
- No invocar endpoints, payloads o selectores de Devolver a actividad anterior, Continuar flujo, Enviar a usuario o Enviar a grupo.
- No incluir ni mostrar datos de respuestas.
- No ejecutar E2E autenticada sin autorización explícita.

## REGLAS DE ANTIRREGRESIÓN

- La devolución a usuario anterior y las demás operaciones no comparten selectores, eventos, estado ni requests.
- Todo contexto válido usa el mismo adaptador moderno de devolución a usuario anterior.

## CRITERIOS DE ACEPTACIÓN

- El modal representa solo JSON autorizado con un único destino histórico de usuario.
- Historial ausente, grupo, usuario retirado o auto-devolución muestran bloqueo y no proponen actividades alternativas.
- Éxito, bloqueo y error mantienen la bandeja en un estado consistente y accesible.

## PRUEBAS OBLIGATORIAS

Agregar pruebas CJS de contratos, eventos aislados, confirmación, historial ausente, grupo, usuario retirado, auto-devolución, error, éxito, bloqueo, cancelación, doble clic, teclado, foco, Escape, responsive y bloqueo durante ejecución. Ejecutar MSBuild y pruebas focales; no E2E sin autorización.

## DOCUMENTACIÓN TÉCNICA

Actualizar arquitectura, contrato, flujo, evidencia y diagramas necesarios con selectores, UI, accesibilidad y recorrido moderno.

## ENTREGABLE FINAL

Reportar ticket, archivos UI, pruebas, compilación y evidencia de ruta moderna/no regresión. No cambiar configuración de ambiente ni realizar QA autenticada sin autorización.
