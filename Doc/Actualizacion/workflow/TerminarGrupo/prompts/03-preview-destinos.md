# 03 — Preview seguro de destinos de grupo

## ROL ESPERADO

Actúa como desarrollador senior de ASP.NET Web Forms, ASMX, MySQL y seguridad de workflows.

## OBJETIVO

Agregar `PreviewEnviarGrupo(idTarea)` al `WebServiceWorkflowModern.asmx` existente y resolver destinos directos de grupo con consultas exclusivamente de lectura.

## RESTRICCIONES CRITICAS

- Leer y aplicar `prompts/00-contexto-obligatorio.md`.
- El preview no adquiere lock, no invoca el motor legacy, no ejecuta eventos, no registra auditoría y no altera estados.
- Usar el ASMX moderno existente; no crear un segundo ASMX ni modificar `PreviewEnviarTarea`.
- No reutilizar `MySqlTransicionRutaRepository` para destinos de grupo: representa conectores salientes y no el listado directo de actividades de ruta.
- Repositorios con SQL parametrizado y modelos tipados; no `Session`, `DataSet` ni HTML.
- El gate existente se evalúa en servidor antes de resolver datos y conserva fail-closed; no modificar su configuración.

## REQUISITOS POSITIVOS

1. Crear repositorio y servicio específicos de destinos de grupo que repliquen la semántica autorizada de `Solicita_listado_actividades_ruta`.
2. Validar contexto, `Cambio_Ruta`, tarea activa, acceso del usuario y token de versión.
3. Rechazar ruta cerrada y, cuando aplique, flujo o actividad de flujo cerrados.
4. Retornar únicamente actividades pertenecientes a la ruta y contexto sanitizado necesario para confirmación.
5. Devolver códigos funcionales estables para inactivo, permiso denegado, tarea no disponible, ruta/flujo cerrado, sin destinos e inconsistencia.

## CRITERIOS DE ACEPTACION

- El endpoint solo realiza SELECT y no deja auditoría ni efectos de motor.
- Un navegador no puede obtener destinos fuera del contexto, permiso y gate permitidos.
- La lista de grupo no queda restringida erróneamente a conectores de continuar flujo.
- `PreviewEnviarTarea` conserva contrato y resultados existentes.

## REGLAS DE ANTIRREGRESION

- Mantener intacto el preview moderno actual de continuar flujo durante toda la etapa.
- No cambiar endpoint, request, DTO, código funcional, consulta ni orden de destinos de `PreviewEnviarTarea`.
- No modificar el preview legacy ni sus controles Web Forms; con gate inactivo no se registra ni consume el preview de grupo.
- Ejecutar las pruebas existentes de preview ruta/flujo y confirmar que sus resultados permanecen idénticos antes y después de la etapa.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas de preview válido, gate inactivo, permiso negado, tarea inaccesible, ruta/flujo/actividad cerrados, sin destinos y destino de otra ruta. Ejecutar `msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug`, registrar código de salida y pruebas focales ejecutadas; si no está disponible, registrar limitación y QA manual reproducible. No E2E autenticado ni carga sin autorización.

## DOCUMENTACION TECNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarGrupo/03-preview-destinos/` con contrato JSON, secuencia de solo lectura, consultas permitidas, códigos funcionales, matriz de pruebas y diagramas necesarios.

## ENTREGABLE FINAL

Entregar rutas de endpoint, servicio y repositorio; payload JSON; pruebas/compilación; evidencia de no escritura; documentación y riesgos. No implementar ejecución ni UI en esta etapa.
