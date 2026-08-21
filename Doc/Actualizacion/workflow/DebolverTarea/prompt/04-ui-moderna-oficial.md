# 04 — Interfaz moderna oficial

## ROL ESPERADO

Actúa como desarrollador senior de ASP.NET Web Forms y JavaScript accesible.

## OBJETIVO

Conectar **Devolver a actividad anterior** a los endpoints modernos, con búsqueda paginada, confirmación accesible y una única experiencia moderna para todo contexto Workflow válido.

## CONTEXTO OBLIGATORIO

- Requiere 03 aprobado y endpoints de preview/ejecución disponibles.
- Leer `00-contexto-obligatorio.md`, evidencia de 03 y componentes modernos existentes.
- Habilita 05 únicamente si no comparte listeners, estado o payload con otras operaciones.

## REQUISITOS POSITIVOS

- Registrar trigger y bootstrap uniformes con un adaptador JavaScript exclusivo de devolución.
- Consumir `PreviewDevolverActividad` y `EjecutarDevolverActividad`.
- Aplicar término mínimo, debounce, paginación, cancelación o descarte de respuestas obsoletas e invalidación de selección antigua.
- Reutilizar modal, foco, trampa de foco, teclado, Escape, ARIA, responsive, cancelación, doble clic y mensajes correlacionados.
- Mientras ejecuta, deshabilitar confirmación y cierre que pueda abandonar un resultado pendiente.
- Tras éxito, actualizar solo tarea afectada, visor, contador, listado y scroll horizontal mediante componentes modernos existentes.

## RESTRICCIONES CRÍTICAS

- No crear framework, bundler, modal paralelo, banderas de habilitación ni autorización JavaScript.
- No usar controles ocultos, postbacks, `GridView`, `UpdatePanel`, `ModalPopupExtender`, SQL ni handlers Web Forms.
- No invocar endpoints, payloads o selectores de Continuar flujo, Enviar a usuario, Enviar a grupo o Usuario anterior.
- No incluir ni mostrar datos de respuestas.
- No ejecutar E2E autenticada sin autorización explícita.

## REGLAS DE ANTIRREGRESIÓN

- La devolución y las demás operaciones no comparten selectores, eventos, estado ni requests.
- Todo contexto válido usa el mismo adaptador moderno de devolución.

## CRITERIOS DE ACEPTACIÓN

- El modal representa solo JSON autorizado y nunca materializa la lista completa.
- La búsqueda, paginación, vacío, error y respuesta obsoleta restauran estado sin iniciar una transición.
- Éxito, bloqueo y error mantienen la bandeja en un estado consistente y accesible.

## PRUEBAS OBLIGATORIAS

Agregar pruebas CJS de contratos, eventos aislados, búsqueda, debounce, páginas, respuesta obsoleta, vacío, error, selección, éxito, bloqueo, cancelación, doble clic, teclado, foco, Escape, responsive y bloqueo durante ejecución. Ejecutar MSBuild y pruebas focales; no E2E sin autorización.

## DOCUMENTACIÓN TÉCNICA

Actualizar arquitectura, contrato, flujo, evidencia y diagramas necesarios con selectores, UI, accesibilidad y recorrido moderno.

## ENTREGABLE FINAL

Reportar ticket, archivos UI, pruebas, compilación y evidencia de ruta moderna/no regresión. No cambiar configuración de ambiente ni realizar QA autenticada sin autorización.
